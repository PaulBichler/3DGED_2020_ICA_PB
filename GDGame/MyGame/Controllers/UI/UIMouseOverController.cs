using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.Controllers
{
    public class UIMouseOverController : Controller
    {
        private MouseManager mouseManager;
        private Color colorActive, colorInactive;

        public UIMouseOverController(string id,
            ControllerType controllerType, MouseManager mouseManager, Color colorActive, Color colorInactive) : base(id, controllerType)
        {
            this.mouseManager = mouseManager;
            this.colorActive = colorActive;
            this.colorInactive = colorInactive;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                if (drawnActor.Transform2D.Bounds.Contains(mouseManager.Bounds))
                {
                    drawnActor.Color = colorActive;
                }
                else
                {
                    drawnActor.Color = colorInactive;
                }
            }

            base.Update(gameTime, actor);
        }

        //to do...Equals, GetHashCode, Clone
    }
}