using MultiplayerUNO.UI.BUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.Players {
    public class Player {
        public string Name {
            get;
        }

        /// <summary>
        /// hand
        /// </summary>
        public List<int> CardsOrigin = null;  // We don't know other people's cards, set it to null
                                              // Only used during initialization, use BtnsInHand afterwards

        public ArrayList BtnsInHand;        // We don't know other people's cards, only one back card
                                            // Count=1, not null

        /// <summary>
        /// Used to display some information: user nickname + number of remaining cards
        /// </summary>
        public Label LblInfo;

        /// <summary>
        /// A center of the deck as a whole
        /// </summary>
        public Point Center;
        public float PosX, PosY;

        /// <summary>
        /// how the cards are arranged
        /// true : arrange up and down,
        /// false: Arrange left and right
        /// </summary>
        public bool IsUpDown;

        /// game parameters
        public readonly int PlayerID;
        public volatile int CardsCount;
        public volatile bool IsRobot;

        /// <summary>
        /// posX, posY refers to the index between [-1,1]*[-1,1]
        /// </summary>
        public Player(MainForm form, string name, bool isUpDown, float posX, float posY,
                int playerID, int cardsCount, bool isRobot, bool isMe) {
            Name = name;
            IsUpDown = isUpDown;
            if (isMe) {
                // Only used during initialization, no concurrency issues
                CardsOrigin = new List<int>();
            }
            // There are concurrency issues, both shuffling and UI will use this array
            BtnsInHand = ArrayList.Synchronized(new ArrayList());
            PosX = posX;
            PosY = posY;
            // center calculate
            int x = (int)((1f + posX * RATE) / 2 * form.REF_WIDTH);
            int y = (int)((1f + posY * RATE) / 2 * form.REF_HEIGHT);
            Center = new Point(x, y);
            PlayerID = playerID;
            CardsCount = cardsCount;
            IsRobot = isRobot;
        }

        /// <summary>
        /// Requires caller to use UIInvoke
        /// </summary>
        public void UpdateInfo() {
            int x = LblInfo.Location.X + LblInfo.Width / 2;
            LblInfo.Text =
                (IsRobot ? "[AI] " : "")
                + Name
                + " (" + CardsCount + ")";
                //+ " (" + CardsCount.ToString().PadLeft(2, '0') + ")"; // 观感不佳
            LblInfo.Location = new Point(
                x - LblInfo.Width / 2, LblInfo.Location.Y);

            // Shout out to UNO!!!
            if (CardsCount == 1) {
                MsgAgency.MainForm.ShowMsgToUser(Name + ": UNO!!!");
            }
        }

        /// <summary>
        /// Play a card, if it is the last one return true
        /// </summary>
        public bool ShowOneCard() {
            return --CardsCount == 0;
        }

        #region 一some UI constants

        // zoom size
        public const float RATE = 0.66f;

        #endregion 一些 UI 的常数

        #region  position encoding
        // |    4 3 2    |    3 2    |     2     |           |   1   |  //
        // |  5       1  |  4     1  |  3     1  |  2     1  |       |  //
        // |      0      |     0     |     0     |     0     |   0   |  //

        /// <summary>
        /// <summary>
        /// The way the cards are placed, true means they are placed up and down
        /// Binary code, 0(false), 1(true)
        /// Purpose:
        /// 1. The layout of the cards is not used now
        /// (For other people's cards, now only use a back card and a number representation)
        /// 2. The position of the name
        /// </summary>
        public static int[] isUpDownMap_CODE = new int[] {
            0, 0,
            0b00, 0b110, 0b1010, 0b10010, 0b100010
        };

        // Central location,
         // encoding method [-1,1]*[-1,1],
         // upper left corner [-1, -1], lower right corner [1, 1]
        public static float[][] posX_CODE = new float[][] {
            null, null,
            new float[2]{ 0,  0},
            new float[3]{ 0,  1f,   -1f},
            new float[4]{ 0,  1f,     0,   -1f},
            new float[5]{ 0,  1f,  0.5f, -0.5f,   -1f},
            new float[6]{ 0,  1f,  0.6f,     0, -0.6f, -1f}
        };
        public static float[][] posY_CODE = new float[][] {
            null, null,
            new float[2]{1f, -1f},
            new float[3]{1f,   0,  0},
            new float[4]{1f,   0, -1f,  0},
            new float[5]{1f,   0, -1f, -1f, 0},
            new float[6]{1f,   0, -1f, -1f, -1f, 0},
        };
        #endregion position encoding
    }
}
