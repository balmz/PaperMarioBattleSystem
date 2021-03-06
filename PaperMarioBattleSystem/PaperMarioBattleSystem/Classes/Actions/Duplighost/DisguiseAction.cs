﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Duplighost's Disguise move. It disguises as one of Mario's Partners.
    /// </summary>
    public sealed class DisguiseAction : MoveAction
    {
        public DisguiseAction(BattleEntity user) : base(user)
        {
            Name = "Disguise";

            MoveInfo = new MoveActionData(null, "Disguise as one of Mario's Partners.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Custom, Enumerations.EntitySelectionType.Single, false, null, null);

            SetMoveSequence(new DisguiseSequence(this));
            actionCommand = null;
        }

        protected override void GetCustomAffectedEntities(List<BattleEntity> entityList)
        {
            BattlePartner partner = User.BManager.Partner;
            if (partner != null) entityList.Add(partner);
        }
    }
}
