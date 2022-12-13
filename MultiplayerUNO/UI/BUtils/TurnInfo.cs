using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiplayerUNO.Utils.Card;

namespace MultiplayerUNO.UI.BUtils {
    public class TurnInfo {
       
        public readonly int State, LastCardID, IntInfo, Time, TurnID, QueryID;
        public readonly int[] PlayerCards;
        public readonly JsonData JsonMsg;

        /// <summary> 
        /// Some meaningful values in GameControl may be assigned inside the constructor
        /// </summary>
        public TurnInfo(JsonData json) {
            // Check the correctness of parsing turninfo (OK)
            JsonMsg = json;
            State = (int)json["state"];
            // state=7 The news of the draw is rather strange, so it will not be parsed directly, and it will be parsed outside
            if (State == 7 && (int)(json["turnID"]) == 0) { return; }

            LastCardID = (int)json["lastCard"];
            if (State == 1 || State == 3 || State == 5 || (State == 7 && LastCardID != -1)) {
                GameControl.CardChange = (GameControl.LastCardID != LastCardID);
                GameControl.LastCardID = LastCardID;
                GameControl.LastCard = new Utils.Card(LastCardID);
            }

            // ATT State=4 no intInfo
            if (!(State == 6 || State == 4 || (State == 1 && LastCardID == -1))) {
                IntInfo = (int)json["intInfo"];
            }

            // ATT State=4 no time
            if (State >= 1 && State <= 5 && State != 4) {
                Time = (int)json["time"];
                GameControl.TimeForYou = Time;
            }

            TurnID = (int)json["turnID"];
            GameControl.TurnID = TurnID;

            if ((State == 4 && TurnID == MsgAgency.MainForm.MyID)
                || State == 6) {
                var jsa = json["playerCards"];
                PlayerCards = new int[jsa.Count];
                for (int i = 0; i < jsa.Count; ++i) {
                    PlayerCards[i] = (int)jsa[i];
                }
            } else if (State == 7) {
                // PlayerMap dealt with outside
            }

            if (State == 1 || State == 2 || State == 3 || State == 5) {
                QueryID = (int)json["queryID"];
                GameControl.QueryID = QueryID;
            }

            if (State == 1 || State == 5 || State == 7) {
                if (LastCardID == -1) {
                    // At the beginning of the game, the last card is -1
                    // May be -1 when State=7 (tie, this is handled outside)
                    GameControl.LastColor = CardColor.Invalid;
                } else if (GameControl.LastCard.Color == CardColor.Invalid) {
                    GameControl.LastColor = (CardColor)(IntInfo & 0b11);
                } else {
                    GameControl.LastColor = GameControl.LastCard.Color;
                }
            }
        }

        ////////////////////////////////////////////////
        // Note that the call of the following functions requires the caller
        // to ensure the existence of the property 
        // otherwise there may be strange bugs or strange results
        ////////////////////////////////////////////////

        /// <summary>
        /// Get playerID through intInfo
        /// (state = 1,7)
        /// </summary>
        public int GetPlayerID() {
            return IntInfo >> 2;
        }

        /// <summary>
        /// Call the GetPlayerID() implementation, the usage conditions are limited by it
        /// (state = 1,7)
        /// </summary>
        /// <returns></returns>
        public int GetPlayerIndex() {
            return GameControl.PlayerId2PlayerIndex[GetPlayerID()];
        }
    }
}