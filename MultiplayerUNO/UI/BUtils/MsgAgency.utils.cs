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
        /// protocol version number
        /// </summary>
        public const string ProtocolVersion = "0.0.1";

        /// <summary>
        /// The adapter to communicate with the backend (will only be set once)
        /// </summary>
        public static PlayerAdapter PlayerAdapter = null;

        /// <summary>
        /// A thread dedicated to receiving server messages (will only be set once)
        /// </summary>
        public static Thread ShowInfoThread = null;

        /// <summary>
        /// Whether to maintain a connection with the server
        /// </summary>
        public static volatile bool LostConnectionWithServer = true;

        /// <summary>
        /// Process messages, note that the message processing here should be sequential
        /// </summary>
        public static void WaitForMsgFromBackend() {
            while (true) {
                string msg = TakeOneMsg();
                if (msg == null) { break; }
                DealWithMsg(msg);
            }
        }

        /// <summary>
        /// get a message
        /// </summary>
        private static string TakeOneMsg() {
            string msg = null;
            try {
                msg = PlayerAdapter.RecvQueue.Take();
            } catch (InvalidOperationException e) {
                Console.WriteLine("[UI]: " + e.Message);
                MessageBox.Show(
                    "You seem to have lost the network connection to the server! It is recommended to restart the program!\n"
                     + "(Cannot receive message from server)"
                );
            }
            return msg;
        }

        /// <summary>
        ///Send a JSON message to the server, in this function will catch the disconnection exception
        /// </summary>
        public static void SendOneJsonDataMsg(JsonData json) {
            try {
                PlayerAdapter.SendMsg2Server(json.ToJson());
            } catch (InvalidOperationException e) {
                Console.WriteLine("[UI]: " + e.Message);
                MessageBox.Show(
                  "You seem to have lost the network connection to the server! It is recommended to restart the program!\n"
                     + "(The server could not respond to your request)"
                );
            }
        }

        /// <summary>
        /// process messages
        /// </summary>
        private static void DealWithMsg(string msg) {
            // TODO (UI DEUBG) Output the obtained JSON information
            //Console.WriteLine("[UI]: " + msg);
            JsonData json = JsonMapper.ToObject(msg);
            if (json.Keys.Contains("state")) {
                // information after the game starts
                DealWithMsgAfterGameStart(json);
            } else if (json.Keys.Contains("type")) {
                // Information before the game starts
                DealWithMsgBeforeGameStart(json);
            } else if (json.Keys.Contains("cardpileLeft")) {
                // Information at the moment the game starts (only sent once)
                InitializeGame(json);
            } else {
                MessageBox.Show("Unrecognized json, does not contain type/state!\n" + msg);
            }
        }
    }
}
