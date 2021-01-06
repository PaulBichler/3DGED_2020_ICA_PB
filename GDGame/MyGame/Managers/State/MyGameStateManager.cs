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
                        EndGame(EventActionType.OnLose);
                        break;
                    case EventActionType.OnWin:
                        EndGame(EventActionType.OnWin);
                        break;
                    case EventActionType.OnStart:
                        StartGame(eventData.Parameters[0] as string);
                        break;
                    case EventActionType.OnSpawn:
                        //get a reference to the player to set the target in the follow camera
                        player = eventData.Parameters[0] as Actor3D;
                        (cameraManager[1].ControllerList.Find(c => c is PlayerFollowCameraController) as PlayerFollowCameraController).SetTargetTransform(player.Transform3D);
                        break;
                    case EventActionType.OnPickup:
                        stars++;
                        break;
                }
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        }

        private void StartGame(string levelId)
        {
            levelManager.LoadLevel(levelId);

            //Start curve camera
            cameraManager[0].ControllerList.Add(new Curve3DController("Start Curve Camera", ControllerType.Curve, levelManager.currentLevel.StartCameraCurve));
            cameraManager.ActiveCameraIndex = 0;
            EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));

            //Switch to player camera after curve has been completed
            TimeManager.ExecuteInSeconds("Camera Switch", 9, StartCamera);
        }

        private void StartCamera()
        {
            player.StatusType = StatusType.Update | StatusType.Drawn;
            cameraManager.CycleActiveCamera();
        }

        private void EndGame(EventActionType endState)
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Menu, endState, null));

            //Remove the curve controller
            cameraManager[0].ControllerList.Remove(c => c is Curve3DController);
        }
    }
}