﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public static class Enumerations
    {
        /// <summary>
        /// The types of BattleEntities.
        /// </summary>
        public enum EntityTypes
        {
            Player, Enemy, Neutral
        }

        /// <summary>
        /// The types of playable characters.
        /// </summary>
        public enum PlayerTypes
        {
            Mario, Partner
        }

        /// <summary>
        /// The types of partners.
        /// </summary>
        public enum PartnerTypes
        {
            None,
            //PM Partners
            Goombario, Kooper, Bombette, Parakarry, Bow, Watt, Sushie, Lakilester,
            //TTYD Partners
            Goombella, Koops, Flurrie, Yoshi, Vivian, Bobbery, MsMowz,
            //Unused or temporary partners
            Goompa, Goombaria, Twink, ProfFrankly, Flavio
        }

        /// <summary>
        /// The types of Collectibles.
        /// </summary>
        public enum CollectibleTypes
        {
            None, Item, Badge
        }

        /// <summary>
        /// The root-level categories of moves.
        /// <para>In the PM games, Tactics, Special, and Enemy moves are never disabled outside of tutorials.
        /// Be extra careful if you want to disable those.</para>
        /// </summary>
        public enum MoveCategories
        {
            None, Tactics, Item, Jump, Hammer, Special, Partner, Enemy
        }

        /// <summary>
        /// The types of BattleEntities a MoveAction affects.
        /// <para>None causes the MoveAction to occur immediately.
        /// Self targets the user of the MoveAction.
        /// Ally targets all BattleEntities that are allies of the user.
        /// Other targets other types of BattleEntities in battle.
        /// Custom targets a set of BattleEntities based on the move's conditions.</para>
        /// <para>This is a bit field.</para>
        /// </summary>
        [Flags]
        public enum MoveAffectionTypes
        {
            None = 0,
            Self = 1 << 0,
            Ally = 1 << 1,
            Other = 1 << 2,
            Custom = 1 << 3
        }

        /// <summary>
        /// The types of ways to target BattleEntities.
        /// </summary>
        public enum EntitySelectionType
        {
            Single, All, First
        }

        /// <summary>
        /// The types of resources that MoveActions can require.
        /// </summary>
        public enum MoveResourceTypes
        {
            /// <summary>
            /// FP
            /// </summary>
            FP,
            /// <summary>
            /// Star Spirit Star Power
            /// </summary>
            SSSP,
            /// <summary>
            /// Crystal Star Star Power
            /// </summary>
            CSSP
        }

        /// <summary>
        /// The display types of MoveAction resources.
        /// </summary>
        public enum CostDisplayTypes
        {
            Shown, Hidden, Special
        }

        /// <summary>
        /// The types of DefensiveActions that exist.
        /// This enum is a bit field, so handle it with bitwise operations.
        /// </summary>
        [Flags]
        public enum DefensiveActionTypes
        {
            None = 0,
            Guard = 1 << 0,
            Superguard = 1 << 1
        }

        /// <summary>
        /// The effects a damage-dealing move can have on a BattleEntity.
        /// This enum is a bit field, so handle it with bitwise operations.
        /// </summary>
        [Flags]
        public enum DamageEffects
        {
            None = 0,
            FlipsShelled = 1 << 0,
            RemovesWings = 1 << 1,
            RemovesSegment = 1 << 2,
            FlipsClefts = 1 << 3,
            SpinsOut = 1 << 4
        }

        /// <summary>
        /// The types of damage elements.
        /// </summary>
        public enum Elements
        {
            Invalid, Normal, Gulp, Sharp, Water, Fire, Electric, Ice, Poison, Explosion, Star
        }

        /// <summary>
        /// The main height states an entity can be in.
        /// Some moves may or may not be able to hit entities in certain height states.
        /// <para>Hovering means that the BattleEntity is right above the ground and can be hit by most, but not all, ground moves.
        /// For example, Quake Hammer cannot hit Hovering BattleEntities, but Tornado Jump's secondary attack will hit Hovering BattleEntities.</para>
        /// </summary>
        public enum HeightStates
        {
            Grounded, Hovering, Airborne, Ceiling
        }

        /// <summary>
        /// The physical attributes assigned to BattleEntities.
        /// These determine if an attack can target a particular BattleEntity, or whether there is an advantage
        /// or disadvantage to using a particular attack on a BattleEntity with a particular PhysicalAttribute.
        /// 
        /// <para>Flying does not mean that the BattleEntity is Airborne. Flying BattleEntities, such as Ruff Puffs,
        /// can still be damaged by ground moves if they hover at ground level.</para>
        /// </summary>
        public enum PhysicalAttributes
        {
            None = 0, Flying = 1, Electrified = 2, Poisonous = 3, Spiked = 4, Icy = 5, Fiery = 6, Explosive = 7, Starry = 8
        }

        /// <summary>
        /// The state of health a BattleEntity can be in.
        /// <para>Danger occurs when the BattleEntity has 2-5 HP remaining.
        /// Peril occurs when the BattleEntity has exactly 1 HP remaining.
        /// Dead occurs when the BattleEntity has 0 HP remaining.</para>
        /// </summary>
        public enum HealthStates
        {
            Normal, Danger, Peril, Dead
        }

        /// <summary>
        /// The type of contact actions will make on BattleEntities.
        /// <para>None is no contact at all (Ex. Hammer Throw, Star Storm).
        /// Latch is direct contact used to start an attack (Ex. Fuzzy's Kissy-Kissy, first part of Kiss Thief).
        /// TopDirect is direct contact from the top (Ex. Jump).
        /// SideDirect is direct contact from the side (Ex. Hammer).</para>
        /// </summary>
        public enum ContactTypes
        {
            None, Latch, TopDirect, SideDirect
        }

        /// <summary>
        /// Properties complementing ContactTypes. They tell more information about the contact that occurred.
        /// <para>None means no special properties or protection.
        /// Ranged means the attack is ranged (Ex. Gus' Spear Throw, Earth Tremor).
        /// WeaponDirect means the attack is performed directly with something attached to or held by the attacker, but not the attacker itself (Ex. Hammer, Gulp, Gus' Spear Charge).
        /// Protected means the attacker is protected in some form (Ex. Koopa Shell; Shell Toss).
        /// </para>
        /// </summary>
        public enum ContactProperties
        {
            None, Ranged, WeaponDirect, Protected
        }

        /// <summary>
        /// The result of a ContactType and the PhysicalAttributes of a BattleEntity.
        /// A Failure indicates that the action backfired and only the attacker takes damage.
        /// PartialSuccess indicates that damage is dealt to both the attacker and the victim.
        /// </summary>
        public enum ContactResult
        {
            Success, PartialSuccess, Failure
        }

        /// <summary>
        /// The types of StatusEffects BattleEntities can be afflicted with.
        /// </summary>
        public enum StatusTypes
        {
            //Neutral
            None, Allergic,
            //Positive
            Charged, DEFUp, Dodgy, Electrified, Fast, Huge, Invisible, Payback, POWUp, HoldFast, HPRegen, FPRegen, Stone,
            TurboCharge, WaterBlock, CloudNine,
            //Negative
            Burn, Confused, DEFDown, Dizzy, Frozen, Stop, NoSkills, Poison, POWDown, Sleep, Slow, Tiny, Injured, Paralyzed,
            KO, Fright, Blown, Lifted
        }

        /// <summary>
        /// The types of ways Status Effects can be suppressed.
        /// </summary>
        public enum StatusSuppressionTypes
        {
            TurnCount, Effects, VFX, Icon
        }

        /// <summary>
        /// Miscellaneous properties that BattleEntities can have.
        /// <para>This includes information such as how much Charge damage a BattleEntity has stored.</para>
        /// </summary>
        public enum AdditionalProperty
        {
            Invincible,
            ConfusionPercent,
            ChargedDamage,

            /// <summary>
            /// The number of Item turns the BattleEntity has. Set when using an item in the Double and Triple Dip menus.
            /// </summary>
            DipItemTurns,

            /// <summary>
            /// Whether a BattleEntity is immobile or not. The BattleEntity's defensive actions are disabled and it cannot move.
            /// </summary>
            Immobile,

            /// <summary>
            /// Used for enemies who are tattled or if Peekaboo is active. This tells them to show their HP underneath them.
            /// <para>Use an integer for the value so removing Peekaboo doesn't remove this property from enemies if they have been tattled.</para>
            /// </summary>
            ShowHP,

            /// <summary>
            /// Tells that this BattleEntity should never have its HP shown despite the value of <see cref="ShowHP"/>.
            /// <para>This should have an integer as the value, as it may potentially be modified in multiple locations.</para>
            /// </summary>
            NeverShowHP,

            /// <summary>
            /// Tells that this BattleEntity is, in general, not targetable.
            /// Certain moves may still be able to target BattleEntities with this property.
            /// <para>Some BattleEntities innately have this (Ex. Bobbery Bombs), but it can also be applied dynamically
            /// through darkness or other types of battle settings.</para>
            /// </summary>
            Untargetable,

            /// <summary>
            /// Tells that this BattleEntity is a light source. If a BattleEntity with this property is in a dark battle,
            /// a region around it will be lit.
            /// <para>The value should be a double with the radius of its light.</para>
            /// </summary>
            LightSource,

            /// <summary>
            /// Tells that this BattleEntity is a helper or part of another BattleEntity.
            /// <para>This should have a BattleEntity as the value.
            /// The BattleEntity will be the BattleEntity that this one supports.</para>
            /// </summary>
            HelperEntity,

            /// <summary>
            /// Tells that there is a BattleEntity defending this one and taking the hits for it.
            /// The only example in the PM games is the Shell from Koops' Shell Shield.
            /// <para>This will take in the BattleEntity defending this one as the value.
            /// This is inactive during this BattleEntity's phase to allow itself and allies to positively affect it, and active otherwise.
            /// If active, the BattleEntity defending this one will be targeted by opponents instead.</para>
            /// </summary>
            DefendedByEntity,

            /// <summary>
            /// Tells that Stylish Move timings should be shown for this BattleEntity.
            /// <para>This should have an integer as the value, as multiple Timing Tutor badges can be equipped.</para>
            /// </summary>
            ShowStylishTimings,

            /// <summary>
            /// Tells that this BattleEntity should automatically complete Action Commands, provided they're enabled.
            /// <para>This should have an integer as the value, as it may be modified in multiple locations.</para>
            /// </summary>
            AutoActionCommands,

            /// <summary>
            /// Tells that this BattleEntity should automatically complete Stylish Moves.
            /// <para>This should have an integer as the value, as it may be modified in multiple locations.</para>
            /// </summary>
            AutoStylishMoves,

            /// <summary>
            /// Tells that this BattleEntity plays sounds from Attack FX badges when dealing damage.
            /// <para>This should have an <see cref="AttackFXManager"/> as the value.
            /// Initialize it when adding this property, and clean it up when removing it.</para>
            /// </summary>
            AttackFXSounds
        }
    }
}
