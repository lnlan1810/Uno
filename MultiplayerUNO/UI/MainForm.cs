using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiplayerUNO.Utils.Card;

/// <summary>
/// some designs
/// 1. Game start process
/// (1) Draw the initial scene
/// (2) Licensing
/// (3) The game starts
/// </summary>
namespace MultiplayerUNO.UI {
    /// <summary>
    /// Functions starting with TEST are for testing only
    /// </summary>
    public partial class MainForm : Form {
        /// <summary>
        /// window size, except title size
        /// </summary>
        public int REF_HEIGHT, REF_WIDTH;

        /// <summary>
        /// number of players
        /// </summary>
        public int PlayersNumber;
        public Player[] Players; 
        public const int ME = 0;  // UI: index(0 means yourself)
        public readonly int MyID; // Backend: playerID

        // pile of cards
        // For the convenience of processing, these two cards will not be removed in MainForm
        private CardButton[] Piles;
        public const int PILES_NUM = 2;
        public const int PileToDistribute = 0;
        public const int PileDropped = 1;

        /// <summary>
        /// Card playing animation, what card someone plays, return to the task of card playing animation
        /// </summary>
        /// <param name="playerIdx">
        /// Note here is playerIdx for UI control, not playerID
        /// </param>
        public void ShowCard(int playerIdx, int lastCardID) {
            // function card, reverse direction
            if (GameControl.LastCard.IsReverse()) {
                // Transparency animation
                UpdateLblDirection();
            }
            CardButton cbtn = null;
            bool win = Players[playerIdx].ShowOneCard(); //Update some attributes after the card is played
            if (ME == playerIdx) {
                // If you play a card yourself, find this card in the deck
                // think that all cards have different numbers (in fact, the same is fine, the problem is not big)
                foreach (CardButton c in Players[ME].BtnsInHand) {
                    if (c.Card.CardId == lastCardID) {
                        cbtn = c;
                        break;
                    }
                }
                //remove this card
                Players[playerIdx].BtnsInHand.Remove(cbtn);
            } else {
                // If it is someone else's card, you need a new card
                cbtn = new CardButton(lastCardID, true, false);
                CardButton cbtnBack = Players[playerIdx].BtnsInHand[0] as CardButton;
                // Must be synchronized, the Location setting here will affect the subsequent licensing action
                UIInvokeSync(() => {
                    this.Controls.Add(cbtn);
                    cbtn.Location = cbtnBack.Location;
                    cbtn.BringToFront();
                    // Note that if it is the last card, we need to erase the bottom card
                    if (win) {
                        this.Controls.Remove((CardButton)Players[playerIdx].BtnsInHand[0]);
                    }
                });
            }

            Animation anima = new Animation(this, cbtn);
            var pos = Piles[PileDropped].Location;
            anima.SetTranslate(pos.X - cbtn.Location.X, pos.Y - cbtn.Location.Y);
            // Others need to turn their cards over
            if (playerIdx != ME) {
                anima.SetRotate();
            }
            UIInvokeSync(() => {
                cbtn.BringToFront();
            });

            var t = anima.Run();
            MsgAgency.TaskQueue.Enqueue(t);
            Task.Run(async () => {
                await t;
                // Added to the discard pile at the end of the animation
                UIInvoke(() => {
                    GameControl.AddDroppedCard(cbtn);
                    // Update the number of cards
                    Players[playerIdx].UpdateInfo();
                });
            });

            if (playerIdx == ME) {
                cbtn.Click -= cbtn.HighLightCard;
            }
            // If the card is played by yourself, the position of the remaining cards will be corrected at the same time
            // Others play the cards and also deal with the cards
            Task.Run(async () => { await ReorganizeMyCardsAsync(); });
        }

        /// <summary>
        /// After the previous player has played +2, choose to play or not
        /// </summary>
        internal void ShowOrGetAfterPlus2(TurnInfo turnInfo) {
            int num = turnInfo.IntInfo;
            UIInvoke(() => {
                this.LblPlus2Total.Text = "Need to draw the number of cards: " + num;
                SetPnlPlus2Visible(true);
            });
        }

        /// <summary>
        /// The game is over (be careful not to use any parsing information in turnInfo at this time)
        /// 1. Card playing animation
        /// 2. Show who wins
        /// 3. Show hand cards
        /// </summary>
        internal void GameOver(TurnInfo turnInfo) {
            int turnID = (int)turnInfo.JsonMsg["turnID"];
            // not a tie
            if (turnID != 0) {
                // 1.Card playing animation
                int playerIdx = turnInfo.GetPlayerIndex();
                ShowCard(playerIdx, turnInfo.LastCardID);
            }
            // 2. show who wins
            string msgGameOver = "";
            if (turnID != 0) {
                // shuffle
                string playerName = Players[
                    GameControl.PlayerId2PlayerIndex[turnID]
                ].Name;
                msgGameOver = "The game is over and the winner is: " + playerName;
            } else {
                // draw
                msgGameOver = "game draw";
            }
            UIInvokeSync(() => {
                Label lbl = this.LblGameOver;
                var h = lbl.Height; // Magical autosize and height
                lbl.AutoSize = false;
                lbl.Height = h;
                lbl.Text = msgGameOver;
                lbl.Visible = true;

                lbl = this.LblGameOverShowInForm;
                lbl.Text = msgGameOver + "\n[Click to show hand cards]";
                lbl.Visible = true;
                lbl.BringToFront();

                // stop countdown
                this.TmrCheckLeftTime.Stop();
                this.LblLeftTime.Visible = false;
            });

            // 3. show hand
            //ShowCardsWhenGameOver(turnInfo);
            this.LblGameOverShowInForm.Tag = turnInfo;
        }

        /// <summary>
        /// Display the hand cards according to the information turninfo saved in the tag
        /// </summary>
        private void LblGameOverShowInForm_Click(object sender, EventArgs e) {
            ShowCardsWhenGameOver((TurnInfo)this.LblGameOverShowInForm.Tag);
        }

        /// <summary>
        /// Animation showing hand cards at the end of the game
        /// </summary>
        private void ShowCardsWhenGameOver(TurnInfo turnInfo) {
            Panel pnl = this.PnlShowResultWhenGameOver;
            // (1) Get a list of players and cards
            JsonData playerCards = turnInfo.JsonMsg["playerCards"];
            int cnt = playerCards.Count;
            string[] name = new string[cnt];
            CardButton[][] cards = new CardButton[cnt][];
            for (int i = 0; i < cnt; ++i) {
                JsonData p = playerCards[i];
                name[i] = (string)p["name"]
                          + ((int)p["isRobot"] == 1 ? "[AI]" : "");
                p = p["handcards"];
                cards[i] = new CardButton[p.Count];
                for (int j = 0; j < p.Count; ++j) {
                    cards[i][j] = new CardButton((int)p[j]);
                }
                name[i] += " (" + p.Count + ")";
            }

            // (2) Build Label and CardButton
            Label[] lbls = new Label[cnt];
            Size lblSize = new Size(0, 0); // Maximum label size
            for (int i = 0; i < cnt; ++i) {
                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = name[i];
                lbl.BackColor = Color.Transparent;
                lbl.Font = new Font("Microsoft", 13.8F,
                    FontStyle.Regular, GraphicsUnit.Point, 134);
                lbls[i] = lbl;
            }
            //float ratio = 0.9f;
            float ratio = 1.0f;
            UIInvokeSync(() => {
                pnl.Size = new Size(
                    (int)(this.REF_WIDTH * ratio), (int)(this.REF_HEIGHT * ratio));
                pnl.Location = new Point(
                    (int)(this.REF_WIDTH * (1 - ratio) / 2),
                    (int)(this.REF_HEIGHT * (1 - ratio) / 2));
                for (int i = 0; i < cnt; ++i) {
                    Label lbl = lbls[i];
                    pnl.Controls.Add(lbl);
                    lblSize.Width = Math.Max(lbl.Size.Width, lblSize.Width);
                }
            });
            lblSize.Height = lbls[0].Height;

            // (3) Put it in the panel
            // [1] If it is >3 people, it needs to be divided into two columns
            // [2] If the number of remaining cards is really too much, we don't fully expand
            int padding = 10;
            int startx = padding * 2 + lblSize.Width;
            Point[] startPoint = new Point[cnt];
            int maxMovementX; // Maximum translation distance
            // The size of each person's deck (rounded)
            int lengthPerPlayer = pnl.Size.Width / 2 - startx - CardButton.WIDTH_MODIFIED;
            lengthPerPlayer = (lengthPerPlayer / CardButton.WIDTH_MODIFIED)
                                * CardButton.WIDTH_MODIFIED;
            if (cnt <= 3) {
                // a row
                lengthPerPlayer += pnl.Width / 2;
                for (int i = 0; i < cnt; ++i) {
                    startPoint[i] = new Point(startx,
                        (int)((i + 1) / (cnt + 1.0f) * pnl.Size.Height));
                }
            } else {
                // in two columns
                int mod = (cnt + 1) / 2; // [4]=>2, [5,6]=>3
                for (int i = 0; i < cnt; ++i) {
                    startPoint[i] = new Point(
                        (int)(startx + (i / mod) * pnl.Size.Width / 2),
                        (int)(((i % mod) + 1) / (mod + 1.0f) * pnl.Size.Height));
                }
            }
            maxMovementX = lengthPerPlayer;

            // (4) Calculate the label position, the panel is visible
            UIInvokeSync(() => {
                for (int i = 0; i < cnt; ++i) {
                    var lbl = lbls[i];
                    int x = startPoint[i].X,
                        y = startPoint[i].Y;
                    lbl.Location = new Point(
                        x - (lbl.Width + lblSize.Width) / 2,
                        y - lbl.Height / 2);
                    foreach (var btn in cards[i]) {
                        pnl.Controls.Add(btn);
                        btn.Location = new Point(x, y - btn.Height / 2);
                    }
                }
                this.TmrCheckLeftTime.Stop(); // stop tmr's stupid activities
                pnl.BringToFront();
                pnl.Visible = true;
            });

            // (5) Animation sequence
            // The i-th card of each person is the i-th group animation
            List<Animation> lst = new List<Animation>();
            bool notok = true;
            int idx = 0;
            while (notok) {
                Animation anima = null;
                notok = false;
                // Calculate card position
                for (int player = 0; player < cnt; ++player) {
                    var p = cards[player];
                    if (idx + 1 < p.Length) { notok = true; }
                    if (idx >= p.Length) { continue; }
                    if (anima == null) {
                        anima = new Animation(this, p[idx]);
                        // Moving distance
                        int x = CardButton.WIDTH_MODIFIED / 2 * (2 + idx);
                        if (x > maxMovementX) {
                            x = maxMovementX;
                        }
                        anima.SetTranslate(x, 0);
                    } else {
                        anima.AddControls(p[idx]);
                    }
                }
                ++idx;
                lst.Add(anima);
            }
            // animation run
            Task.Run(async () => {
                foreach (var anima in lst) { 
                    UIInvokeSync(() => {
                        foreach (var c in anima.Controls) {
                            c.BringToFront();
                        }
                    });
                    await anima.Run();
                }
            });
        }

        /// <summary>
        /// Response +4
        /// 1. The response panel is displayed
        /// </summary>
        internal void RespondToPlus4(TurnInfo turnInfo) {
            UIInvokeSync(() => {
                SetPnlQuestionVisible(true);
            });
        }

        /// <summary>
        /// Someone touched several cards, here only need to construct animation
        /// </summary>
        public void GetManyCards(TurnInfo turnInfo) {
            int num = turnInfo.LastCardID;
            if (turnInfo.TurnID != MyID) {
                //Others, just pretend to draw a card
                GetCardForOtherOne(turnInfo, num);
                return;
            }
            // For myself, I need to draw several cards
            AnimationSeq animaseq = new AnimationSeq();
            var pos = ((CardButton)Players[ME].BtnsInHand[0]).Location;
            pos.Y = Players[ME].Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            for (int i = 0; i < num; ++i) {
                int cardID = turnInfo.PlayerCards[i];
                CardButton cbtn = new CardButton(cardID, true, true);
                cbtn.Location = Piles[PileToDistribute].Location;
                UIInvokeSync(() => {
                    this.Controls.Add(cbtn);
                    cbtn.BringToFront();
                });
                // (1)
                Animation anima = new Animation(this, cbtn);
                pos.X +=
                    (int)(INTERVAL_BETWEEN_CARDS_RATIO * CardButton.WIDTH_MODIFIED);
                anima.SetTranslate(pos.X - cbtn.Location.X,
                                   pos.Y - cbtn.Location.Y);
                anima.SetRotate();
                Players[ME].BtnsInHand.Insert(0, cbtn);
                animaseq.AddAnimation(anima);
            }
            var w = animaseq.Run();
            MsgAgency.TaskQueue.Enqueue(w);
            Task.Run(async () => {
                await w; //Synchronize
                Players[ME].CardsCount += num; // Update the number of cards
                UIInvoke(() => { Players[ME].UpdateInfo(); });
                await ReorganizeMyCardsAsync();
            });
        }

        /// <summary>
        /// Can play cards normally
        /// </summary>
        internal void ShowOrGetNormal(TurnInfo turnInfo) {
            UIInvoke(() => {
                SetPnlNormalShowCardorNotVisible(true);
            });
        }

        private void LblGetCard_Click(object sender, EventArgs e) {
            //construct json
            JsonData json = new JsonData() {
                ["state"] = 2,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.SendOneJsonDataMsg(json);
            if (GameControl.CBtnSelected != null) {
                // pick card back
                GameControl.CBtnSelected.PerformClick();
                GameControl.CBtnSelected = null;
            }
            SetPnlNormalShowCardorNotVisible(false);
        }

        /// <summary>
        /// Others draw num cards
        /// </summary>
        public void GetCardForOtherOne(TurnInfo turnInfo, int num = 1) {
            int playerIdx = GameControl.PlayerId2PlayerIndex[turnInfo.TurnID];
            Animation anima = GetACardAnima(playerIdx, turnInfo.LastCardID);
            var w = anima.Run();
            MsgAgency.TaskQueue.Enqueue(w);
            Task.Run(async () => {
                await w;
                Players[playerIdx].CardsCount += num;
                UIInvoke(() => {
                    Players[playerIdx].UpdateInfo();
                    // Remove redundant cards
                    while (Players[playerIdx].BtnsInHand.Count > 1) {
                        CardButton cbtn = Players[playerIdx].BtnsInHand[0] as CardButton;
                        this.Controls.Remove(cbtn);
                        Players[playerIdx].BtnsInHand.RemoveAt(0); // Add the head and remove the head
                    }
                });
            });
        }

        /// <summary>
        /// draw a card
        /// (1) Card drawing animation
        /// (2) Highlight the drawn card
        /// (3) Then set all the card buttons to be unresponsive, only to play cards or not to play cards
        /// </summary>
        public void GetACardForMe(TurnInfo turnInfo) {
            // (1)
            Animation anima = GetACardAnima(ME, turnInfo.LastCardID);
            var t = anima.Run();
            MsgAgency.TaskQueue.Enqueue(t);
            CardButton cbtn = Players[ME].BtnsInHand[0] as CardButton;
            Card c = new Card(turnInfo.LastCardID);
            bool canResponed = GameControl.FirstTurn() ||
                  c.CanResponseTo(GameControl.LastCard, GameControl.LastColor);
            Task.Run(async () => {
                // (2)
                await t; // Synchronize
                Players[ME].CardsCount++; // update card
                // Shuffle after draw
                await ReorganizeMyCardsAsync();
                // (3) Are you ready to play cards?
                UIInvoke(() => {
                    if (canResponed) {
                        // Can respond, choose whether to play the card
                        cbtn.PerformClick();
                        // The setting card buttons are unresponsive
                        SetCardButtonEnable(false);
                        SetPnlAfterGetOneVisible(true);
                    } else {
                        // Unable to respond Reply directly No license
                        DonotShowCardAfterGetJson();
                        // update info
                    }
                    Players[ME].UpdateInfo();
                });
            });
        }

        private void DonotShowCardAfterGetJson() {
            // construct json
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["action"] = 0,
                ["color"] = 0, // Without this, it seems that the backend will report an error TODO
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.SendOneJsonDataMsg(json);
        }

        /// <summary>
        /// Get the animation of drawing a card
        /// </summary>
        public Animation GetACardAnima(int playerIdx, int cardID) {
            bool isMe = (playerIdx == ME);
            // Add button to Form
            CardButton cbtn = new CardButton(cardID, true, isMe);
            cbtn.Location = Piles[PileToDistribute].Location;
            UIInvokeSync(() => {
                this.Controls.Add(cbtn);
                cbtn.BringToFront();
            });
            // (1)
            Animation anima = new Animation(this, cbtn);
            var pos = ((CardButton)Players[playerIdx].BtnsInHand[0]).Location; // Card 0 is the rightmost
            pos.Y = Players[playerIdx].Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            // When others draw cards, they don't need to be turned over, they can be stacked together directly
            // Draw the cards by yourself, you need to turn them over, and you need to insert the cards in your hand
            if (isMe) {
                pos.X +=
                    (int)(INTERVAL_BETWEEN_CARDS_RATIO * CardButton.WIDTH_MODIFIED);
            }
            anima.SetTranslate(pos.X - cbtn.Location.X,
                               pos.Y - cbtn.Location.Y);
            if (isMe) {
                anima.SetRotate();
            }
            // Note that even if it is not ourselves, we have added the card (deleted after the await animation ends)
            Players[playerIdx].BtnsInHand.Insert(0, cbtn);
            return anima;
        }

        /// <summary>
        /// Play button click event
        /// </summary>
        private void LblShowCard_Click(object sender, EventArgs e) {
            // This problem should not occur, but in order to avoid problems, it is better to set this
            // may be clicked soon before disappearing, stress test
            var btn = GameControl.CBtnSelected;
            if (btn == null) { return; }

            if (btn.Card.Color == CardColor.Invalid) {
                // +4/wild cards
                // TODO animation here (don't do it for now, directly visible=true)
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    GameControl.ChooseColorIsTriggerAfterGetOneCard = false;
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson();
            }
            SetPnlNormalShowCardorNotVisible(false);
        }

        /// <summary>
        /// Construct the json of the cards
        /// </summary>
        /// <param name="afterGetOne">
        /// true means it was played after drawing a card, false means it was played normally
        /// </param>
        private void SendShowCardJson(bool afterGetOne=false) {
            var btn = GameControl.CBtnSelected;
            // construct json
            // state=2: This field is meaningless
            // state=1: not wild cards/+4, set to -1
            int color = -1;
            if (btn.Card.Color == CardColor.Invalid) {
                color = (int)GameControl.InvalidCardToChooseColor;
            }
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["color"] = color,
                ["queryID"] = GameControl.QueryID
            };
            if (afterGetOne) {
                json["action"] = 1;
            } else {
                json["card"] = btn.Card.CardId;
            }
            MsgAgency.SendOneJsonDataMsg(json);

            // At this time, you need to block, seek verification from the server, and then play the card after the server gives feedback
            // It is not processed here, it is considered that the event processing is over, and the message receiving thread in Msg can be processed directly

            // The card playing animation is not done here
            // ShowCard(GameControl.CBtnSelected, Players[ME]);
            GameControl.CBtnSelected = null;
        }

        /// <summary>
        /// Question button click event occurs
        /// </summary>
        private void LblQuestion_Click(object sender, EventArgs e) {
            SendMsgRespondToPlus4(6);
            SetPnlQuestionVisible(false);
        }

        /// <summary>
        /// Don't challenge the button click event to occur
        /// </summary>
        private void LblNoQuestion_Click(object sender, EventArgs e) {
            SendMsgRespondToPlus4(4);
            SetPnlQuestionVisible(false);
        }

        /// <summary>
        ///Send the json that responds to +4
        /// </summary>
        private void SendMsgRespondToPlus4(int state) {
            JsonData json = new JsonData() {
                ["state"] = state,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.SendOneJsonDataMsg(json);
        }


        /// <summary>
        /// someone shows a hand
        /// </summary>
        public void DisplayOnesCard(TurnInfo turnInfo) {
            List<CardButton> lst = new List<CardButton>();
            foreach (int i in turnInfo.PlayerCards) {
                lst.Add(new CardButton(i, false, false));
            }
            string playerName = Players[
                GameControl.PlayerId2PlayerIndex[turnInfo.GetPlayerID()]
            ].Name;
            Panel pnl = this.PnlDisplayCard;
            Label lbl = this.LblDisplayCardsPlayerName;
            UIInvoke(() => {
                lbl.Text = playerName;
                int h = lbl.Height;
                for (int i = 0; i < lst.Count; ++i) {
                    var cbtn = lst[i];
                    pnl.Controls.Add(cbtn);
                    cbtn.Location = new Point(CardButton.WIDTH_MODIFIED * i, h);
                }
                lbl.Width = pnl.Width;
                pnl.Visible = true;
            });
        }

        /// <summary>
        /// When the visibility is modified, the event occurs
        /// 1. Remove all buttons and release resources when it becomes invisible
        /// 2. When it becomes visible, the timer starts, and after 5 seconds, the display card ends
        /// </summary>
        private void PnlDisplayCard_VisibleChanged(object sender, EventArgs e) {
            if (this.PnlDisplayCard.Visible) {
                this.TmrDisplayCard.Interval = 5000;
                this.TmrDisplayCard.Start();
                return;
            }
            Panel pnl = this.PnlDisplayCard;

            // Remove button, release resources
            List<CardButton> lst = new List<CardButton>();
            foreach (var c in pnl.Controls) {
                var cbtn = c as CardButton;
                if (cbtn != null) {
                    lst.Add(cbtn);
                }
            }
            UIInvoke(() => {
                foreach (var c in lst) {
                    pnl.Controls.Remove(c);
                    c.Dispose();
                }
            });
        }

        private void TmrDisplayCard_Tick(object sender, EventArgs e) {
            this.TmrDisplayCard.Stop();
            this.PnlDisplayCard.Visible = false;
        }

        /// <summary>
        /// hit +2
        /// </summary>
        private void LblPlayPlus2_Click(object sender, EventArgs e) {
            CardButton cbtn = GameControl.CBtnSelected;
            if (cbtn == null || !cbtn.Card.IsPlus2()) { return; }

            JsonData json = new JsonData() {
                ["state"] = 3,
                ["card"] = cbtn.Card.CardId,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.SendOneJsonDataMsg(json);
            SetPnlPlus2Visible(false);
        }

        /// <summary>
        /// Do not play +2
        /// </summary>
        private void LblDonotPlayPlus2_Click(object sender, EventArgs e) {
            JsonData json = new JsonData() {
                ["state"] = 4,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.SendOneJsonDataMsg(json);
            SetPnlPlus2Visible(false);
        }

        public void SetCardButtonEnable(bool enable) {
            var lst = Players[ME].BtnsInHand;
            for (int i = 0; i < lst.Count; ++i) {
                var cbtn = lst[i] as CardButton;
                cbtn.Enabled = enable;
            }
        }

        /// <summary>
        /// The card drawn after playing the draw command
        /// </summary>
        private void LblShowAfterGetOne_Click(object sender, EventArgs e) {
            var btn = GameControl.CBtnSelected;
            if (btn == null) { return; }
            if (btn.Card.Color == CardColor.Invalid) {
                // +4/wild cards
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    GameControl.ChooseColorIsTriggerAfterGetOneCard = true;
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson(true);
            }
            UIInvoke(() => {
                SetCardButtonEnable(true);
                SetPnlAfterGetOneVisible(false);
            });
        }

        private void LblDonotShowAfterGetOne_Click(object sender, EventArgs e) {
            // no cards
            DonotShowCardAfterGetJson();
            // UI recovery
            UIInvoke(() => {
                SetCardButtonEnable(true);
                SetPnlAfterGetOneVisible(false);
            });
        }

        private void PnlPlus2_VisibleChanged(object sender, EventArgs e) {
            Control c = sender as Control;
            this.LblPlus2Total.Visible = c.Visible;
        }

        // TODO (UI DEBUG)

        private bool TxtDebugIsFront = false;
        private void TxtDebug_Click(object sender, EventArgs e) {
            TxtDebugIsFront = !TxtDebugIsFront;
            if (TxtDebugIsFront) {
                this.TxtDebug.BringToFront();
            } else {
                this.TxtDebug.SendToBack();
            }
        }

        private void LblDisplayCardsPlayerName_Click(object sender, EventArgs e)
        {

        }

        public void DebugLog(string v) {
            this.TxtDebug.Text += v + "\r\n";
            this.TxtDebug.ScrollToCaret();
        }
    }
}
