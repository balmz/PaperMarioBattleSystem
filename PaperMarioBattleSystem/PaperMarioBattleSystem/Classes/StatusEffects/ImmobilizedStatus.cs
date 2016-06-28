﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Immobilized Status Effect.
    /// Entities afflicted with this cannot move until it wears off.
    /// This Status Effect cannot be prevented via Guarding, only Superguarding
    /// <para>Mario and his Partner cannot Guard or Superguard when afflicted with this Status Effect</para>
    /// </summary>
    public class ImmobilizedStatus : StatusEffect
    {
        public ImmobilizedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Immobilized;
            Alignment = StatusAlignments.Negative;

            Duration = duration;
        }

        protected override void OnAfflict()
        {
            //Prevent the entity from moving on affliction and mark it as using up all of its turns
            EntityAfflicted.SetMaxTurns(0);
            EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
        }

        protected override void OnPhaseStart()
        {
            EntityAfflicted.SetMaxTurns(0);
        }

        protected override void OnPhaseEnd()
        {
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new ImmobilizedStatus(Duration);
        }
    }
}