﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Guard action when being attacked.
    /// It's unique in that it reduces the damage Piercing attacks do
    /// </summary>
    public class Guard : DefensiveAction
    {
        public Guard(BattleEntity user) : base(user)
        {
            Name = "Guard";
            DefensiveActionType = DefensiveActionTypes.Guard;

            actionCommand = new GuardCommand(this);

            CommandSuccessTimer = (8d / 60d) * Time.MsPerS;

            AllowedStatuses = null;
        }

        public override BattleGlobals.DefensiveActionHolder HandleSuccess(int damage, StatusChanceHolder[] statusEffects, DamageEffects damageEffects)
        {
            int newDamage = damage - 1;
            StatusChanceHolder[] newStatuses = FilterStatuses(statusEffects);

            User.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage,
                new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn },
                new WaitForAnimBattleEvent(User, AnimationGlobals.PlayerBattleAnimations.GuardName, true));

            User.BManager.battleObjManager.AddBattleObject(new ActionCommandVFX(ActionCommand.CommandRank.Nice, User.Position, new Vector2(-15, -15)));
            SoundManager.Instance.PlaySound(SoundManager.Sound.ActionCommandSuccess);

            return new BattleGlobals.DefensiveActionHolder(newDamage, newStatuses, Enumerations.DamageEffects.None, DefensiveActionType);
        }
    }
}
