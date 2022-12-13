using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiplayerUNO.Utils.Card;

namespace MultiplayerUNO.UI {
    public partial class MainForm : Form {
        /// <summary>
        /// json text for initialization
        /// </summary>
        private readonly JsonData InitJsonMsg;

        public MainForm(JsonData jsonMsg) {
            InitializeGlobalControl(); // must be in the first sentence
            // MyID is readonly
            MyID = (int)jsonMsg["yourID"];
            InitJsonMsg = jsonMsg;
            InitializeComponent();
        }

        /// <summary>
        /// Set some global variables, mainly in MsgAgency and GameControl
        /// </summary>
        private void InitializeGlobalControl() {
            MsgAgency.MainForm = this;
            GameControl.PlayerId2PlayerIndex = new Dictionary<int, int>();
            GameControl.GameInitialized = false;
            GameControl.CBtnSelected = null;
            GameControl.LastColor = CardColor.Invalid;
            GameControl.CardsDropped = ArrayList.Synchronized(new ArrayList());
        }

        /// <summary>
        /// The card deal animation during initialization,
        /// will only be called during initialization,
        /// Dealing method for each person once
        /// </summary>
        private async Task DistributeCardAtGameStart_OnePersonOneTimeAsync() {
            // 1. 4 cards are dealt on the face
            AnimationSeq animaSeq = new AnimationSeq();
            // Post by yourself last, convenient to implement
            List<CardButton> add2FormControl = new List<CardButton>();
            int xSrc = Piles[PileToDistribute].Location.X,
                ySrc = Piles[PileToDistribute].Location.Y;
            for (int i = 0; i < PlayersNumber; ++i) {
                // How many players
                int p = (i + 1) % PlayersNumber;
                var player = Players[p];
                // We don't know other people's cards
                int cardNumber = (p == ME)
                    ? player.CardsOrigin[player.CardsOrigin.Count - 1]
                    : CardButton.BACK;
                var btn = new CardButton(
                    cardNumber,
                    // back
                    true,
                    // own cards
                    p == ME
                );
                btn.Location = Piles[PileToDistribute].Location;
                player.BtnsInHand.Add(btn);
                add2FormControl.Add(btn);
                Animation anima = new Animation(this, btn);
                anima.SetTranslate(
                    player.Center.X - CardButton.WIDTH_MODIFIED / 2 - xSrc,
                    player.Center.Y - CardButton.HEIGHT_MODIFIED / 2 - ySrc
                );
                if (p == ME) {
                    anima.SetRotate();
                }
                animaSeq.AddAnimation(anima);
            }
            // You should wait until the addition is complete before you can start the animation
            UIInvokeSync(() => {
                this.Controls.AddRange(add2FormControl.ToArray());
                for (int i = add2FormControl.Count - 1; i >= 0; --i) {
                    add2FormControl[i].BringToFront();
                }
            });
            await animaSeq.Run(); // Synchronize
            //number of cards
            UIInvokeSync(() => {
                foreach (var i in Players) {
                    i.LblInfo.Text = i.Name + " (" + i.CardsCount + ")";
                }
            });

            // 2. Add the remaining cards after dealing (only your own cards)
            add2FormControl.Clear();
            var playerME = Players[ME];
            int dstX = playerME.Center.X - CardButton.WIDTH_MODIFIED / 2,
                dstY = playerME.Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            // Add remaining cards
            for (int j = playerME.CardsOrigin.Count - 2; j >= 0; --j) {
                var btn = new CardButton(
                    playerME.CardsOrigin[j],
                    // The other person's hand is on the back
                    false,
                    // own cards
                    true
                );
                btn.Location = new Point(dstX, dstY);
                add2FormControl.Add(btn);
                playerME.BtnsInHand.Add(btn);
            }
            UIInvokeSync(() => {
                foreach (var btn in add2FormControl) {
                    this.Controls.Add(btn);
                    btn.SendToBack();
                }
            });

            // 3. unfold your cards
            await ReorganizeMyCardsAsync();

            // 4. Need to randomly initialize a card? No

            // 5. Games start
            GameControl.GameInitialized = true;
        }

        /// <summary>
        /// Initialize Players
        /// </summary>
        private void InitializePlayers(JsonData jsonMsg) {
            int cardPileLeft = (int)jsonMsg["cardpileLeft"];
            int direction = (int)jsonMsg["direction"];
            GameControl.SetGameDirection(direction);

            TurnInfo turnInfo = new TurnInfo(jsonMsg["turnInfo"]);

            JsonData playerMap = jsonMsg["playerMap"];
            PlayersNumber = playerMap.Count;

            // Find yourself the first few
            int idxME;
            for (idxME = 0; idxME < PlayersNumber; ++idxME) {
                if ((int)playerMap[idxME]["playerID"] == MyID) { break; }
            }

            // Construct Players
            Players = new Player[PlayersNumber];
            var posX = Player.posX_CODE[PlayersNumber];
            var posY = Player.posY_CODE[PlayersNumber];
            var isUpDown = Player.isUpDownMap_CODE[PlayersNumber];
            for (int i = 0; i < PlayersNumber; ++i) {
                JsonData p = playerMap[i];
                int idx = (i + PlayersNumber - idxME) % PlayersNumber;
                // {"name":"127.0.0.1","playerID":1,"cardsCount":7,"isRobot":0}
                int playerID = (int)p["playerID"];
                GameControl.PlayerId2PlayerIndex[playerID] = idx;
                Players[idx] = new Player(
                    this,
                    (string)p["name"],
                    (isUpDown & (1 << idx)) != 0,
                    posX[idx], posY[idx],
                    playerID,
                    (int)p["cardsCount"],
                    (int)p["isRobot"] == 1,
                    idx == ME
                );
            }

            // Build your deck
            JsonData piles = playerMap[idxME]["handcards"];
            for (int i = 0; i < piles.Count; ++i) {
                Players[ME].CardsOrigin.Add((int)piles[i]);
            }
        }

        /// <summary>
        ///Constructs a panel for selecting colors
        /// </summary>
        private void ConstructPnlChooseColor() {
            //Add 4 labels inside the panel of the selected color, referring to the color
            const int size = SIGN_LABLE_SIZE;
            const int padding = SIGN_LABLE_PDDING;
            const int totalSize = padding * 2 + size;
            this.PnlChooseColor.Size = new Size(totalSize * 4, totalSize);

            Bitmap[] img4 = new Bitmap[4] {
                UIImage._oButBlue, UIImage._oButGreen,
                UIImage._oButRed, UIImage._oButYellow
            };
            CardColor[] color4 = new CardColor[4] {
                CardColor.Blue, CardColor.Green,
                CardColor.Red, CardColor.Yellow
            };

            for (int i = 0; i < 4; ++i) {
                Control lbl = new Label();
                lbl.AutoSize = false;
                lbl.Size = new Size(size, size);
                this.PnlChooseColor.Controls.Add(lbl);
                lbl.Location = new Point(padding + totalSize * i, padding);
                lbl.BackgroundImage = img4[i];
                lbl.BackgroundImageLayout = ImageLayout.Stretch;
                lbl.Tag = color4[i];// Use Tag to save the color corresponding to the button
                lbl.Click += (sender, e) => {
                    // set up GameControl.InvalidCardToChooseColor
                    GameControl.InvalidCardToChooseColor =
                        (CardColor)(((Label)sender).Tag);
                    this.PnlChooseColor.Visible = false;
                    SendShowCardJson(
                        GameControl.ChooseColorIsTriggerAfterGetOneCard);
                };
            }
        }

        /// <summary>
        /// Actions on window load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e) {
            ScreenDisplaySetting();
            InitializeAllPlayers();
            GameControl.FinishDistributeCard =
                DistributeCardAtGameStart_OnePersonOneTimeAsync();

            // After waiting for the card deal animation to finish completely, set some components
            Task.Run(async () => {
                await GameControl.FinishDistributeCard;
                SettingComponentsAfterDistributeAnima();
            });
        }

        /// <summary>
        /// Draw player position and related information
        /// </summary>
        private void DrawPlayers() {
          
            for (int i = 0; i < PlayersNumber; ++i) {
                var player = Players[i];

                var lbl = new Label();
                lbl.Visible = false;
                this.Controls.Add(lbl); // Note that Autosize will only take effect after Add

                lbl.ForeColor = Color.White;
                lbl.Font = new Font("Arial", 15F, FontStyle.Regular,
                                    GraphicsUnit.Point, ((byte)(134)));
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = player.Name + " (0)";
                lbl.AutoSize = true;
                int xOff = -lbl.Width / 2,
                    yOff = -lbl.Height / 2;
               
                if (player.IsUpDown) {
                    yOff += (int)(
                        (CardButton.HEIGHT_MODIFIED / 2
                            + lbl.Height / 2 + OFFSET_FOR_LBLINFO));
                } else {
                    yOff += (int)(
                        (CardButton.HEIGHT_MODIFIED / 2
                            + lbl.Height / 2 + OFFSET_FOR_LBLINFO)
                        * (player.PosY < 0 ? -1 : 1));
                }
                lbl.Location = new Point(
                    player.Center.X + xOff,
                    player.Center.Y + yOff
                );
                player.LblInfo = lbl;
                lbl.Visible = true;
                lbl.SendToBack(); // put it at the bottom
            }
        }

        /// <summary>
        /// Draw the pile position
        /// </summary>
        private void DrawPiles() {
            Piles = new CardButton[2];
            int x = this.REF_WIDTH / 2,
                y = this.REF_HEIGHT / 2;
            for (int i = 0; i < PILES_NUM; ++i) {
                var btn = new CardButton(CardButton.BACK);
                float xOff = (i == PileToDistribute ? -1 : 1)
                    * PILE_OFFSET_RATE * x
                    - btn.Width / 2;
                float yOff = -btn.Height / 2;
                btn.Location = new Point(x + (int)xOff, y + (int)yOff);
                Piles[i] = btn;
                this.Controls.Add(btn);
            }
        }

        /// <summary>
        /// Set the initial properties of some components after the initial card deal animation ends
        /// </summary>
        private void SettingComponentsAfterDistributeAnima() {
            var lbldir = this.LblDirection as Control;
            var lblcolor = this.LblColor as Control;
            UIInvoke(() => {
                // direction
                lbldir.BackgroundImage = GameControl.DirectionIsClockwise
                    ? UIImage.clockwise : UIImage.counterclockwise;
                lbldir.BackgroundImageLayout = ImageLayout.Stretch;
                lbldir.Visible = true;

                // color
                UpdateLblColor();
                lblcolor.BackgroundImageLayout = ImageLayout.Stretch;
                lblcolor.Visible = true;

                this.LblLeftTime.Location = GetLblLeftTimeLocation();
                this.LblLeftTime.Visible = true;
                // The first player to play at the start will show who played first
                this.LblFirstShowCard.Visible = GameControl.FirstTurnFirstShow();
                SetPnlNormalShowCardorNotVisible(GameControl.TurnID == MyID);

                this.TmrCheckLeftTime.Start();
                this.TmrControlGame.Start();
            });
        }

        /// <summary>
        /// Initialize all players, initialize the interface
        /// </summary>
        private void InitializeAllPlayers() {
            int w = this.REF_WIDTH,
                h = this.REF_HEIGHT;

            InitializePlayers(InitJsonMsg);

            UIInvoke(DrawOriginScene);
        }

        /// <summary>
        /// draw the original scene
        /// </summary>
        private void DrawOriginScene() {
            DrawPiles();
            DrawPlayers();
            DrawControlsDesignedByDesigner();
        }

        /// <summary>
        /// Some settings displayed on the screen
        /// </summary>
        private void ScreenDisplaySetting() {
            bool FullScreen = false;
            float segs = 1.5f;

            if (FullScreen) {
                // hide window borders
                this.FormBorderStyle = FormBorderStyle.None;
                segs = 1f;
            }

            // Get the width and height of the screen
            int w = (int)(SystemInformation.VirtualScreen.Width / segs);
            int h = (int)(SystemInformation.VirtualScreen.Height / segs);

            // Set the maximum and minimum size
            this.MaximumSize = new Size(w, h);
            this.MinimumSize = new Size(w, h);
            //set window position
            this.Location = new Point(0, 0);
            // set window size
            this.Width = w;
            this.Height = h;


            // Set some global relative size information
            REF_HEIGHT = this.ClientSize.Height;
            REF_WIDTH = this.ClientSize.Width;
            CardButton.ScaleRatio = Math.Min(h / 1152f * 0.8f, w / 2048f * 0.8f);
        }

        /// <summary>
        /// Set some components designed using the visual interface (size, position, visibility properties)
        /// Initially all are set to invisible
        /// </summary>
        private void DrawControlsDesignedByDesigner() {
            ConstructPnlChooseColor();

            // draw function button
            DrawButtons();

            // parameter
            int padding = SIGN_LABLE_PDDING,
                lblsize = SIGN_LABLE_SIZE;
            int totalsize = padding * 2 + lblsize;

            // other controls
            Control lbl = null;

            #region 3 long-term control

            // playing direction label, LblDirection
            lbl = this.LblDirection;
            // tag Two background images are saved on the above, 0->clockwise, 1->counterclockwise
            lbl.Tag = new Bitmap[2] { UIImage.clockwise, UIImage.counterclockwise };
            lbl.Location = new Point(padding, padding);
            lbl.BackColor = Color.Transparent;
            lbl.Visible = false;

            //playing card color label, LblColor
            lbl = this.LblColor;
            lbl.Location = new Point(padding + totalsize, padding);
            lbl.BackColor = Color.Transparent;
            lbl.Visible = false;

            // countdown label, LblLeftTime
            lbl = this.LblLeftTime;
            lbl.Location = new Point(padding + totalsize * 2, padding);
            lbl.Visible = false;

            #endregion 3 long-term control

            #region Prompt information label

            // This position serves as a benchmark for the following positions
            Point loc = new Point(padding, padding + totalsize);
           
            List<Control> lbls = new List<Control>();

            // (1) The label used to display some information, LblMsg
            lbl = this.LblMsg;
            lbls.Add(lbl);
            lbl.Location = loc;
            // The remaining display time is recorded on the Tag
            lbl.Tag = 0;
      
            lbl.VisibleChanged += (sender, e) => {
                if (this.LblMsg.Visible) {
                    this.LblMsg.Tag = MSG_SHOW_TIME;
                }
            };
            lbl.Visible = false;

            loc.Y += lbl.Height + 5;//offset:5

            lbl = this.LblFirstShowCard;
            lbls.Add(lbl);
            lbl.Location = loc;
            lbl.Visible = false;

            lbl = this.LblGameOverShowInForm;
            lbls.Add(lbl);
            lbl.Location = loc;

            lbl = this.LblPlus2Total;
            lbls.Add(lbl);
            lbl.Location = loc;

            lbl = this.PnlChooseColor;
            lbls.Add(lbl);
            lbl.Location = loc;
         
            //lbl.VisibleChanged += (sender, e) => { (Control(sender)).BringToFront(); };
            lbl.Visible = false;

            foreach (Control control in lbls) {
                control.VisibleChanged +=
                    (sender, e) => { ((Control)sender).BringToFront(); };
            }

            #endregion prompt information label

            lbl = this.PnlDisplayCard;
            lbl.Location = this.PnlChooseColor.Location;
            lbl.Visible = false;

            lbl = this.PnlShowResultWhenGameOver;
            lbl.BackColor = Color.DimGray;
            lbl.Visible = false;

            lbl = this.TxtDebug;
            lbl.Location = new Point(20, 110 + this.PnlChooseColor.Location.Y);
            lbl.SendToBack();
            lbl.Visible = false;
        }

        private void DrawButtons() {
            List<Control> lbls = new List<Control>();
            lbls.Add(this.LblGetCard);
            lbls.Add(this.LblShowCard);
            lbls.Add(this.LblPlayPlus2);
            lbls.Add(this.LblDonotPlayPlus2);
            lbls.Add(this.LblShowAfterGetOne);
            lbls.Add(this.LblDonotShowAfterGetOne);
            lbls.Add(this.LblQuestion);
            lbls.Add(this.LblNoQuestion);

            foreach (Control c in lbls) {
                c.BackColor = Color.Transparent;
                c.BackgroundImage = NetImage.RoundedRectangle;
                c.BackgroundImageLayout = ImageLayout.Stretch;
            }

          
            List<Control> pnls = new List<Control>();
            pnls.Add(this.PnlAfterGetOne);
            pnls.Add(this.PnlQuestion);
            pnls.Add(this.PnlPlus2);
            pnls.Add(this.PnlNormalShowCardorNot);

            foreach (Control c in pnls) {
                c.BackColor = Color.Transparent;
                c.VisibleChanged += (sender, e) => {
                    ((Control)sender).SendToBack();
                };
                
                c.Size = TWO_BUTTON_PANEL_SIZE;
   
                int idx = 0;
                int lblHeight = 0;
                foreach (Control l in c.Controls) {
                    //if (!lbls.Contains(l)) { continue; }
                    l.Location = new Point(
                        (1 + idx) * c.Size.Width / 3 - l.Width / 2,
                        (c.Height - l.Height) / 2
                    );
                    lblHeight = l.Height;
                    ++idx;
                }
                c.Location = new Point(
                    (this.REF_WIDTH - c.Size.Width) / 2,
                    //(this.REF_HEIGHT + CardButton.HEIGHT_MODIFIED) / 2
                    Players[ME].Center.Y - (int)(
                        (CardButton.HighLightRatio + 0.5f) * CardButton.HEIGHT_MODIFIED
                        + (lblHeight + c.Height) / 2 + 1 // offset:1
                    )
                );

                while (c.Controls.Count > 0) {
                    Control l = c.Controls[0];
                    var loc = l.Location;
                    c.Controls.Remove(l);
                    this.Controls.Add(l);
                    l.Location = new Point(loc.X + c.Location.X,
                                           loc.Y + c.Location.Y);
                }
                this.Controls.Remove(c);
                c.Dispose(); 
            }
            pnls.Clear();

            SetPnlAfterGetOneVisible(false);
            SetPnlQuestionVisible(false);
            SetPnlPlus2Visible(false);
            SetPnlNormalShowCardorNotVisible(false);

            foreach (Control c in pnls) {
                c.Visible = false;
            }
        }
    }
}
