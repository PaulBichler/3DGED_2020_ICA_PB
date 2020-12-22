using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UIRotationController : Controller
    {
        public UIRotationController(string id, ControllerType controllerType) : base(id, controllerType)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                drawnActor.Transform2D.RotationInDegrees += (float)(0.1f * gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            base.Update(gameTime, actor);
        }

        //to do...Equals, GetHashCode, Clone
    }
}