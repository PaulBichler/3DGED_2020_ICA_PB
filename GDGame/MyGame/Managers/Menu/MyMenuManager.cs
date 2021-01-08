using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.MyGame.Managers
{
    public class MyMenuManager : MenuManager
    {
        private MouseManager mouseManager;

        public MyMenuManager(Game game, StatusType statusType, SpriteBatch spriteBatch,
            MouseManager mouseManager)
            : base(game, statusType, spriteBatch)
        {
            this.mouseManager = mouseManager;
        }

        public override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                switch (eventData.EventActionType)
                {
                    case EventActionType.OnPause:
                        SetMenuVisibility(true);
                        break;
                    case EventActionType.OnPlay:
                        SetMenuVisibility(false);
                        break;
                    case EventActionType.OnLose:
                        SetScene("lose");
                        //sending new event since other managers are also handling pause
                        EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
                        break;
                    case EventActionType.OnWin:
                        SetScene("win");
                        //sending new event since other managers are also handling pause
                        EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
                        break;
                    case EventActionType.OnPauseGame:
                        SetScene("pause");
                        EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
                        break;
                }
            }
        }

        private void SetMenuVisibility(bool show)
        {
            if (show)
                this.StatusType = StatusType.Drawn | StatusType.Update;
            else
                this.StatusType = StatusType.Off;
        }

        protected override void HandleInput(GameTime gameTime)
        {
            //bug fix - 7.12.20 - Exit button was hidden but we were still testing for mouse click
            if ((this.StatusType & StatusType.Update) != 0)
            {
                HandleMouse(gameTime);
            }

            HandleKeyboard(gameTime);
            //base.HandleInput(gameTime); //nothing happening in the base method
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            foreach (DrawnActor2D actor in this.ActiveList)
            {
                if (actor is UIButtonObject)
                {
                    if (actor.Transform2D.Bounds.Contains(this.mouseManager.Bounds))
                    {
                        if (this.mouseManager.IsLeftButtonClickedOnce())
                        {
                            HandleClickedButton(gameTime, actor as UIButtonObject);
                        }
                    }
                }
            }
            base.HandleMouse(gameTime);
        }

        private void HandleClickedButton(GameTime gameTime, UIButtonObject uIButtonObject)
        {
            //benefit of switch vs if...else is that code will jump to correct switch case directly
            switch (uIButtonObject.ID)
            {
                case "play":
                    SetScene("levelselect");
                    break;
                case "level1":
                    EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnStart,  new []{ "Level 1" }));
                    break;
                case "level2":
                    EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnStart,  new []{ "Level 2" }));
                    break;
                case "resume":
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                    break;
                case "controls":
                    this.SetScene("controls");
                    break;
                case "exit":
                    Game.Exit();
                    break;
                case "tomenu":
                    SetScene("main");
                    EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnLeaveGame, null));
                    break;
                case "levelselect":
                    SetScene("levelselect");
                    break;
                default:
                    break;
            }
        }
    }
}