﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
    }

    public struct ContactResultInfo
    {
        public Enumerations.Elements Element;
        public Enumerations.ContactResult ContactResult;

        public static ContactResultInfo Default => new ContactResultInfo(Enumerations.Elements.Normal, Enumerations.ContactResult.Success);

        public ContactResultInfo(Enumerations.Elements element, Enumerations.ContactResult contactResult)
        {
            Element = element;
            ContactResult = contactResult;
        }
    }

    #endregion

    #region Classes

    /// <summary>
    /// A class containing all the stats in the game.
    /// Only playable characters use Level, and only Mario uses FP
    /// </summary>
    public class Stats
    {
        public int Level;

        //Max stats
        public int MaxHP;
        public int MaxFP;

        //Base stats going into battle
        public int BaseAttack;
        public int BaseDefense;

        public int HP;
        public int FP;
        public int Attack;
        public int Defense;

        /// <summary>
        /// Default stats
        /// </summary>
        public static Stats Default => new Stats(1, 10, 5, 1, 0);

        public Stats(int level, int maxHp, int maxFP, int attack, int defense)
        {
            Level = level;
            MaxHP = HP = maxHp;
            MaxFP = FP = maxFP;
            BaseAttack = Attack = attack;
            BaseDefense = Defense = defense;
        }
    }

    public class DamageHolder
    {
        public int Damage = 0;
        public Enumerations.Elements Element = Enumerations.Elements.Normal;
        public Enumerations.ContactTypes ContactType = Enumerations.ContactTypes.None;
        public bool Piercing = false;

        public DamageHolder(int damage, Enumerations.Elements element, Enumerations.ContactTypes contactType, bool piercing)
        {
            Damage = damage;
            Element = element;
            ContactType = contactType;
            Piercing = piercing;
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

        public enum BattleActions
        {
            Misc, Item, Jump, Hammer, Focus, Special
        }

        /// <summary>
        /// The types of damage elements
        /// </summary>
        public enum Elements
        {
            Normal, Sharp, Water, Fire, Electric, Ice, Poison, Explosion, Star
        }

        /// <summary>
        /// The main height states an entity can be in.
        /// Some moves may or may not be able to hit entities in certain height states.
        /// </summary>
        public enum HeightStates
        {
            Grounded, Airborne, Ceiling
        }

        /// <summary>
        /// The physical attributes assigned to entities.
        /// These determine if an attack can target a particular entity, or whether there is an advantage
        /// or disadvantage to using a particular attack on an entity with a particular physical attribute.
        /// 
        /// <para>Flying does not mean that the entity is Airborne. Flying entities, such as Ruff Puffs,
        /// can still be damaged by ground moves if they hover at ground level.</para>
        /// </summary>
        public enum PhysicalAttributes
        {
            None, Flying, Spiked, Electrified, Fiery, Poisony, Starry
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
        public const string RunningName = "Run";
        public const string HurtName = "Hurt";
        public const string DeathName = "Death";
        public const string VictoryName = "Victory";

        public const string SpikedTipHurtName = "SpikedTipHurt";

        /// <summary>
        /// Battle animations specific to playable characters
        /// </summary>
        public static class PlayerBattleAnimations
        {
            public const string ChoosingActionName = "ChoosingAction";
            public const string GuardName = "Guard";
            public const string DangerName = "Danger";
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
    }

    /// <summary>
    /// Class for global values dealing with Battles
    /// </summary>
    public static class BattleGlobals
    {
        #region Constants

        public const float DefaultElementModifier = 1f;

        public const int MaxEnemies = 5;

        public const int MinDamage = 0;
        public const int MaxDamage = 99;

        public const int MaxPowerBounces = 100;

        public const int MinDangerHP = 2;
        public const int MaxDangerHP = 5;
        public const int PerilHP = 1;
        public const int DeathHP = 0;

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

        public static int Wrap(int value, int min, int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(float value, float min, float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(double value, double min, double max) => (value < min) ? max : (value > max) ? min : value;

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
    }
}