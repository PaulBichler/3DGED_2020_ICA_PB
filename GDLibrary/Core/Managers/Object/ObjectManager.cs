using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Stores and calls an Update on all drawn 3D objects. This is a pausable component (i.e. listens for menu play/pause events)
    /// and so it extends PausableDrawableGameComponent. This class also subscribes to, and adds events to handle, relevent events
    /// (e.g. object remove, Object add, object change transparency)
    /// </summary>
    /// <see cref="GDLibrary.GameComponents.PausableDrawableGameComponent.SubscribeToEvents"/>
    public class ObjectManager : PausableGameComponent, IEventHandler
    {
        #region Fields

        private List<DrawnActor3D> opaqueList, transparentList;
        private List<DrawnActor3D> removeList;
        #endregion Fields

        #region Properties

        public List<DrawnActor3D> OpaqueList
        {
            get
            {
                return opaqueList;
            }
        }

        public List<DrawnActor3D> TransparentList
        {
            get
            {
                return transparentList;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public ObjectManager(Game game, StatusType statusType,
          int initialOpaqueDrawSize, int initialTransparentDrawSize) : base(game, statusType)
        {
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);
            removeList = new List<DrawnActor3D>();
        }

        #region Handle Events

        public override void SubscribeToEvents()
        {
            //opacity
            EventDispatcher.Subscribe(EventCategoryType.Opacity, HandleEvent);

            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add more ObjectManager specfic subscriptions here...
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);

            //call base method to subscribe to menu event
            base.SubscribeToEvents();
        }

        public override void HandleEvent(EventData eventData)
        {
            DrawnActor3D actor = null;

            switch (eventData.EventActionType)
            {
                case EventActionType.OnWin:
                    //call code to handle win here...
                    break;

                case EventActionType.OnLose:
                    //call code to handle win here...
                    break;

                case EventActionType.OnAddActor:
                    Add(eventData.Parameters[0] as DrawnActor3D);
                    break;

                case EventActionType.OnRemoveActor:
                    Remove(eventData.Parameters[0] as DrawnActor3D);
                    break;

                case EventActionType.OnOpaqueToTransparent:
                    actor = eventData.Parameters[0] as DrawnActor3D;
                    opaqueList.Remove(actor);
                    transparentList.Add(actor);
                    break;

                case EventActionType.OnTransparentToOpaque:
                    actor = eventData.Parameters[0] as DrawnActor3D;
                    transparentList.Remove(actor);
                    opaqueList.Add(actor);
                    break;

                case EventActionType.OnApplyActionToFirstMatchActor:
                    ApplyActionToActor(eventData);
                    break;

                case EventActionType.OnApplyActionToAllActors:
                    ApplyActionToAllActors(eventData);
                    break;
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        }

        /// <summary>
        /// Applies an action to an actor found in the object manager based on a predicate (and action) defined in the eventData object
        ///
        /// Usage:
        ///    EventDispatcher.Publish(new EventData(
        ///         EventCategoryType.Object, EventActionType.OnApplyActionToActor,
        ///        (actor) => actor.StatusType = StatusType.Drawn,
        ///        (actor) => actor.ActorType == ActorType.Decorator
        ///       && actor.ID.Equals("green key"), null));
        ///
        ///
        /// </summary>
        /// <param name="eventData"></param>
        private void ApplyActionToActor(EventData eventData)
        {
            if (eventData.Predicate != null && eventData.Action != null)
            {
                DrawnActor3D actor = null;

                //we need to look in both lists for the actor since we dont know which it is in
                actor = opaqueList.Find(eventData.Predicate);
                if (actor != null)
                    eventData.Action(actor);

                actor = transparentList.Find(eventData.Predicate);
                if (actor != null)
                    eventData.Action(actor);
            }
        }

        /// <summary>
        /// Applies an action to ALL actors found in the list based on a matching predicate (and action) defined in the eventData object
        ///
        /// Usage:
        ///    EventDispatcher.Publish(new EventData(
        ///         EventCategoryType.UI, EventActionType.OnApplyActionToFirstMatchActor,
        ///        (actor) => actor.StatusType = StatusType.Drawn,
        ///        (actor) => actor.ActorType == ActorType.UITextureObject
        ///       && actor.ID.Equals("green key"), null));
        ///
        ///
        /// </summary>
        /// <param name="eventData"></param>
        public void ApplyActionToAllActors(EventData eventData)
        {
            if (eventData.Predicate != null && eventData.Action != null)
            {
                List<DrawnActor3D> list = null;

                //we need to look in both lists for the actor since we dont know which it is in
                list = opaqueList.FindAll(eventData.Predicate);
                if (list != null)
                {
                    foreach (DrawnActor3D actor in list)
                        eventData.Action(actor);
                }
                // list.Clear();

                list = transparentList.FindAll(eventData.Predicate);
                if (list != null)
                {
                    foreach (DrawnActor3D actor in list)
                        eventData.Action(actor);
                }
            }
        }

        #endregion Handle Events

        /// <summary>
        /// Add the actor to the appropriate list based on actor transparency
        /// </summary>
        /// <param name="actor"></param>
        public void Add(DrawnActor3D actor)
        {
            if (actor == null)
                return;

            if (actor.EffectParameters?.Alpha < 1)
            {
                transparentList.Add(actor);
            }
            else
            {
                opaqueList.Add(actor);
            }
        }

        /// <summary>
        /// Add a collection of actors
        /// </summary>
        /// <see cref="GDLibrary.Utilities.LevelLoader.Load(Microsoft.Xna.Framework.Graphics.Texture2D, float, float, float, Vector3)"/>
        /// <param name="list"></param>
        public void Add(List<DrawnActor3D> list)
        {
            if (list == null)
                return;

            foreach (DrawnActor3D actor in list)
                Add(actor);
        }

        /// <summary>
        /// Removes an actor from the list by adding to a batch remove list which is processed before each update
        /// </summary>
        /// <param name="actor"></param>
        public void Remove(DrawnActor3D actor)
        {
            if (actor == null)
                return;

            removeList.Add(actor);
        }

        /// <summary>
        /// Remove the first instance of an actor corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify an actor</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool RemoveFirstIf(Predicate<DrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int position = -1;
            bool wasRemoved = false;

            position = opaqueList.FindIndex(predicate);   //N
            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                wasRemoved = true;
            }

            position = transparentList.FindIndex(predicate);  //M
            if (position != -1)
            {
                transparentList.RemoveAt(position);
                wasRemoved = true;
            }

            //O(N + M)
            return wasRemoved;
        }

        /// <summary>
        /// Remove all occurences of any actors corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows ObjectManager to uniquely identify one or more actors</param>
        /// <returns>Count of the number of removed actors</returns>
        public int RemoveAll(Predicate<DrawnActor3D> predicate)
        {
            //to do...improve efficiency by adding DrawType enum
            int count = 0;
            count = opaqueList.RemoveAll(predicate);
            count += transparentList.RemoveAll(predicate);
            return count;
        }

        /// <summary>
        /// Called to update the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Update(GameTime)"/>
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyUpdate(GameTime gameTime)
        {
            ApplyBatchRemove();

            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }
        }

        private void ApplyBatchRemove()
        {
            foreach (DrawnActor3D actor in removeList)
            {
                if (actor.EffectParameters.Alpha < 1)
                {
                    transparentList.Remove(actor);
                }
                else
                {
                    opaqueList.Remove(actor);
                }
            }

            removeList.Clear();
        }

        /// <summary>
        /// Clears all content - Use when we restart or start next level (e.g. level 2)
        /// </summary>
        public void Clear()
        {
            opaqueList.Clear();
            transparentList.Clear();
        }
        #endregion Constructors & Core
    }
}