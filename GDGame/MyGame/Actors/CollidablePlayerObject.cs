using GDGame;
using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Utilities;
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
        private KeyboardManager keyboardManager;
        private Keys[] moveKeys;

        private bool isMoving;
        private Vector3 moveDir;
        private Actor ground;
        #endregion Fields

        public CollidablePlayerObject(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            EffectParameters effectParameters, IVertexData vertexData, ICollisionPrimitive collisionPrimitive, 
            ObjectManager objectManager, Keys[] moveKeys, KeyboardManager keyboardManager)
            : base(id, actorType, statusType, transform, effectParameters, vertexData, collisionPrimitive, objectManager)
        {
            this.moveKeys = moveKeys;
            this.keyboardManager = keyboardManager;
        }

        public void Initialize()
        {
            //Notify the GameStateManager that the player has been spawned (used to set the target of the camera)
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
                Move();
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (keyboardManager.IsKeyDown(moveKeys[0])) //Forward
                moveDir = -Vector3.UnitZ;
            else if (keyboardManager.IsKeyDown(moveKeys[1])) //Backward
                moveDir = Vector3.UnitZ;
            else if (keyboardManager.IsKeyDown(moveKeys[2])) //Left
                moveDir = -Vector3.UnitX;
            else if (keyboardManager.IsKeyDown(moveKeys[3])) //Right
                moveDir = Vector3.UnitX;
        }

        /********************************************************************************************/

        //this is where you write the application specific CDCR response for your game
        protected override void HandleCollisionResponse(Actor collidee)
        {
            if (collidee is CollidableZoneObject zone)
            {
                if(zone.ActorType == ActorType.WinZone)
                    EventDispatcher.Publish(new EventData(EventCategoryType.GameState, EventActionType.OnWin, null));

                Collidee = null;
            }
            else if (collidee is CollidablePrimitiveObject obstacle)
            {
                if (collidee.ActorType == ActorType.Obstacle || collidee is CollidableProjectile)
                {
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

        private void Move()
        {
            if (!isMoving && moveDir != Vector3.Zero)
            {
                Actor obstacleCheck = CheckCollisionAfterTranslation(moveDir);
                if (obstacleCheck != null && obstacleCheck.ActorType == ActorType.BlockingObstacle)
                    return;

                //we are currently on a water platform --> detach from it 
                if (ground != null && ground.ActorType == ActorType.WaterPlatform)
                    EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnRemoveChild, new[] {ground as Actor3D, this}));

                TranslationTween jumpDown = new TranslationTween(this, GameConstants.Player_MovementTimeInMs / 2, 
                    moveDir / 2 - Vector3.Up, true, 
                    MovementCallback, LoopType.PlayOnce, EasingType.easeOut);
                    
                TranslationTween jumpUp = new TranslationTween(this, GameConstants.Player_MovementTimeInMs / 2, 
                    moveDir / 2 + Vector3.Up, true, 
                    actor3D => EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new [] { jumpDown })), 
                    LoopType.PlayOnce, EasingType.easeOut);

                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new object[] { jumpUp }));
                isMoving = true;
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
            Actor3D newGround = CheckCollisionAfterTranslation(-Vector3.UnitY) as Actor3D;

            if (newGround == null)
            {
                //No ground detected --> player dies (Water tiles have no collision, so water will kill the player too)
                EventDispatcher.Publish(new EventData(EventCategoryType.Tween, EventActionType.OnAdd, new []
                {
                    new TranslationTween(this, 200, new Vector3(0, -2, 0), 
                        true, actor3D => Die()) 
                }));
                return;
            }

            //Correcting the X and Y position of the player
            Transform3D groundTransform = newGround.Transform3D;
            Vector3 position = new Vector3(groundTransform.Translation.X, Transform3D.Translation.Y, groundTransform.Translation.Z);
            Transform3D.Translation = position;

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

        public new object Clone()
        {
            CollidablePlayerObject player = new CollidablePlayerObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, 
                EffectParameters.Clone() as EffectParameters, IVertexData.Clone() as IVertexData, CollisionPrimitive.Clone() as ICollisionPrimitive, 
                ObjectManager, moveKeys, keyboardManager);

            player.ControllerList.AddRange(GetControllerListClone());
            return player;
        }
    }
}