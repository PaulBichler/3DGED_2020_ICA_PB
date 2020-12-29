﻿using GDGame.MyGame.Enums;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// Moveable, collidable player using keyboard and checks for collisions
    /// </summary>
    public class CollidablePlayerObject : CollidablePrimitiveObject
    {
        #region Fields
        private float moveSpeed, rotationSpeed;
        private KeyboardManager keyboardManager;
        private Keys[] moveKeys;

        private bool isMoving;
        private Vector3 moveDir;
        private Actor ground;
        #endregion Fields

        public CollidablePlayerObject(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, IVertexData vertexData,
            ICollisionPrimitive collisionPrimitive, ObjectManager objectManager,
            Keys[] moveKeys, float moveSpeed, float rotationSpeed, KeyboardManager keyboardManager)
            : base(id, actorType, statusType, transform, effectParameters, vertexData, collisionPrimitive, objectManager)
        {
            this.moveKeys = moveKeys;
            this.moveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;

            //for movement
            this.keyboardManager = keyboardManager;

            //Notify the GameStateManager that the player has been spawned (used for the camera)
            EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnSpawn, new []{ this }));
        }

        public override void Update(GameTime gameTime)
        {
            //reset translate and rotate and update primitive
            base.Update(gameTime);

            //read any input and store suggested increments
            HandleInput(gameTime);

            //have we collided with something?
            Collidee = CheckAllCollisions(gameTime);

            //how do we respond to this collidee e.g. pickup?
            HandleCollisionResponse(Collidee);

            //if no collision then move - see how we set this.Collidee to null in HandleCollisionResponse()
            //below when we hit against a zone
            if (Collidee == null)
            {
                if (!isMoving && moveDir != Vector3.Zero)
                {
                    EventDispatcher.Publish(new EventData
                    (
                        EventCategoryType.Tween, EventActionType.OnAdd, new object[]
                        {
                            new Tweener(this, 300, moveDir, true, MovementCallback, LoopType.PlayOnce, EasingType.easeOut)
                        }
                    ));
                    isMoving = true;
                }
            }
        }

        private void MovementCallback(Actor3D actor)
        {
            moveDir = Vector3.Zero;
            isMoving = false;
            CheckGround();
        }

        /// <summary>
        /// Checks what tile is directly underneath the player and performs an action based on it.
        /// This is called directly after the player's move has finished.
        /// </summary>
        private void CheckGround()
        {
            Actor newGround = Raycast(Transform3D.Translation, -Vector3.UnitY, 1f);
            if (newGround == null)
            {
                //No ground detected --> player dies (Water tiles have no collision, so water will kill the player too)
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new []
                {
                    new Tweener(this, 200, new Vector3(0, -2, 0), 
                        true, actor3D => Die()) 
                }));
                return;
            }

            //Detach from the last water platform
            if(ground != null && ground.ActorType == ActorType.WaterPlatform)
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnRemoveChild, new [] {ground as Actor3D, this}));

            //Attach to a new water platform
            if (newGround.ActorType == ActorType.WaterPlatform)
            {
                Vector3 platformTrans = (newGround as Actor3D).Transform3D.Translation;
                Transform3D.Translation = new Vector3(platformTrans.X, Transform3D.Translation.Y, platformTrans.Z);
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAddChild, new [] {newGround as Actor3D, this}));
            }

            //Update the ground
            ground = newGround;
        }

        private void Die()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnLose, null));
            EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, new object[] { this }));
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (keyboardManager.IsKeyDown(moveKeys[0])) //Forward
                Transform3D.TranslateIncrement = moveDir = -Vector3.UnitZ;
            else if (keyboardManager.IsKeyDown(moveKeys[1])) //Backward
                Transform3D.TranslateIncrement = moveDir = Vector3.UnitZ;
            else if (keyboardManager.IsKeyDown(moveKeys[2])) //Left
                Transform3D.TranslateIncrement = moveDir = -Vector3.UnitX;
            else if (keyboardManager.IsKeyDown(moveKeys[3])) //Right
                Transform3D.TranslateIncrement = moveDir = Vector3.UnitX;
        }

        /********************************************************************************************/

        //this is where you write the application specific CDCR response for your game
        protected override void HandleCollisionResponse(Actor collidee)
        {
            if (collidee is CollidableZoneObject)
            {
                CollidableZoneObject simpleZoneObject = collidee as CollidableZoneObject;

                //do something based on the zone type - see Main::InitializeCollidableZones() for ID
                if (simpleZoneObject.ID.Equals("camera trigger zone 1"))
                {
                    //publish an event e.g sound, health progress
                    object[] additionalParameters = { "boing" };
                    EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, additionalParameters));
                }

                //IMPORTANT - setting this to null means that the ApplyInput() method will get called and the player can move through the zone.
                Collidee = null;
            }
            else if (collidee is CollidablePrimitiveObject obstacle)
            {
                if (collidee.ActorType == ActorType.Obstacle)
                {
                    System.Diagnostics.Debug.WriteLine(CollisionPrimitive);
                    System.Diagnostics.Debug.WriteLine(Transform3D.Translation);
                    System.Diagnostics.Debug.WriteLine((collidee as CollidablePrimitiveObject).CollisionPrimitive);
                    System.Diagnostics.Debug.WriteLine((collidee as CollidablePrimitiveObject).Transform3D.Translation);

                    //Distance check to prevent collision issues
                    if(Vector3.Distance(obstacle.Transform3D.Translation, Transform3D.Translation) <= 1f)
                        Die();
                }
                else if (collidee.ActorType == ActorType.CollidablePickup)
                {
                    //remove the object
                    object[] parameters = { collidee };
                    EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, parameters));
                }
            }
        }

        public new object Clone()
        {
            CollidablePlayerObject player = new CollidablePlayerObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, 
                EffectParameters.Clone() as EffectParameters, IVertexData.Clone() as IVertexData, CollisionPrimitive.Clone() as ICollisionPrimitive, 
                ObjectManager, moveKeys, moveSpeed, rotationSpeed, keyboardManager);

            player.ControllerList.AddRange(GetControllerListClone());
            return player;
        }
    }
}