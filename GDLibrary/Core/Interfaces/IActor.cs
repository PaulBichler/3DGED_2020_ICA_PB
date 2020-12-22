using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Parent interface for all drawn and undrawn game components.
    /// Rename to IUpdateable for 2021.
    /// </summary>
    /// <see cref="GDLibrary.Actors.Actor"/>
    /// <see cref="GDLibrary.Actors.DrawnActor3D"/>
    public interface IActor : ICloneable
    {
        void Update(GameTime gameTime);

        //moved Draw() to IGDDrawable and DrawnActor3D
        //  void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice);
    }
}