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
    /// A UIElement that holds a <see cref="CroppedTexture2D"/>, which is drawn in 4 pieces.
    /// <para>This is for graphics that constitute 1/4 of a whole.
    /// This class assumes the graphic is the upper-left piece and rotates the other pieces according to draw them combined (Ex. top-left of "+" sign image).</para>
    /// <para>Place their pivots in the center to allow them to be rotated as one.</para>
    /// </summary>
    public sealed class UIFourPiecedTex : UICroppedTexture2D, ICopyable<UIFourPiecedTex>
    {
        /// <summary>
        /// The origin offset. This affects all the pieces.
        /// </summary>
        public Vector2 OriginOffset = Vector2.Zero;

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D): base(croppedtex2D)
        {

        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, Vector2 originOffset) : this(croppedtex2D)
        {
            OriginOffset = originOffset;
        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, float depth, Color tintColor) : base(croppedtex2D, depth, tintColor)
        {
        }

        public UIFourPiecedTex(CroppedTexture2D croppedtex2D, Vector2 originOffset, float depth, Color tintColor) : base(croppedtex2D, depth, tintColor)
        {
            OriginOffset = originOffset;
        }

        public override void Draw()
        {
            DrawPieces();
        }

        private void DrawPieces()
        {
            Vector2 widthHeight = CroppedTex2D.WidthHeightToVector2();
            Texture2D tex = CroppedTex2D.Tex;
            Rectangle? sourcerect = CroppedTex2D.SourceRect;

            bool absOrigin = true;

            //Upper-left
            SpriteRenderer.Instance.DrawUI(tex, Position, sourcerect, TintColor, Rotation, OriginOffset, Scale, false, false, Depth, absOrigin);
            //Upper-right
            SpriteRenderer.Instance.DrawUI(tex, Position, sourcerect, TintColor, Rotation, OriginOffset + new Vector2(-widthHeight.X, 0f), Scale, true, false, Depth, absOrigin);
            //Lower-left
            SpriteRenderer.Instance.DrawUI(tex, Position, sourcerect, TintColor, Rotation, OriginOffset + new Vector2(0f, -widthHeight.Y), Scale, false, true, Depth, absOrigin);
            //Lower-right
            SpriteRenderer.Instance.DrawUI(tex, Position, sourcerect, TintColor, Rotation, OriginOffset + new Vector2(-widthHeight.X, -widthHeight.Y), Scale, true, true, Depth, absOrigin);
        }

        public UIFourPiecedTex Copy()
        {
            UIFourPiecedTex fourPieceTex = new UIFourPiecedTex(CroppedTex2D.Copy(), OriginOffset, Depth, TintColor);

            fourPieceTex.Position = Position;

            fourPieceTex.Rotation = Rotation;
            fourPieceTex.FlipX = FlipX;
            fourPieceTex.FlipY = FlipY;

            return fourPieceTex;
        }
    }
}
