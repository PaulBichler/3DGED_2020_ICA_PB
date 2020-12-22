using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Parent interface for all vertex data objects
    /// </summary>
    /// <see cref="GDLibrary.Actors.PrimitiveObject"/>
    public interface IVertexData : ICloneable
    {
        void Draw(GameTime gameTime, BasicEffect effect);

        //getters
        PrimitiveType GetPrimitiveType();

        int GetPrimitiveCount();

        //setters
        void SetPrimitiveType(PrimitiveType primitiveType);

        void SetPrimitiveCount(int primitiveCount);
    }
}