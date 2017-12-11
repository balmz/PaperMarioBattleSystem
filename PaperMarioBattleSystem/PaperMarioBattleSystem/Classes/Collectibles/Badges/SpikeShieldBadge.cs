﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Spike Shield Badge - Allows jumping on spiked entities
    /// </summary>
    public sealed class SpikeShieldBadge : Badge
    {
        public SpikeShieldBadge()
        {
            Name = "Spike Shield";
            Description = "Make Mario damage-proof when jumping on spiky foes.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.SpikeShield;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.EntityProperties.AddContactException(Enumerations.ContactTypes.TopDirect, Enumerations.PhysicalAttributes.TopSpiked);
            EntityEquipped.EntityProperties.AddContactException(Enumerations.ContactTypes.SideDirect, Enumerations.PhysicalAttributes.SideSpiked);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemoveContactException(Enumerations.ContactTypes.TopDirect, Enumerations.PhysicalAttributes.TopSpiked);
            EntityEquipped.EntityProperties.RemoveContactException(Enumerations.ContactTypes.SideDirect, Enumerations.PhysicalAttributes.SideSpiked);
        }
    }
}
