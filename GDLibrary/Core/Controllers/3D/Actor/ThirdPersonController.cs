using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Core.Controllers
{
    public class ThirdPersonController : TargetController
    {
        #region Fields
        private float elevationAngleInDegrees;
        private float distanceFromTarget;
        private Vector3 oldCameraTranslation;
        private float lerpSpeed;
        private MouseManager mouseManager;
        #endregion

        public ThirdPersonController(string id, ControllerType controllerType, 
            IActor targetActor, float elevationAngleInDegrees, 
            float distanceFromTarget, float lerpSpeed, 
            MouseManager mouseManager) : base(id, controllerType, targetActor)
        {
            this.elevationAngleInDegrees = elevationAngleInDegrees;
            this.distanceFromTarget = distanceFromTarget;
            this.lerpSpeed = lerpSpeed;

            this.mouseManager = mouseManager;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            UpdateDistanceElevation(gameTime);
            UpdateParent(gameTime, actor);

            //   base.Update(gameTime, actor);
        }

        private void UpdateDistanceElevation(GameTime gameTime)
        {
            int delta = this.mouseManager.GetDeltaFromScrollWheel();
           // this.distanceFromTarget += gameTime.ElapsedGameTime.Milliseconds * delta / 100;
            this.elevationAngleInDegrees += gameTime.ElapsedGameTime.Milliseconds * delta / 1000;
        }

        private void UpdateParent(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D; //camera
            Actor3D target = this.TargetActor as Actor3D; //car

            if (parent != null)
            {
                //get target look and rotate around target right by elevationAngle
                Vector3 targetToCamera = Vector3.Transform(target.Transform3D.Look,
                    Matrix.CreateFromAxisAngle(target.Transform3D.Right,
                    MathHelper.ToRadians(this.elevationAngleInDegrees)));

                //want unit length
                targetToCamera.Normalize();

                //final camera position <- intermediate camera positions
                parent.Transform3D.Translation
                    = Vector3.Lerp(this.oldCameraTranslation, target.Transform3D.Translation
                                    + targetToCamera * this.distanceFromTarget, this.lerpSpeed);

                parent.Transform3D.Look = -targetToCamera;

                //store the old camera for the next round of lerp in next update(
                this.oldCameraTranslation = parent.Transform3D.Translation;

            }
        }
    }
}
