﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base sequence for Star Spirit moves.
    /// </summary>
    public abstract class StarSpiritMoveSequence : SpecialMoveSequence
    {
        public StarSpiritMoveSequence(SpecialMoveAction specialAction) : base(specialAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //NOTE: Mario moves up a tiny bit when he's in the front, I haven't confirmed how it works in the back yet

                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName);
                    CurSequenceAction = new WaitForAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName);
                    break;
                case 2:
                    User.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarPrayName);
                    //NOTE: Show Star Spirit appearing and VFX and such
                    CurSequenceAction = new WaitForAnimation(AnimationGlobals.PlayerBattleAnimations.StarPrayName);
                    break;
                case 3:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}