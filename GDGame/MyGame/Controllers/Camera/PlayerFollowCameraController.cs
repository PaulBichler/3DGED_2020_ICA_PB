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
        private Transform3D targetActorTransform, parentTransform;
        private float angle, distance;
        private Vector3 look;
        private float initialY;

        public PlayerFollowCameraController(string id, ControllerType controllerType, Transform3D targetActorTransform, float angle, float distance) : base(id, controllerType)
        {
            this.angle = angle;
            this.distance = distance;
            this.targetActorTransform = targetActorTransform;
        }

        public void SetTargetTransform(Transform3D target)
        {
            this.targetActorTransform = target;
            initialY = target.Translation.Y;
            if (parentTransform != null)
                parentTransform.Translation = target.Translation;

            look = Vector3.Transform(targetActorTransform.Look, Matrix.CreateFromAxisAngle(targetActorTransform.Right, MathHelper.ToRadians(angle)));
            look.Normalize();
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parentTransform ??= (actor as Actor3D)?.Transform3D;
            
            if (targetActorTransform == null || parentTransform == null) 
                return;

            Vector3 targetActorTranslation = targetActorTransform.Translation;
            targetActorTranslation.Y = initialY;
            Vector3 newTranslation = targetActorTranslation + look * distance;
            newTranslation.X = parentTransform.Translation.X;
            parentTransform.Translation = newTranslation;
            parentTransform.Look = -look;
        }
    }
}
