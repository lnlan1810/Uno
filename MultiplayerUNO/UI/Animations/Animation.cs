using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.Animations {

    public class Animation {
        // types of animation possible
        public const int TRANSLATE = 0b1;
        public const int ROTATE = 0b10;
        //public const int SCALE = 0b100;
        public const int SLEEP_TIME = 10;

        // possible state
        private Translate Trans;
        private Rotate Rot;
        //private Scale Scale;

        // Current animation type
        private int Kind;

        Form Form;

        /// <summary>
        /// controlled component
        /// </summary>
        public readonly List<Control> Controls;

        /// <summary>
        /// Estimated number of animation iterations
        /// </summary>
        private int StepCost;

        public Animation(Form form, Control ButtonControlled) {
            Controls = new List<Control>();
            Form = form;
            Kind = 0;
            Trans = null;
            Rot = null;
            Controls.Add(ButtonControlled);
        }

        public void AddControls(Control ButtonControlled) {
            Controls.Add(ButtonControlled);
        }

        public void SetRotate() {
            // direct coverage
            Kind |= ROTATE;
            Rot = new Rotate();
        }

        public void SetTranslate(int dx, int dy) {

            Kind |= TRANSLATE;
            Trans = new Translate(dx, dy);
            StepCost = Trans.StepCost;
            if (Rot != null) {
                Rot.ResetStep(StepCost);
            }
        }

        /// <summary>
        /// Return true to indicate that the update can continue
        /// </summary>
        private bool UpdateState() {
            bool canUpdate = false;
            if ((Kind & TRANSLATE) != 0 && !Trans.Finished) {
                canUpdate |= Trans.GetNextState();
            }
            if ((Kind & ROTATE) != 0) {
                canUpdate |= Rot.GetNextState();
            }
            return canUpdate;
        }

        /// <summary>
        /// Open a separate thread to do animation
        /// </summary>
        public Task Run() {
            return Task.Run(() => {
                Point[] pos = new Point[Controls.Count];
                for (int i = 0; i < Controls.Count; ++i) {
                    pos[i] = Controls[i].Location;  // value copy
                }
                bool first = true;
                while (UpdateState()) {
                    Form.BeginInvoke(new Action(() => {
                        for (int i = 0; i < Controls.Count; ++i) {
                            // When modifying width, it should be modified relative to the center
                            int offX = 0, offY = 0;
                            var btn = Controls[i];
                            if (Rot != null) {
                                // card flip
                                if (first && Rot.FlipOver) {
                                    first = false;
                                    if (btn != null && btn is CardButton) {
                                        ((CardButton)btn).Flip();
                                    }
                                }
                                btn.Width = (int)(CardButton.WIDTH_MODIFIED * Rot.GetXScale());
                                offX = (CardButton.WIDTH_MODIFIED - btn.Width) / 2;
                            }
                            if (Trans != null) {
                                offX += (int)Trans.NowX;
                                offY += (int)Trans.NowY;
                            }
                            btn.Location = new Point(pos[i].X + offX, pos[i].Y + offY);
                        }
                    }));
                    Thread.Sleep(SLEEP_TIME);
                }
            });
        }
    }
}