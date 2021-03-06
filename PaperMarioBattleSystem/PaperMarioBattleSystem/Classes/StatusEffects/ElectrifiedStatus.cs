﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;
using static PaperMarioBattleSystem.StatusGlobals;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Electrified Status Effect.
    /// This grants the Electrified PhysicalAttribute to the entity, causing direct contact from non-Electrified entities to hurt the attacker.
    /// </summary>
    public sealed class ElectrifiedStatus : MessageEventStatus
    {
        private readonly PaybackHolder ElectrifiedPayback = new PaybackHolder(PaybackTypes.Constant, PhysicalAttributes.Electrified,
            Elements.Electric, new ContactTypes[] { ContactTypes.Latch, ContactTypes.SideDirect, ContactTypes.TopDirect }, 
            new ContactProperties[] { ContactProperties.None },
            ContactResult.PartialSuccess, ContactResult.Success, 1, null);

        private CroppedTexture2D SparkIcon = null;

        public ElectrifiedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Electrified;
            //Despite having positive effects, Electrified seems to be considered a Negative StatusEffect in the PM games
            //Several examples: 
            //-The lights that fall down from the stage in TTYD afflict it. No damaging move in either PM game afflicts a positive status
            //-Feeling Fine and Feeling Fine P prevent it from being afflicted (TTYD) or cause it to wear off early (PM)
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(658, 106, 38, 46));

            Duration = duration;

            AfflictedMessage = "Electrified! Enemies that\nmake contact will get hurt!";

            SparkIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(458, 103, 26, 30));
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            EntityAfflicted.EntityProperties.AddPayback(ElectrifiedPayback);

            base.OnAfflict();
        }

        protected override void OnEnd()
        {
            EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            EntityAfflicted.EntityProperties.RemovePayback(ElectrifiedPayback);

            base.OnEnd();
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                EntityAfflicted.EntityProperties.RemovePayback(ElectrifiedPayback);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                EntityAfflicted.EntityProperties.AddPayback(ElectrifiedPayback);
            }
        }

        public override StatusEffect Copy()
        {
            return new ElectrifiedStatus(Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //Draw the Spark symbol
            //It's stored as a separate graphic since it scales

            //The spark is initially at a smaller scale and scales to its normal size roughly every 2 seconds for roughly 10 frames (I eyeballed it from a video)
            float sparkScale = .8f;
            const double smallInterval = 2000d;
            const double largeInterval = 170d;

            //Get the remainder of the small interval. If it's less than the large interval value, then scale up the spark
            double scaleInterval = (Time.ActiveMilliseconds % smallInterval);
            if (scaleInterval < largeInterval) sparkScale = 1f;

            Vector2 sparkOrigin = SparkIcon.SourceRect.Value.GetCenterOrigin();
            sparkOrigin = new Vector2((int)(sparkOrigin.X * 1.5f), (int)(sparkOrigin.Y * 1.5f) - 3);

            Vector2 sparkPos = iconPos + sparkOrigin;
            float sparkDepth = depth + .00001f;

            SpriteRenderer.Instance.DrawUI(SparkIcon.Tex, sparkPos, SparkIcon.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), sparkScale, false, false, sparkDepth);
        }
    }
}
