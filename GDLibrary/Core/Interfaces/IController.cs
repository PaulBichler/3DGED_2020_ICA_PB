using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Interfaces
{
    /// <summary>
    /// Parent interface for all controllers attached to a drawn or undrawn game component
    /// </summary>
    public interface IController : ICloneable
    {
        void Update(GameTime gameTime, IActor actor); //update the actor controller by this controller

        ControllerType GetControllerType();
    }
}