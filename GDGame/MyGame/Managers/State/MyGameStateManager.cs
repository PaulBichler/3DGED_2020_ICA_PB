using GDGame.MyGame.Controllers;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;

namespace GDLibrary.Core.Managers.State
{
    /// <summary>
    /// Use this manager to listen for related events and perform actions in your game based on events received
    /// </summary>
    public class MyGameStateManager : PausableGameComponent, IEventHandler
    {
        private CameraManager<Camera3D> cameraManager;
        private LevelManager levelManager;
        private Actor3D player;
        private int stars;

        public MyGameStateManager(Game game, StatusType statusType, CameraManager<Camera3D> cameraManager, LevelManager levelManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            this.levelManager = levelManager;
        }

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
                        EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnLose, null));
                        break;
                    case EventActionType.OnWin:
                        EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnWin, null));
                        break;
                    case EventActionType.OnStart:
                        //start curve camera
                        levelManager.LoadLevel(eventData.Parameters[0] as string);
                        StartGame();
                        break;
                    case EventActionType.OnSpawn:
                        //get a reference to the player to set the target in the follow camera
                        player = eventData.Parameters[0] as Actor3D;
                        (cameraManager[2].ControllerList.Find(controller => controller is PlayerFollowCameraController) as PlayerFollowCameraController).SetTargetTransform(player.Transform3D);
                        break;
                    case EventActionType.OnPickup:
                        stars++;
                        System.Diagnostics.Debug.WriteLine("Picked up a star!");
                        break;
                }
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        }

        private void StartGame()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
            StartCamera();
        }

        private void StartCamera()
        {
            cameraManager.ActiveCameraIndex = 2;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //add code here to check for the status of a particular set of related events e.g. collect all inventory items then...

            base.ApplyUpdate(gameTime);
        }
    }
}