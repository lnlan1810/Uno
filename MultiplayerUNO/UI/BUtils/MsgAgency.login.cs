using LitJson;
using MultiplayerUNO.UI.Login;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.BUtils {
    public static partial class MsgAgency {
        /// <summary>
        /// login window
        /// </summary>
        public static LoginForm LoginForm = null;

        /// <summary>
        /// This field is true to indicate case (1), used in InitState().
        /// (1) When entering the room, the server will send two messages to the client {type=0,type=3};
        /// (2) When actively requesting room information, the server will only send a message {type=0};
        /// </summary>
        public static bool WhenFirstEnterTheRoom = true;

        private static void DealWithMsgBeforeGameStart(JsonData json) {
            //Console.WriteLine("UI: " + json.ToJson());
            int type = (int)json["type"];
            switch (type) {
                case 0: // It will only be received when you first enter, the initial waiting situation
                    InitState(json);
                    break;
                case 1: // someone started preparing
                case 2: // someone left the room
                case 3: // someone joined the room
                case 4: // Someone canceled the preparation and turned it into a wait
                    UpdatePlayerState(json);
                    break;
                case 5: // The game starts (nothing happens, anyway, as long as the initialization JSON is received, it means the start is successful)
                    break;
                case 6: //game start failed
                        // At this time, it may be caused by the inconsistency between the front-end information and the back-end,
                        // Need to resend a message to the backend to confirm the room information and update the interface
                    SendMsgToQueryRoomStateWhenLogin();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Encapsulates complex BeginInvoke (asynchronous)
        /// </summary>
        private static void LoginMainUIInvoke(Action fun) {
            if (LoginForm.InvokeRequired) {
                LoginForm.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        /// <summary>
        /// update status
        /// Someone canceled the preparation (4)
        /// Someone joined the room (3)
        /// Someone left the room (2)
        /// Someone started preparing (1)
        /// </summary>
        private static void UpdatePlayerState(JsonData json) {
            int type = (int)json["type"];
            int seatID = (int)json["player"]["seatID"];
            PlayerState ps = PlayerState.LEFT;
            if (type == 1) {
                ps = PlayerState.READY;
            } else if (type == 2) {
                ps = PlayerState.LEFT;
            } else if (type == 3 || type == 4) {
                ps = PlayerState.WAIT;
            }
            LoginForm.SetPlayerState(seatID, ps);
        }

        /// <summary>
        /// Initialize the waiting situation
        /// </summary>
        private static void InitState(JsonData json) {
  
            LoginForm.ResetPlayerState();
            var players = json["player"];
            int num = players.Count;
            for (int i = 0; i < num; ++i) {
                var p = players[i];
                PlayerState ps =
                    (bool)(p["isReady"]) ? PlayerState.READY : PlayerState.WAIT;
                LoginForm.SetPlayerState((int)p["seatID"], ps);
            }

            if (WhenFirstEnterTheRoom) {
                WhenFirstEnterTheRoom = false;
            } else { return; }

            string msg = TakeOneMsg();
            if (msg == null) {
                MessageBox.Show("Failed to receive a type=3 message immediately during initialization");
                return;
            }
            JsonData js2 = JsonMapper.ToObject(msg);
            if (!js2.Keys.Contains("type") || ((int)js2["type"]) != 3) {
                MessageBox.Show("Failed to receive a type=3 message immediately during initialization\n" + js2.ToJson());
                return;
            }
            LoginForm.SeatID = (int)js2["player"]["seatID"];
        }

        /// <summary>
        /// Send a message to the server confirming the waiting state in the room
        /// </summary>
        public static void SendMsgToQueryRoomStateWhenLogin() {
            JsonData json = new JsonData() { ["type"] = 0 };
            SendOneJsonDataMsg(json);
        }
    }
}