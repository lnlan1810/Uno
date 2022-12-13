using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using System;
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
        /// Encapsulates complex BeginInvoke (asynchronous)
        /// </summary>
        public void UIInvoke(Action fun) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        public void UIInvokeSync(Action fun) {
            if (this.InvokeRequired) {
                var tmp = this.BeginInvoke(new Action(() => { fun(); }));
                this.EndInvoke(tmp);
            } else { fun(); }
        }

 
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            this.TmrCheckLeftTime.Stop();
            this.TmrControlGame.Stop();
            this.TmrDisplayCard.Stop();
            MsgAgency.MainForm = null;
            if (MsgAgency.LoginForm != null) {
                MsgAgency.SendMsgToQueryRoomStateWhenLogin();
                MsgAgency.LoginForm.Show();
            }
        }

        private async Task ReorganizeMyCardsAsync() {
            var myBtns = Players[ME].BtnsInHand;
            GameControl.CBtnSelected = null; 
        
            UIInvokeSync(() => {
                foreach (CardButton btn in myBtns) {
                    btn.SendToBack();
                }
           
                SetCardButtonEnable(false);
            });
            AnimationSeq animaseq2 = new AnimationSeq();
         
            float dx = (int)(CardButton.WIDTH_MODIFIED * INTERVAL_BETWEEN_CARDS_RATIO);
           
            float totalWidth = (dx * (myBtns.Count - 1) + CardButton.WIDTH_MODIFIED);
            float firstX = this.REF_WIDTH * 0.5f + (totalWidth / 2) - CardButton.WIDTH_MODIFIED;
            for (int i = 0; i < myBtns.Count; ++i) {
                var btn = myBtns[i] as CardButton;
                btn.IsHighlighted = false; 
                Animation anima = new Animation(this, btn);
                int offX = (int)(firstX - dx * i - btn.Location.X);
                int offY = Players[ME].Center.Y - CardButton.HEIGHT_MODIFIED / 2
                    - btn.Location.Y;
                anima.SetTranslate(offX, offY); 
                animaseq2.AddAnimation(anima);
            }
            await animaseq2.RunAtTheSameTime();
            UIInvokeSync(() => {
                SetCardButtonEnable(true);
            });
        }

        public void UpdateLblDirection() {
            bool clockwise = !GameControl.DirectionIsClockwise;
            AnimationHighLight anima = new AnimationHighLight(this, this.LblDirection);
            anima.SetDirection(!clockwise);
            anima.SetSteps(100);
            anima.SetScale(2.0f, 1);

            var t = GameControl.LastLblDirectionTask;
            if (t != null && !t.IsCompleted) {
          
                while (!t.IsCanceled && !t.IsCompleted) {
                    
                    GameControl.LastLblDirectionTaskCancleToken.Cancel();
                    Thread.Sleep(10);
                }
                UIInvokeSync(()=> {
                    
                    var c = this.LblDirection as Control;
                    c.Size = new Size(SIGN_LABLE_SIZE, SIGN_LABLE_SIZE);
                    c.Location = new Point(SIGN_LABLE_PDDING, SIGN_LABLE_PDDING);
                   
                    c.BackgroundImage = ((Bitmap[])c.Tag)[clockwise ? 1 : 0];
                });
            }
            var token = new CancellationTokenSource();
            GameControl.LastLblDirectionTaskCancleToken = token;
            GameControl.LastLblDirectionTask = anima.Run(token);
            GameControl.DirectionIsClockwise = clockwise;
        }

      
        private void UpdateLblColor() {
            var c = GameControl.LastColor;
            Bitmap bmp = null;
            if (c == CardColor.Blue) {
                bmp = UIImage._oButBlue;
            } else if (c == CardColor.Green) {
                bmp = UIImage._oButGreen;
            } else if (c == CardColor.Red) {
                bmp = UIImage._oButRed;
            } else if (c == CardColor.Yellow) {
                bmp = UIImage._oButYellow;
            };
            var lbl = this.LblColor as Control;
            UIInvoke(() => {
                lbl.BackgroundImage = bmp;
            });
        }

        private void TmrControlGame_Tick(object sender, EventArgs e) {
            var tmr = this.TmrControlGame;
            // 1
            bool myturn = (GameControl.TurnID == MyID);
            // 2
            bool ff = GameControl.FirstTurnFirstShow();

            int t = ((int)this.LblMsg.Tag);
            bool msgVisible = (t > 0);
            this.LblMsg.Tag = msgVisible ? t - tmr.Interval : 0;

            UIInvoke(() => {

                UpdateLblColor();
          
                this.LblFirstShowCard.Visible = ff;

                this.LblMsg.Visible = msgVisible;
            });
        }

        private void TmrCheckLeftTime_Tick(object sender, EventArgs e) {
            if (GameControl.TimeForYou <= 0) {
                UIInvoke(() => {
                    ImDummy();
                });
                return;
            }
            Label lbl = this.LblLeftTime;
            int t = GameControl.TimeForYou;
            t -= this.TmrCheckLeftTime.Interval;
            if (t < 0) { t = 0; }
            GameControl.TimeForYou = t;
            t /= 1000;

            Point pos = GetLblLeftTimeLocation();
            UIInvoke(() => {
                lbl.Location = pos;
                lbl.BringToFront();
                lbl.Visible = true;
                lbl.Text = t.ToString().PadLeft(2, '0');
            });
        }

        private Point GetLblLeftTimeLocation() {
            Label lbl = this.LblLeftTime;
            Point pos;
            int playerIdx = GameControl.PlayerId2PlayerIndex[GameControl.TurnID];
            if (playerIdx != ME) {
               
                var player = Players[playerIdx];
                pos = player.Center;
                int offset = (CardButton.WIDTH_MODIFIED + lbl.Width) / 2 + 5;//offset:5
                if (/*player.IsUpDown && */player.PosX < 0) {
        
                    pos.X -= offset;
                } else {
                    pos.X += offset;
                }
            } else {
                var pnl = this.PnlNormalShowCardorNot;
                pos = pnl.Location;
                pos.Y += pnl.Height / 2;
            }
            pos.X -= lbl.Width / 2;
            pos.Y -= lbl.Height / 2;
            return pos;
        }

      
        private void ImDummy() {
            this.PnlChooseColor.Visible = false;
            SetPnlAfterGetOneVisible(false);
            SetPnlNormalShowCardorNotVisible(false);
            SetPnlPlus2Visible(false);
            SetPnlQuestionVisible(false);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }

        public void ShowMsgToUser(string msg) {
            UIInvoke(() => {
                this.LblMsg.Text = msg;
                this.LblMsg.Visible = false;
                this.LblMsg.Visible = true;
            });
        }

        #region panel Turn into transparent bankruptcy, transforming into directly disappearing the panel

        public void SetPnlAfterGetOneVisible(bool visible) {
            //this.PnlAfterGetOne.Visible = visible;
            this.LblDonotShowAfterGetOne.Visible = visible;
            this.LblShowAfterGetOne.Visible = visible;
        }

        public void SetPnlQuestionVisible(bool visible) {
            //this.PnlQuestion.Visible = visible;
            this.LblQuestion.Visible = visible;
            this.LblNoQuestion.Visible = visible;
        }

        public void SetPnlPlus2Visible(bool visible) {
            //this.PnlPlus2.Visible = visible;
            this.LblPlayPlus2.Visible = visible;
            this.LblDonotPlayPlus2.Visible = visible;
            this.LblPlus2Total.Visible = visible;
        }

        public void SetPnlNormalShowCardorNotVisible(bool visible) {
            //this.PnlNormalShowCardorNot.Visible = visible;
            this.LblShowCard.Visible = visible;
            this.LblGetCard.Visible = visible;
        }

        #endregion panel Turn into transparent bankruptcy, transforming into directly disappearing the panel

        #region 一some constants
        // (2) UI Location

        public const float PILE_OFFSET_RATE = 0.08f;
      
        public const int OFFSET_FOR_LBLINFO = 10;

        public const int SIGN_LABLE_SIZE = 80, SIGN_LABLE_PDDING = 10;

        public static Size TWO_BUTTON_PANEL_SIZE = new Size(300, 140);

        public const float INTERVAL_BETWEEN_CARDS_RATIO = 0.5f;

        // (2) game control
       
        public const int MSG_SHOW_TIME = 2000;
        #endregion 一some constants
    }
}