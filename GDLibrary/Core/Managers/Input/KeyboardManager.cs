using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Provides methods to determine the state of keyboard keys.
    /// </summary>
    /// <see cref="GDLibrary.Controllers.FirstPersonController"/>
    public class KeyboardManager : GameComponent
    {
        #region Fields

        protected KeyboardState newState, oldState;

        #endregion Fields

        #region Constructors & Core

        public KeyboardManager(Game game)
          : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //store the old keyboard state for later comparison
            oldState = newState;
            //get the current state in THIS update
            newState = Keyboard.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// Checks if any key is currently pressed
        /// </summary>
        /// <returns>True if currently pressed, otherwise false</returns>
        public bool IsKeyPressed()
        {
            return (newState.GetPressedKeys().Length != 0);
        }

        /// <summary>
        /// Checks if a user-defined key is currently pressed
        /// </summary>
        /// <param name="key">A user-defined key</param>
        /// <returns>True if currently pressed, otherwise false</returns>
        public bool IsKeyDown(Keys key)
        {
            return newState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if a user-defined key is currently pressed that was not pressed in the last update
        /// </summary>
        /// <param name="key">A user-defined key</param>
        /// <returns>True if currently pressed, otherwise false</returns>
        public bool IsFirstKeyPress(Keys key)
        {
            return newState.IsKeyDown(key) && oldState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if any keys have been pressed, or released, since the last update
        /// </summary>
        /// <returns>True if a key has been pressed/released, otherwise false</returns>
        public bool IsStateChanged()
        {
            return !newState.Equals(oldState); //false if no change, otherwise true
        }

        //public bool IsAnyKeyPressed()
        //{
        //    return newState.GetPressedKeys().Length == 0 ? false : true;
        //}

        #endregion Constructors & Core
    }
}