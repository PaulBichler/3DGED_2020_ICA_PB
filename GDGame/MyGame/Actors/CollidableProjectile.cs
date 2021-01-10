using GDGame.MyGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class CollidableProjectile : CollidablePrimitiveObject
    {
        public CollidableProjectile(string id, ActorType actorType, StatusType statusType, Transform3D transform3D, EffectParameters effectParameters, IVertexData vertexData, ICollisionPrimitive collisionPrimitive, ObjectManager objectManager) : base(id, actorType, statusType, transform3D, effectParameters, vertexData, collisionPrimitive, objectManager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleCollisionResponse(CheckAllCollisions(gameTime));
        }

        protected override void HandleCollisionResponse(Actor collidee)
        {
            if (collidee == null) return;

            if (collidee.ActorType == ActorType.PC || collidee.ActorType == ActorType.BlockingObstacle || collidee.ActorType == ActorType.Obstacle)
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnRemoveActor, new[] { this }));
                EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, new[] { this }));
            }
        }

        public new object Clone()
        {
            CollidableProjectile projectile = new CollidableProjectile(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, 
                EffectParameters.Clone() as EffectParameters, IVertexData.Clone() as IVertexData, CollisionPrimitive.Clone() as ICollisionPrimitive, 
                ObjectManager);

            projectile.ControllerList.AddRange(GetControllerListClone());
            return projectile;
        }
    }
}
