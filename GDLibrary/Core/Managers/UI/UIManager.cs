using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class UIManager : PausableDrawableGameComponent
    {
        #region Fields
        private SpriteBatch spriteBatch;
        private List<DrawnActor2D> uiObjectList;
        private List<DrawnActor2D> removeList;
        #endregion Fields

        #region Properties
        public List<DrawnActor2D> UIObjectList
        {
            get
            {
                return uiObjectList;
            }
        }

        #endregion Properties
        public UIManager(Game game, StatusType statusType, SpriteBatch spriteBatch, int initialDrawSize) : base(game, statusType)
        {
            this.spriteBatch = spriteBatch;
            uiObjectList = new List<DrawnActor2D>(initialDrawSize);
            removeList = new List<DrawnActor2D>();
        }

        public override void SubscribeToEvents()
        {
            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvent);
            //we want to subscribe to menu
            base.SubscribeToEvents();
        }
        public override void HandleEvent(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnAddActor:
                    Add(eventData.Parameters[0] as DrawnActor2D);
                    break;

                case EventActionType.OnRemoveActor:
                    Remove(eventData.Parameters[0] as DrawnActor2D);
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
        /// Applies an action to the FIRST actor found in the list based on a matching predicate (and action) defined in the eventData object
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
        public void ApplyActionToActor(EventData eventData)
        {
            if (eventData.Predicate != null && eventData.Action != null)
            {
                DrawnActor2D actor = uiObjectList.Find(eventData.Predicate);
                if (actor != null)
                {
                    eventData.Action(actor);
                }
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
                List<DrawnActor2D> list = uiObjectList.FindAll(eventData.Predicate);
                if (list != null)
                {
                    foreach (DrawnActor2D actor in list)
                        eventData.Action(actor);
                }
            }
        }

        /// <summary>
        /// Add an actor to the ui
        /// </summary>
        /// <param name="actor"></param>
        public void Add(DrawnActor2D actor)
        {
            uiObjectList.Add(actor);
        }

        /// <summary>
        /// Removes an actor from the list by adding to a batch remove list which is processed before each update
        /// </summary>
        /// <param name="actor"></param>
        public void Remove(DrawnActor2D actor)
        {
            removeList.Add(actor);
        }

        /// <summary>
        /// Remove the first instance of an actor corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows UIManager to uniquely identify an actor</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool RemoveFirstIf(Predicate<DrawnActor2D> predicate)
        {
            int position = uiObjectList.FindIndex(predicate);
            if (position != -1)
            {
                uiObjectList.RemoveAt(position);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all occurences of any actors corresponding to the predicate
        /// </summary>
        /// <param name="predicate">Lambda function which allows UIManager to uniquely identify one or more actors</param>
        /// <returns>Count of the number of removed actors</returns>
        public int RemoveAll(Predicate<DrawnActor2D> predicate)
        {
            return uiObjectList.RemoveAll(predicate);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            ApplyBatchRemove();

            foreach (DrawnActor2D actor in uiObjectList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            base.ApplyUpdate(gameTime);
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null);
            foreach (DrawnActor2D actor in uiObjectList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            base.ApplyDraw(gameTime);
        }

        private void ApplyBatchRemove()
        {
            foreach (DrawnActor2D actor in removeList)
                uiObjectList.Remove(actor);

            removeList.Clear();
        }
    }
}