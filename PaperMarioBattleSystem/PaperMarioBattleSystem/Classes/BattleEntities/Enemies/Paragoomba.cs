﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Paragoomba - A Goomba with wings.
    /// </summary>
    public sealed class Paragoomba : Goomba, ITattleableEntity, IWingedEntity
    {
        public IWingedBehavior WingedBehavior { get; private set; } = null;

        public Paragoomba()
        {
            Name = "Paragoomba";

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesWings);

            AIBehavior = new ParagoombaAI(this);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            WingedBehavior = new ParagoombaWingedBehavior(this, -1, EntityProperties.GetVulnerableDamageEffects(), new Goomba());
        }

        public override void CleanUp()
        {
            base.CleanUp();

            WingedBehavior?.CleanUp();
        }

        public override void LoadAnimations()
        {
            base.LoadAnimations();

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Paragoomba.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            LoopAnimation wingedIdle = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 45, 27, 28), 200d),
                new Animation.Frame(new Rectangle(1, 7, 27, 30), 200d, new Vector2(0, 1)));
            LoopAnimation flyingName = new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(129, 45, 27, 28), 100d),
                new Animation.Frame(new Rectangle(1, 7, 27, 30), 100d));
            Animation hurtName = new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(97, 48, 29, 27), 80d),
                new Animation.Frame(new Rectangle(98, 89, 27, 26), 80d));
            Animation deathName = new Animation(spriteSheet, new Animation.Frame(new Rectangle(98, 89, 27, 26), 300d));
            Animation diveKickName = new Animation(spriteSheet, new Animation.Frame(new Rectangle(33, 89, 27, 30), 1000d));

            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.WingedIdleName, wingedIdle);
            AnimManager.AddAnimation(AnimationGlobals.WingedBattleAnimations.FlyingName, flyingName);
            AnimManager.AddAnimation(AnimationGlobals.HurtName, hurtName);
            AnimManager.AddAnimation(AnimationGlobals.DeathName, deathName);
            AnimManager.AddAnimation(AnimationGlobals.ParagoombaBattleAnimations.DiveKickName, diveKickName);

            //Wings are offset from the Paragoomba's body
            //Both Wings for each frame are in a single cropped texture
            //The wings are rendered underneath the Paragoomba's body

            wingedIdle.SetChildFrames(
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 200d, new Vector2(0, -6), -.01f),
                new Animation.Frame(new Rectangle(50, 161, 41, 14), 200d, new Vector2(0, 6), -.01f));
            flyingName.SetChildFrames(
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 100d, new Vector2(0, -6), -.01f),
                new Animation.Frame(new Rectangle(50, 161, 41, 14), 100d, new Vector2(0, 6), -.01f));
            hurtName.SetChildFrames(
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 80d, new Vector2(3, -5), -.01f),
                new Animation.Frame(new Rectangle(3, 166, 41, 18), 80d, new Vector2(3, -5), -.01f));
            deathName.SetChildFrames(new Animation.Frame(new Rectangle(3, 166, 41, 18), 100d, new Vector2(3, -5), -.01f));

            diveKickName.SetChildFrames(new Animation.Frame(new Rectangle(120, 121, 31, 21), 1000d, new Vector2(1, -13), -.01f));
        }

        public override void OnEnteredBattle()
        {
            base.OnEnteredBattle();

            AnimManager.PlayAnimation(GetIdleAnim());
        }

        public override string GetIdleAnim()
        {
            if (WingedBehavior.Grounded == false) return AnimationGlobals.WingedBattleAnimations.WingedIdleName;

            return base.GetIdleAnim();
        }

        #region Tattle Information

        public new string[] GetTattleLogEntry()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleLogEntry();

            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A Goomba with wings. Can't\nreach it with a hammer while",
                "it's in the air, but once\n it's damaged, its wings get",
                $"clipped. It's kind of sad,\nreally."
            };
        }

        public new string GetTattleDescription()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleDescription();

            return "That's a Paragoomba.\n<wait value=\"250\">Basically a Goomba with\nwings.<wait value=\"300\"> I'm jealous!\n<k><p>" +
                   "Maximum HP is 2, Attack is 1,\nand Defense is 0.\n<k><p>" +
                   "You can't hammer it while it's\nflying, but rough it up and\nit'll totally plummet!<k>";
        }

        #endregion
    }
}
