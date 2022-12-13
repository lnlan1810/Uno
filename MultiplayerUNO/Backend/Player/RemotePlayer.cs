using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;
 
namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    /// Đầu phát từ xa, kết nối với phòng qua Socket
    /// Удаленный плеер, подключение к комнате через Socket
    /// </summary>
    public class RemotePlayer : Player
    {
        public static string ProtocolVersion = "0.0.1";

        protected Thread sendThread;    
        protected Thread recvThread;    // accept thread

        protected Socket clientSocket; 

        public Room GameRoom { get; }     // in the room


        public RemotePlayer(Socket socket, Room gameRoom)
        {
            sendQueue = new BlockingCollection<string>();

            clientSocket = socket;
            GameRoom = gameRoom;

            sendThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string msg = sendQueue.Take(); //blocked here
                        socket.Send(Encoding.UTF8.GetBytes(msg + "$")); // Add $ at the end for easy splitting
                    }
                    catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (InvalidOperationException e) {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
            sendThread.IsBackground = true; // set as background thread

            recvThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] content = new byte[BUFFERSIZE];
                        int n = clientSocket.Receive(content); //blocked here
                        string word = Encoding.UTF8.GetString(content, 0, n);

                        foreach(string sw in word.Split('$'))   // Split by $
                        {
                            if (sw.Length <= 0) continue;
                            GameRoom.InfoQueue.Add(new Room.MsgArgs
                            {
                                player = this,
                                msg = sw
                            }); // Thông tin được đưa vào InfoQueue để luồng xử lý trò chơi xử lý
                            //Информация помещается в InfoQueue для обработки потоком обработки игры.
                        }

                    }
                    catch(ObjectDisposedException se)
                    {
                        Console.WriteLine(se.Message);
                        break;

                    }catch(SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Console.WriteLine(ioe.Message);
                        break;
                    }catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

                GameRoom.InfoQueue.Add(new Room.MsgArgs
                {
                    player = this,
                    type = Room.MsgType.PlayerLeave
                }); //Thông báo chuỗi xử lý trò chơi mà người chơi đã rời đi
                //Сообщите потоку обработки игры, что игрок покинул
            });

            recvThread.IsBackground = true; 
        }


        public static int BUFFERSIZE = 8192;
        /// <summary>
        /// Hoạt động sau khi kết nối socket được thiết lập, 
        /// đánh giá xem định dạng của tin nhắn đầu tiên được gửi có đúng hay không và liệu phiên bản giao thức có nhất quán hay không
        /// 
        /// Операция после установления соединения «Socket»,
        /// определяющая правильность формата первого отправленного сообщения и согласованность версии протокола.
        /// </summary>
        /// <returns>Thông tin bên kia gửi có hợp pháp không? Является ли информация, отправленная другой стороной, законной? </returns>
        public bool OpenStream()
        {
            try
            {
                byte[] content = new byte[BUFFERSIZE];
                int n = clientSocket.Receive(content);
                string word = Encoding.UTF8.GetString(content, 0, n);
                JsonData json = JsonMapper.ToObject(word.Split('$')[0]);
                if (!((string)json["version"] == ProtocolVersion))   // phiên bản phải phù hợp версия должна соответствовать
                    throw new ArgumentException("Inconsistent Version", "version");

                name = (string)json["name"];   // get player name

                Console.WriteLine(word);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                clientSocket.Close();
                return false;   // Bất hợp pháp, trực tiếp cắt bỏ
                //Любое незаконное, прямо отрезанное
            }


            sendThread.Start(); 
            recvThread.Start();
            return true;
        }

        /// <summary>
        /// Được phụ trợ gọi để đuổi người chơi ra khỏi phòng
        /// Вызывается бэкэндом, чтобы выкинуть игрока из комнаты
        /// </summary>
        public void Leave()
        {
            sendQueue.CompleteAdding();
            clientSocket.Close();
        }
    }
}
