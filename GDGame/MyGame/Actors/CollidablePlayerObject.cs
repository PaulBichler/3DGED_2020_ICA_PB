using GDGame.MyGame.Enums;
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
        }

        public override void Update(GameTime gameTime)
        {
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
                        EventCategoryType.Tween,
                        EventActionType.OnAdd,
                        null, null,
                        new object[] {new Tweener(this, 300, moveDir, true, MovementCallback, LoopType.PlayOnce, EasingType.easeOut)}
                    ));
                    isMoving = true;
                }

                //ApplyInput(gameTime);
            }

            //reset translate and rotate and update primitive
            base.Update(gameTime);
        }

        private void MovementCallback()
        {
            moveDir = Vector3.Zero;
            isMoving = false;
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (keyboardManager.IsKeyDown(moveKeys[0])) //Forward
            {
                System.Diagnostics.Debug.WriteLine("forward");
                //Transform3D.TranslateIncrement
                //    = Transform3D.Look * gameTime.ElapsedGameTime.Milliseconds
                //            * moveSpeed;
                Transform3D.TranslateIncrement = moveDir = -Vector3.UnitZ;
            }
            else if (keyboardManager.IsKeyDown(moveKeys[1])) //Backward
            {
                //Transform3D.TranslateIncrement
                //   = -Transform3D.Look * gameTime.ElapsedGameTime.Milliseconds
                //           * moveSpeed;

                Transform3D.TranslateIncrement = moveDir = Vector3.UnitZ;
            }

            if (keyboardManager.IsKeyDown(moveKeys[2])) //Left
            {
                //Transform3D.RotateIncrement = gameTime.ElapsedGameTime.Milliseconds * rotationSpeed;
                Transform3D.TranslateIncrement = moveDir = -Vector3.UnitX;
            }
            else if (keyboardManager.IsKeyDown(moveKeys[3])) //Right
            {
                //Transform3D.RotateIncrement = -gameTime.ElapsedGameTime.Milliseconds * rotationSpeed;
                Transform3D.TranslateIncrement = moveDir = Vector3.UnitX;
            }
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
            else if (collidee is CollidablePrimitiveObject)
            {
                //the boxes on the left that we loaded from level loader
                if (collidee.ActorType == ActorType.CollidablePickup)
                {
                    //remove the object
                    object[] parameters = { collidee };
                    EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, parameters));
                }
                //the boxes on the right that move up and down
                else if (collidee.ActorType == ActorType.CollidableDecorator)
                {
                    (collidee as DrawnActor3D).EffectParameters.DiffuseColor = Color.Blue;
                }
            }
        }

        public new object Clone()
        {
            CollidablePlayerObject player = new CollidablePlayerObject(ID, ActorType, StatusType, Transform3D, EffectParameters, IVertexData,
                CollisionPrimitive, ObjectManager, moveKeys, moveSpeed, rotationSpeed, keyboardManager);

            player.ControllerList.AddRange(GetControllerListClone());
            return player;
        }
    }
}