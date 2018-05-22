﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Yoshi's Gulp.
    /// </summary>
    public sealed class GulpSequence : Sequence
    {
        public double WalkDuration = 4000f;
        private BattleEntity BehindEntity = null;

        private GulpActionCommandUI GulpUI = null;

        public GulpSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            //Check if there is a BattleEntity behind the one eaten and if it can be hit by this move
            List<BattleEntity> behindEntities = new List<BattleEntity>();

            BattleManager.Instance.GetEntitiesBehind(behindEntities, EntitiesAffected[0]);
            BattleManagerUtils.FilterEntitiesByHeights(behindEntities, Action.MoveProperties.HeightsAffected);

            //Store the reference to the behind entity and tell it it's being targeted
            if (behindEntities.Count > 0)
            {
                BehindEntity = behindEntities[0];
                BehindEntity.TargetForMove(User);
            }

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                GulpUI = new GulpActionCommandUI(actionCommand as GulpCommand);
                BattleUIManager.Instance.AddUIElement(GulpUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            BehindEntity?.StopTarget();

            if (GulpUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(GulpUI);
                GulpUI = null;
            }
        }

        protected override void CommandSuccess()
        {
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(BattleManagerUtils.GetPositionInFront(BattleManager.Instance.FrontPlayer,
                        User.EntityType != Enumerations.EntityTypes.Player), WalkDuration / 4f);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
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
                    StartActionCommandInput();
                    CurSequenceAction = new MoveToSeqAction(BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], User.EntityType != Enumerations.EntityTypes.Enemy), WalkDuration);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.YoshiBattleAnimations.GulpEatName, true);

                    //Deal damage to the entity spit out
                    InteractionResult[] interactions = AttemptDamage(BaseDamage, EntitiesAffected[0], Action.DamageProperties, false);

                    //Account for a miss or interruption
                    if (interactions[0] != null && interactions[0].WasVictimHit == true && interactions[0].WasAttackerHit == false)
                    {
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);

                        //Deal damage to the entity behind, if one exists
                        if (BehindEntity != null)
                        {
                            AttemptDamage(BaseDamage, BehindEntity, Action.DamageProperties, false);
                        }
                    }

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.YoshiBattleAnimations.GulpEatName, true);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration / 4f);
                    break;
                case 1:
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
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
