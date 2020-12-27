using System;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Controllers
{
    public class ObstacleSpawnController : Controller
    {
        private int count = 0;
        private Actor3D parent;
        private CollidablePrimitiveObject obstacleArchetype;
        private int levelWidth;
        private Vector3 moveDirection, spawnOffset;

        public ObstacleSpawnController(string id, ControllerType controllerType, CollidablePrimitiveObject obstacleToSpawn, Vector3 moveDir, float levelWidth, Vector3 spawnOffset = default) : base(id, controllerType)
        {
            this.obstacleArchetype = obstacleToSpawn;
            this.levelWidth = (int)MathF.Round(levelWidth);
            moveDirection = moveDir;
            this.spawnOffset = spawnOffset;
        }

        public void SpawnObstacle()
        {
            CollidablePrimitiveObject obstacle = obstacleArchetype.Clone() as CollidablePrimitiveObject;
            obstacle.ID += count++;
            obstacle.ObjectManager.Add(obstacle);

            obstacle.Transform3D.Translation = parent.Transform3D.Translation + spawnOffset;
            EventDispatcher.Publish(new EventData(
                EventCategoryType.Tween, EventActionType.OnAdd, 
                null, null, 
                new object[] { new Tweener(
                    obstacle, levelWidth * 500, 
                    obstacle.Transform3D.Translation + moveDirection * levelWidth, 
                    false, DestroyObstacle)
                })
            );
        }

        private void DestroyObstacle(Actor3D actor)
        {
            EventDispatcher.Publish(new EventData(
                EventCategoryType.Object, EventActionType.OnRemoveActor, 
                null, null, 
                new object[] { actor as DrawnActor3D })
            );
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            parent ??= actor as Actor3D;
        }

        public new object Clone()
        {
            return new ObstacleSpawnController(ID, ControllerType, obstacleArchetype, moveDirection, levelWidth);
        }
    }
}
