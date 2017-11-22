﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Bobbery's Bombs created in his Bomb Squad move.
    /// They're untargetable neutral entities that explode after a number of phase cycles, dealing explosive damage to those in their range.
    /// </summary>
    public sealed class BobberyBomb : BattleEntity
    {
        /// <summary>
        /// The number of turns the Bobbery Bomb has taken.
        /// </summary>
        private int TurnsTaken = 0;

        private LoopAnimation SparkAnimation = null;

        public BobberyBomb(int damage) : base(new Stats(0, 1, 0, damage, 0))
        {
            Name = "Bobbery Bomb";

            EntityType = EntityTypes.Neutral;

            //Explosions cause the bombs to detonate (only possible from other Bobbery Bombs in the actual games)
            EntityProperties.AddWeakness(Elements.Explosion, new WeaknessHolder(WeaknessTypes.KO, 0));

            //15 frame color change normal, 6 frame color change faster
            //2 frame spark change for normal and faster
            //Bomb starts brown, turns red 1 frame after fully stopping, then starts the spark
            //Spark order is red, orange, grey. It cycles back to red after grey
            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Neutral/BobberyBomb");

            double bombFrameRate = (1d / 15d) * 1000d;
            double sparkFrameRate = (1d / 30d) * 1000d;

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(62, 1, 65, 68), bombFrameRate),
                new Animation.Frame(new Rectangle(62, 72, 65, 68), bombFrameRate)));

            //Offset the spark animation so we can play it in the same position as the bombs
            Vector2 sparkOffset = new Vector2(0, 68 / 2);

            SparkAnimation = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(0, 0, 32, 24), sparkFrameRate, sparkOffset),
                new Animation.Frame(new Rectangle(0, 24, 32, 24), sparkFrameRate, sparkOffset),
                new Animation.Frame(new Rectangle(0, 48, 32, 24), sparkFrameRate, sparkOffset));

            //Pause the spark animation until we're ready to play it
            SparkAnimation.Pause();
        }

        //Bobbery's Bombs are added to battle after they finish moving during Bomb Squad
        public override void OnBattleStart()
        {
            base.OnBattleStart();

            SetBattlePosition(Position);

            SparkAnimation.Play();
        }

        //protected override void OnTakeDamage(InteractionHolder damageInfo)
        //{
        //    base.OnTakeDamage(damageInfo);
        //
        //    //When taking explosive damage, the bombs explode
        //    //If they're already in the process of exploding
        //    if (damageInfo.DamageElement == Elements.Explosion)
        //    {
        //        
        //    }
        //}

        public override void OnTurnStart()
        {
            //This is handled for turns instead of Phase Cycles in the event that the bomb's turn count is modified
            //This way, if it is somehow inflicted with Fast, it will blow up in one turn instead of two
            TurnsTaken++;

            //Blink faster on the first turn
            if (TurnsTaken <= 1)
            {
                StartAction(new BlinkFasterAction(AnimationGlobals.IdleName, 1.6f), true, null);
            }
            //Explode on the second turn
            else
            {
                
            }
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            //Bomb Squad Bombs don't have any held items or badges
            return 0;
        }

        public override void Draw()
        {
            base.Draw();

            //Draw the spark animation if it's playing
            if (SparkAnimation.IsPlaying == true)
                SparkAnimation.Draw(Position, Color.White, false, .11f);
        }
    }
}
