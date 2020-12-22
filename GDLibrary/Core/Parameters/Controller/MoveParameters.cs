using GDLibrary.Managers;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the move specific parameters for use with a IController object
    /// </summary>
    /// <see cref="GDLibrary.Controllers.FirstPersonCameraController"/>
    public class MoveParameters : ICloneable
    {
        #region Fields

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private float moveSpeed, strafeSpeed, rotateSpeed;
        private Keys[] moveKeys;

        #endregion Fields

        #region Properties

        public Keys[] MoveKeys { get => moveKeys; set => moveKeys = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public float StrafeSpeed { get => strafeSpeed; set => strafeSpeed = value; }
        public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }
        public MouseManager MouseManager { get => mouseManager; set => mouseManager = value; }
        public KeyboardManager KeyboardManager { get => keyboardManager; set => keyboardManager = value; }

        #endregion Properties

        #region Constructors & Core

        public MoveParameters(KeyboardManager keyboardManager, MouseManager mouseManager,
        float moveSpeed, float strafeSpeed, float rotateSpeed, Keys[] moveKeys)
        {
            KeyboardManager = keyboardManager;
            MouseManager = mouseManager;
            MoveSpeed = moveSpeed;
            StrafeSpeed = strafeSpeed;
            RotateSpeed = rotateSpeed;
            MoveKeys = moveKeys;
        }

        public object Clone()
        {
            return new MoveParameters(keyboardManager, mouseManager,
                moveSpeed, strafeSpeed, rotateSpeed, moveKeys);
        }

        #endregion Constructors & Core
    }
}