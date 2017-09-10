﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Tasty Tonic item. It removes Poison and Shrinking on Mario.
    /// </summary>
    public sealed class TastyTonic : BattleItem, IStatusHealingItem
    {
        public StatusTypes[] StatusesHealed { get; private set; }

        public TastyTonic()
        {
            Name = "Tasty Tonic";
            Description = "Removes all negative status effects from Mario.";

            ItemType = ItemTypes.Healing;

            StatusesHealed = new StatusTypes[] { StatusTypes.Poison, StatusTypes.Tiny, StatusTypes.Allergic, StatusTypes.DEFDown,
                                                 StatusTypes.Dizzy, StatusTypes.Confused, StatusTypes.Frozen, StatusTypes.Burn,
                                                 StatusTypes.Slow, StatusTypes.Sleep, StatusTypes.Immobilized };

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            EntityType = EntityTypes.Player;
        }
    }
}
