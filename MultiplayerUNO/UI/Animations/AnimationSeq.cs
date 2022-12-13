using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.Animations {
    /// <summary>
    /// A set of animations, in sequence
    /// </summary>
    public class AnimationSeq {
        List<Animation> Animations;

        public AnimationSeq() {
            Animations = new List<Animation>();
        }

        public void AddAnimation(Animation anima) {
            Animations.Add(anima);
        }

        public Task Run() {
            return Task.Run(async () => {
                foreach (var anima in Animations) {
                    await anima.Run();
                };
            });
        }

        public Task RunAtTheSameTime() {
            return Task.Run(async () => {
                List<Task> waitQueue = new List<Task>();
                foreach (var anima in Animations) {
                    waitQueue.Add(anima.Run());
                };
                foreach (var w in waitQueue) {
                    await w;
                }
            });
        }
    }
}
