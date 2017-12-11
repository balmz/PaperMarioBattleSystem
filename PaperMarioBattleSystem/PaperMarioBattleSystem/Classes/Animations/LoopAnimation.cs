﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class LoopAnimation : Animation
    {
        public int MaxLoops { get; protected set; } = 1;
        public int Loops { get; protected set; } = 0;

        public LoopAnimation(Texture2D spriteSheet, int maxLoops, params Frame[] frames) : this(spriteSheet, maxLoops, false, frames)
        {
            
        }

        public LoopAnimation(Texture2D spriteSheet, int maxLoops, bool isUIAnim, params Frame[] frames) : this(spriteSheet, maxLoops, 1f, isUIAnim, frames)
        {
            
        }

        public LoopAnimation(Texture2D spriteSheet, int maxLoops, float speed, bool isUIAnim, params Frame[] frames) : base(spriteSheet, speed, isUIAnim, frames)
        {
            MaxLoops = maxLoops;
        }

        protected override void Progress()
        {
            CurFrameNum++;

            //Done with a loop
            if (CurFrameNum >= MaxFrames)
            {
                Loops++;

                //If the animation goes on forever or we're not done, reset back to the first frame
                if (MaxLoops <= AnimationGlobals.InfiniteLoop || Loops < MaxLoops)
                {
                    CurFrameNum = 0;
                }
                //Otherwise stop on the last frame
                else
                {
                    Loops = MaxLoops;
                    CurFrameNum = MaxFrameIndex;
                    End();
                }
            }

            ResetFrameDur();
        }

        protected override void DerivedReset()
        {
            Loops = 0;
        }
    }
}
