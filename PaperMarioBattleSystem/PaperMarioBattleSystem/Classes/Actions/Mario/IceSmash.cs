﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Ice Smash action.
    /// It's identical to Hammer except it deals Ice damage, can freeze enemies, and has a slightly longer Action Command.
    /// </summary>
    public sealed class IceSmash : Hammer
    {
        public IceSmash()
        {
            Name = "Ice Smash";

            MoveInfo.Description = "Wear this to use Ice Smash. " +
                           "3 FP are required to use this attack, which can freeze and immobilize an enemy if executed superbly. " +
                           "Wearing two or more of these badges requires more FP for the move, but enemies stay frozen longer.";

            //NOTE: May want to rethink this to make it easier to make a few changes, but for now it works
            InteractionParamHolder damageInfo = (InteractionParamHolder)DamageInfo;
            damageInfo.DamagingElement = Enumerations.Elements.Ice;
            DamageInfo = damageInfo;

            SetMoveSequence(new IceSmashSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 1000d);
        }
    }
}