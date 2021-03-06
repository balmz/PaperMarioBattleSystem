﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent for when a BattleEntity is blown away via Gale Force or Hurricane.
    /// The BattleEntity is removed from battle.
    /// </summary>
    public sealed class BlownAwayBattleEvent : MoveToBattleEvent
    {
        public BlownAwayBattleEvent(BattleEntity entity, Vector2 endPos, double duration) : base(entity, endPos, duration, Interpolation.InterpolationTypes.CubicInOut, Interpolation.InterpolationTypes.CubicInOut)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            //Play the hurt animation
            Entities[0].AnimManager.PlayAnimation(AnimationGlobals.HurtName);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Remove the BattleEntity first
            Entities[0].BManager.RemoveEntity(Entities[0], true);
            Entities[0].Die();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Entities[0].Rotation += UtilityGlobals.ToRadians(5f);
        }
    }
}
