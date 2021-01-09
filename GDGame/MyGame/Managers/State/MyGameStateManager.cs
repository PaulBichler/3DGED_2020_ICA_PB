using GDGame.MyGame.Controllers;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Core.Managers.State
{
    /// <summary>
    /// Use this manager to listen for related events and perform actions in your game based on events received
    /// </summary>
    public class MyGameStateManager : PausableGameComponent, IEventHandler
    {
        #region Fields
        private CameraManager<Camera3D> cameraManager;
        private LevelManager levelManager;
        private KeyboardManager keyboardManager;
        private Actor3D player;
        private int stars;
        private bool hasGameStarted;
        private string currentLevelId;
        #endregion

        #region Constructor & Core
        public MyGameStateManager(Game game, StatusType statusType, CameraManager<Camera3D> cameraManager, LevelManager levelManager, KeyboardManager keyboardManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            this.levelManager = levelManager;
            this.keyboardManager = keyboardManager;
        }

        private void StartGame(string levelId)
        {
            //load the level and reset the game state
            levelManager.LoadLevel(levelId);
            hasGameStarted = true;
            currentLevelId = levelId;
            stars = 0;

            //Start the intro curve camera
            cameraManager[0].ControllerList.Add(new Curve3DController("Start Curve Camera", ControllerType.Curve, levelManager.CurrentLevel.StartCameraCurve));
            cameraManager.ActiveCameraIndex = 0;

            //hide the menu and reset the HUD
            EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
            EventDispatcher.Publish(new EventData(EventCategoryType.UI, EventActionType.OnStarPickup, new object[] { stars }));

            //Switch to player camera after curve has been completed
            TimeManager.ExecuteInSeconds("Intro End", 9, ActivatePlayer);
        }

        private void ActivatePlayer()
        {
            //activate the player and switch to the player camera
            player.StatusType = StatusType.Update | StatusType.Drawn;
            cameraManager.CycleActiveCamera();

            //some entities need to have a reference to the player 
            EventDispatcher.Publish(new EventData(EventCategoryType.Player, EventActionType.OnSpawn, new[] { player }));
        }

        private void EndGame(EventActionType endState)
        {
            //display menu
            hasGameStarted = false;
            EventDispatcher.Publish(new EventData(EventCategoryType.Menu, endState, null));

            //win or lose sound?
            if (endState == EventActionType.OnWin)
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, new object[] { "win" }));
            else if (endState == EventActionType.OnLose)
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, new object[] { "lose" }));

            //Remove the curve controller
            cameraManager[0].ControllerList.Remove(c => c is Curve3DController);
        }

        protected override void HandleInput(GameTime gameTime)
        {
            HandleKeyboard(gameTime);
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            //pause / unpause functionality
            if (this.keyboardManager.IsFirstKeyPress(Keys.Escape) && hasGameStarted)
            {
                if (StatusType == StatusType.Off)
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
                else
                {
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPauseGame, null));
                    StatusType = StatusType.Off;
                }
            }
        } 
        #endregion

        #region Events
        public override void SubscribeToEvents()
        {
            //add new events here...
            EventDispatcher.Subscribe(EventCategoryType.GameState, HandleEvent);
            base.SubscribeToEvents();
        }

        public override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.GameState)
            {
                switch (eventData.EventActionType)
                {
                    case EventActionType.OnLose:
                        EndGame(EventActionType.OnLose);
                        break;
                    case EventActionType.OnWin:
                        EndGame(EventActionType.OnWin);
                        break;
                    case EventActionType.OnLeaveGame:
                        hasGameStarted = false;
                        break;
                    case EventActionType.OnStart:
                        StartGame(eventData.Parameters[0] as string);
                        break;
                    case EventActionType.OnRestart:
                        StartGame(currentLevelId);
                        break;
                    case EventActionType.OnSpawn:
                        player = eventData.Parameters[0] as Actor3D;
                        EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnSetListener, new[] { player.Transform3D }));
                        (cameraManager[1].ControllerList.Find(c => c is PlayerFollowCameraController) as PlayerFollowCameraController).SetTargetTransform(player.Transform3D);
                        break;
                    case EventActionType.OnStarPickup:
                        EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, new object[] { "star" }));
                        EventDispatcher.Publish(new EventData(EventCategoryType.UI, EventActionType.OnStarPickup, new object[] { ++stars }));
                        break;
                }
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        } 
        #endregion
    }
}