using System.Collections.Specialized;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Represents a simple (non-JigLibX) form of axis-aligned box collision
    /// primitive that can be attached to a PrimitiveObject to enable simple
    /// CD/CR.
    /// Used for your I-CA .
    /// </summary>
    public class BoxCollisionPrimitive : ICollisionPrimitive
    {
        #region Variables
        private static Vector3 min = -1 * Vector3.One, max = Vector3.One;
        private BoundingBox boundingBox, originalBoundingBox;
        private Vector3 scaleFactor;
        private Transform3D transform3D;
        #endregion Variables

        #region Properties
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }
        private Transform3D Transform3D
        {
            get
            {
                return transform3D;
            }
        }
        #endregion Properties

        public BoxCollisionPrimitive(Transform3D transform3D, Vector3 scaleFactor = default)
        {
            this.transform3D = transform3D;
            if (scaleFactor == default) scaleFactor = Vector3.One;
            boundingBox = new BoundingBox(transform3D.Scale * scaleFactor / 2 * min, transform3D.Scale * scaleFactor / 2 * max);
            originalBoundingBox = boundingBox;
            this.scaleFactor = scaleFactor;
        }

        public bool Intersects(BoundingBox box)
        {
            return boundingBox.Intersects(box);
        }

        public bool Intersects(BoundingSphere sphere)
        {
            return boundingBox.Intersects(sphere);
        }

        public bool Intersects(ICollisionPrimitive collisionPrimitive)
        {
            return collisionPrimitive.Intersects(boundingBox);
        }

        //tests if the bounding box for this primitive, when moved, will intersect with the collisionPrimitive passed into the method
        public bool Intersects(ICollisionPrimitive collisionPrimitive, Vector3 translation)
        {
            BoundingBox projectedBox = boundingBox;
            projectedBox.Max += translation;
            projectedBox.Min += translation;
            return collisionPrimitive.Intersects(projectedBox);
        }

        public bool Intersects(Ray ray)
        {
            return (ray.Intersects(boundingBox) > 0);
        }

        //detect intersection and passes back distance to intersected primitive
        public bool Intersects(Ray ray, out float? distance)
        {
            distance = ray.Intersects(boundingBox);
            return (distance > 0);
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            return ((frustum.Contains(boundingBox) == ContainmentType.Contains)
            || (frustum.Contains(boundingBox) == ContainmentType.Intersects));
        }

        public void Update(GameTime gameTime, Transform3D transform)
        {
            boundingBox.Max = originalBoundingBox.Max + transform.Translation;
            boundingBox.Min = originalBoundingBox.Min + transform.Translation;
        }

        public override string ToString()
        {
            return boundingBox.ToString();
        }

        public object Clone()
        {
            return new BoxCollisionPrimitive((Transform3D)Transform3D.Clone(), scaleFactor);
        }
    }
}