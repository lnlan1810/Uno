using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using MultiplayerUNO.UI.Players;
using static MultiplayerUNO.Utils.Card;
using MultiplayerUNO.Utils;
using System.Threading;

namespace MultiplayerUNO.UI.BUtils {
    /// <summary>
    /// Some parameters are used to control the game (global variables)
    /// </summary>
    public static class GameControl {
        /// <summary>
        /// Quickly playing the reverse card may cause the previous animation to start before the next animation ends
        /// Build a waiting list through this variable (built through await)
        /// </summary>
        public static volatile Task LastLblDirectionTask = null;
        public static volatile CancellationTokenSource LastLblDirectionTaskCancleToken;

        /// <summary>
        /// true: The event of choosing a color is triggered by drawing a card after drawing a card
        /// false: The event of choosing a color is triggered by the normal card playing
        /// </summary>
        public static volatile bool ChooseColorIsTriggerAfterGetOneCard = false;

        /// <summary>
        ///When the card played is a +4/wild card, you need to choose a color
        /// </summary>
        public static volatile CardColor InvalidCardToChooseColor = CardColor.Invalid;

        /// <summary>
        /// Logo licensing animation (can wait)
        /// </summary>
        public static volatile Task FinishDistributeCard = null;

        /// <summary>
        ///Mapping from player to player code on UI
        /// </summary>
        public static volatile Dictionary<int, int> PlayerId2PlayerIndex;

        /// <summary>
        /// Indicates whether the game is initialized
        /// </summary>
        public static volatile bool GameInitialized = false;

        /// <summary>
        /// play clockwise
        /// </summary>
        public static volatile bool DirectionIsClockwise;
        public static void SetGameDirection(int direction) {
            //Direction code:
            // 1 (counterclockwise, number ascending), -1 (clockwise, number descending)
            DirectionIsClockwise = (direction == -1);
        }

        /// <summary>
        /// Round Game Parameters
        /// </summary>
        public static volatile int QueryID, LastCardID, TurnID, TimeForYou = -1;
        public static volatile CardColor LastColor = CardColor.Invalid;
        public static volatile Card LastCard = null;
        /// <summary>
        /// Did the last person play cards
        /// </summary>
        public static volatile bool CardChange;

        /// <summary>
        /// currently selected card
        /// </summary>
        public static volatile CardButton CBtnSelected = null;

        /// <summary>
        /// Discard pile, guaranteed to have only one discarded card (thread-safe)
        /// </summary>
        public static volatile ArrayList CardsDropped;

        /// <summary>
        ///Note that only when the animation ends can it be added to the discard pile
        /// </summary>
        public static void AddDroppedCard(CardButton cbtn) {
            CardsDropped.Add(cbtn);
            while (CardsDropped.Count > 1) {
                MsgAgency.MainForm.Controls.Remove((CardButton)CardsDropped[0]);
                CardsDropped.RemoveAt(0);
            }
        }

        /// <summary>
        /// I'm the first to play
        /// </summary>
        public static bool FirstTurnFirstShow() {
            return FirstTurn() && TurnID == MsgAgency.MainForm.MyID;
        }

        /// <summary>
        ///first play
        /// </summary>
        public static bool FirstTurn() {
            return LastCardID == -1;
        }
    }
}