using System;
using System.Collections.Generic;
using System.Threading;
using LitJson;
using MultiplayerUNO.Utils;

namespace MultiplayerUNO.Backend
{
    public partial class Room
    {
        protected const int NormalWaitingTime = 45000; //default wait time 
        protected const int ShortWaitingTime = 10000; 

        /// <summary>
        /// Không hợp lệ, hãy gửi lại json mẫu trò chơi cho người chơi tương ứng
        /// Недействительно, повторно отправьте шаблон игры в формате json соответствующему игроку.
        /// </summary>
        protected void SendInvalidIInfo(Player.Player sendPlayer)
        {
            sendPlayer.SendMessage(BuildGamePatternJson().ToJson());
        }

        /// <summary>
        ///+4 xử lý câu hỏi
        ///+4 обработка вопросов
        /// </summary>
        protected void ProcPlus4Loop(JsonData jsonData, Player.Player sendPlayer)
        {
            // Nếu yêu cầu trạng thái trò chơi json
            //Если запросить состояние игры json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }

            //Xác định xem trình phát gửi và ID yêu cầu có tương ứng hay không
            //Определить, соответствуют ли отправляющий игрок и идентификатор запроса
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID)
            {
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if(state == 4)
            {  // Đừng thắc mắc.Chuẩn bị rút bài 
                //Не задавай вопросов, приготовься тянуть карты
                DrawCardBack2Common(4, sendPlayer);
            }
            else
            {
                if (plus4Player.CanHandoutPlus4(plus4ResponseCard, plus4ColorID & 3))
                    DrawCardBack2Common(6, sendPlayer); 
                else
                    DrawCardBack2Common(4, plus4Player, false); 

            }
        }

        /// <summary>
        /// stack +2 processing
        /// </summary>
        protected void ProcPlus2Loop(JsonData jsonData, Player.Player sendPlayer)
        {
            // 如果请求游戏状态json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }
            // Xác định xem id yêu cầu và người gửi có hợp pháp không
            //Определите, являются ли идентификатор запроса и отправитель законными
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID)
            {
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if (state == 3)
            {    //стек +2 карты
                int cardId = (int)jsonData["card"];
                Card responseCard = FindCardInPlayerHandcards(cardId, sendPlayer);

                if (responseCard == null || responseCard.Number != 11)
                {
                    // Player does not have this card, or this card is not +2
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                // Ở trên, phán quyết về tính hợp pháp đã hoàn thành và các quân bài bắt đầu được chơi
                //Выше суждение о законности завершено, и карты начинают разыгрываться.
                PlayerHandCard(sendPlayer, responseCard, -1);
            }
            else
            {
                DrawCardBack2Common(drawingCardCounter, sendPlayer); 
                drawingCardCounter = 0; //Counter returns to 0
            }

        }

        /// <summary>
        /// hỏi sau khi bốc thăm
        /// спросить после розыгрыша
        /// </summary>
        protected void ProcQuery(JsonData jsonData, Player.Player sendPlayer)
        {

            int state = (int)jsonData["state"];
            if(state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }


            int action = (int)jsonData["action"];
            int responseID = (int)jsonData["queryID"];
            int colorId = -1;

            if(gainCard.Color == Card.CardColor.Invalid)
            {
                colorId = (int)jsonData["color"];
                if(colorId >> 2 != 0)// Thẻ phổ thông/+4, cần chỉ định màu
                                     //Универсальная карта/+4, необходимо указать цвет
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
            }

            if(responseID != queryID)
            {
                // Số yêu cầu cần nhất quán
                //Номер запроса должен быть последовательным
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if(action == 1)
            {
                // Out of the situation
                if (lastCard != null && !gainCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)))
                {
                    // Responsive to needs
                    SendInvalidIInfo(sendPlayer);
                    return;
                }

                // Đã được chứng minh hợp pháp
                //Доказанная законность
                PlayerHandCard(sendPlayer, gainCard, colorId);  // играя в карты

            }
            else
            {
                // Not selected
                gameTimer.Dispose(); // off timer
                Change2NextTurnPlayerNode(); // Update the current player as the player to be operated
                queryID++;// Yêu cầu truy vấn của khách hàng sắp được bắt đầu
                //Запрос запроса клиента собирается быть инициирован

                TimerStart(new MsgArgs
                {
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                    player = currentPlayerNode.Value,
                    type = MsgType.PlayerTimeout
                }); // thời gian bắt đầu
                //Сроки начинаются

                currentStatus = GameStatus.Common; //Game state is set to 1

                if (currentPlayerNode.Value.isRobot == 1) // When AI takes over
                {
                    // come out
                    InfoQueue.Add(new MsgArgs
                    {
                        msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                        player = currentPlayerNode.Value,
                        type = MsgType.PlayerTimeout
                    });
                }

                string json = BuildGamePatternJson().ToJson();
                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(json); // Gửi mẫu trò chơi Json cho mỗi người chơi
                //Отправить игровой шаблон Json каждому игроку
            }


        }

        protected void ProcCommon(JsonData jsonData, Player.Player sendPlayer)
        {
            // Nếu yêu cầu trạng thái trò chơi json
            //Если запросить состояние игры json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }
            // Need to request the same id
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID) {
                SendInvalidIInfo(sendPlayer);
                return;
            }
            
            if(state == 1)
            {
                // The situation where the cards are played
                int cardId = (int)jsonData["card"];
                int colorId = (int)jsonData["color"];
                Card responseCard = FindCardInPlayerHandcards(cardId, sendPlayer);

                // Must be a responsive card
                if (responseCard == null || (responseCard.Color == Card.CardColor.Invalid && colorId >> 2 != 0))
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                bool condition2 = responseCard.IsPlus4() && sendPlayer.handCards.Count <= 1; 
                bool condition3 = lastCard != null && !responseCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)); 

                if(condition2 || condition3)
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                // Ở trên, phán quyết về tính pháp lý đã hoàn thành; ở dưới, thao tác đã được xử lý thành công
                //Вверху суждение о законности завершено, внизу операция успешно завершена

                PlayerHandCard(sendPlayer, responseCard, colorId);
            }
            else
            { // Tình trạng không đánh bài
                //Ситуация с не игрой в карты
                gameTimer.Dispose(); // hẹn giờ dừng
                                     //остановить таймер

                gainCard = cardPile.DrawOneCard();
                sendPlayer.GainCard(gainCard);  // rút thẻ Нарисовать карточку
                queryID++; // Yêu cầu truy vấn của khách hàng sắp được bắt đầu
                //Запрос запроса клиента собирается быть инициирован

                TimerStart(new MsgArgs
                {
                    msg = AutoResponseOrNot().ToJson(), //ra khỏi  Убирайся
                    player = currentPlayerNode.Value,
                    type = MsgType.PlayerTimeout
                }, ShortWaitingTime); // thời gian bắt đầu Сроки начинаются

                if (sendPlayer.isRobot == 1) // Khi AI tiếp quản 
                                             //Когда AI берет верх
                {
                    InfoQueue.Add(new MsgArgs
                    {
                        msg = AutoResponseOrNot(true).ToJson(), //ra khỏi  Убирайся
                        player = currentPlayerNode.Value,
                        type = MsgType.RobotResponse
                    });
                }

                currentStatus = GameStatus.QueryPlayer; // vào trạng thái 2  войти в состояние 2
                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(BuildGamePatternJson(p).ToJson()); // Gửi mẫu trò chơi Json cho mỗi người chơi
                // Отправить игровой шаблон Json каждому игроку

            }
        }

        /// <summary>
        /// The processing of the room is waiting
        /// </summary>
        protected void ProcWaiting(JsonData jsonData, Player.Player sendPlayer)
        {
            int type = (int)jsonData["type"];

            switch (type)
            {
                case 0:
                    // request status json
                    sendPlayer.SendMessage(BuildPlayerWaitingJson().ToJson());
                    return;
                case 1:
                    // player ready
                    if (sendPlayer.IsReady == false)
                    {
                        sendPlayer.IsReady = true;
                        string readyJson = new JsonData
                        {
                            ["type"] = 1,
                            ["player"] = sendPlayer.GetPlayerJson()
                        }.ToJson();

                        foreach (Player.Player p in ingamePlayers)
                        {
                            p.SendMessage(readyJson);
                        }
                    }
                    return;
                case 2:
                    // player cancel preparation
                    if (sendPlayer.IsReady)
                    {
                        sendPlayer.IsReady = false;
                        string readyJson = new JsonData
                        {
                            ["type"] = 4,
                            ["player"] = sendPlayer.GetPlayerJson()
                        }.ToJson();

                        foreach (Player.Player p in ingamePlayers)
                        {
                            p.SendMessage(readyJson);
                        }
                    }
                    return;
            }

            if (type != 3) return;

            // player starts game
            bool canStart = true;
            foreach (Player.Player p in ingamePlayers)
                if(p.IsReady == false)
                {
                    canStart = false;
                    break;
                }
            if (!canStart || ingamePlayers.Count < MinPlayerNumber)
            {
                // Someone did not prepare or did not reach the minimum number of players,
                // and the start failed
                sendPlayer.SendMessage("{\"type\":6}");
                return;
            }

            // Games start
            ShuffleIngamePlayers();  // sắp xếp lại người chơi переставить игроков 
            cardPile = new GameCardPile();  // новая куча
            cardPile.ShuffleCards();    // xáo trộn перемешивать
            direction = 1;  // lần lượt theo thứ tự bình thường
                            //повернуть в обычном порядке 
            lastCard = null;  // Không có thẻ trước đó cần được trả lời khi bắt đầu
                              // Нет предыдущей карты, на которую нужно ответить в начале
            lastCardInfo = -1; // Chỉ có ý nghĩa khi LastCard là +4/phổ quát
                               // Имеет смысл, только если lastCard +4/универсальный 
            queryID = 1;  // Đặt lại bộ đếm số yêu cầu Сбросить счетчик количества запросов
            drawingCardCounter = 0; //+2 lượt truy cập +2 счетчик розыгрыша
            plus4ColorID = -1; 
            plus4Player = null; 
            plus4ResponseCard = null; 

            currentStatus = GameStatus.Common;  // Go to State 1

            foreach (Player.Player p in ingamePlayers)
            {
                p.StartGameReset();
        
                p.GainCard(cardPile.DrawCards(7)); // Each player is dealt an initial 7 cards
            }
            currentPlayerNode = ingamePlayers.First;    // first player first action

            GC.Collect(); // dọn rác 

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            });

            foreach (Player.Player p in ingamePlayers)
            {
                p.SendMessage(BuildGameStateJson(p).ToJson());  //phát ra trạng thái trò chơi json cho mỗi người chơi
                                                                //передать состояние игры в json каждому игроку 
            }

        }

        /// <summary>
        /// Xáo trộn ngẫu nhiên người chơi trong phòng và gán ID người chơi
        /// Случайным образом перетасовывать игроков в комнате и назначать идентификаторы игроков.
        /// </summary>
        protected void ShuffleIngamePlayers()
        {
            Player.Player[] tempPlayers = new Player.Player[ingamePlayers.Count];
            int[] indexes = new int[ingamePlayers.Count];
            Random random = new Random();

            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = i;
            for (int i = indexes.Length - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int t = indexes[k];
                indexes[k] = indexes[i];
                indexes[i] = t;
            }

            int counter = 0;
            foreach(Player.Player p in ingamePlayers)
            {
                tempPlayers[indexes[counter]] = p;
                counter++;
            }

            ingamePlayers.Clear();

            for (int i = 0; i < indexes.Length; i++)
            {
                tempPlayers[i].ingameID = i + 1;
                ingamePlayers.AddLast(tempPlayers[i]);
            }
        }

        /// <summary>
        ///Tìm một quân bài có số nhất định trên tay người chơi, nếu không có quân bài nào thì trả về null
        ///Найти карту с определенным номером в руке игрока, если карты нет, вернуть null
        /// </summary>
        protected Card FindCardInPlayerHandcards(int cardID, Player.Player player)
        {
            foreach(Card card in player.handCards)
            {
                if (card.CardId == cardID) return card;
            }
            return null;
        }

        /// <summary>
        /// 4Quá trình xử lý trạng thái số 4, chuyển về trạng thái bình thường ngay sau khi rút thẻ
        /// Обработка состояния №4, переход в нормальное состояние сразу после розыгрыша карты
        /// 
        /// </summary>
        protected void DrawCardBack2Common(int cardCount, Player.Player drawnPlayer, bool toNext = true)
        {
            Card[] drawnCards = cardPile.DrawCards(cardCount);
            drawnPlayer.GainCard(drawnCards); // rút bài рисовать карты

            // Dưới đây, xây dựng json
            //Ниже создайте json
            JsonData json = new JsonData
            {
                ["state"] = 4,
                ["lastCard"] = cardCount,// number of cards drawn
                ["turnID"] = drawnPlayer.ingameID
            };
            string basicJson = json.ToJson(); // json với thông tin tay json с информацией о руке

            json["playerCards"] = new JsonData();
            json["playerCards"].SetJsonType(JsonType.Array);
            foreach (Card card in drawnCards)
            {
                json["playerCards"].Add(card.CardId);
            }
            string personalJson = json.ToJson(); // json với thông tin tay json с информацией о руке
            foreach (Player.Player p in ingamePlayers)
            {    // state 4
                p.SendMessage(p.ingameID == drawnPlayer.ingameID ? personalJson : basicJson);
            }
            // The card drawing is over, return to state No. 1

            if (toNext) Change2NextTurnPlayerNode(); //Skip to next player, lastCard unchanged
            currentStatus = GameStatus.Common;  // Switch to the No. 1 state, attributes such as lastCard remain unchanged

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            }); // Timing begins

            if (currentPlayerNode.Value.isRobot == 1) // If taken over by AI
            {
                InfoQueue.Add(new MsgArgs
                {
                    player = currentPlayerNode.Value,
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                    type = MsgType.RobotResponse
                });
            }

            string patternJson = BuildGamePatternJson().ToJson();
            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(patternJson);  // Send game pattern Json to each player

        }

        /// <summary>
        /// người chơi rút bài
        /// игрок тянет карты
        /// </summary>
        protected void PlayerHandCard(Player.Player sendPlayer, Card responseCard, int colorId)
        {
            if (colorId < 0) colorId = 0;

            gameTimer.Dispose();  // hẹn giờ tắt  Таймер выключения
            if (responseCard.Number == 12) direction *= -1; //
                                                            // TODO  +2/+4 situation, not yet written

            sendPlayer.RemoveCard(responseCard); //người chơi mất thẻ  игрок теряет карту
            cardPile.Move2DiscardPile(responseCard); //thẻ đi đến đống loại bỏ
                                                     //карты отправляются в стопку сброса

            Card tempCard = lastCard;
            int tempinfo = lastCardInfo;

            lastCard = responseCard; //Cập nhật quân bài cần phản hồi thành quân bài đang chơi
            //Обновите карту, на которую нужно ответить, на текущую сыгранную карту

            lastCardInfo = 4 * sendPlayer.ingameID +
                (responseCard.Color == Card.CardColor.Invalid ? colorId : 0); // định dạng giao thức формат протокола
            Change2NextTurnPlayerNode(responseCard.Number == 10); // Cập nhật trình phát hiện tại thành trình phát sẽ được vận hành
                                                                  // Обновите текущий проигрыватель как управляемый проигрыватель. 

            if (sendPlayer.handCards.Count <= 0)
                throw new PlayerFinishException(sendPlayer);

            queryID++; // Yêu cầu truy vấn của khách hàng sắp được bắt đầu
                       // Запрос запроса клиента собирается быть инициирован
            currentStatus = GameStatus.Common; 

            if (responseCard.Number == 11) // +2 карты
            {
                currentStatus = GameStatus.Plus2Loop; // +2 карты начинают операцию укладки
                drawingCardCounter += 2; // ngăn xếp truy cập   встречный стек
            }
            else if(responseCard.CardId >= 104)
            {
                plus4ResponseCard = tempCard;// Cập nhật thẻ phản hồi +4
                                             // Обновить +4 подсказки с ответами
                plus4Player = sendPlayer; // Update the player who hit +4
                plus4ColorID = tempinfo;
                currentStatus = GameStatus.Plus4Loop; // +4 cards, ask questions
            }

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            });  // Timing begins

            if (currentPlayerNode.Value.isRobot == 1) // If taken over by AI
            {
                InfoQueue.Add(new MsgArgs
                {
                    player = currentPlayerNode.Value,
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                    type = MsgType.RobotResponse
                });
            }

            string json = BuildGamePatternJson().ToJson();
            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(json); // Send game pattern Json to each player
        }


        protected GameCardPile cardPile;
        protected LinkedListNode<Player.Player> currentPlayerNode;
        protected Card lastCard, gainCard, plus4ResponseCard;
        protected int lastCardInfo, plus4ColorID;
        protected int direction;
        protected Timer gameTimer;
        protected long lastTimerStartMs;
        protected int lastTimerWaitSec;
        protected int drawingCardCounter;
        protected Player.Player plus4Player;

        protected int queryID;  // request number

        /// <summary>
        /// start timer
        /// </summary>
        protected void TimerStart(MsgArgs msg, int waitingTime = NormalWaitingTime)
        {
            // gameTimer?.Dispose();

            gameTimer = new Timer((state) =>
            {
                InfoQueue.Add(msg);
            }, msg, waitingTime + 4000, Timeout.Infinite);
            lastTimerStartMs = DateTime.Now.Ticks;
            lastTimerWaitSec = waitingTime;
        }

        /// <summary>
        /// Construct game pattern json
        /// </summary>
        protected JsonData BuildGamePatternJson(Player.Player player = null)
        {
            JsonData json = new JsonData
            {
                ["state"] = (int)currentStatus,
                ["queryID"] = queryID
            };

            switch (currentStatus)
            {
                case GameStatus.Common:
                case GameStatus.Plus4Loop:
                    json["lastCard"] = lastCard == null ? -1 : lastCard.CardId;
                    json["intInfo"] = lastCardInfo;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;
                case GameStatus.QueryPlayer:
                    json["lastCard"] = (player.ingameID==currentPlayerNode.Value.ingameID) ? gainCard.CardId : -1;
                    json["intInfo"] = cardPile.CardPileLeft;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;
                case GameStatus.Plus2Loop:
                    json["lastCard"] = lastCard.CardId;
                    json["intInfo"] = drawingCardCounter;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["playerID"] = lastCardInfo >> 2;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;

            }

            return json;
        }

        protected JsonData BuildGameStateJson(Player.Player queryPlayer)
        {
            JsonData json = new JsonData {
                ["cardpileLeft"] = cardPile.CardPileLeft,
                ["direction"] = direction,
                ["turnInfo"] = BuildGamePatternJson(queryPlayer),
                ["yourID"] = queryPlayer.ingameID
            };

            json["playerMap"] = new JsonData();
            json["playerMap"].SetJsonType(JsonType.Array);
            foreach(Player.Player p in ingamePlayers)
            {
                json["playerMap"].Add(p.BuildPlayerMapJson(queryPlayer.ingameID == p.ingameID));
            }

            return json;
        }

        /// <summary>
        /// next player
        /// </summary>
        /// <param name="skip">Whether to skip the next player</param>
        protected void Change2NextTurnPlayerNode(bool skip = false)
        {
            if(direction == 1) 
            {
                currentPlayerNode = currentPlayerNode.Next;
                if (currentPlayerNode == null) currentPlayerNode = ingamePlayers.First;
                if (skip) Change2NextTurnPlayerNode(false); ///play ban, skip a player
            }
            else //ngược chiều kim đồng hồ ///  реверс по часовой стрелке
            {
                currentPlayerNode = currentPlayerNode.Previous;
                if (currentPlayerNode == null) currentPlayerNode = ingamePlayers.Last;
                if (skip) Change2NextTurnPlayerNode(false); // Played ban, skipping a player
            }
        }

    }
}
