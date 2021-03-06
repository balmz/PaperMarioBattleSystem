﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The NoSkills Status Effect.
    /// The entity has MoveCategories disabled for a number of turns.
    /// <para>The MoveCategory disabled should always be one that isn't currently disabled.
    /// This Status Effect also refreshes differently than other Status Effects due to its nature (you can't remove a skill twice).</para>
    /// </summary>
    public sealed class NoSkillsStatus : MessageEventStatus
    {
        /// <summary>
        /// The MoveCategory to disable.
        /// </summary>
        private MoveCategories CategoryDisabled = MoveCategories.None;

        /// <summary>
        /// The MoveCategories currently disabled.
        /// </summary>
        private readonly Dictionary<MoveCategories, int> CategoriesDisabled = new Dictionary<MoveCategories, int>();

        public NoSkillsStatus(MoveCategories categoryDisabled, int duration)
        {
            StatusType = StatusTypes.NoSkills;
            Alignment = StatusAlignments.Negative;

            StatusIcon = null;

            Duration = duration;

            CategoryDisabled = categoryDisabled;

            AfflictedMessage = GetMessageFromCategory(CategoryDisabled);
            ShouldQueueEndEvent = false;
        }

        //NoSkills cannot be refreshed like other status effects, as a move category cannot be disabled more than once
        //This serves to add new categories to disable and track how long they should be disabled
        public override void Refresh(StatusEffect newStatus)
        {
            //Convert the new status, then check if its category isn't already in the status' disabled dictionary
            NoSkillsStatus noSkills = (NoSkillsStatus)newStatus;
            
            if (CategoriesDisabled.ContainsKey(noSkills.CategoryDisabled) == false)
            {
                //Adjust the Duration to account for a new category disabled while the status is already active
                //The Duration will match the exact number of turns required for all disabled categories to finish
                //AdditionalDuration applies to all disabled MoveCategories and the status itself,
                //so this works with that regardless of its value
                int turnOverflow = TurnsPassed + newStatus.Duration;
                if (turnOverflow > Duration)
                {
                    Duration = turnOverflow;
                }
                
                //Get the total duration of the new status
                //The AdditionalDuration will be the same across all statuses of this type since it's based on the entity afflicted by it
                int totalDur = newStatus.Duration + AdditionalDuration;

                CategoriesDisabled.Add(noSkills.CategoryDisabled, totalDur);
                EntityAfflicted.EntityProperties.DisableMoveCategory(noSkills.CategoryDisabled);
            }
            else
            { 
                //Display an error message if trying to disable a move category that NoSkills has already disabled
                Debug.LogError($"Moves cannot be disabled multiple times by NoSkills. Not refreshing {noSkills.CategoryDisabled} moves' disabled duration");
            }

            AfflictedMessage = GetMessageFromCategory(noSkills.CategoryDisabled);
            ShowAfflictedMessage();
        }

        protected override void OnAfflict()
        {
            //Disable the move category
            CategoriesDisabled.Add(CategoryDisabled, TotalDuration);
            EntityAfflicted.EntityProperties.DisableMoveCategory(CategoryDisabled);

            base.OnAfflict();
        }

        protected override void OnEnd()
        {
            //Immediately re-enable all categories on end
            foreach (KeyValuePair<MoveCategories, int> moveCategory in CategoriesDisabled)
            {
                EntityAfflicted.EntityProperties.EnableMoveCategory(moveCategory.Key);
            }

            base.OnEnd();
        }

        protected override void OnPhaseCycleStart()
        {
            if (IsSuppressed(StatusSuppressionTypes.TurnCount) == false)
            {
                //Go through each disabled move category and progress their turn counts
                KeyValuePair<MoveCategories, int>[] moveCategories = CategoriesDisabled.ToArray();
                for (int i = 0; i < moveCategories.Length; i++)
                {
                    MoveCategories category = moveCategories[i].Key;
                    CategoriesDisabled[category]--;

                    //Get the number of turns remaining for this disabled category
                    int turnsRemaining = CategoriesDisabled[category];

                    //If there are no turns left for this category, re-enable it
                    if (turnsRemaining <= 0)
                    {
                        CategoriesDisabled.Remove(category);
                        EntityAfflicted.EntityProperties.EnableMoveCategory(category);
                    }
                }
            }

            ProgressTurnCount();
        }

        protected override void OnSuppress(StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                //Immediately re-enable all categories when suppressed
                foreach (KeyValuePair<MoveCategories, int> moveCategory in CategoriesDisabled)
                {
                    EntityAfflicted.EntityProperties.EnableMoveCategory(moveCategory.Key);
                }
            }
        }

        protected override void OnUnsuppress(StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                //Immediately re-disable all categories when unsuppressed
                foreach (KeyValuePair<MoveCategories, int> moveCategory in CategoriesDisabled)
                {
                    EntityAfflicted.EntityProperties.DisableMoveCategory(moveCategory.Key);
                }
            }
        }

        public override StatusEffect Copy()
        {
            return new NoSkillsStatus(CategoryDisabled, Duration);
        }

        private string GetMessageFromCategory(MoveCategories categoryDisabled)
        {
            //NOTE: From what I've tested, NoSkills doesn't remove the Item command in TTYD, so the PM message is used
            //The messages for Tactics, Special, and Enemy are made up, as they are either never disabled
            //or disabled only in Tutorials where there is no message

            switch (categoryDisabled)
            {
                case MoveCategories.Item: return "You can't use any items now!";
                case MoveCategories.Jump: return "You can't jump!";
                case MoveCategories.Hammer: return "Can't use a hammer!";
                case MoveCategories.Partner: return "You can't use moves!";

                case MoveCategories.Tactics: return "You can't use any tactics now!";
                case MoveCategories.Special: return "You can't use special moves!";
                case MoveCategories.Enemy: return "Enemies can't attack now!";

                default: return string.Empty;
            }
        }
    }
}
