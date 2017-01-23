﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    #region Enums

    /// <summary>
    /// The result of elemental damage dealt on an entity based on its weaknesses and/or resistances
    /// </summary>
    public enum ElementInteractionResult
    {
        Damage, KO, Heal
    }

    /// <summary>
    /// The ways to handle weaknesses
    /// </summary>
    public enum WeaknessTypes
    {
        None, PlusDamage, KO
    }

    /// <summary>
    /// The ways to handle resistances
    /// </summary>
    public enum ResistanceTypes
    {
        None, MinusDamage, NoDamage, Heal
    }

    #endregion

    #region Structs

    public struct WeaknessHolder
    {
        public WeaknessTypes WeaknessType;
        public int Value;

        public static WeaknessHolder Default => new WeaknessHolder(WeaknessTypes.None, 0);

        public WeaknessHolder(WeaknessTypes weaknessType, int value)
        {
            WeaknessType = weaknessType;
            Value = value;
        }

        #region Comparison Operators

        public static bool operator==(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType == holder2.WeaknessType && holder1.Value == holder2.Value);
        }

        public static bool operator!=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType != holder2.WeaknessType || holder1.Value != holder2.Value);
        }

        public static bool operator>(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType > holder2.WeaknessType || holder1.Value > holder2.Value);
        }

        public static bool operator<(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1.WeaknessType < holder2.WeaknessType || holder1.Value < holder2.Value);
        }

        public static bool operator>=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1 > holder2 || holder1 == holder2);
        }

        public static bool operator<=(WeaknessHolder holder1, WeaknessHolder holder2)
        {
            return (holder1 < holder2 || holder1 == holder2);
        }

        #endregion
    }

    public struct ResistanceHolder
    {
        public ResistanceTypes ResistanceType;
        public int Value;

        public static ResistanceHolder Default => new ResistanceHolder(ResistanceTypes.None, 0);

        public ResistanceHolder(ResistanceTypes resistanceType, int value)
        {
            ResistanceType = resistanceType;
            Value = value;
        }

        #region Comparison Operators

        public static bool operator==(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType == holder2.ResistanceType && holder1.Value == holder2.Value);
        }

        public static bool operator!=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType != holder2.ResistanceType || holder1.Value != holder2.Value);
        }

        public static bool operator>(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType > holder2.ResistanceType || holder1.Value > holder2.Value);
        }

        public static bool operator<(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1.ResistanceType < holder2.ResistanceType || holder1.Value < holder2.Value);
        }

        public static bool operator>=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1 > holder2 || holder1 == holder2);
        }

        public static bool operator<=(ResistanceHolder holder1, ResistanceHolder holder2)
        {
            return (holder1 < holder2 || holder1 == holder2);
        }

        #endregion
    }

    public struct StrengthHolder
    {
        public int Value;

        public static StrengthHolder Default => new StrengthHolder(0);

        public StrengthHolder(int value)
        {
            Value = value;
        }
    }

    public struct ContactResultInfo
    {
        public StatusGlobals.PaybackHolder Paybackholder;
        public Enumerations.ContactResult ContactResult;
        public bool SuccessIfSameAttr;

        public static ContactResultInfo Default => 
            new ContactResultInfo(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, Enumerations.Elements.Normal, 1, null), Enumerations.ContactResult.Success, false);

        public ContactResultInfo(StatusGlobals.PaybackHolder paybackHolder, Enumerations.ContactResult contactResult, bool successIfSameAttr)
        {
            Paybackholder = paybackHolder;
            ContactResult = contactResult;
            SuccessIfSameAttr = successIfSameAttr;
        }
    }

    /// <summary>
    /// Holds immutable data for a StatusProperty.
    /// </summary>
    public struct StatusPropertyHolder
    {
        /// <summary>
        /// The likelihood of being afflicted by a StatusEffect-inducing move. This value cannot be lower than 0.
        /// </summary>
        public int StatusPercentage { get; private set; }

        /// <summary>
        /// The number of turns to add onto the StatusEffect's base duration. This can be negative to reduce the duration.
        /// </summary>
        public int AdditionalTurns { get; private set; }

        /// <summary>
        /// Whether the entity is currently immune to the StatusEffect or not.
        /// </summary>
        public bool Immune { get; private set; }

        public static StatusPropertyHolder Default => new StatusPropertyHolder(100, 0);

        public StatusPropertyHolder(int statusPercentage, int additionalTurns)
        {
            StatusPercentage = UtilityGlobals.Clamp(statusPercentage, 0, int.MaxValue);
            AdditionalTurns = additionalTurns;
            Immune = false;
        }

        public StatusPropertyHolder(int statusPercentage, int additionalTurns, bool immune) : this (statusPercentage, additionalTurns)
        {
            Immune = immune;
        }
    }

    /// <summary>
    /// Holds immutable data for elemental damage
    /// </summary>
    public struct ElementDamageHolder
    {
        /// <summary>
        /// The damage dealt
        /// </summary>
        public int Damage { get; private set; }

        /// <summary>
        /// The type of Elemental damage dealt
        /// </summary>
        public Enumerations.Elements Element { get; private set; }

        public static ElementDamageHolder Default => new ElementDamageHolder(0, Enumerations.Elements.Normal);

        public ElementDamageHolder(int damage, Enumerations.Elements element)
        {
            Damage = damage;
            Element = element;
        }
    }

    /// <summary>
    /// Holds the required data for initiating a damage interaction.
    /// This is passed to methods that involve calculating damage interactions.
    /// </summary>
    public struct InteractionParamHolder
    {
        public BattleEntity Attacker;
        public BattleEntity Victim;
        public int Damage;
        public Enumerations.Elements DamagingElement;
        public bool Piercing;
        public Enumerations.ContactTypes ContactType;
        public StatusEffect[] Statuses;

        public InteractionParamHolder(BattleEntity attacker, BattleEntity victim, int damage, Enumerations.Elements element,
            bool piercing, Enumerations.ContactTypes contactType, StatusEffect[] statuses)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
            DamagingElement = element;
            Piercing = piercing;
            ContactType = contactType;
            Statuses = statuses;
        }
    }

    /// <summary>
    /// Holds immutable data for the result of a damage interaction.
    /// It includes the BattleEntity that got damaged, the amount and type of damage dealt, the Status Effects inflicted, and more.
    /// </summary>
    public struct InteractionHolder
    {
        public BattleEntity Entity { get; private set; }
        public int TotalDamage { get; private set; }
        public Enumerations.Elements DamageElement { get; private set; }
        public ElementInteractionResult ElementResult { get; private set; }
        public Enumerations.ContactTypes ContactType { get; private set; }
        public bool Piercing { get; private set; }
        public StatusEffect[] StatusesInflicted { get; private set; }
        public bool Hit { get; private set; }

        /// <summary>
        /// Tells if the InteractionHolder has a usable value
        /// </summary>
        public bool HasValue => (Entity != null);
        public static InteractionHolder Default => new InteractionHolder();

        public InteractionHolder(BattleEntity entity, int totalDamage, Enumerations.Elements damageElement, ElementInteractionResult elementResult,
            Enumerations.ContactTypes contactType, bool piercing, StatusEffect[] statusesInflicted, bool hit)
        {
            Entity = entity;
            TotalDamage = totalDamage;
            DamageElement = damageElement;
            ElementResult = elementResult;
            ContactType = contactType;
            Piercing = piercing;
            StatusesInflicted = statusesInflicted;
            Hit = hit;
        }
    }

    /// <summary>
    /// Holds properties of a MoveAction.
    /// </summary>
    public struct MoveActionData
    {
        public Texture2D Icon;
        public int FPCost;
        public bool UsesCharge;
        public string Description;
        public TargetSelectionMenu.EntitySelectionType SelectionType;
        public Enumerations.EntityTypes EntityType;
        public bool TargetsEntity;
        public Enumerations.HeightStates[] HeightsAffected;
        public bool HideCost;

        public static MoveActionData Default => new MoveActionData(null, 0, "Error", TargetSelectionMenu.EntitySelectionType.Single,
            Enumerations.EntityTypes.Enemy, true, null);

        public MoveActionData(Texture2D icon, int fpCost, string description, TargetSelectionMenu.EntitySelectionType selectionType,
            Enumerations.EntityTypes entityType, Enumerations.HeightStates[] heightsAffected)
        {
            Icon = icon;
            FPCost = fpCost;
            UsesCharge = false;
            Description = description;
            SelectionType = selectionType;
            EntityType = entityType;
            TargetsEntity = true;
            HeightsAffected = heightsAffected;
            HideCost = false;
        }

        public MoveActionData(Texture2D icon, int fpCost, bool usesCharge, string description, TargetSelectionMenu.EntitySelectionType selectionType,
            Enumerations.EntityTypes entityType, Enumerations.HeightStates[] heightsAffected)
        {
            Icon = icon;
            FPCost = fpCost;
            UsesCharge = usesCharge;
            Description = description;
            SelectionType = selectionType;
            EntityType = entityType;
            TargetsEntity = true;
            HeightsAffected = heightsAffected;
            HideCost = false;
        }

        public MoveActionData(Texture2D icon, int fpCost, string description, TargetSelectionMenu.EntitySelectionType selectionType,
            Enumerations.EntityTypes entityType, bool targetsEntity, Enumerations.HeightStates[] heightsAffected) : this(icon, fpCost,
                description, selectionType, entityType, heightsAffected)
        {
            TargetsEntity = targetsEntity;
        }
    }

    /// <summary>
    /// Holds data regarding healing, including HP, FP, and Status Effects.
    /// </summary>
    public struct HealingData
    {
        /// <summary>
        /// The amount of HP healed.
        /// </summary>
        public int HPHealed;

        /// <summary>
        /// The amount of FP healed.
        /// </summary>
        public int FPHealed;

        /// <summary>
        /// The StatusEffects healed.
        /// </summary>
        public Enumerations.StatusTypes[] StatusEffectsHealed;

        public static HealingData Default => new HealingData(0, 0, null);

        public HealingData(int hpHealed, int fpHealed, Enumerations.StatusTypes[] statusesHealed)
        {
            HPHealed = hpHealed;
            FPHealed = fpHealed;
            StatusEffectsHealed = statusesHealed;
        }
    }

    #endregion

    #region Classes

    /// <summary>
    /// A class containing the main stats in the game.
    /// Enemies have internal levels, likely related to Star Point gain.
    /// </summary>
    public class Stats
    {
        public int Level;

        //Max stats
        public int MaxHP;
        public int MaxFP;

        //Base stats going into battle

        /// <summary>
        /// Base Attack without any modifications.
        /// </summary>
        public int BaseAttack;

        /// <summary>
        /// Base Defense without any modifications.
        /// </summary>
        public int BaseDefense;

        public int HP;
        public int FP;

        /// <summary>
        /// The Attack amount modified.
        /// </summary>
        public int Attack;

        /// <summary>
        /// The Defense amount modified.
        /// </summary>
        public int Defense;

        /// <summary>
        /// Different from Defense - this modifies the total damage the attack itself does, making it possible to reduce damage dealt
        /// to you from Piercing attacks. This value can be negative.
        /// <para>The P-Up, D-Down and P-Down, D-Up Badges modify this value.</para>
        /// </summary>
        public int DamageReduction;

        public int Accuracy = 100;
        public int Evasion = 0;

        /// <summary>
        /// Default stats
        /// </summary>
        public static Stats Default => new Stats(1, 10, 5, 0, 0);

        /// <summary>
        /// The BattleEntity's BaseAttack combined with any modifiers.
        /// </summary>
        public int TotalAttack => BaseAttack + Attack;

        /// <summary>
        /// The BattleEntity's BaseDefense combined with any modifiers.
        /// </summary>
        public int TotalDefense => BaseDefense + Defense;

        public Stats(int level, int maxHP, int maxFP, int attack, int defense)
        {
            Level = level;
            MaxHP = HP = maxHP;
            MaxFP = FP = maxFP;
            BaseAttack = attack;
            BaseDefense = defense;
            Attack = 0;
            Defense = 0;
        }
    }

    /// <summary>
    /// Stats for Mario's Partners.
    /// </summary>
    public sealed class PartnerStats : Stats
    {
        public PartnerGlobals.PartnerRanks PartnerRank = PartnerGlobals.PartnerRanks.Normal;

        public PartnerStats(PartnerGlobals.PartnerRanks partnerRank, int maxHP, int attack, int defense)
            : base((int)partnerRank, maxHP, 0, attack, defense)
        {
            PartnerRank = partnerRank;
        }
    }

    /// <summary>
    /// Stats for Mario.
    /// </summary>
    public sealed class MarioStats : Stats
    {
        /// <summary>
        /// The level of Mario's Boots
        /// </summary>
        public EquipmentGlobals.BootLevels BootLevel = EquipmentGlobals.BootLevels.Normal;

        /// <summary>
        /// The level of Mario's Hammer
        /// </summary>
        public EquipmentGlobals.HammerLevels HammerLevel = EquipmentGlobals.HammerLevels.Normal;

        /// <summary>
        /// Mario's Star Spirit Star Power.
        /// </summary>
        public StarSpiritPower SSStarPower = new StarSpiritPower();

        /// <summary>
        /// Mario's Crystal Star Power.
        /// </summary>
        public CrystalStarPower CSStarPower = new CrystalStarPower();

        /// <summary>
        /// The number of Star Points Mario has.
        /// When it reaches 100, it resets back to 0 and Mario goes up one level.
        /// </summary>
        public int StarPoints = 0;

        public MarioStats(int level, int maxHp, int maxFP, int attack, int defense,
            EquipmentGlobals.BootLevels bootLevel, EquipmentGlobals.HammerLevels hammerLevel) : base(level, maxHp, maxFP, attack, defense)
        {
            BootLevel = bootLevel;
            HammerLevel = hammerLevel;
        }

        /// <summary>
        /// Retrieves the type of Star Power based on a given StarPowerTypes.
        /// </summary>
        /// <param name="starPowerType">The type of Star Power to get.</param>
        /// <returns>StarSpiritPower or CrystalStarPower if the respective type is passed in. Otherwise, it returns null.</returns>
        public StarPowerBase GetStarPowerFromType(StarPowerGlobals.StarPowerTypes starPowerType)
        {
            if (starPowerType == StarPowerGlobals.StarPowerTypes.StarSpirit)
            {
                return SSStarPower;
            }
            else if (starPowerType == StarPowerGlobals.StarPowerTypes.CrystalStar)
            {
                return CSStarPower;
            }

            return null;
        }
    }

    /// <summary>
    /// The final result of an interaction, containing InteractionHolders for both the attacker and victim
    /// </summary>
    public class InteractionResult
    {
        public InteractionHolder AttackerResult;
        public InteractionHolder VictimResult;

        public InteractionResult()
        {
            
        }

        public InteractionResult(InteractionHolder attackerResult, InteractionHolder victimResult)
        {
            AttackerResult = attackerResult;
            VictimResult = victimResult;
        }
    }

    #endregion

    public static class Enumerations
    {
        public enum EntityTypes
        {
            Player, Enemy
        }
        
        /// <summary>
        /// The types of playable characters
        /// </summary>
        public enum PlayerTypes
        {
            Mario, Partner
        }

        /// <summary>
        /// The types of partners in the game
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
        /// The types of Collectibles in the game
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
        /// The types of damage elements
        /// </summary>
        public enum Elements
        {
            Invalid, Normal, Sharp, Water, Fire, Electric, Ice, Poison, Explosion, Star
        }

        /// <summary>
        /// The main height states an entity can be in.
        /// Some moves may or may not be able to hit entities in certain height states.
        /// </summary>
        public enum HeightStates
        {
            Grounded, /*Elevated, */Airborne, Ceiling
        }

        /// <summary>
        /// The physical attributes assigned to entities.
        /// These determine if an attack can target a particular entity, or whether there is an advantage
        /// or disadvantage to using a particular attack on an entity with a particular physical attribute.
        /// 
        /// <para>Flying does not mean that the entity is Airborne. Flying entities, such as Ruff Puffs,
        /// can still be damaged by ground moves if they hover at ground level.</para>
        /// </summary>
        //NOTE: The case of Explosive on contact in the actual games are with enraged Bob-Ombs and when Bobbery uses Hold Fast
        //If you make contact with these enemies, they deal explosive damage and die instantly, with Hold Fast being an exception
        //to the latter
        public enum PhysicalAttributes
        {
            None, Flying, Electrified, Poisony, Spiked, Icy, Fiery, Explosive, Starry
        }

        /// <summary>
        /// The state of health an entity can be in.
        /// 
        /// <para>Danger occurs when the entity has 2-5 HP remaining.
        /// Peril occurs when the entity has exactly 1 HP remaining.
        /// Dead occurs when the entity has 0 HP remaining.</para>
        /// </summary>
        public enum HealthStates
        {
            Normal, Danger, Peril, Dead
        }

        /// <summary>
        /// The type of contact actions will make on entities.
        /// JumpContact and HammerContact means the action attacks from the top and side, respectively
        /// </summary>
        //NOTE: Rename these; JumpContact should be renamed Direct, and HammerContact probably isn't necessary, as it's indirect
        public enum ContactTypes
        {
            None, JumpContact, HammerContact
        }

        /// <summary>
        /// The result of a ContactType and the PhysicalAttributes of an entity.
        /// A Failure indicates that the action backfired.
        /// PartialSuccess indicates that damage is dealt and the attacker suffers a backfire.
        /// </summary>
        public enum ContactResult
        {
            Success, Failure, PartialSuccess
        }

        /// <summary>
        /// The types of StatusEffects BattleEntities can be afflicted with
        /// </summary>
        public enum StatusTypes
        {
            //Neutral
            None, Allergic,
            //Positive
            Charged, DEFUp, Dodgy, Electrified, Fast, Huge, Invisible, Payback, POWUp, HPRegen, FPRegen, Stone,
            TurboCharge, WaterBlock, CloudNine,
            //Negative
            Burn, Confused, DEFDown, Dizzy, Frozen, Immobilized, NoSkills, Poison, POWDown, Sleep, Slow, Soft, Tiny, Injured, Paralyzed
        }

        public enum AdditionalProperty
        {
            Frightened,
            LiftedAway,
            BlownAway,
            InstantKO,
            PositiveStatusImmune,
            NeutralStatusImmune,
            NegativeStatusImmune,
            Invincible,
            ConfusionPercent,
            ChargedDamage,

            /// <summary>
            /// The number of Item turns the BattleEntity has. Set when using an item in the Double and Triple Dip menus.
            /// </summary>
            DipItemTurns
        }
    }

    /// <summary>
    /// Class for general global values and references
    /// </summary>
    public static class GeneralGlobals
    {
        public static readonly Random Randomizer = new Random();
    }

    /// <summary>
    /// Class for global values dealing with Animations
    /// </summary>
    public static class AnimationGlobals
    {
        /// <summary>
        /// A value corresponding to an animation that loops infinitely
        /// </summary>
        public const int InfiniteLoop = -1;
        public const float DefaultAnimSpeed = 1f;

        //Shared animations
        public const string IdleName = "Idle";
        public const string JumpName = "Jump";
        public const string JumpMissName = "JumpMiss";
        public const string RunningName = "Run";
        public const string HurtName = "Hurt";
        public const string DeathName = "Death";
        public const string VictoryName = "Victory";

        public const string GetItemName = "GetItem";

        public const string SpikedTipHurtName = "SpikedTipHurt";

        /// <summary>
        /// Battle animations specific to playable characters
        /// </summary>
        public static class PlayerBattleAnimations
        {
            public const string ChoosingActionName = "ChoosingAction";
            public const string GuardName = "Guard";
            public const string SuperguardName = "Superguard";
            public const string DangerName = "Danger";
            public const string StarSpecialName = "StarSpecial";
            public const string StarWishName = "StarWish";
        }

        /// <summary>
        /// Mario-specific battle animations
        /// </summary>
        public static class MarioBattleAnimations
        {
            public const string HammerPickupName = "HammerPickup";
            public const string HammerWindupName = "HammerWindup";
            public const string HammerSlamName = "HammerSlam";
        }

        /// <summary>
        /// Kooper-specific battle animations
        /// </summary>
        public static class KooperBattleAnimations
        {
            public const string ShellSpinName = "ShellSpin";
        }

        /// <summary>
        /// Yoshi-specific battle animations
        /// </summary>
        public static class YoshiBattleAnimations
        {
            public const string GulpEatName = "GulpEat";
            public const string EggLayName = "EggLay";
            public const string EggThrowName = "EggThrow";
        }

        /// <summary>
        /// Status Effect-related animations in battle
        /// </summary>
        public static class StatusBattleAnimations
        {
            public const string StoneName = "StoneName";
        }
    }

    /// <summary>
    /// Class for global values dealing with Battles
    /// </summary>
    public static class BattleGlobals
    {
        #region Enums

        public enum StartEventPriorities
        {
            Message = 0, Stage = 500, Status = 1000, Dialogue = 1500
        }

        #endregion

        #region Constants

        public const int DefaultTurnCount = 1;
        public const int MaxEnemies = 5;

        public const int MinDamage = 0;
        public const int MaxDamage = 99;

        public const int MaxPowerBounces = 100;

        public const int MinDangerHP = 2;
        public const int MaxDangerHP = 5;
        public const int PerilHP = 1;
        public const int DeathHP = 0;

        #endregion

        #region Structs

        /// <summary>
        /// Holds information about a MoveAction being used and the BattleEntities it targets
        /// </summary>
        public struct ActionHolder
        {
            /// <summary>
            /// The MoveAction being used.
            /// </summary>
            public MoveAction Action { get; private set; }

            /// <summary>
            /// The BattleEntities the action targets.
            /// </summary>
            public BattleEntity[] Targets { get; private set; }

            public ActionHolder(MoveAction action, params BattleEntity[] targets)
            {
                Action = action;
                Targets = targets;
            }
        }

        public struct DefensiveActionHolder
        {
            /// <summary>
            /// The final damage, influenced by the Defensive Action
            /// </summary>
            public int Damage { get; private set; }

            /// <summary>
            /// A filtered set of StatusEffects, influenced by the Defensive Action
            /// </summary>
            public StatusEffect[] Statuses { get; private set; }

            /// <summary>
            /// The type and amount of damage dealt to the attacker.
            /// If none, set to null.
            /// </summary>
            public ElementDamageHolder? ElementHolder { get; private set; }

            public DefensiveActionHolder(int damage, StatusEffect[] statuses) : this(damage, statuses, null)
            {
            }

            public DefensiveActionHolder(int damage, StatusEffect[] statuses, ElementDamageHolder? elementHolder)
            {
                Damage = damage;
                Statuses = statuses;
                ElementHolder = elementHolder;
            }
        }

        /// <summary>
        /// Holds a pending Battle Event with its priority and the Battle States it should be added in.
        /// The fields in this struct are immutable.
        /// </summary>
        public struct PendingBattleEventHolder
        {
            public int Priority { get; private set; }
            public BattleManager.BattleState[] States { get; private set; }
            public BattleEvent PendingBattleEvent { get; private set; }

            public PendingBattleEventHolder(int priority, BattleManager.BattleState[] battleStates, BattleEvent battleEvent)
            {
                Priority = priority;
                States = battleStates;
                PendingBattleEvent = battleEvent;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with BattlePlayers.
    /// </summary>
    public static class BattlePlayerGlobals
    {

    }

    /// <summary>
    /// Class for global values dealing with StarPower.
    /// </summary>
    public static class StarPowerGlobals
    {
        #region Enums

        /// <summary>
        /// The types of Star Power.
        /// <para>PM has the Star Spirits, and TTYD has the Crystal Stars.</para>
        /// </summary>
        public enum StarPowerTypes
        {
            None, StarSpirit, CrystalStar
        }

        #endregion

        #region Command Rank Data

        /// <summary>
        /// The table of Crystal Star Power modifiers based on the highest CommandRank earned.
        /// </summary>
        private static readonly Dictionary<ActionCommand.CommandRank, float> CommandRankModifierTable = new Dictionary<ActionCommand.CommandRank, float>()
        {
            { ActionCommand.CommandRank.None, 0.00f },
            { ActionCommand.CommandRank.NiceM2, 0.50f },
            { ActionCommand.CommandRank.NiceM1, 0.75f },
            { ActionCommand.CommandRank.Nice, 1.00f },
            { ActionCommand.CommandRank.Good, 1.25f },
            { ActionCommand.CommandRank.Great, 1.50f },
            { ActionCommand.CommandRank.Wonderful, 1.75f },
            { ActionCommand.CommandRank.Excellent, 2.00f }
        };

        /// <summary>
        /// The table of Crystal Star Power modifiers based on whether any Stylish moves were performed or not for a particular CommandRank.
        /// </summary>
        private static readonly Dictionary<ActionCommand.CommandRank, float> StylishModifierTable = new Dictionary<ActionCommand.CommandRank, float>()
        {
            { ActionCommand.CommandRank.None, 1.00f },
            { ActionCommand.CommandRank.NiceM2, 3.00f },
            { ActionCommand.CommandRank.NiceM1, 3.50f },
            { ActionCommand.CommandRank.Nice, 4.00f },
            { ActionCommand.CommandRank.Good, 4.50f },
            { ActionCommand.CommandRank.Great, 5.00f },
            { ActionCommand.CommandRank.Wonderful, 5.50f },
            { ActionCommand.CommandRank.Excellent, 6.00f }
        };

        /// <summary>
        /// Gets the total CommandRank value based on how well Mario or his Partner performed an Action Command.
        /// This is factored in when calculating the amount of Crystal Star Star Power gained from an attack.
        /// </summary>
        /// <param name="highestRank">The highest CommandRank earned while performing the Action Command.</param>
        /// <param name="performedStylish">Whether any Stylish moves were performed or not.</param>
        /// <returns>A float of the CommandRank value.</returns>
        public static float GetCommandRankValue(ActionCommand.CommandRank highestRank, bool performedStylish)
        {
            if (performedStylish == true) return StylishModifierTable[highestRank];
            else return CommandRankModifierTable[highestRank];
        }

        #endregion

        #region Danger Status Values

        public const float NormalMod = 1f;

        public const float MarioDangerMod = 2f;
        public const float MarioPerilMod = 3f;

        public const float PartnerDangerMod = 1.5f;
        public const float PartnerPerilMod = 2f;

        /// <summary>
        /// Gets Mario's Danger status value based on his current HealthState.
        /// </summary>
        /// <param name="partner">Mario.</param>
        /// <returns>A float of Mario's Danger status value.</returns>
        private static float GetMarioDangerStatusValue(BattleMario mario)
        {
            if (mario == null)
            {
                Debug.LogError($"{nameof(mario)} is null, which should never happen");
                return NormalMod;
            }

            Enumerations.HealthStates marioHealthState = mario.HealthState;

            switch (marioHealthState)
            {
                case Enumerations.HealthStates.Normal:
                    return NormalMod;
                case Enumerations.HealthStates.Danger:
                    return MarioDangerMod;
                case Enumerations.HealthStates.Peril:
                case Enumerations.HealthStates.Dead:
                default:
                    return MarioPerilMod;
            }
        }

        /// <summary>
        /// Gets a Partner's Danger status value based on its current HealthState.
        /// </summary>
        /// <param name="partner">Mario's Partner.</param>
        /// <returns>A float of the Partner's Danger status value.</returns>
        private static float GetPartnerDangerStatusValue(BattlePartner partner)
        {
            if (partner == null)
            {
                return NormalMod;
            }

            Enumerations.HealthStates partnerHealthState = partner.HealthState;

            switch (partnerHealthState)
            {
                case Enumerations.HealthStates.Normal:
                    return NormalMod;
                case Enumerations.HealthStates.Danger:
                    return PartnerDangerMod;
                case Enumerations.HealthStates.Peril:
                case Enumerations.HealthStates.Dead:
                default:
                    return PartnerPerilMod;
            }
        }

        /// <summary>
        /// Gets the total Danger status value for Mario and his Partner based on their HealthStates.
        /// This is factored in when calculating the amount of Crystal Star Star Power gained from an attack.
        /// </summary>
        /// <returns>A float of the Danger status value based on the HealthStates of both Mario and his Partner.</returns>
        public static float GetDangerStatusValue(BattleMario mario, BattlePartner partner)
        {
            float marioDangerStatusValue = GetMarioDangerStatusValue(mario);
            float partnerDangerStatusValue = GetPartnerDangerStatusValue(partner);

            return marioDangerStatusValue * partnerDangerStatusValue;
        }

        #endregion

        #region Constants

        /// <summary>
        /// The amount of Star Power Units (SPU) per usable Star Power (how much SPU each full bar/circle equates to).
        /// </summary>
        public const float SPUPerStarPower = 100f;

        /// <summary>
        /// The amount of Star Spirit Star Power the Focus move gives.
        /// </summary>
        public const float FocusSPUGain = SPUPerStarPower / 2f;

        /// <summary>
        /// The amount of additional Star Spirit Star Power each Deep Focus Badge gives to Focus.
        /// </summary>
        public const float DeepFocusSPUIncrease = SPUPerStarPower / 4f;

        /// <summary>
        /// The amount of Star Spirit Star Power Mario gains each turn.
        /// </summary>
        public const float StarSpiritSPUPerTurn = SPUPerStarPower / 8f;

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with StatusEffects
    /// </summary>
    public static class StatusGlobals
    {
        #region Enums

        public enum PaybackTypes
        {
            Constant, Half, Full
        }

        #endregion

        #region Structs

        /// <summary>
        /// Holds information about Payback damage
        /// </summary>
        public struct PaybackHolder
        {
            /// <summary>
            /// The type of Payback damage
            /// </summary>
            public PaybackTypes PaybackType { get; private set; }

            /// <summary>
            /// The Elemental damage dealt
            /// </summary>
            public Enumerations.Elements Element { get; private set; }

            /// <summary>
            /// The amount of damage to deal.
            /// <para>If the PaybackType is Constant, this is the total damage dealt. Otherwise, this damage is added to the total.</para>
            /// </summary>
            public int Damage { get; private set; }

            /// <summary>
            /// The Status Effects to inflict
            /// </summary>
            public StatusEffect[] StatusesInflicted { get; private set; }

            public static PaybackHolder Default => new PaybackHolder(PaybackTypes.Constant, Enumerations.Elements.Normal, 1, null);

            public PaybackHolder(PaybackTypes paybackType, Enumerations.Elements element, params StatusEffect[] statusesInflicted)
            {
                PaybackType = paybackType;
                Element = element;
                Damage = 0;
                StatusesInflicted = statusesInflicted;
            }

            public PaybackHolder(PaybackTypes paybackType, Enumerations.Elements element, int constantDamage, params StatusEffect[] statusesInflicted)
                : this(paybackType, element, statusesInflicted)
            {
                Damage = constantDamage;
            }

            /// <summary>
            /// Gets the Payback damage that this PaybackHolder deals
            /// </summary>
            /// <param name="damageDealt">The amount of damage to deal</param>
            /// <returns>All the damage dealt, half of it, or a constant amount of damage based on the PaybackType</returns>
            public int GetPaybackDamage(int damageDealt)
            {
                switch (PaybackType)
                {
                    case PaybackTypes.Full: return damageDealt + Damage;
                    case PaybackTypes.Half: return (damageDealt / 2) + Damage;
                    default: return Damage;
                }
            }
        }

        /// <summary>
        /// Holds information about Confusion
        /// </summary>
        public struct ConfusionHolder
        {
            public int ConfusionPercent { get; private set; }

            public ConfusionHolder(int confusionPercent)
            {
                ConfusionPercent = confusionPercent;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Defines the priority of StatusEffects. Higher priorities affect BattleEntities sooner.
        /// <para>Related StatusEffects are grouped together in lines for readability</para>
        /// </summary>
        private readonly static Dictionary<Enumerations.StatusTypes, int> StatusOrder = new Dictionary<Enumerations.StatusTypes, int>()
        {
            { Enumerations.StatusTypes.WaterBlock, 250 }, { Enumerations.StatusTypes.CloudNine, 249 }, { Enumerations.StatusTypes.TurboCharge, 248 },
            { Enumerations.StatusTypes.Poison, 200 }, { Enumerations.StatusTypes.Burn, 199 },
            { Enumerations.StatusTypes.Fast, 150 }, { Enumerations.StatusTypes.Slow, 149 },
            { Enumerations.StatusTypes.Stone, 2 }, { Enumerations.StatusTypes.Sleep, 147 }, { Enumerations.StatusTypes.Immobilized, 146 }, { Enumerations.StatusTypes.Frozen, 145 }, { Enumerations.StatusTypes.Injured, 144 }, { Enumerations.StatusTypes.Paralyzed, 143 },
            { Enumerations.StatusTypes.POWDown, 130 }, { Enumerations.StatusTypes.POWUp, 129 }, { Enumerations.StatusTypes.DEFDown, 128 }, { Enumerations.StatusTypes.DEFUp, 127 },
            { Enumerations.StatusTypes.Soft, 110 }, { Enumerations.StatusTypes.Tiny, 109 }, { Enumerations.StatusTypes.Huge, 108 },
            { Enumerations.StatusTypes.HPRegen, 90 }, { Enumerations.StatusTypes.FPRegen, 89 },
            { Enumerations.StatusTypes.Dizzy, 80 }, { Enumerations.StatusTypes.Dodgy, 79 },
            { Enumerations.StatusTypes.Electrified, 70 }, { Enumerations.StatusTypes.Invisible, 69 },
            { Enumerations.StatusTypes.Confused, 50 },
            { Enumerations.StatusTypes.Payback, 20 },
            { Enumerations.StatusTypes.NoSkills, 10 },
            { Enumerations.StatusTypes.Charged, 5 },
            { Enumerations.StatusTypes.Allergic, 1 }
        };

        #endregion

        #region Constants

        /// <summary>
        /// Denotes a duration value for a StatusEffect that does not go away
        /// </summary>
        public const int InfiniteDuration = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the Priority value of a particular type of StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusType to get priority for</param>
        /// <returns>The Priority value corresponding to the StatusType if it has an entry, otherwise 0</returns>
        public static int GetStatusPriority(Enumerations.StatusTypes statusType)
        {
            if (StatusOrder.ContainsKey(statusType) == false) return 0;

            return StatusOrder[statusType];
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with Partners.
    /// </summary>
    public static class PartnerGlobals
    {
        #region Enums

        /// <summary>
        /// The Ranks for Partners.
        /// </summary>
        public enum PartnerRanks
        {
            Normal = 1, Super = 2, Ultra = 3
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with general equipment
    /// </summary>
    public static class EquipmentGlobals
    {
        #region Enums

        /// <summary>
        /// The types of Boot levels for Mario.
        /// </summary>
        public enum BootLevels
        {
            Normal = 1, Super = 2, Ultra = 3
        }

        /// <summary>
        /// The types of Hammer levels for Mario.
        /// </summary>
        public enum HammerLevels
        {
            Normal = 1, Super = 2, Ultra = 3
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with Badges
    /// </summary>
    public static class BadgeGlobals
    {
        #region Fields

        /// <summary>
        /// The max number of Simplifiers that can affect ActionCommands.
        /// </summary>
        public const int MaxSimplifierCount = 3;

        /// <summary>
        /// The max number of Unsimplifiers that can affect ActionCommands.
        /// </summary>
        public const int MaxUnsimplifierCount = 3;

        #endregion

        #region Enums

        /// <summary>
        /// The various types of Badges (what the actual Badges are).
        /// <para>The values are defined by each Badge type's Type Number.
        /// If Badges exist in the same spot and aren't in both games, Badges with lower alphabetical values will be placed first.
        /// In cases where one Badge is before another Badge in one game and after that Badge in the other game, the Badge is grouped
        /// with similar Badges around it.</para>
        /// <para>If adding brand new badges, put them at the bottom.</para>
        /// </summary>
        public enum BadgeTypes
        {
            //Default value
            None = 0,
            PowerJump = 1, MegaJump = 2, Multibounce = 3, JumpCharge = 4, SJumpCharge = 5, ShrinkStomp = 6,
            SleepStomp = 7, DizzyStomp = 8, SoftStomp = 9, DDownJump = 10, TornadoJump = 11,
            PowerBounce = 12, PowerSmash = 13, MegaSmash = 14, PiercingBlow = 14,
            SmashCharge = 15, SSmashCharge = 16, SpinSmash = 17, HammerThrow = 18,
            HeadRattle = 19, IceSmash = 20,
            QuakeHammer = 21, PowerQuake = 22, MegaQuake = 23, DDownPound = 24,
            FireDrive = 25, Charge = 26, ChargeP = 27,
            DoubleDip = 28, DoubleDipP = 29, TripleDip = 30, GroupFocus = 31, 
            DodgeMaster = 32, DeepFocus = 33, HPPlus = 34, HPPlusP = 35, FPPlus = 36,
            PowerPlus = 37, PowerPlusP = 38, AllOrNothing = 39, Jumpman = 40, Hammerman = 41,
            PUpDDown = 42, PUpDDownP = 43, PDownDUp = 44, PDownDUpP = 45,
            DefendPlus = 46, DefendPlusP = 47, DamageDodge = 48, DamageDodgeP = 49,
            DoublePain = 50, PowerRush = 51, PowerRushP = 52, LastStand = 53, LastStandP = 54,
            MegaRush = 55, MegaRushP = 56, CloseCall = 57, CloseCallP = 58,
            PrettyLucky = 59, PrettyLuckyP = 60, LuckyDay = 61, LuckyStart = 62,
            HappyHeart = 63, HappyHeartP = 64, HappyFlower = 65,
            FlowerSaver = 66, FlowerSaverP = 67, PityFlower = 68, HPDrain = 69, HPDrainP = 70,
            FPDrain = 71, HeartFinder = 72, FlowerFinder = 73, ItemHog = 74, RunawayPay = 75,
            Refund = 76, PayOff = 77, MoneyMoney = 78,
            IcePower = 79, FireShield = 80, SpikeShield = 81,
            ZapTap = 82, ReturnPostage = 83,
            FeelingFine = 84, FeelingFineP = 85, SuperAppeal = 86, SuperAppealP = 87,
            Peekaboo = 88, ISpy = 89, QuickChange = 90, TimingTutor = 91,
            Simplifier = 92, Unsimplifier = 93, ChillOut = 94,
            SpeedySpin = 95, DizzyAttack = 96, SpinAttack = 97, FirstAttack = 98, BumpAttack = 99,
            LEmblem = 100, WEmblem = 101, SlowGo = 102,
            AttackFXA = 103, AttackFXB = 104, AttackFXC = 105, AttackFXD = 106, AttackFXE = 107,
            AttackFXR = 108, AttackFXY = 109, AttackFXG = 110, AttackFXP = 111,
            //Unused & Beta Badges
            AngersPower = 112
            //New badges
        }

        /// <summary>
        /// Who the Badge affects.
        /// <para>For Players, Self refers to Mario. For Enemies, Partner doesn't have any effect.
        /// Both is for Badges such as Simplifier and Unsimplifier that affect both Mario and Partners.</para>
        /// </summary>
        public enum AffectedTypes
        {
            Self, Partner, Both
        }

        /// <summary>
        /// Filter options for finding Badges
        /// </summary>
        public enum BadgeFilterType
        {
            All, Equipped,UnEquipped
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a non-Partner BadgeTypes corresponding to a particular BadgeTypes.
        /// </summary>
        /// <param name="pBadgeType">The Partner version of the BadgeTypes to find a non-Partner version for.</param>
        /// <returns>A non-Partner version of the BadgeTypes passed in. If already a non-Partner version, it will be returned. null if none was found.</returns>
        public static BadgeTypes? GetNonPartnerBadgeType(BadgeTypes pBadgeType)
        {
            string pBadgeName = pBadgeType.ToString();

            //Check the last character for a "P"
            string checkP = pBadgeName.Substring(pBadgeName.Length - 1);

            //This is the non-Partner version, so return it
            if (checkP != "P")
                return pBadgeType;

            //Remove the "P" and see if there is a corresponding value
            string nonPBadgeName = pBadgeName.Substring(0, pBadgeName.Length - 1);

            BadgeTypes nonPBadgeType;
            bool success = Enum.TryParse(nonPBadgeName, out nonPBadgeType);

            if (success == true) return nonPBadgeType;
            return null;
        }

        /// <summary>
        /// Returns a Partner BadgeTypes corresponding to a particular non-Partner BadgeTypes.
        /// </summary>
        /// <param name="pBadgeType">The non-Partner version of the BadgeTypes to find a Partner version for.</param>
        /// <returns>A Partner version of the BadgeTypes passed in. If already a Partner version, it will be returned. null if none was found.</returns>
        public static BadgeTypes? GetPartnerBadgeType(BadgeTypes badgeType)
        {
            string badgeName = badgeType.ToString();

            //Check the last character for a "P"
            string checkP = badgeName.Substring(badgeName.Length - 1);
            
            //This is the Partner version, so return it
            if (checkP == "P")
                return badgeType;

            //Add a "P" and see if there is a corresponding value
            string pBadgeName = badgeName + "P";

            BadgeTypes pBadgeType;
            bool success = Enum.TryParse(pBadgeName, out pBadgeType);

            if (success == true) return pBadgeType;
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Class for global values dealing with rendering
    /// </summary>
    public static class RenderingGlobals
    {
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;
    }

    public static class AudioGlobals
    {
        
    }

    /// <summary>
    /// Class for global values dealing with loading and unloading content
    /// </summary>
    public static class ContentGlobals
    {
        public const string ContentRoot = "Content";
        public const string AudioRoot = "Audio";
        public const string SoundRoot = "Audio/SFX/";
        public const string MusicRoot = "Audio/Music/";
        public const string SpriteRoot = "Sprites";
        public const string UIRoot = "UI";
    }

    /// <summary>
    /// Class for global utility functions
    /// </summary>
    public static class UtilityGlobals
    {
        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
        public static float Clamp(float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;
        public static double Clamp(double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;
        public static uint Clamp(uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

        public static int Wrap(int value, int min, int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(float value, float min, float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(double value, double min, double max) => (value < min) ? max : (value > max) ? min : value;

        public static T Min<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) < 0) ? val1 : (val2.CompareTo(val1) < 0) ? val2 : val1;
        public static T Max<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) > 0) ? val1 : (val2.CompareTo(val1) > 0) ? val2 : val1;

        /// <summary>
        /// Tests a random condition.
        /// </summary>
        /// <param name="minValue">The minimum possible value</param>
        /// <param name="maxValue">The maximum possible value</param>
        /// <param name="valueTested">The value to test against</param>
        /// <param name="checkEquals">If true, will also check if the randomized value matches the value tested</param>
        /// <returns>true if the condition succeeded, false otherwise</returns>
        //NOTE: (Leaving this here for now)
        //TTYD checks rand(100) < enemy susceptibility for a given value, such as chance of being inflicted with Dizzy
        //Clock Out and Showstopper are a bit different:
        //Clock Out has a 1x multiplier if the bar is filled at all and a 1.27x multiplier if the bar is full
        //Showstopper has a .5x multiplier that's increased by .1x for each successful button set, totaling a 1x multiplier
        //These multipliers are multiplied by the random value to increase or decrease the chances of the condition evaluating to true

        public static bool TestRandomCondition(int minValue, int maxValue, int valueTested, bool checkEquals)
        {
            int value = GeneralGlobals.Randomizer.Next(minValue, maxValue);

            return (checkEquals == false) ? (value < valueTested) : (value <= valueTested);
        }

        /// <summary>
        /// Chooses a random index in a list of percentages
        /// </summary>
        /// <param name="percentages">The container of percentages, each with positive values, with the sum adding up to 1</param>
        /// <returns>The index in the container of percentages that was chosen</returns>
        public static int ChoosePercentage(IList<double> percentages)
        {
            double randomVal = GeneralGlobals.Randomizer.NextDouble();
            double value = 0d;

            for (int i = 0; i < percentages.Count; i++)
            {
                value += percentages[i];
                if (value > randomVal)
                {
                    return i;
                }
            }

            //Return the last one if it goes through
            return percentages.Count - 1;
        }

        public static T[] GetEnumValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
