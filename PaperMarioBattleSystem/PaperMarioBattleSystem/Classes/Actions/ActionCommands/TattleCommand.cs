﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press A when the cursor is near the middle of the big circle.
    /// </summary>
    public sealed class TattleCommand : ActionCommand
    {
        /*I'm making this based on position instead of time as I feel this is linear enough where neither implementation would make a difference
         If for some reason this doesn't work out, feel free to make it timing dependent*/

        private const int BigCursorSize = 46;
        private const int SmallCursorStartOffset = 150;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;

        private Vector2 BigCursorPos = Vector2.Zero;
        private Vector2 SmallCursorPos = Vector2.Zero;

        private float SmallCursorSpeed = 2f;
        private float ElapsedTime = 0f;

        private Keys InputButton = Keys.Z;

        private Rectangle SuccessRect = new Rectangle(0, 0, BigCursorSize, BigCursorSize);
        private Texture2D DebugBoxTex = null;

        private bool WithinRange => (SuccessRect.Contains(SmallCursorPos));
        private bool PastRange => (SmallCursorPos.X > (SuccessRect.Right + SuccessRect.Width));

        public TattleCommand(IActionCommandHandler commandHandler, float smallCursorSpeed) : base(commandHandler)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX");
            
            //NOTE: Handle rotation by combining them at runtime, possibly...
            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));

            SmallCursorSpeed = smallCursorSpeed;

            DebugBoxTex = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");

            //Description = "Line up the small cursor with\n the center of the big cursor!"
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            if (values == null || values.Length == 0)
            {
                Debug.LogError($"{nameof(TattleCommand)} requires the position of the BattleEntity targeted to place the cursor in the correct spot!");
                return;
            }

            BigCursorPos = Camera.Instance.SpriteToUIPos((Vector2)values[0]);
            SmallCursorPos = BigCursorPos - new Vector2(SmallCursorStartOffset, 0);

            SuccessRect = new Rectangle((int)BigCursorPos.X - (BigCursorSize / 2), (int)BigCursorPos.Y - (BigCursorSize / 2), BigCursorSize, BigCursorSize);
        }

        public override void EndInput()
        {
            base.EndInput();
        }

        protected override void ReadInput()
        {
            //Failed if no input was pressed on time
            if (PastRange == true)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Check if the player pressed the input button at the right time
            if (Input.GetKeyDown(InputButton) == true)
            {
                //Out of range, so a failure
                if (WithinRange == false)
                {
                    OnComplete(CommandResults.Failure);
                    return;
                }
                //In range, so success
                else
                {
                    OnComplete(CommandResults.Success);
                    return;
                }
            }

            SmallCursorPos.X += SmallCursorSpeed;
            ElapsedTime += (float)Time.ElapsedMilliseconds;
        }

        protected override void OnDraw()
        {
            //The cursor is drawn on top of the entity being targeted
            //Only 1/4 of the full cursor is stored as a texture, so we can just draw 3 more versions flipped differently

            string text = "NO!";
            Color color = Color.Red;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);

            Vector2 bigOrigin = new Vector2(BigCursor.SourceRect.Value.Width, BigCursor.SourceRect.Value.Height);
            Vector2 bigOriginHalf = bigOrigin / 2f;

            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, bigOrigin, false, false, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos + new Vector2(bigOrigin.X, 0), BigCursor.SourceRect, Color.White, bigOrigin, true, false, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos + new Vector2(0, bigOrigin.Y), BigCursor.SourceRect, Color.White, bigOrigin, false, true, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos + new Vector2(bigOrigin.X, bigOrigin.Y), BigCursor.SourceRect, Color.White, bigOrigin, true, true, .2f, true);

            Vector2 smallOrigin = new Vector2(SmallCursor.SourceRect.Value.Width, SmallCursor.SourceRect.Value.Height);
            Vector2 smallOriginHalf = smallOrigin / 2f;

            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos, SmallCursor.SourceRect, Color.White, smallOrigin, false, false, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(smallOrigin.X, 0), SmallCursor.SourceRect, Color.White, smallOrigin, true, false, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(0, smallOrigin.Y), SmallCursor.SourceRect, Color.White, smallOrigin, false, true, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(smallOrigin.X, smallOrigin.Y), SmallCursor.SourceRect, Color.White, smallOrigin, true, true, .25f, true);

            //Show success rectangle (comment out if not debugging)
            //SpriteRenderer.Instance.Draw(DebugBoxTex, new Vector2(SuccessRect.X, SuccessRect.Y), null, Color.Red, 0f, Vector2.Zero, 
            //    new Vector2(SuccessRect.Width, SuccessRect.Height), false, false, .21f, true);
        }
    }
}
