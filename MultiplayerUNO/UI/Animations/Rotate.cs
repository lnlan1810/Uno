using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.Animations {
    /// <summary>
    /// Rotation animation, the current implementation only supports horizontal flip
    /// </summary>
    public class Rotate {
        //constant
        public const int DEFAULT_STEPS = 40;
        public const float TOTAL_ROTATE_TRANSFORM = 2.0f;
        public const float END_SCALE = 2.0f;
        public const float HALF_SCALE = 1.0f;

        // state quantity
        public bool Finished;
        public bool FlipOver;

        /// <summary>
        /// 0 -> 2 means 1 -> 0 -> 1,
        /// Zoom out and flip over
        /// </summary>
        private float XScale;
        public float DeltaScale;
        public int StepCost;
        private int StepNow;

        public float GetXScale() {
            return Math.Abs(XScale - 1.0f);
        }

        /// <summary>
        /// true means the animation is not over yet,
        /// false means the animation has ended
        /// </summary>
        public bool GetNextState() {
            if (Finished) {
                XScale = END_SCALE;
                return false;
            }
            XScale += DeltaScale;

            // turn over
            if (XScale > HALF_SCALE) { FlipOver = true; }

            if (++StepNow == StepCost) {
                XScale = END_SCALE;
                Finished = true;
            }
            return true;
        }

        public Rotate(int steps = DEFAULT_STEPS) {
            Finished = false;
            FlipOver = false;
            StepNow = 0;
            ResetStep(steps);
        }

        public void ResetStep(int steps) {
            StepCost = steps;
            DeltaScale = TOTAL_ROTATE_TRANSFORM / steps;
        }
    }
}
