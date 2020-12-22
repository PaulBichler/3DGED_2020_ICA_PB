using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDLibrary.GameComponents
{
    /// <summary>
    /// Creates a class based on the GameComponent class that can be paused when the menu is shown.
    /// </summary>
    /// <see cref="GDLibrary.Managers.KeyboardManager"/>
    public class PausableGameComponent : GameComponent, IEventHandler
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

        public PausableGameComponent(Game game, StatusType statusType)
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
                    this.StatusType = StatusType.Update;
            }
        }

        #endregion Handle Events

        public override void Update(GameTime gameTime)
        {
            //screen manager needs to listen to input even when paused i.e. hide/show menu - see ScreenManager::HandleInput()
            HandleInput(gameTime);

            if ((this.statusType & StatusType.Update) != 0) //if update flag is set
                ApplyUpdate(gameTime);
            // base.Update(gameTime); //does notthing so comment out
        }

        protected virtual void ApplyUpdate(GameTime gameTime)
        {
        }

        protected virtual void HandleInput(GameTime gameTime)
        {
        }

        protected virtual void HandleMouse(GameTime gameTime)
        {
        }

        protected virtual void HandleKeyboard(GameTime gameTime)
        {
        }

        protected virtual void HandleGamePad(GameTime gameTime)
        {
        }

        #endregion Constructors & Core
    }
}