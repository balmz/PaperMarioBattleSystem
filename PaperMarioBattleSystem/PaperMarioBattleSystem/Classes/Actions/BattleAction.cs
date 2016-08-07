﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An action that is performed by a BattleEntity in battle
    /// </summary>
    public abstract class BattleAction
    {
        /// <summary>
        /// Values for each branch of a sequence.
        /// BattleActions switch branches based on what happens.
        /// <para>The None branch is only used to indicate whether to jump to a certain branch after the sequence updates.
        /// The most common use-case is switching to the Interruption branch</para>
        /// </summary>
        public enum SequenceBranch
        {
            None, Start, End, Command, Success, Failed, Interruption, Miss
        }

        /// <summary>
        /// A delegate for handling the sequence interruption branch.
        /// It should follow the same conventions as the other branches
        /// </summary>
        protected delegate void InterruptionDelegate();

        #region Fields/Properties

        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name { get; protected set; } = "Action";

        /// <summary>
        /// The icon representing the action
        /// </summary>
        public Texture2D Icon { get; protected set; } = null;

        /// <summary>
        /// How much FP it costs to use the action
        /// </summary>
        public int FPCost { get; protected set; } = 0;

        /// <summary>
        /// The base damage of the action
        /// </summary>
        public int BaseDamage { get; protected set; } = 0;

        /// <summary>
        /// The description of the action
        /// </summary>
        public string Description { get; protected set; } = "Error";

        /// <summary>
        /// The amount of entities this action can select
        /// </summary>
        public EntitySelectionType SelectionType { get; protected set; } = EntitySelectionType.Single;

        /// <summary>
        /// The type of entities this action selects
        /// </summary>
        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        /// <summary>
        /// The type of Elemental damage this action deals
        /// </summary>
        public Elements Element { get; protected set; } = Elements.Normal;

        /// <summary>
        /// The type of contact this action makes
        /// </summary>
        public ContactTypes ContactType { get; protected set; } = ContactTypes.None;

        /// <summary>
        /// The heights of enemies this action affects
        /// </summary>
        public HeightStates[] HeightsAffected { get; protected set; } = null;

        /// <summary>
        /// The Status Effects this action can afflict
        /// </summary>
        public StatusEffect[] StatusesInflicted { get; protected set; } = null;

        /// <summary>
        /// The user of this action.
        /// Aside from the Guard action, it will be the entity whose turn it currently is
        /// </summary>
        public virtual BattleEntity User => BattleManager.Instance.EntityTurn;

        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        public ActionCommand Command { get; protected set; } = null;

        /// <summary>
        /// Whether the action's sequence is being performed or not
        /// </summary>
        public bool InSequence { get; protected set; } = false;

        /// <summary>
        /// The current step of the sequence
        /// </summary>
        public int SequenceStep { get; protected set; } = 0;

        /// <summary>
        /// The current branch of the sequence
        /// </summary>
        protected SequenceBranch CurBranch { get; private set; } = SequenceBranch.Start;

        /// <summary>
        /// The current SequenceAction being performed
        /// </summary>
        protected SequenceAction CurSequence { get; set; } = null;

        protected BattleEntity[] EntitiesAffected { get; private set; } = null;

        /// <summary>
        /// A value denoting if we should jump to a particular branch or not after the sequence progresses.
        /// This allows the sequences to remain flexible and not cause any sequence or branch conflicts with this branch
        /// </summary>
        protected SequenceBranch JumpToBranch { get; private set; } = SequenceBranch.None;

        /// <summary>
        /// The handler used for interruptions. This exists so each action can specify different handlers for
        /// different types of damage. It defaults to the base method at the start and end of each action
        /// </summary>
        protected InterruptionDelegate InterruptionHandler = null;

        /// <summary>
        /// Tells whether the action command is enabled or not.
        /// Action commands are always disabled for enemies
        /// </summary>
        protected bool CommandEnabled => (Command != null && User.EntityType != EntityTypes.Enemy);

        #endregion

        protected BattleAction()
        {
            InterruptionHandler = BaseInterruptionHandler;
        }

        /// <summary>
        /// Attempt to deal damage to a set of entities with this BattleAction.
        /// <para>Based on the ContactType of this BattleAction, this can fail, resulting in an interruption.
        /// In the event of an interruption, no further entities are tested, the ActionCommand is ended, and 
        /// we go into the Interruption branch</para>
        /// </summary>
        /// <param name="damage">The damage the BattleAction deals to the entity if the attempt was successful</param>
        /// <param name="entities">The BattleEntities to attempt to inflict damage on</param>
        // <param name="isTotalDamage">Indicates if the damage value passed in is already the total damage or not.
        // Some BattleActions such as Power Bounce need to calculate the total damage ahead of time.</param>
        protected void AttemptDamage(int damage, BattleEntity[] entities/*, bool isTotalDamage = false*/)
        {
            if (entities == null || entities.Length == 0)
            {
                Debug.LogWarning($"{nameof(entities)} is null or empty in {nameof(AttemptDamage)} for Action {Name}!");
                return;
            }

            //Get the true total damage, factoring in the attacker's properties
            //If the damage passed in is designated as the total damage, use that damage value instead
            int totalDamage = damage;//isTotalDamage == true ? damage : GetTotalDamage(damage);

            //Go through all the entities and attempt damage
            for (int i = 0; i < entities.Length; i++)
            {
                BattleEntity victim = entities[i];

                InteractionResult finalResult = Interactions.GetDamageInteraction(User, victim, totalDamage, Element, ContactType, StatusesInflicted);

                //Make the victim take damage upon a PartialSuccess or a Success
                if (finalResult.VictimResult.HasValue == true)
                {
                    //Check if the attacker hit
                    if (finalResult.VictimResult.Hit == true)
                    {
                        finalResult.VictimResult.Entity.TakeDamage(finalResult.VictimResult);
                    }
                    //Handle a miss otherwise
                    else
                    {
                        OnMiss();
                    }
                }

                //Make the attacker take damage upon a PartialSuccess or a Failure
                //Break out of the loop when the attacker takes damage
                if (finalResult.AttackerResult.HasValue == true)
                {
                    finalResult.AttackerResult.Entity.TakeDamage(finalResult.AttackerResult);

                    break;
                }
            }
        }

        /// <summary>
        /// Convenience function for attempting damage with only one entity.
        /// </summary>
        /// <param name="damage">The damage the BattleAction deals to the entity if the attempt was successful</param>
        /// <param name="entity">The BattleEntity to attempt to inflict damage on</param>
        protected void AttemptDamage(int damage, BattleEntity entity)
        {
            AttemptDamage(damage, new BattleEntity[] { entity });
        }

        /// <summary>
        /// Gets the total raw damage a BattleEntity can deal using this BattleAction
        /// </summary>
        /// <param name="actionDamage">The damage the BattleAction deals</param>
        /// <returns>An int with the total raw damage the BattleEntity can deal when using this BattleAction</returns>
        protected int GetTotalDamage(int actionDamage)
        {
            int totalDamage = actionDamage + User.BattleStats.Attack;

            totalDamage = UtilityGlobals.Clamp(totalDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            return totalDamage;
        }

        /// <summary>
        /// Starts the action sequence
        /// </summary>
        /// <param name="targets">The targets to perform the sequence on</param>
        public void StartSequence(params BattleEntity[] targets)
        {
            CurBranch = SequenceBranch.Start;
            InSequence = true;
            SequenceStep = 0;

            ChangeJumpBranch(SequenceBranch.None);

            EntitiesAffected = targets;
            //Brace for the attack
            for (int i = 0; i < EntitiesAffected.Length; i++)
                EntitiesAffected[i].BraceAttack(User);

            InterruptionHandler = BaseInterruptionHandler;

            OnStart();

            //Start the first sequence
            ProgressSequence(0);
        }

        /// <summary>
        /// BattleAction-specific logic when the action is started
        /// </summary>
        protected virtual void OnStart()
        {
            
        }

        /// <summary>
        /// Ends the action sequence
        /// </summary>
        public void EndSequence()
        {
            CurBranch = SequenceBranch.End;
            InSequence = false;
            SequenceStep = 0;
            CurSequence = null;

            ChangeJumpBranch(SequenceBranch.None);

            //Stop bracing for the attack
            for (int i = 0; i < EntitiesAffected.Length; i++)
                EntitiesAffected[i].StopBracing();

            EntitiesAffected = null;

            InterruptionHandler = BaseInterruptionHandler;

            OnEnd();

            if (User == BattleManager.Instance.EntityTurn)
            {
                User.EndTurn();
            }
        }

        /// <summary>
        /// BattleAction-specific logic when the action is complete
        /// </summary>
        protected virtual void OnEnd()
        {

        }

        /// <summary>
        /// What occurs when the action command is successfully performed
        /// </summary>
        public abstract void OnCommandSuccess();

        /// <summary>
        /// What occurs when the action command is failed
        /// </summary>
        public abstract void OnCommandFailed();

        /// <summary>
        /// Handles BattleAction responses sent from an ActionCommand that are not a definite Success or Failure.
        /// Unlike a Success or Failure, the ActionCommand is not required to send this down at all
        /// <para>For example, the Hammer command sends back the number of lights lit up, and the Hammer action responds
        /// by speeding up Mario's hammer windup animation</para>
        /// </summary>
        /// <param name="response">A number representing a response from the action command</param>
        public abstract void OnCommandResponse(int response);

        /// <summary>
        /// What happens when the BattleAction is selected on the menu.
        /// The default behavior is to start target selection with the ActionStart method
        /// </summary>
        public virtual void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection(ActionStart, SelectionType, BattleManager.Instance.GetEntities(EntityType, HeightsAffected));
        }

        /// <summary>
        /// Clears the menu stack and makes the entity whose turn it is start performing this action
        /// </summary>
        /// <param name="targets"></param>
        private void ActionStart(BattleEntity[] targets)
        {
            BattleUIManager.Instance.ClearMenuStack();
            User.StartAction(this, targets);
        }

        /// <summary>
        /// Prints an error message when an invalid sequence is occurred.
        /// It includes information such as the action and the entity performing it, the sequence branch, and the sequence step
        /// </summary>
        protected void PrintInvalidSequence()
        {
            Debug.LogError($"{User.Name} entered an invalid state in {Name} with a {nameof(SequenceStep)} of {SequenceStep} in {nameof(CurBranch)}: {CurBranch}");
        }

        /// <summary>
        /// Progresses the BattleAction further into its sequence
        /// </summary>
        /// <param name="progressAmount">The amount to progress the sequence</param>
        private void ProgressSequence(uint progressAmount)
        {
            SequenceStep += (int)progressAmount;

            OnProgressSequence();
            if (InSequence == true)
            {
                CurSequence.Start();
            }
        }

        /// <summary>
        /// Switches to a new sequence branch. This also resets the current step
        /// </summary>
        /// <param name="newBranch">The new branch to switch to</param>
        protected void ChangeSequenceBranch(SequenceBranch newBranch)
        {
            CurBranch = newBranch;

            //Set to -1 as it'll be incremented next time the sequence progresses
            SequenceStep = -1;
        }

        /// <summary>
        /// Sets the branch to jump to after the current sequence updates
        /// </summary>
        /// <param name="newJumpBranch">The new branch to jump to</param>
        protected void ChangeJumpBranch(SequenceBranch newJumpBranch)
        {
            JumpToBranch = newJumpBranch;
        }

        /// <summary>
        /// What occurs next in the sequence when it's progressed.
        /// </summary>
        private void OnProgressSequence()
        {
            switch (CurBranch)
            {
                case SequenceBranch.Start:
                    SequenceStartBranch();
                    break;
                case SequenceBranch.Command:
                    SequenceCommandBranch();
                    break;
                case SequenceBranch.Success:
                    SequenceSuccessBranch();
                    break;
                case SequenceBranch.Failed:
                    SequenceFailedBranch();
                    break;
                case SequenceBranch.Interruption:
                    SequenceInterruptionBranch();
                    break;
                case SequenceBranch.Miss:
                    SequenceMissBranch();
                    break;
                case SequenceBranch.End:
                default:
                    SequenceEndBranch();
                    break;
            }
        }

        /// <summary>
        /// The start of the action sequence
        /// </summary>
        protected abstract void SequenceStartBranch();

        /// <summary>
        /// The end of the action sequence
        /// </summary>
        protected abstract void SequenceEndBranch();

        /// <summary>
        /// The part of the action sequence revolving around the action command
        /// </summary>
        protected abstract void SequenceCommandBranch();
        
        /// <summary>
        /// What occurs when the action command for this action is performed successfully
        /// </summary>
        protected abstract void SequenceSuccessBranch();

        /// <summary>
        /// What occurs when the action command for this action is failed
        /// </summary>
        protected abstract void SequenceFailedBranch();

        /// <summary>
        /// What occurs when the action is interrupted.
        /// The most notable example of this is when Mario takes damage from jumping on a spiked enemy
        /// <para>This is overrideable through the InterruptionHandler, as actions can handle this in more than one way</para>
        /// </summary>
        protected void SequenceInterruptionBranch()
        {
            if (InterruptionHandler == null)
            {
                Debug.LogError($"{nameof(InterruptionHandler)} is null for {Name}! This should NEVER happen - look into it ASAP");
                return;
            }

            InterruptionHandler();
        }

        /// <summary>
        /// What occurs when the action misses
        /// </summary>
        protected abstract void SequenceMissBranch();

        /// <summary>
        /// The base interruption handler
        /// </summary>
        protected void BaseInterruptionHandler()
        {
            float moveX = -20f;
            float moveY = 70f;

            double time = 500d;

            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.HurtName, true);

                    Vector2 pos = User.Position + new Vector2(moveX, -moveY);
                    CurSequence = new MoveTo(pos, time / 2d);
                    break;
                case 1:
                    CurSequence = new WaitForAnimation(AnimationGlobals.HurtName);
                    break;
                case 2:
                    CurSequence = new MoveAmount(new Vector2(0f, moveY), time);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        /// <summary>
        /// Starts the interruption, which occurs when a BattleEntity takes damage mid-sequence
        /// </summary>
        /// <param name="element">The elemental damage being dealt</param>
        public void StartInterruption(Elements element)
        {
            //End the ActionCommand's input if it's active
            if (CommandEnabled == true) Command.EndInput();

            ChangeJumpBranch(SequenceBranch.Interruption);

            //Call the action-specific interruption method to set the interruption handler
            OnInterruption(element);
        }

        /// <summary>
        /// How the action handles a miss.
        /// The base implementation is to do nothing, but actions such as Jump may go to the Miss branch
        /// </summary>
        protected virtual void OnMiss()
        {
            Debug.Log($"{User.Name} has missed with the {Name} Action and will act accordingly");
        }

        /// <summary>
        /// Sets the InterruptionHandler based on the type of damage dealt
        /// </summary>
        /// <param name="element">The elemental damage being dealt</param>
        protected virtual void OnInterruption(Elements element)
        {
            InterruptionHandler = BaseInterruptionHandler;
        }

        public void Update()
        {
            //Perform sequence
            if (InSequence == true)
            {
                //If the action command is enabled, let it handle the sequence
                if (CommandEnabled == true)
                {
                    if (Command.AcceptingInput == true)
                        Command.Update();
                }

                CurSequence.Update();
                if (CurSequence.IsDone == true)
                {
                    ProgressSequence(1);
                }
            }
        }

        /// <summary>
        /// Handles anything that needs to be done directly after updating the sequence.
        /// This is where it jumps to a new branch, if it should
        /// </summary>
        public void PostUpdate()
        {
            if (InSequence == true && JumpToBranch != SequenceBranch.None)
            {
                //Change the sequence action itself to cancel out anything that it will be waiting for to finish
                //We don't end the previous sequence action because it has been interrupted by the new branch
                CurSequence = new Wait(0d);
                ChangeSequenceBranch(JumpToBranch);

                ChangeJumpBranch(SequenceBranch.None);
            }
        }

        public void Draw()
        {
            if (InSequence == true)
            {
                if (CommandEnabled == true)
                {
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(SpriteRenderer.Instance.WindowCenter.X, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f, true);

                    Command?.Draw();
                }
            }
        }
    }
}
