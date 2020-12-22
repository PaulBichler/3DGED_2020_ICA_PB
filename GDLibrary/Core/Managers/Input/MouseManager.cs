using GDLibrary.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Provide mouse input functions
    /// </summary>
    /// <see cref="GDLibrary.Controllers.FlightCameraController"/>
    public class MouseManager : GameComponent
    {
        #region Fields
        private MouseState newState, oldState;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns a 1x1 pixel bounding box representing the mouse pointer position
        /// </summary>
        public Microsoft.Xna.Framework.Rectangle Bounds
        {
            get
            {
                return new Microsoft.Xna.Framework.Rectangle(newState.X, newState.Y, 1, 1);
            }
        }

        /// <summary>
        /// Returns the current position of the mouse pointer
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(newState.X, newState.Y);
            }
        }

        /// <summary>
        /// Gets/sets mouse visibility
        /// </summary>
        public bool MouseVisible
        {
            get
            {
                return Game.IsMouseVisible;
            }
            set
            {
                Game.IsMouseVisible = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public MouseManager(Game game, bool bMouseVisible, Vector2 screenCentre)
            : base(game)
        {
            MouseVisible = bMouseVisible;
            this.SetPosition(screenCentre);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //store the old state
            oldState = newState;

            //get the new state
            newState = Mouse.GetState();

            base.Update(gameTime);
        }

        #region Button State

        /// <summary>
        /// Checks if left button is currently clicked
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsLeftButtonClicked()
        {
            return (newState.LeftButton.Equals(ButtonState.Pressed));
        }

        /// <summary>
        /// Checks if left button is currently clicked and was NOT clicked in the last update
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsLeftButtonClickedOnce()
        {
            return ((newState.LeftButton.Equals(ButtonState.Pressed)) && (!oldState.LeftButton.Equals(ButtonState.Pressed)));
        }

        /// <summary>
        /// Checks if middle button is currently clicked
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsMiddleButtonClicked()
        {
            return (newState.MiddleButton.Equals(ButtonState.Pressed));
        }

        /// <summary>
        /// Checks if middle button is currently clicked and was NOT clicked in the last update
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsMiddleButtonClickedOnce()
        {
            return ((newState.MiddleButton.Equals(ButtonState.Pressed)) && (!oldState.MiddleButton.Equals(ButtonState.Pressed)));
        }

        /// <summary>
        /// Checks if right button is currently clicked
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsRightButtonClickedOnce()
        {
            return ((newState.RightButton.Equals(ButtonState.Pressed)) && (!oldState.RightButton.Equals(ButtonState.Pressed)));
        }

        /// <summary>
        /// Checks if right button is currently clicked and was NOT clicked in the last update
        /// </summary>
        /// <returns>True if clicked, otherwise false</returns>
        public bool IsRightButtonClicked()
        {
            return (newState.RightButton.Equals(ButtonState.Pressed));
        }

        /// <summary>
        /// Checks if any button, scrollwheel, or mouse movement has taken place since last update
        /// </summary>
        /// <returns>True if state changed, otherwise false</returns>
        public bool IsStateChanged()
        {
            return (newState.Equals(oldState)) ? false : true;
        }

        #endregion Button State

        #region Scroll Wheel State

        /// <summary>
        /// Gets the current -ve/+ve scroll wheel value
        /// </summary>
        /// <returns>A positive or negative integer</returns>
        public int GetScrollWheelValue()
        {
            return newState.ScrollWheelValue;
        }

        /// <summary>
        /// Checks if the scroll wheel been moved since the last update
        /// </summary>
        /// <returns>True if the scroll wheel has been moved, otherwise false</returns>
        public int GetDeltaFromScrollWheel()
        {
            if (IsStateChanged()) //if state changed then return difference
            {
                return newState.ScrollWheelValue - oldState.ScrollWheelValue;
            }

            return 0;
        }

        #endregion Scroll Wheel State

        #region Position State

        /// <summary>
        /// Sets the mouse position
        /// </summary>
        /// <param name="position">User-defined Vector2 position on screen (i.e. between (0,0) and (width,height)</param>
        public void SetPosition(Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Checks if the mouse has moved moved more than <paramref name="mouseSensitivity"/> since the last update
        /// </summary>
        /// <param name="mouseSensitivity">An floating-point radius value of mouse sensitivity </param>
        /// <returns>True if the mouse has moved outside the <paramref name="mouseSensitivity"/> radius since last update, otherwise false</returns>
        public bool HasMoved(float mouseSensitivity)
        {
            float deltaPositionLength = new Vector2(newState.X - oldState.X,
                newState.Y - oldState.Y).Length();

            return (deltaPositionLength > mouseSensitivity) ? true : false;
        }

        ////did the mouse move above the limits of precision from centre position
        //public bool IsStateChangedOutsidePrecision(float mousePrecision)
        //{
        //    return ((Math.Abs(newState.X - oldState.X) > mousePrecision) || (Math.Abs(newState.Y - oldState.Y) > mousePrecision));
        //}

        /// <summary>
        /// Calculates the mouse pointer distance (in X and Y) from a user-defined target position
        /// </summary>
        /// <param name="target">Delta from this target position</param>
        /// <param name="activeCamera">Currently active camera</param>
        /// <returns>Vector2</returns>
        public Vector2 GetDeltaFromPosition(Vector2 target, Camera3D activeCamera)
        {
            Vector2 delta;
            //remember Position is the Property
            if (Position != target) //e.g. not the centre
            {
                if (activeCamera.View.Up.Y == -1)
                {
                    delta.X = 0;
                    delta.Y = 0;
                }
                else
                {
                    delta.X = Position.X - target.X;
                    delta.Y = Position.Y - target.Y;
                }
                SetPosition(target);
                return delta;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Calculates the mouse pointer distance (in X and Y) from the screen centre (e.g. width/2, height/2)
        /// </summary>
        /// <param name="screenCentre">Delta from this screen centre position</param>
        /// <returns>Vector2</returns>
        public Vector2 GetDeltaFromCentre(Vector2 screenCentre)
        {
            return new Vector2(newState.X - screenCentre.X, newState.Y - screenCentre.Y);
        }

        #endregion Position State

        #endregion Constructors & Core
    }
}