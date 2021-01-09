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
    /// <summary>
    /// Controller that allows launching projectiles in 4 directions
    /// </summary>
    public class ShootingController : Controller
    {
        #region Fields
        private Transform3D parentTransform, playerTransform;
        private CollidableProjectile projectileArchetype;
        private List<Vector3> launchDirections;
        private float maxDistance;
        private int unitSpeedInMs;
        #endregion

        #region Constructor & Core
        public ShootingController(string id, ControllerType controllerType, CollidableProjectile projectile, float maxDistance, int unitSpeedInMs) : base(id, controllerType)
        {
            this.projectileArchetype = projectile;
            this.maxDistance = maxDistance;
            this.unitSpeedInMs = unitSpeedInMs;

            EventDispatcher.Subscribe(EventCategoryType.Player, HandleGameStateEvent);
        }

        private void Initialize(Actor3D parent)
        {
            if (parent == null)
                throw new ArgumentException("parent is null!");

            parentTransform = parent.Transform3D;
            launchDirections = new List<Vector3> { parentTransform.Right, -parentTransform.Right, parentTransform.Look, -parentTransform.Look };

            //Start the rotation and scaling animation (Projectiles are launched at each iteration)
            EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new object[]
            {
                new RotationTween(parent, GameConstants.Projectile_CooldownInMs,
                    new Vector3(0, 45, 0), true, LaunchProjectiles, LoopType.Repeat),
                new ScaleTween(parent, GameConstants.Projectile_CooldownInMs / 2,
                    Vector3.One * 1.5f, false, null, LoopType.ReverseAndRepeat)
            }));
        }

        private void LaunchProjectiles(Actor3D actor)
        {
            //Only shoot if the player is within activation range
            if (playerTransform == null || Vector3.Distance(parentTransform.Translation, playerTransform.Translation) >
                GameConstants.Projectile_ActivationDistance)
                return;

            //Launch a projectile in each direction
            for (int i = 0; i < launchDirections.Count; i++)
            {
                CollidableProjectile projectile = projectileArchetype.Clone() as CollidableProjectile;
                projectile.Transform3D.Translation = parentTransform.Translation;

                //Rotate the direction 
                launchDirections[i] = Vector3.Transform(launchDirections[i], Quaternion.CreateFromAxisAngle(Vector3.Up, 45));

                //Add the projectile to the ObjectManager and start its movement animation
                EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnAddActor, new[] { projectile }));
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new[]
                {
                    new TranslationTween(projectile, (int)MathF.Round(maxDistance * unitSpeedInMs),
                        launchDirections[i] * maxDistance, true, actor3D =>
                        {
                            EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, new[] { actor3D }));
                        }, LoopType.PlayOnce, EasingType.easeIn)
                }));
            }

            //Play a shooting sound
            EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay3D, new object[] { "shoot", parentTransform }));
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
        #endregion

        #region Events
        private void HandleGameStateEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnSpawn)
                playerTransform = eventData.Parameters[0] as Transform3D;
        } 
        #endregion
    }
}