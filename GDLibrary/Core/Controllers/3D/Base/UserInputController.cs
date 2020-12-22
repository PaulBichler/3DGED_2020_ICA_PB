using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// Parent class for all controllers which accept keyboard input and apply to an actor (e.g. a FirstPersonCameraController inherits from this class).
    /// </summary>
    /// <see cref="FirstPersonController"/>
    /// <seealso cref="DriveController"/>
    public class UserInputController : Controller
    {
        #region Fields
        private Keys[] moveKeys;
        private float moveSpeed, strafeSpeed, rotationSpeed;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private GamePadManager gamePadManager;

        #endregion Fields

        #region Properties

        public KeyboardManager KeyboardManager
        {
            get
            {
                return keyboardManager;
            }
        }

        public MouseManager MouseManager
        {
            get
            {
                return mouseManager;
            }
        }

        public GamePadManager GamePadManager
        {
            get
            {
                return gamePadManager;
            }
        }

        public Keys[] MoveKeys
        {
            get
            {
                return moveKeys;
            }
            set
            {
                moveKeys = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
            }
        }

        public float StrafeSpeed
        {
            get
            {
                return strafeSpeed;
            }
            set
            {
                strafeSpeed = value;
            }
        }

        public float RotationSpeed
        {
            get
            {
                return rotationSpeed;
            }
            set
            {
                rotationSpeed = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public UserInputController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            GamePadManager gamePadManager,
            Keys[] moveKeys, float moveSpeed, float strafeSpeed, float rotationSpeed)
            : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.gamePadManager = gamePadManager;
            MoveKeys = moveKeys;
            MoveSpeed = moveSpeed;
            StrafeSpeed = strafeSpeed;
            RotationSpeed = rotationSpeed;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            if (parentActor != null)
            {
                HandleMouseInput(gameTime, parentActor);
                HandleKeyboardInput(gameTime, parentActor);
                HandleGamePadInput(gameTime, parentActor);
            }
            base.Update(gameTime, actor);
        }

        public virtual void HandleGamePadInput(GameTime gameTime, Actor3D parent)
        {
        }

        public virtual void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
        }

        public virtual void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
        }

        #endregion Constructors & Core

        //to do...add Clone, Equals, GetHashCode
    }
}