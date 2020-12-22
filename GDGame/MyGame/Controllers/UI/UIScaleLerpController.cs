using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDGame.Controllers
{
    public class UIScaleLerpController : Controller
    {
        private MouseManager mouseManager;
        private TrigonometricParameters trigonometricParameters;

        public UIScaleLerpController(string id,
            ControllerType controllerType, MouseManager mouseManager, TrigonometricParameters trigonometricParameters) : base(id, controllerType)
        {
            this.mouseManager = mouseManager;
            this.trigonometricParameters = trigonometricParameters;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                if (drawnActor.Transform2D.Bounds.Contains(mouseManager.Bounds))
                {
                    //A * Sin(wT + phase)
                    float lerpFactor = trigonometricParameters.MaxAmplitude * (float)Math.Sin(MathHelper.ToRadians(trigonometricParameters.AngularSpeed * (float)gameTime.TotalGameTime.TotalMilliseconds + trigonometricParameters.PhaseAngleInDegrees));
                    drawnActor.Transform2D.Scale = drawnActor.Transform2D.OriginalScale + lerpFactor * drawnActor.Transform2D.OriginalScale;
                }
                else
                {
                    drawnActor.Transform2D.Scale = drawnActor.Transform2D.OriginalScale;
                }
            }

            base.Update(gameTime, actor);
        }

        public override bool Equals(object obj)
        {
            return obj is UIScaleLerpController controller &&
                   ID == controller.ID &&
                   ControllerType == controller.ControllerType &&
                   EqualityComparer<MouseManager>.Default.Equals(mouseManager, controller.mouseManager) &&
                   EqualityComparer<TrigonometricParameters>.Default.Equals(trigonometricParameters, controller.trigonometricParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, ControllerType, mouseManager, trigonometricParameters);
        }

        public new object Clone()
        {
            return new UIScaleLerpController(this.ID, this.ControllerType, this.mouseManager,
                this.trigonometricParameters.Clone() as TrigonometricParameters); //hybrid
        }
    }
}