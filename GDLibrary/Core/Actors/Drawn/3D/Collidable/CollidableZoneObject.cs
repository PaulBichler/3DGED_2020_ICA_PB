using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Represents an area that can detect collisions by using only a simple
    /// BoundingSphere or AA BoundingBox. It does NOT have an associated model.
    /// We can use this class to create activation zones e.g. for camera switching or event generation
    /// </summary>
    public class CollidableZoneObject : DrawnActor3D
    {
        #region Variables
        private ICollisionPrimitive collisionPrimitive;
        #endregion Variables

        #region Properties
        public ICollisionPrimitive CollisionPrimitive
        {
            get
            {
                return collisionPrimitive;
            }
            set
            {
                collisionPrimitive = value;
            }
        }
        #endregion Properties

        public CollidableZoneObject(string id, ActorType actorType,
            StatusType statusType, Transform3D transform,
            ICollisionPrimitive collisionPrimitive)
            : base(id, actorType, statusType, transform, null)
        {
            this.collisionPrimitive = collisionPrimitive;
        }

        public override void Update(GameTime gameTime)
        {
            //update collision primitive with new object position
            if (collisionPrimitive != null)
            {
                collisionPrimitive.Update(gameTime, Transform3D);
            }
        }
    }
}