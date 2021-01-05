using System;
using System.Collections.Generic;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Utilities;
using GDLibrary;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Controllers
{
    public class ShootingController : Controller
    {
        private Transform3D parentTransform;
        private CollidableProjectile projectileArchetype;
        private List<Vector3> launchDirections;
        private float maxDistance;
        private int unitSpeedInMs;

        public ShootingController(string id, ControllerType controllerType, CollidableProjectile projectile, float maxDistance, int unitSpeedInMs) : base(id, controllerType)
        {
            this.projectileArchetype = projectile;
            this.maxDistance = maxDistance;
            this.unitSpeedInMs = unitSpeedInMs;
        }

        private void Initialize(Actor3D parent)
        {
            if(parent == null)
                throw new ArgumentException("parent is null!");

            parentTransform = parent.Transform3D;
            launchDirections = new List<Vector3> {parentTransform.Right, -parentTransform.Right, parentTransform.Look, -parentTransform.Look};
            EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new object[]
            {
                new RotationTween(parent, 2000, new Vector3(0, 45, 0), true, LaunchProjectiles, LoopType.Repeat),
                new ScaleTween(parent, 1000, Vector3.One * 1.5f, false, null, LoopType.ReverseAndRepeat) 
            }));
        }

        private void LaunchProjectiles(Actor3D actor)
        {
            for (int i = 0; i < launchDirections.Count; i++)
            {
                CollidableProjectile projectile = projectileArchetype.Clone() as CollidableProjectile;
                projectile.Transform3D.Translation = parentTransform.Translation;
                launchDirections[i] = Vector3.Transform(launchDirections[i], Quaternion.CreateFromAxisAngle(Vector3.Up, 45));
                
                projectile.Transform3D.RotateBy(Vector3.Up * CustomHelper.GetAngleBetweenVectors(projectile.Transform3D.Look, launchDirections[i]));

                EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnAddActor, new [] { projectile }));
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new []
                {
                    new TranslationTween(projectile, (int)MathF.Round(maxDistance * unitSpeedInMs), 
                        launchDirections[i] * maxDistance, true, actor3D =>
                        {
                            EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, new[] { actor3D }));
                        }, LoopType.PlayOnce, EasingType.easeIn) 
                }));
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (parentTransform == null)
                Initialize(actor as Actor3D);
        }

        public new object Clone()
        {
            return new ShootingController(ID, ControllerType, projectileArchetype, maxDistance, unitSpeedInMs);
        }
    }
}