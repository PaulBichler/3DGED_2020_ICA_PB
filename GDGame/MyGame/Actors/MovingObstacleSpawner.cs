using System;
using System.Collections.Generic;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Actors
{
    public class MovingObstacleSpawner : CollidablePrimitiveObject
    {
        private CollidablePrimitiveObject obstacleArchetype;
        private int levelWidth;
        private Vector3 moveDirection, spawnOffset, destination;

        public MovingObstacleSpawner(string id, ActorType actorType, StatusType statusType, Transform3D transform3D, 
            EffectParameters effectParameters, IVertexData vertexData, ICollisionPrimitive collisionPrimitive, ObjectManager objectManager) 
            : base(id, actorType, statusType, transform3D, effectParameters, vertexData, collisionPrimitive, objectManager)
        {
        }

        public void InitSpawner(CollidablePrimitiveObject obstacleToSpawn, Vector3 moveDir, float levelWidth, Vector3 spawnOffset = default)
        {
            this.obstacleArchetype = obstacleToSpawn;
            this.levelWidth = (int)MathF.Round(levelWidth);
            moveDirection = moveDir;
            this.spawnOffset = spawnOffset;

            destination = Transform3D.Translation + spawnOffset + moveDirection * levelWidth;
        }

        //used if the obstacles are pre-spawned (called in the level loader)
        public void StartSpawner(List<Actor3D> movingObstacles = null)
        {
            foreach (Actor3D movingObstacle in movingObstacles)
            {
                movingObstacle.Transform3D.TranslateBy(spawnOffset);
                MoveObstacle(movingObstacle);
            }
        }

        /// <summary>
        /// Manually spawns an obstacle
        /// </summary>
        public void SpawnObstacle()
        {
            CollidablePrimitiveObject obstacle = obstacleArchetype.Clone() as CollidablePrimitiveObject;
            //obstacle.ID += count++;
            obstacle.ObjectManager.Add(obstacle);

            obstacle.Transform3D.Translation = Transform3D.Translation + spawnOffset;
            MoveObstacle(obstacle);
        }

        /// <summary>
        /// Starts moving the obstacle
        /// </summary>
        /// <param name="obstacle">The obstacle to move</param>
        private void MoveObstacle(Actor3D obstacle)
        {
            EventDispatcher.Publish(new EventData(
                EventCategoryType.Tween, EventActionType.OnAdd,
                new object[] { new Tweener(
                    obstacle, 1000 * (int)(destination - obstacle.Transform3D.Translation).Length(), 
                    destination, false, DestroyObstacle)
                })
            );
        }

        /// <summary>
        /// Movement callback for the obstacle. Destroys the obstacle and resets it
        /// </summary>
        /// <param name="actor">The actor to reset</param>
        private void DestroyObstacle(Actor3D actor)
        {
            EventDispatcher.Publish(new EventData(
                EventCategoryType.Object, EventActionType.OnRemoveActor, 
                null, null, 
                new object[] { actor as DrawnActor3D })
            );

            //new obstacle is spawned when the old one is destroyed
            SpawnObstacle();
        }

        public new object Clone()
        {
            MovingObstacleSpawner spawner = new MovingObstacleSpawner(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, 
                EffectParameters.Clone() as EffectParameters, IVertexData.Clone() as IVertexData, CollisionPrimitive.Clone() as ICollisionPrimitive, 
                ObjectManager);

            spawner.ControllerList.AddRange(GetControllerListClone());
            return spawner;
        }
    }
}
