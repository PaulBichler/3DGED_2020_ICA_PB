using GDGame.MyGame.Controllers;
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
        private Actor3D player;

        public MyGameStateManager(Game game, StatusType statusType, CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
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
                        StartGame();
                        break;
                    case EventActionType.OnSpawn:
                        //get a reference to the player to set the target in the follow camera
                        player = eventData.Parameters[0] as Actor3D;
                        break;
                }
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        }

        private void StartGame()
        {


            StartCamera();
        }

        private void StartCamera()
        {
            (cameraManager[2].ControllerList.Find(controller => controller is PlayerFollowCameraController) as PlayerFollowCameraController).SetTargetTransform(player.Transform3D);
            cameraManager.ActiveCameraIndex = 2;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //add code here to check for the status of a particular set of related events e.g. collect all inventory items then...

            base.ApplyUpdate(gameTime);
        }
    }
}