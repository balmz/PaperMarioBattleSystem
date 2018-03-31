﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Charge
    /// </summary>
    public sealed class ChargeSequence : Sequence
    {
        /// <summary>
        /// The amount to charge.
        /// </summary>
        private int ChargeAmount = 0;

        public ChargeSequence(MoveAction moveAction, int chargeAmount) : base(moveAction)
        {
            ChargeAmount = chargeAmount;
        }

        protected override void SequenceStartBranch()
        {
            //Have the entity move back and forth quickly for a bit
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 1:
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition - new Vector2(10, 0), 100d);
                    break;
                case 2:
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, 100d);
                    break;
                case 3:
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition + new Vector2(10, 0), 100d);
                    break;
                case 4:
                    goto case 2;
                case 5:
                    goto case 1;
                case 6:
                    goto case 2;
                case 7:
                    goto case 3;
                case 8:
                    ChangeSequenceBranch(SequenceBranch.Main);
                    goto case 2;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AfflictStatus(new ChargedStatus(ChargeAmount), true);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceFailedBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            PrintInvalidSequence();
        }
    }
}
