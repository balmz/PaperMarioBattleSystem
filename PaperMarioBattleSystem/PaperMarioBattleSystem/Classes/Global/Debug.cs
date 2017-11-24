﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Static class for debugging
    /// </summary>
    public static class Debug
    {
        public static bool DebugEnabled { get; private set; } = false;
        public static bool LogsEnabled { get; private set; } = false;

        public static bool DebugPaused { get; private set; } = false;
        public static bool AdvanceNextFrame { get; private set; } = false;

        private static KeyboardState DebugKeyboard = default(KeyboardState);

        static Debug()
        {
            #if DEBUG
                DebugEnabled = true;
                LogsEnabled = true;
            #else
                DebugEnabled = false;
            #endif
        }

        private static void ToggleDebug()
        {
            #if DEBUG
                DebugEnabled = !DebugEnabled;
            //Failsafe
            #else
                DebugEnabled = false;
            #endif
        }

        private static void ToggleLogs()
        {
            LogsEnabled = !LogsEnabled;
        }

        private static string GetStackInfo(int skipFrames)
        {
            StackFrame trace = new StackFrame(skipFrames, true);
            int line = 0;
            string method = "";

            string traceFileName = trace.GetFileName();
            if (string.IsNullOrEmpty(traceFileName) == true)
                traceFileName = "N/A";

            string[] file = traceFileName.Split('\\');
            string fileName = file?[file.Length - 1];
            if (string.IsNullOrEmpty(fileName) == true)
                fileName = "N/A FileName";

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;
            if (string.IsNullOrEmpty(method) == true)
                method = "N/A MethodName";

            return $"{fileName}->{method}({line}):";
        }

        private static string GetStackInfo()
        {
            return GetStackInfo(3);
        }

        public static void Log(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Information: {GetStackInfo()} {value}");
        }

        public static void LogWarning(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Warning: {GetStackInfo()} {value}");
        }

        public static void LogError(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Error: {GetStackInfo()} {value}");
        }

        private static void LogAssert()
        {
            if (LogsEnabled == false) return;
            string stackInfo = GetStackInfo(3);
            stackInfo = stackInfo.Remove(stackInfo.Length - 1);

            WriteLine($"ASSERT FAILURE AT: {stackInfo}");
        }

        public static void Assert(bool condition)
        {
            if (condition == false)
                LogAssert();
        }

        public static void DebugUpdate()
        {
            #if DEBUG
                //Toggle debug
                if (Input.GetKey(Keys.LeftControl, DebugKeyboard) && Input.GetKeyDown(Keys.D, DebugKeyboard))
                {
                    ToggleDebug();
                }
            #endif

            //Return if debug isn't enabled
            if (DebugEnabled == false)
            {
                if (Time.InGameTimeEnabled == false)
                    Time.ToggleInGameTime(true);
                return;
            }

            //Reset frame advance
            AdvanceNextFrame = false;

            //Debug controls
            if (Input.GetKey(Keys.LeftControl, DebugKeyboard))
            {
                //Toggle pause
                if (Input.GetKeyDown(Keys.P, DebugKeyboard))
                {
                    DebugPaused = !DebugPaused;
                }
                //Toggle frame advance
                else if (Input.GetKeyDown(Keys.OemSemicolon, DebugKeyboard))
                {
                    AdvanceNextFrame = true;
                }
                //Toggle logs
                else if (Input.GetKeyDown(Keys.L, DebugKeyboard))
                {
                    ToggleLogs();
                }
            }

            //Camera controls
            if (Input.GetKey(Keys.LeftShift, DebugKeyboard))
            {
                if (Input.GetKeyDown(Keys.Space, DebugKeyboard))
                {
                    //Reset camera coordinates
                    Camera.Instance.SetTranslation(Vector2.Zero);
                    Camera.Instance.SetRotation(0f);
                    Camera.Instance.SetZoom(1f);
                }
                else
                {
                    Vector2 translation = Vector2.Zero;
                    float rotation = 0f;
                    float zoom = 0f;

                    //Translation
                    if (Input.GetKey(Keys.Left, DebugKeyboard)) translation.X -= 2;
                    if (Input.GetKey(Keys.Right, DebugKeyboard)) translation.X += 2;
                    if (Input.GetKey(Keys.Down, DebugKeyboard)) translation.Y += 2;
                    if (Input.GetKey(Keys.Up, DebugKeyboard)) translation.Y -= 2;

                    //Rotation
                    if (Input.GetKey(Keys.OemComma, DebugKeyboard)) rotation -= .1f;
                    if (Input.GetKey(Keys.OemPeriod, DebugKeyboard)) rotation += .1f;

                    //Scale
                    if (Input.GetKey(Keys.OemMinus, DebugKeyboard)) zoom -= .1f;
                    if (Input.GetKey(Keys.OemPlus, DebugKeyboard)) zoom += .1f;

                    if (translation != Vector2.Zero) Camera.Instance.Translate(translation);
                    if (rotation != 0f) Camera.Instance.Rotate(rotation);
                    if (zoom != 0f) Camera.Instance.Zoom(zoom);
                }
            }

            //Battle Debug
            if (Input.GetKey(Keys.RightShift, DebugKeyboard) == true)
            {
                DebugBattle();
            }

            //Unit Tests
            if (Input.GetKey(Keys.U, DebugKeyboard) == true)
            {
                DebugUnitTests();
            }

            //Damage Mario
            if (Input.GetKey(Keys.Tab, DebugKeyboard) == true)
            {
                if (Input.GetKeyDown(Keys.H) == true)
                {
                    BattleManager.Instance.GetMario().TakeDamage(Enumerations.Elements.Normal, 1, true);
                }
            }

            //If a pause is eventually added that can be performed normally, put a check for it in here to
            //prevent the in-game timer from turning on when it shouldn't
            Time.ToggleInGameTime(DebugPaused == false || AdvanceNextFrame == true);

            FPSCounter.Update();
            Input.UpdateInputState(ref DebugKeyboard);
        }

        public static void DebugBattle()
        {
            //Default to Players - if holding 0, switch to Enemies
            Enumerations.EntityTypes entityType = Enumerations.EntityTypes.Player;
            if (Input.GetKey(Keys.D0, DebugKeyboard) == true) entityType = Enumerations.EntityTypes.Enemy;

            int turnCount = 3;

            //Inflict Poison, Payback, or Paralyzed
            if (Input.GetKeyDown(Keys.P, DebugKeyboard) == true)
            {
                StatusEffect status = new PoisonStatus(turnCount);
                //Inflict Payback
                if (Input.GetKey(Keys.B, DebugKeyboard) == true) status = new PaybackStatus(turnCount);
                //Inflict Paralyzed
                else if (Input.GetKey(Keys.Z, DebugKeyboard) == true) status = new ParalyzedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Invisible or Immobilized
            else if (Input.GetKeyDown(Keys.I, DebugKeyboard) == true)
            {
                StatusEffect status = new InvisibleStatus(turnCount);
                //Inflict Immobilized
                if (Input.GetKey(Keys.M, DebugKeyboard) == true) status = new ImmobilizedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Electrified
            else if (Input.GetKeyDown(Keys.E, DebugKeyboard) == true)
            {
                DebugInflictStatus(new ElectrifiedStatus(turnCount), entityType);
            }
            //Inflict Fast, Frozen, or FPRegen
            else if (Input.GetKeyDown(Keys.F, DebugKeyboard) == true)
            {
                StatusEffect status = new FastStatus(turnCount);
                //Inflict Frozen
                if (Input.GetKey(Keys.R, DebugKeyboard) == true) status = new FrozenStatus(turnCount);
                //Inflict FPRegen
                else if (Input.GetKey(Keys.P, DebugKeyboard) == true) status = new FPRegenStatus(2, turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Dizzy or Dodgy
            else if (Input.GetKeyDown(Keys.D, DebugKeyboard) == true)
            {
                StatusEffect status = new DizzyStatus(turnCount);
                //Inflict Dodgy
                if (Input.GetKey(Keys.O, DebugKeyboard) == true) status = new DodgyStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Sleep, Stone, or Slow
            else if (Input.GetKeyDown(Keys.S, DebugKeyboard) == true)
            {
                StatusEffect status = new SleepStatus(turnCount);
                //Inflict Stone
                if (Input.GetKey(Keys.T, DebugKeyboard) == true) status = new StoneStatus(turnCount);
                //Inflict Slow
                else if (Input.GetKey(Keys.L, DebugKeyboard) == true) status = new SlowStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Confused
            else if (Input.GetKeyDown(Keys.C, DebugKeyboard) == true)
            {
                StatusEffect status = new ConfusedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Burn
            else if (Input.GetKeyDown(Keys.B, DebugKeyboard) == true)
            {
                StatusEffect status = new BurnStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Tiny
            else if (Input.GetKeyDown(Keys.T, DebugKeyboard) == true)
            {
                StatusEffect status = new TinyStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Huge or HPRegen
            else if (Input.GetKeyDown(Keys.H, DebugKeyboard) == true)
            {
                StatusEffect status = new HugeStatus(turnCount);
                //Inflict HPRegen
                if (Input.GetKey(Keys.P, DebugKeyboard) == true) status = new HPRegenStatus(2, turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Allergic
            else if (Input.GetKeyDown(Keys.A, DebugKeyboard) == true)
            {
                StatusEffect status = new AllergicStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
        }

        private static void DebugInflictStatus(StatusEffect status, Enumerations.EntityTypes entityType)
        {
            BattleEntity[] entities = BattleManager.Instance.GetEntities(entityType, null);

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].EntityProperties.AfflictStatus(status, true);
            }
        }

        #region Debug Unit Tests

        private static void DebugUnitTests()
        {
            if (Input.GetKeyDown(Keys.D0, DebugKeyboard))
            {
                UnitTests.InteractionUnitTests.ElementOverrideInteractionUT1();
            }
        }

        #endregion

        #region Debug Drawing Methods

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="layer">The layer of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="uiBatch">Whether to draw the line in the UI layer or not.</param>
        public static void DebugDrawLine(Vector2 start, Vector2 end, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");

            //Get rotation with the angle between the start and end vectors
            float lineRotation = (float)UtilityGlobals.Angle360(start, end);

            //Get the scale; use the X as the length and the Y as the width
            Vector2 diff = end - start;
            Vector2 lineScale = new Vector2(diff.Length(), thickness);
            
            SpriteRenderer.Instance.Draw(box, start, null, color, lineRotation, new Vector2(0f, 0f), lineScale, false, false, layer, uiBatch);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        /// <param name="uiBatch">Whether to draw the rectangle in the UI layer or not.</param>
        public static void DebugDrawRect(Rectangle rect, Color color, float layer, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");

            SpriteRenderer.Instance.Draw(box, new Vector2(rect.X, rect.Y), null, color, 0f, Vector2.Zero, new Vector2(rect.Width, rect.Height), false, false, layer, uiBatch);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        /// <param name="uiBatch">Whether to draw the hollow rectangle in the UI layer or not.</param>
        public static void DebugDrawHollowRect(Rectangle rect, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Rectangle[] rects = new Rectangle[4]
            {
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness)
            };

            Texture2D box = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");

            for (int i = 0; i < rects.Length; i++)
            {
                SpriteRenderer.Instance.Draw(box, new Vector2(rects[i].X, rects[i].Y), null, color, 0f, Vector2.Zero, new Vector2(rects[i].Width, rects[i].Height), false, false, layer, uiBatch);
            }
        }

        #endregion

        public static void DebugDraw()
        {
            if (DebugEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            //Camera info
            Vector2 cameraBasePos = new Vector2(0, 510);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Pos: {Camera.Instance.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Rot: {Camera.Instance.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Zoom: {Camera.Instance.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
        }
    }
}
