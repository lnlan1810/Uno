using LitJson;
using MultiplayerUNO.UI.Login;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.BUtils {
    /// <summary>
    /// front-end communication
    /// </summary>
    public static partial class MsgAgency {

        /// <summary>
        ///For some tasks after receiving this message, you need to wait for these tasks to end before processing the next message.
        /// only handles after starting the game
        /// </summary>
        public static ConcurrentQueue<Task> TaskQueue = new ConcurrentQueue<Task>();

        /// <summary>
        /// The MainForm main window where the game runs, set in the constructor, and set to null when it is closed
        /// </summary>
        public static volatile MainForm MainForm = null;

        /// <summary>
        /// Initialize parameters, open a new interface
        /// </summary>
        private static void InitializeGame(JsonData json) {
            GC.Collect();
            // Parsing Json to construct MainForm
            MainForm mainForm = new MainForm(json);

            // close the window
            LoginMainUIInvoke(() => {
                LoginForm.Hide();
                mainForm.Show();
            });
        }

        /// <summary>
        /// Process state information (messages sent from the server after the game starts)
        /// </summary>
        private static void DealWithMsgAfterGameStart(JsonData json) {
            if (MainForm == null) { return; }

            // DEBUG START
            //MainForm.UIInvokeSync(() => {
            //    MainForm.DebugLog("MyID: " + MainForm.MyID + "\r\n" + json.ToJson());
            //});
            // DEBUG END

            int state = (int)json["state"];
            if (state == -1) {
                ToBeAI((int)json["playerID"]);
                return;
            }
            TurnInfo turnInfo = new TurnInfo(json);
            switch (state) {
                case 1: // someone played a card
                    SomeBodyShowCard(turnInfo);
                    break;
                case 2: // Someone draws a card (may also be able to play this card)
                    GetACard(turnInfo);
                    break;
                case 3: // +2 cumulative, need to have card playing animation
                    ResponedToPlus2(turnInfo);
                    break;
                case 4: // Someone draws a number of cards (it ends after drawing the cards)
                    SomebodyGetSomeCards(turnInfo);
                    break;
                case 5:// Response +4
                    ResponedToPlus4(turnInfo);
                    break;
                case 6: // TODO Question result display (will not receive this message now, and have not tested it)
                    ShowCardAfterPlus4(turnInfo);
                    break;
                case 7: // Game over, show all hands
                    GameOver(turnInfo);
                    break;
                default: break;
            }
            // Sleep for a while after processing the message, used to deal with AI battles
            Thread.Sleep(1000);
            // Wait for all animations to end before processing the next message
            // In order to avoid the deadlock problem caused by async/await, sleep is used here
            while (!TaskQueue.IsEmpty) {
                Task task;
                TaskQueue.TryDequeue(out task);
                while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted) {
                    Thread.Sleep(500);
                }
            }
            return;
        }

        private static void ResponedToPlus2(TurnInfo turnInfo) {
            // Everyone will receive a +2 message, only the next one needs to reply
            // animations for everyone (depending on the new protocol)
            // 1. If a state=1 is sent immediately afterwards, there will be no animation here, and the +2 lastCard will not affect the global update
            // 2. If you directly add a playerId, there will be animation here, and the +2 lastCard will affect the global update (status quo)
            // Solved: use method 2
            int playerID = (int)turnInfo.JsonMsg["playerID"];
            MainForm.ShowCard(GameControl.PlayerId2PlayerIndex[playerID], turnInfo.LastCardID);
            if (turnInfo.TurnID != MainForm.MyID) { return; }
            MainForm.ShowOrGetAfterPlus2(turnInfo);
        }

        /// <summary>
        /// A player goes offline, the AI takes over
        /// </summary>
        private static void ToBeAI(int playerID) {
            int playerIdx = GameControl.PlayerId2PlayerIndex[playerID];
            MainForm.Players[playerIdx].IsRobot = true;
            MainFormUIInvoke(() => {
                MainForm.Players[playerIdx].UpdateInfo();
            });
        }

        /// <summary>
        /// game over
        /// 1. Card playing animation
        /// 2. Show who wins
        /// 3. Show hand cards
        /// </summary>
        private static void GameOver(TurnInfo turnInfo) {
            MainForm.GameOver(turnInfo);
        }

        /// <summary>
        /// Question result display, only need to show the hand card
        /// </summary>
        private static void ShowCardAfterPlus4(TurnInfo turnInfo) {
            MainForm.DisplayOnesCard(turnInfo);
        }

        /// <summary>
        ///In response to +4, make the panel with the two buttons in question visible
        /// </summary>
        private static void ResponedToPlus4(TurnInfo turnInfo) {
            // Show the card animation first
            MainForm.ShowCard(turnInfo.GetPlayerIndex(), turnInfo.LastCardID);
            // Everyone will receive a +4 message, only the next player can question it
            if (turnInfo.TurnID != MainForm.MyID) { return; }
            MainForm.RespondToPlus4(turnInfo);
        }

        /// <summary>
        /// Someone draws several cards, then ends
        /// </summary>
        private static void SomebodyGetSomeCards(TurnInfo turnInfo) {
            MainForm.GetManyCards(turnInfo);
        }

        /// <summary>
        /// draw a card
        /// </summary>
        private static void GetACard(TurnInfo turnInfo) {
            // Vẽ không phản hồi Làm thế nào để đạt được nó, nó có nghĩa là gì?
            // Không xử lý, trạng thái = 7 sẽ được gửi sau khi hòa, ngay lập tức vào tình huống giải quyết trò chơi

            // những người khác rút thẻ
            if (turnInfo.TurnID != MainForm.MyID) {
                MainForm.GetCardForOtherOne(turnInfo);
                return;
            }
            // Draw cards by yourself, are you ready to play the cards you caught?
            MainForm.GetACardForMe(turnInfo);
        }

        /// <summary>
        ///someone played a card
        /// 1. Game animation
        /// 2. Other states (press the button to enable = true)
        /// (1) If it is the card played by yourself
        /// (2) If the card is played by someone else, and the next player is not your own, end the response
        /// (3) If the card is played by someone else, and the next player is yourself, prepare to play the card
        /// </summary>
        private static void SomeBodyShowCard(TurnInfo turnInfo) {
            // 1
            if (GameControl.CardChange) {
                //Not necessarily, maybe the last person did not play a card (judged by GameControl.CardChange)
                MainForm.ShowCard(turnInfo.GetPlayerIndex(), turnInfo.LastCardID);
            }
            // 2
            // (1), (2), (3)
            MainFormUIInvoke(() => {
                MainForm.SetCardButtonEnable(true);
            });
            // (3)
            if (turnInfo.TurnID == MainForm.MyID) {
                MainForm.ShowOrGetNormal(turnInfo);
            }
        }

        /// <summary>
        /// Encapsulates complex BeginInvoke (asynchronous)
        /// </summary>
        private static void MainFormUIInvoke(Action fun) {
            if (MainForm.InvokeRequired) {
                MainForm.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }
    }
}
