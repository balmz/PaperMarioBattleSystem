﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Jump action
    /// </summary>
    public class Jump : BattleAction
    {
        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";
            BaseDamage = 1;

            Command = new JumpCommand(this, 1000f, 500f);
        }

        public override void OnCommandSuccess()
        {
            //Show "NICE" here or something
            SequenceStep += 1;
        }

        public override void OnCommandFailed()
        {

        }

        public override void OnMenuSelected()
        {
            base.OnMenuSelected();
        }

        protected override void OnProgressSequence()
        {
            //NOTE: Add a MoveAmountTowards class or something so you can have them move towards the center from their
            //battle positions, eliminating the need for this check entirely
            float x = User.EntityType == Enumerations.EntityTypes.Player ? 100f : -100f;

            switch(SequenceStep)
            {
                case 0:
                    CurSequence = new MoveAmount(new Vector2(x, 0f), 1000f);
                    break;
                case 1:
                    CurSequence = new MoveAmount(new Vector2(0f, -100f), 1000f);
                    break;
                case 2:
                    if (SequenceStep == 2 && CommandEnabled == true) Command.StartInput();
                    CurSequence = new MoveAmount(new Vector2(0f, 100f), 1000f);
                    break;
                case 3:
                    DealDamage(BaseDamage);
                    base.OnProgressSequence();
                    if (SequenceStep == 3) SequenceStep += 500;
                    break;
                case 4:
                    DealDamage(BaseDamage);
                    goto case 1;
                case 5:
                    goto case 2;
                case 6:
                    goto case 3;
                default:
                    EndSequence();
                    break;
            }
        }
    }
}
