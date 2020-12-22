using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDLibrary.GameComponents
{
    /// <summary>
    /// Creates a class based on the DrawableGameComponent class that can be paused when the menu is shown.
    /// </summary>
    /// <see cref="GDLibrary.Managers.ObjectManager"/>
    public class PausableDrawableGameComponent : DrawableGameComponent, IEventHandler
    {
        #region Fields
        private StatusType statusType;
        #endregion Fields

        #region Properties

        public StatusType StatusType
        {
            get
            {
                return this.statusType;
            }
            set
            {
                this.statusType = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public PausableDrawableGameComponent(Game game, StatusType statusType)
            : base(game)
        {
            //allows us to start the game component with drawing and/or updating paused
            this.statusType = statusType;

            //subscribe to events that will affect the state of any child class (e.g. a menu play event for the object manager
            SubscribeToEvents();
        }

        #region Handle Events

        /// <summary>
        /// Subscribe to any events that will affect any child class (e.g. menu pause in ObjectManager)
        /// </summary>
        public virtual void SubscribeToEvents()
        {
            //menu
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);
        }

        public virtual void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                    this.StatusType = StatusType.Off;
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    this.StatusType = StatusType.Drawn | StatusType.Update;
            }
        }

        #endregion Handle Events

        #region Update & Draw

        public override void Update(GameTime gameTime)
        {
            //any child class manager needs to listen to input even when paused i.e. hide/show menu - see ScreenManager::HandleInput()
            HandleInput(gameTime);

            if ((this.statusType & StatusType.Update) != 0) //if update flag is set
                ApplyUpdate(gameTime);

            // base.Update(gameTime); //does nothing so comment out
        }

        public override void Draw(GameTime gameTime)
        {
            if ((this.statusType & StatusType.Drawn) != 0) //if draw flag is set
                ApplyDraw(gameTime);

            // base.Draw(gameTime); //does nothing so comment out
        }

        /// <summary>
        /// Called in any child class that wants to be pausable
        /// </summary>
        /// <remarks>All child classes who subscribe to menu pause/play events should call this method instead of Update() </remarks>
        /// <see cref="GDLibrary.Managers.ObjectManager"/>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void ApplyUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        /// Called in any child class that wants to be pausable
        /// </summary>
        /// <remarks>All child classes who subscribe to menu pause/play events should call this method instead of Draw() </remarks>
        /// <see cref="GDLibrary.Managers.ObjectManager"/>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void ApplyDraw(GameTime gameTime)
        {
        }

        #endregion Update & Draw

        #region Handle User Input

        /// <summary>
        /// Handle all input
        /// </summary>
        /// <remarks>May be overridden in child class</remarks>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void HandleInput(GameTime gameTime)
        {
        }

        /// <summary>
        /// Handle mouse input
        /// </summary>
        /// <remarks>May be overridden in child class</remarks>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void HandleMouse(GameTime gameTime)
        {
        }

        /// <summary>
        /// Handle keyboard input
        /// </summary>
        /// <remarks>May be overridden in child class</remarks>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void HandleKeyboard(GameTime gameTime)
        {
        }

        /// <summary>
        /// Handle gamepad input
        /// </summary>
        /// <remarks>May be overridden in child class</remarks>
        /// <param name="gameTime">GameTime object</param>
        protected virtual void HandleGamePad(GameTime gameTime)
        {
        }

        #endregion Handle User Input

        #endregion Constructors & Core
    }
}