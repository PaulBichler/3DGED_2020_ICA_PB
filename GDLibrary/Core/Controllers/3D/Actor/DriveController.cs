using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Controllers
{
    public class DriveController : Controller
    {
        private KeyboardManager keyboardManager;
        private float moveSpeed;
        private float rotationSpeed;

        public DriveController(string id, ControllerType controllerType,
         KeyboardManager keyboardManager,
            float moveSpeed, float rotationSpeed) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.moveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                HandleKeyboardInput(gameTime, parent);
            }

            base.Update(gameTime, actor);
        }

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            Vector3 moveVector = Vector3.Zero;

            if (keyboardManager.IsKeyDown(Keys.U))
            {
                moveVector = parent.Transform3D.Look * moveSpeed;
            }
            else if (keyboardManager.IsKeyDown(Keys.J))
            {
                moveVector = -1 * parent.Transform3D.Look * moveSpeed;
            }

            if (keyboardManager.IsKeyDown(Keys.H))
            {
                parent.Transform3D.RotateAroundUpBy(this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds);
                //parent.Transform3D.RotateBy(new Vector3(0, 45, 0));
            }
            else if (keyboardManager.IsKeyDown(Keys.K))
            {
                parent.Transform3D.RotateAroundUpBy(-1 * this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds);
                //parent.Transform3D.RotateBy(new Vector3(0, -45, 0));
            }

            //constrain movement in Y-axis to stop object moving up/down in space
            moveVector.Y = 0;

            //apply the forward/backward movement
            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        //to do...add Clone, Equals, GetHashCode
    }
}