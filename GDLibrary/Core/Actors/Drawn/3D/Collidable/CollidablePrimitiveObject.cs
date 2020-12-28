using System;
using System.Collections.Generic;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Parent class for all collidable primitive objects. Inherit from this
    /// class to add your own collidable objects (e.g. PlayerCollidablePrimitiveObject)
    /// </summary>
    public class CollidablePrimitiveObject : PrimitiveObject
    {
        private readonly EffectParameters effectParameters;
        #region Variables
        //the skin used to wrap the object
        private ICollisionPrimitive collisionPrimitive;

        //the object that im colliding with
        private Actor collidee;
        private ObjectManager objectManager;

        #endregion Variables

        #region Properties
        //returns a reference to whatever this object is colliding against
        public Actor Collidee
        {
            get
            {
                return collidee;
            }
            set
            {
                collidee = value;
            }
        }
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
        public ObjectManager ObjectManager
        {
            get
            {
                return objectManager;
            }
        }

        #endregion Properties

        //used to draw collidable primitives that have a texture i.e. use VertexPositionColor vertex types only
        public CollidablePrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            EffectParameters effectParameters, IVertexData vertexData,
             ICollisionPrimitive collisionPrimitive, ObjectManager objectManager)
            : base(id, actorType, statusType, transform3D, effectParameters, vertexData)
        {
            this.effectParameters = effectParameters;
            this.collisionPrimitive = collisionPrimitive;

            //unusual to pass this in but we use it to test for collisions - see Update();
            this.objectManager = objectManager;
        }

        public override void Update(GameTime gameTime)
        {
            //Update the collider
            collisionPrimitive.Update(gameTime, Transform3D);
            base.Update(gameTime);
        }

        //read and store movement suggested by keyboard input
        protected virtual void HandleInput(GameTime gameTime)
        {
        }

        //define what happens when a collision occurs
        protected virtual void HandleCollisionResponse(Actor collidee)
        {
        }

        //test for collision against all opaque and transparent objects
        protected virtual Actor CheckAllCollisions(GameTime gameTime)
        {
            foreach (IActor actor in objectManager.OpaqueList)
            {
                collidee = CheckCollision(gameTime, actor as Actor3D);
                if (collidee != null)
                {
                    return collidee;
                }
            }

            foreach (IActor actor in objectManager.TransparentList)
            {
                collidee = CheckCollision(gameTime, actor as Actor3D);
                if (collidee != null)
                {
                    return collidee;
                }
            }

            return null;
        }

        //test for collision against a specific object
        private Actor CheckCollision(GameTime gameTime, Actor3D actor3D)
        {
            //dont test for collision against yourself - remember the player is in the object manager list too!
            if (this != actor3D)
            {
                if (actor3D is CollidablePrimitiveObject)
                {
                    CollidablePrimitiveObject collidableObject = actor3D as CollidablePrimitiveObject;
                    if (CollisionPrimitive.Intersects(collidableObject.CollisionPrimitive, Transform3D.TranslateIncrement))
                    {
                        return collidableObject;
                    }
                }
                else if (actor3D is CollidableZoneObject)
                {
                    CollidableZoneObject zoneObject = actor3D as CollidableZoneObject;
                    if (CollisionPrimitive.Intersects(zoneObject.CollisionPrimitive, Transform3D.TranslateIncrement))
                    {
                        return zoneObject;
                    }
                }
            }

            return null;
        }

        protected Actor Raycast(Vector3 origin, Vector3 direction, float distance)
        {
            Actor raycastCollidee = null;
            Ray ray = new Ray(origin, direction);

            foreach (IActor actor in objectManager.OpaqueList)
            {
                raycastCollidee = CheckRayCollision((Actor3D)actor, ray, distance);
                if (raycastCollidee != null)
                    return raycastCollidee;
            }

            foreach (IActor actor in objectManager.TransparentList)
            {
                raycastCollidee = CheckRayCollision((Actor3D)actor, ray, distance);
                if (raycastCollidee != null)
                    return raycastCollidee;
            }

            return null;
        }

        private Actor CheckRayCollision(Actor3D actor3D, Ray ray, float distance)
        {
            //dont test for collision against yourself
            if (!Equals(actor3D))
            {
                if (actor3D is CollidablePrimitiveObject collidableObject)
                {
                    if (collidableObject.CollisionPrimitive.Intersects(ray, out float? distanceToObj))
                        if(distanceToObj <= distance)
                            return collidableObject;
                }
                else if (actor3D is CollidableZoneObject zoneObject)
                {
                    if (zoneObject.CollisionPrimitive.Intersects(ray, out float? distanceToObj))
                        if(distanceToObj <= distance)
                            return zoneObject;
                }
            }

            return null;
        }

        //apply suggested movement since no collision will occur if the player moves to that position
        protected virtual void ApplyInput(GameTime gameTime)
        {
            //was a move/rotate key pressed, if so then these values will be > 0 in dimension
            if (Transform3D.TranslateIncrement != Vector3.Zero)
            {
                Transform3D.TranslateBy(Transform3D.TranslateIncrement);
                Transform3D.TranslateIncrement = Vector3.Zero;
            }

            if (Transform3D.RotateIncrement != 0)
            {
                Transform3D.RotateAroundUpBy(Transform3D.RotateIncrement);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is CollidablePrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<ICollisionPrimitive>.Default.Equals(CollisionPrimitive, @object.CollisionPrimitive);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), CollisionPrimitive);
        }

        public new object Clone()
        {
            CollidablePrimitiveObject primitive = new CollidablePrimitiveObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, 
                EffectParameters.Clone() as EffectParameters, IVertexData.Clone() as IVertexData, CollisionPrimitive.Clone() as ICollisionPrimitive, 
                ObjectManager);

            primitive.ControllerList.AddRange(GetControllerListClone());
            return primitive;
        }
    }
}