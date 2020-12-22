using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.Controllers
{
    public class UIColorLerpController : Controller
    {
        private Color startColor;
        private Color endColor;

        public UIColorLerpController(string id, ControllerType controllerType,
            Color startColor, Color endColor) : base(id, controllerType)
        {
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                //-1 to +1
                float lerpFactor = (float)Math.Sin(0.5f * MathHelper.ToRadians((float)gameTime.TotalGameTime.TotalMilliseconds));
                //0 to 1
                lerpFactor = lerpFactor * 0.5f + 0.5f;

                drawnActor.Color = MathUtility.Lerp(this.startColor, this.endColor, lerpFactor);
            }

            base.Update(gameTime, actor);
        }

        //to do...Equals, GetHashCode, Clone
    }
}