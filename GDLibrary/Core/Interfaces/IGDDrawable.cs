using GDLibrary.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Parent interface for all drawn 3D objects (e.g. ModelObject, PrimitiveObject)
    /// </summary>
    public interface IGDDrawable : ICloneable
    {
        void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }
}