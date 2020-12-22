using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Represents the parent interface for all
    /// simple collision primitives (sphere and AA box)
    /// </summary>
    public interface ICollisionPrimitive
    {
        bool Intersects(BoundingBox box);
        bool Intersects(BoundingSphere sphere);
        bool Intersects(BoundingFrustum frustum);
        bool Intersects(Ray ray);
        bool Intersects(ICollisionPrimitive collisionPrimitive);

        //projected/predicted CD test
        bool Intersects(ICollisionPrimitive collisionPrimitive, Vector3 translation);
        bool Intersects(Ray ray, out float? distance);
        void Update(GameTime gameTime, Transform3D transform);
        object Clone();
    }
}