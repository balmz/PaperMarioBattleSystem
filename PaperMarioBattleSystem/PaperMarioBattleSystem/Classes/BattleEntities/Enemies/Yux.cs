﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Yux. They create 1 Mini-Yux per turn up to a max of 2; Mini-Yuxes put a shield around them that makes them invincible.
    /// </summary>
    public class Yux : BattleEnemy, ITattleableEntity
    {
        /// <summary>
        /// How many Mini-Yuxes the Yux creates each turn.
        /// </summary>
        protected int MiniYuxesPerTurn = 1;

        /// <summary>
        /// The maximum number of Mini-Yuxes the Yux can have.
        /// </summary>
        protected int MaxMiniYuxes = 2;

        /// <summary>
        /// How many Mini-Yuxes it takes to get a shield.
        /// </summary>
        protected int ShieldOnNumMiniYuxes = 1;

        /// <summary>
        /// Tells if the Yux has a shield or not.
        /// </summary>
        protected bool HasShield => this.IsInvincible();

        /// <summary>
        /// The number of Mini-Yuxes the Yux created.
        /// </summary>
        protected int NumMiniYuxes => MiniYuxes.Count;

        /// <summary>
        /// The Mini-Yuxes the Yux has in place.
        /// </summary>
        private readonly List<BattleEntity> MiniYuxes = new List<BattleEntity>();

        public Yux() : base(new Stats(12, 3, 0, 2, 0))
        {
            Name = "Yux";
            
            ChangeHeightState(HeightStates.Airborne);

            Scale = new Vector2(.3f, .3f);

            EntityProperties.AddStatusProperty(StatusTypes.Sleep, new StatusPropertyHolder(30, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Dizzy, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Confused, new StatusPropertyHolder(70, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Immobilized, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(StatusTypes.DEFDown, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Burn, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Frozen, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Fright, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(StatusTypes.Blown, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(StatusTypes.KO, new StatusPropertyHolder(95, 0));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Yux");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(45, 50, 187, 189), 1000d)));
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            //Do nothing for now; we'll implement the attack later
            StartAction(new NoAction(), false, null);
        }

        public override void CleanUp()
        {
            base.CleanUp();

            //Clean up all links to the Mini-Yuxes
            for (int i = 0; i < MiniYuxes.Count; i++)
            {
                //Set to null since the Yux the Mini-Yux was helping is out of battle
                MiniYuxes[i].EntityProperties.AddAdditionalProperty(AdditionalProperty.HelperEntity, null);

                //Unsubscribe from event
                MiniYuxes[i].DamageTakenEvent -= OnMiniYuxDamageTaken;

                //Remove the Mini-Yux from the list
                MiniYuxes.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Handles adding/removing the Yux's Status Effect immunities.
        /// </summary>
        /// <param name="immune">Whether to add or remove the immunity.
        /// This should be true when adding the shield and false when removing it.</param>
        protected void AddRemoveImmunities(bool immune)
        {
            //Yuxes become immune to all Status Effects with the shield on
            StatusTypes[] allStatusTypes = UtilityGlobals.GetEnumValues<StatusTypes>();
            for (int i = 0; i < allStatusTypes.Length; i++)
            {
                this.AddRemoveStatusImmunity(allStatusTypes[i], immune);
            }
        }

        public override void OnPhaseEnd()
        {
            base.OnPhaseEnd();

            //Create Mini-Yuxes when the phase ends (they do this before Bobbery Bombs go in the actual games)
            //If the Yux is immobile or dead, don't do anything
            if (IsDead == true || EntityProperties.HasAdditionalProperty(AdditionalProperty.Immobile) == true)
                return;

            //NOTE: Rearranging should eventually go in a BattleEvent so we can replicate the animation from the games

            //Tell if we created any Mini-Yuxes so we can rearrange their formation if so
            bool createdAny = false;

            //Create Mini-Yuxes
            for (int i = 0; i < MiniYuxesPerTurn; i++)
            {
                //Stop if we have the max number of Mini-Yuxes
                if (NumMiniYuxes >= MaxMiniYuxes)
                    break;

                //Create a new Mini-Yux
                MiniYux miniYux = new MiniYux();

                //Mark it as a Helper, telling that it helps the Yux
                miniYux.EntityProperties.AddAdditionalProperty(AdditionalProperty.HelperEntity, this);

                //Handle when the Mini-Yux takes damage
                //If it dies, we want to remove it from the list
                miniYux.DamageTakenEvent -= OnMiniYuxDamageTaken;
                miniYux.DamageTakenEvent += OnMiniYuxDamageTaken;

                //Add the Mini-Yux to the Yux's list
                MiniYuxes.Add(miniYux);

                //Add the Mini-Yux to battle with the same BattleIndex as the Yux
                BattleManager.Instance.AddEntities(new BattleEntity[] { miniYux }, new int[] { BattleIndex }, true);

                createdAny = true;
            }

            //If we created any Mini-Yuxes, set the formation and add the shield if necessary
            if (createdAny == true)
            {
                //Add the shield if the Yux doesn't have it
                if (HasShield == false)
                {
                    AddRemoveShield(true);
                }

                //Get the formation
                Vector2[] formation = GetMiniFormation(NumMiniYuxes);

                for (int i = 0; i < MiniYuxes.Count; i++)
                {
                    //Set position and battle position
                    MiniYuxes[i].SetBattlePosition(BattlePosition + formation[i]);
                    MiniYuxes[i].Position = MiniYuxes[i].BattlePosition;
                }

                //Re-sort the enemy list since their positions have changed
                BattleManager.Instance.SortEntityList(EntityType);
            }
        }

        /// <summary>
        /// Gets the formation of Mini-Yuxes based on how many there are.
        /// The formation returns BattlePositions relative to the Yux's BattlePosition.
        /// </summary>
        /// <param name="numMinis">The number of Mini-Yuxes.</param>
        /// <returns>An array of Vector2s representing the relative BattlePosition of each Mini-Yux from the Yux.</returns>
        protected virtual Vector2[] GetMiniFormation(int numMinis)
        {
            switch (numMinis)
            {
                case 1: return new Vector2[] { new Vector2(-15, 0) };
                case 2: return new Vector2[] { new Vector2(-15, 0), new Vector2(15, 0) };
                case 3: return new Vector2[] { new Vector2(-15, 15), new Vector2(0, -15), new Vector2(15, 15) };
                case 4: return new Vector2[] { new Vector2(-15, -15), new Vector2(-15, 15), new Vector2(15, -15), new Vector2(15, 15) };
                default:
                    return new Vector2[0];
            }
        }

        /// <summary>
        /// Adds or removes the Yux's shield.
        /// </summary>
        /// <param name="add">Whether to add the shield or not. true adds the shield, and false removes the shield.</param>
        protected void AddRemoveShield(bool add)
        {
            if (add == true)
            {
                AddRemoveImmunities(true);
                this.AddIntAdditionalProperty(AdditionalProperty.Invincible, 1);

                //Make it orange for now to indicate a shield
                TintColor = Color.Orange;
            }
            else
            {
                AddRemoveImmunities(false);
                this.SubtractIntAdditionalProperty(AdditionalProperty.Invincible, 1);

                TintColor = Color.White;
            }
        }

        #region Event Handlers

        private void OnMiniYuxDamageTaken(InteractionHolder damageInfo)
        {
            //NOTE: This should happen after the Mini-Yuxes finish their death animations so the shield stays up until they're gone

            //If the Mini-Yux is dead, remove it
            if (damageInfo.Entity.IsDead == true)
            {
                BattleEntity miniYux = damageInfo.Entity;
                
                //Remove the Mini-Yux from the list and unsubscribe from its event
                MiniYuxes.Remove(miniYux);
                miniYux.DamageTakenEvent -= OnMiniYuxDamageTaken;

                //Remove the Helper AdditionalProperty since the Mini-Yux is out of battle
                miniYux.EntityProperties.RemoveAdditionalProperty(AdditionalProperty.HelperEntity);

                //Remove the shield if there are no more Mini-Yuxes
                if (NumMiniYuxes == 0 && HasShield == true)
                {
                    AddRemoveShield(false);
                }
            }
        }

        #endregion

        #region Tattle Information

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                "These pathetically ugly creatures were created in X-Naut laboratories." +
                "With Mini-Yux around them, they're impervious to all attacks."
            };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Yux. Says here it's a creature created in the X-Naut labs." +
                "Max HP is 3, Attack is 2, and Defense is 0. According to this, attacks and" +
                "items won't affect it if it has Mini-Yux around it. So, if any Mini-Yux appear, take those out first. Duh!"
            };
        }

        #endregion
    }
}