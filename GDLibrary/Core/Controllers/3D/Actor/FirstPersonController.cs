using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Implements a first person camera controller
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    public class FirstPersonController : Controller
    {
        #region Fields
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private float moveSpeed, strafeSpeed, rotationSpeed;
        #endregion Fields

        #region Properties
        public KeyboardManager KeyboardManager { get => keyboardManager; set => keyboardManager = value; }
        public MouseManager MouseManager { get => mouseManager; set => mouseManager = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public float StrafeSpeed { get => strafeSpeed; set => strafeSpeed = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        #endregion Properties

        #region Constructors & Core
        public FirstPersonController(string id, ControllerType controllerType,
         KeyboardManager keyboardManager,
         MouseManager mouseManager,
         float moveSpeed,
         float strafeSpeed, float rotationSpeed) : base(id, controllerType)
        {
            this.KeyboardManager = keyboardManager;
            this.MouseManager = mouseManager;
            this.MoveSpeed = moveSpeed;
            this.StrafeSpeed = strafeSpeed;
            this.RotationSpeed = rotationSpeed;
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

        protected virtual void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            Vector3 moveVector = Vector3.Zero;

            if (KeyboardManager.IsKeyDown(Keys.W))
            {
                moveVector = parent.Transform3D.Look * MoveSpeed;
            }
            else if (KeyboardManager.IsKeyDown(Keys.S))
            {
                moveVector = -1 * parent.Transform3D.Look * MoveSpeed;
            }

            if (KeyboardManager.IsKeyDown(Keys.A))
            {
                moveVector -= parent.Transform3D.Right * StrafeSpeed;
            }
            else if (KeyboardManager.IsKeyDown(Keys.D))
            {
                moveVector += parent.Transform3D.Right * StrafeSpeed;
            }

            //constrain movement in Y-axis to stop object moving up/down in space
            moveVector.Y = 0;

            //apply the movement
            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        protected virtual void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = MouseManager.GetDeltaFromCentre(new Vector2(512, 384)); //REFACTOR - NMCG
            mouseDelta *= RotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0)
                parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));
        }

        public new object Clone()
        {
            return new FirstPersonController(ID, ControllerType, KeyboardManager,
                MouseManager, MoveSpeed, StrafeSpeed, RotationSpeed);
        }

        //to do...add Equals, GetHashCode

        #endregion Constructors & Core
    }
}