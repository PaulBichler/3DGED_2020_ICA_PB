using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Controllers
{
    public class PlayerFollowCameraController : Controller
    {
        #region Fields
        private Transform3D targetActorTransform, parentTransform;
        private float angle, distance;
        private Vector3 look;
        private float initialY;
        #endregion

        #region Constructor & Core
        public PlayerFollowCameraController(string id, ControllerType controllerType, Transform3D targetActorTransform, float angle, float distance) : base(id, controllerType)
        {
            this.angle = angle;
            this.distance = distance;
            this.targetActorTransform = targetActorTransform;
        }

        /// <summary>
        /// Set the follow target of the camera
        /// </summary>
        /// <param name="target">The target to follow</param>
        public void SetTargetTransform(Transform3D target)
        {
            if (target == null) return;

            this.targetActorTransform = target;
            initialY = target.Translation.Y;
            if (parentTransform != null)
                parentTransform.Translation = target.Translation;

            //Modify the look (needs to be done only once since we don't allow rotation by input)
            look = Vector3.Transform(targetActorTransform.Look, Matrix.CreateFromAxisAngle(targetActorTransform.Right, MathHelper.ToRadians(angle)));
            look.Normalize();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (parentTransform == null)
            {
                parentTransform = (actor as Actor3D)?.Transform3D;
                SetTargetTransform(targetActorTransform);
            }

            if (targetActorTransform == null || parentTransform == null)
                return;

            //Update the camera's position 
            Vector3 targetActorTranslation = targetActorTransform.Translation;
            targetActorTranslation.Y = initialY;
            Vector3 newTranslation = targetActorTranslation + look * distance;
            newTranslation.X = parentTransform.Translation.X;
            parentTransform.Translation = newTranslation;
            parentTransform.Look = -look;
        }

        public new object Clone()
        {
            return new PlayerFollowCameraController(ID, ControllerType, targetActorTransform, angle, distance);
        } 
        #endregion
    }
}
