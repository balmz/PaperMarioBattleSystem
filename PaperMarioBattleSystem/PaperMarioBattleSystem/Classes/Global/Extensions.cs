﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A class for defining Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region Texture2D Extensions

        /// <summary>
        /// Gets the origin of a Texture2D by ratio instead of specifying width and height
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Texture2D texture2D, float x, float y)
        {
            int xVal = (int)(texture2D.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(texture2D.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Texture2D
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Texture2D texture2D)
        {
            return texture2D.GetOrigin(.5f, .5f);
        }

        #endregion

        #region SpriteFont Extensions

        /// <summary>
        /// Gets the origin of a SpriteFont by ratio instead of specifying width and height
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this SpriteFont spriteFont, string text, float x, float y)
        {
            Vector2 size = spriteFont.MeasureString(text);
            size.X *= UtilityGlobals.Clamp(x, 0f, 1f);
            size.Y *= UtilityGlobals.Clamp(y, 0f, 1f);

            return size;
        }

        /// <summary>
        /// Gets the center origin of a SpriteFont
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this SpriteFont spriteFont, string text)
        {
            return spriteFont.GetOrigin(text, .5f, .5f);
        }

        #endregion

        #region Rectangle Extensions

        /// <summary>
        /// Gets the origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Rectangle rectangle, float x, float y)
        {
            int xVal = (int)(rectangle.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(rectangle.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Rectangle rectangle)
        {
            return rectangle.GetOrigin(.5f, .5f);
        }

        #endregion

        #region Vector2 Extensions

        /// <summary>
        /// Halves the Vector2
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved</returns>
        public static Vector2 Halve(this Vector2 vector2)
        {
            return vector2 / 2f;
        }

        /// <summary>
        /// Halves the Vector2, truncating the X and Y components to the nearest integer
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved as integer values</returns>
        public static Vector2 HalveInt(this Vector2 vector2)
        {
            return new Vector2((int)(vector2.X / 2f), (int)(vector2.Y / 2f));
        }

        #endregion
    }
}