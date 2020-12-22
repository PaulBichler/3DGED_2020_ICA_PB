using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Implements a flight person camera controller which uses keyboard and mouse input
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    /// <seealso cref="GDLibrary.Parameters.MoveParameters"/>
    public class FlightCameraController : UserInputController
    {
        #region Constructors & Core

        public FlightCameraController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            GamePadManager gamePadManager,
            Keys[] moveKeys, float moveSpeed, float strafeSpeed, float rotationSpeed)
            : base(id, controllerType,
            keyboardManager, mouseManager, gamePadManager,
            moveKeys, moveSpeed, strafeSpeed, rotationSpeed)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                HandleKeyboardInput(gameTime, parent);
                HandleMouseInput(gameTime, parent);
            }
        }

        #region Movement

        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            if (this.KeyboardManager.IsKeyDown(Keys.W))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Look * this.MoveSpeed);
            }
            else if (this.KeyboardManager.IsKeyDown(Keys.S))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Look * -this.MoveSpeed);
            }

            if (this.KeyboardManager.IsKeyDown(Keys.A))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Right * -this.StrafeSpeed);
            }
            else if (this.KeyboardManager.IsKeyDown(Keys.D))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Right * this.StrafeSpeed);
            }
        }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = this.MouseManager.GetDeltaFromCentre(new Vector2(512, 384)); //REFACTOR - NMCG
            mouseDelta *= this.RotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0)
            {
                parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));
            }
        }

        #endregion Movement

        public new object Clone()
        {
            return new FlightCameraController(ID, ControllerType,
                this.KeyboardManager, this.MouseManager, this.GamePadManager,
                this.MoveKeys,
                this.MoveSpeed, this.StrafeSpeed, this.RotationSpeed);
        }

        //to do...add Equals, GetHashCode
        #endregion Constructors & Core
    }
}