using MultiplayerUNO.Backend.Player;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;

namespace MultiplayerUNO.Backend
{
  
    public partial class Room
    {
        public static readonly int MinPlayerNumber = 2;
        public static readonly int MaxPlayerNumber = 6;

        public static readonly int MaxSeatIDCounter = 4096;

        public class MsgArgs
        {
            public Player.Player player;  
            public string msg;  
            public MsgType type = MsgType.Remote;  // information type
        }


        public enum MsgType
        {
            Remote,     
            PlayerJoin, 
            PlayerLeave, 
            PlayerTimeout, 
            RobotResponse 
        }


        public BlockingCollection<MsgArgs> InfoQueue { get; } 

        public const string Version = "0.1.0"; 

        protected IPEndPoint iPEndPoint;
        protected Thread listenThread;

        protected Thread processThread;
        public LocalPlayer LocalPlayer { get; }

        public Room(LocalPlayerAdapter localPlayerAdapter)
        {
            iPEndPoint = localPlayerAdapter.EndPoint;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InfoQueue = new BlockingCollection<MsgArgs>();
            
            
            ingamePlayers =  new LinkedList<Player.Player>(); 

            LocalPlayer = new LocalPlayer(localPlayerAdapter);
            ingamePlayers.AddLast(LocalPlayer);

            currentStatus = GameStatus.Waiting;
        }

        protected Socket listenSocket;
        protected LinkedList<Player.Player> ingamePlayers;
        protected int assignedSeatID = 0; // UID được phân bổ, không bao giờ giảm

        /// <summary>
        /// Initialize the room
        /// </summary>
        public void InitializeRoom()
        {
            // Start the game processing thread
            processThread = new Thread(ProcessThreadFunc);
            processThread.IsBackground = true;
            processThread.Start();

            // bind socket
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(10);

            // listening thread
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Socket rSocket = listenSocket.Accept(); // thiết lập kết nối
                        RemotePlayer remotePlayer = new RemotePlayer(rSocket, this);
                        new Thread(() =>
                        {
                            if (remotePlayer.OpenStream())
                            {
                                // Nhập phòng thành công, thông báo thread đang xử lý
                                //Успешно войти в комнату, уведомить поток обработки
                                InfoQueue.Add(new MsgArgs()
                                {
                                    player = remotePlayer,
                                    type = MsgType.PlayerJoin
                                });
                            }
                        }).Start();

                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }catch(SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });

            listenThread.IsBackground = true; //background thread
            listenThread.Start();
        }

        /// <summary>
        /// state of game
        /// </summary>
        protected enum GameStatus
        {
            Waiting, 
            Common,  
            QueryPlayer,
            Plus2Loop, 
            CardsDrawing, 
            Plus4Loop, 
            Questioning, 
            GameEnd 
        };


        protected GameStatus currentStatus;

        /// <summary>
        ///  Core: Game Processing Thread
        /// </summary>
        public void ProcessThreadFunc()
        {
            while (true)
            {
                MsgArgs msgArgs = null;
                try
                {
                    msgArgs = InfoQueue.Take();  // Constantly trying to extract information from the message
                }
                catch(ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                
                if(msgArgs.type == MsgType.PlayerJoin)
                {
                    // Thông tin tham gia người chơi do chính Server gửi
                    //Информация о присоединении игрока, отправленная самим сервером
                    PlayerJoin(msgArgs.player);
                    continue;
                }
                if(msgArgs.type == MsgType.PlayerLeave)
                {
                    // Người chơi để lại tin nhắn được gửi bởi máy chủ
                    //Игрок оставил сообщение, отправленное сервером
                    PlayerLeave(msgArgs.player);
                    continue;
                }

                JsonData cameJson = null;
                try{ cameJson = JsonMapper.ToObject(msgArgs.msg); }
                catch (JsonException) { continue; }

                string info = string.Format("[{0}] {1}", msgArgs.player.Name, msgArgs.msg);
                Console.WriteLine(info);

                try
                {
                    // Gọi hàm xử lý tương ứng theo trạng thái hiện tại
                    //Вызов соответствующей функции обработки в соответствии с текущим состоянием
                    switch (currentStatus)
                    {
                        case GameStatus.Waiting:
                            ProcWaiting(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Common:
                            ProcCommon(cameJson, msgArgs.player);
                            break;
                        case GameStatus.QueryPlayer:
                            ProcQuery(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Plus2Loop:
                            ProcPlus2Loop(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Plus4Loop:
                            ProcPlus4Loop(cameJson, msgArgs.player);
                            break;
                    }
                }
                catch (TieExceptions)
                {
                    // exception, game over
                    JsonData json = new JsonData
                    {
                        ["turnID"] = 0
                    };
                    GameEndProcess(json);
                }
                catch (PlayerFinishException e)
                {
                    // When a player finishes playing cards, the game ends
                    JsonData json = new JsonData
                    {
                        ["turnID"] = e.player.ingameID,
                        ["lastCard"] = lastCard.CardId,
                        ["intInfo"] = lastCardInfo
                    };
                    GameEndProcess(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                

                // foreach (Player.Player p in ingamePlayers)
                //     p.SendMessage(info);
            }
        }

        /// <summary>
        /// Actions after the game
        /// </summary>
        protected void GameEndProcess(JsonData json)
        {
            gameTimer.Dispose();
            currentStatus = GameStatus.GameEnd;

            json["state"] = (int)currentStatus;
            json["playerCards"] = new JsonData();
            json["playerCards"].SetJsonType(JsonType.Array);

            foreach (Player.Player p in ingamePlayers)
                json["playerCards"].Add(p.BuildPlayerMapJson(true)); // Hiển thị tay của tất cả người chơi tại thời điểm này
            string sendJson = json.ToJson();

            //  // Bên dưới: Loại bỏ tất cả những người chơi bị AI tiếp quản
            //Внизу: отбраковывает всех игроков, захваченных ИИ.
            LinkedList<Player.Player> tempPlayers = new LinkedList<Player.Player>();
            foreach (Player.Player p in ingamePlayers)
            {
                if (p.isRobot == 0)
                {
                    tempPlayers.AddLast(p);
                    p.SendMessage(sendJson);
                }
            }
            ingamePlayers = tempPlayers;

            currentStatus = GameStatus.Waiting; //Switch back to waiting state

        }

        /// <summary>
        /// player joins room
        /// </summary>
        public void PlayerJoin(Player.Player player)
        {
            if(ingamePlayers.Count >= MaxPlayerNumber || currentStatus != GameStatus.Waiting
                || assignedSeatID >= MaxSeatIDCounter)
            {
                // no longer accepting players
                RemotePlayer remote = player as RemotePlayer;
                remote?.Leave(); //Kicked the player directly
                return;
            }

            // player joins room
            ingamePlayers.AddLast(player);
            
            assignedSeatID++;  // Tăng UID người chơi UID игрока увеличен
            player.seatID = assignedSeatID; // UID copy

            JsonData json = BuildPlayerWaitingJson();// Construct the room information json in the waiting state
            player.SendMessage(json.ToJson()); // sent to joining players

            string joinJson = new JsonData
            {
                ["type"] = 3,
                ["player"] = player.GetPlayerJson()
            }.ToJson();

            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(joinJson);
        }

        /// <summary>
        /// player leaves
        /// </summary>
        public bool PlayerLeave(Player.Player player)
        {
            if(currentStatus == GameStatus.Waiting)
            {
                // A player quits in the waiting state: quit directly
                bool res = ingamePlayers.Remove(player);
                if (!res) return false;

                string leaveJson = new JsonData
                {
                    ["type"] = 2,
                    ["player"] = player.GetPlayerJson()
                }.ToJson();

                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(leaveJson);  // người chơi phát sóng rời đi
            }
            else
            {
                // Player quits during gameplay: AI takes over
                player.isRobot = 1;
                string leaveJson = new JsonData
                {
                    ["state"] = -1,
                    ["playerID"] = player.ingameID
                }.ToJson();

                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(leaveJson); // broadcast
            }

            return true;
        }


        /// <summary>
        /// Được gọi bởi giao diện người dùng để đóng cửa phòng
        /// Вызывается передним концом, чтобы закрыть комнату
        /// </summary>
        public void CloseRoom()
        {
            listenSocket.Close(); // close socket

            foreach (Player.Player player in ingamePlayers)
            {
                RemotePlayer remotePlayer = player as RemotePlayer;
                remotePlayer?.Leave(); // Force remote players to leave
            }
        }

        /// <summary>
        /// Construct player json in the waiting state
        /// </summary>
        public JsonData BuildPlayerWaitingJson()
        {

            JsonData json = new JsonData();
            json["type"] = 0;
            json["player"] = new JsonData();
            json["player"].SetJsonType(JsonType.Array);

            foreach (Player.Player player in ingamePlayers)
            {
                json["player"].Add(player.GetPlayerJson());
            }

            return json;
        }
    }
}
