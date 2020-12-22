using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Events
{
    public class EventDispatcher : GameComponent
    {
        #region Statics
        /// <summary>
        /// Controls access to the queue and prevents the same event from existing in the queue for a single update cycle (e.g. when playing a sound based on keyboard press)
        /// </summary>
        private static HashSet<EventData> sentinelSet;

        /// <summary>
        /// Stores FIFO queue of the events sent in the game
        /// </summary>
        private static Queue<EventData> queue;

        /// <summary>
        /// Stores mapping of event category to delegate function provided by the subscriber
        /// </summary>
        private static Dictionary<EventCategoryType, List<EventHandlerDelegate>> dictionary;
        #endregion Statics

        #region Delegates
        public delegate void EventHandlerDelegate(EventData eventData);
        #endregion Delegates

        #region Constructors & Core
        public EventDispatcher(Game game) : base(game)
        {
            queue = new Queue<EventData>();
            sentinelSet = new HashSet<EventData>();
            dictionary = new Dictionary<EventCategoryType, List<EventHandlerDelegate>>();
        }
        #endregion Constructors & Core

        #region Subscribe & Publish
        public static void Subscribe(EventCategoryType eventCategoryType, EventHandlerDelegate del)
        {
            if (dictionary == null)
                throw new Exception("Have you initialized the EventDispatcher before calling Subscribe?");

            if (!dictionary.ContainsKey(eventCategoryType))
                dictionary.Add(eventCategoryType, new List<EventHandlerDelegate>());

            dictionary[eventCategoryType].Add(del);
        }
        public static bool Unsubscribe(EventCategoryType eventCategoryType, EventHandlerDelegate del)
        {
            if (dictionary.ContainsKey(eventCategoryType))
            {
                List<EventHandlerDelegate> list = dictionary[eventCategoryType];
                list.Remove(del);
                return true;
            }
            return false;
        }
        public static void Publish(EventData eventData)
        {
            if (!sentinelSet.Contains(eventData))
            {
                queue.Enqueue(eventData);
                sentinelSet.Add(eventData);
            }
        }
        #endregion Subscribe & Publish

        #region Update & Notify
        public override void Update(GameTime gameTime)
        {
            NotifyAll();

            //leave events for the next update in case our game is pre-empted
            //queue.Clear();
            //sentinelSet.Clear();

            //base.Update(gameTime); //does nothing so comment out
        }
        private void NotifyAll()
        {
            for (int i = 0; i < queue.Count; i++)
            {
                //access the event
                EventData eventData = queue.Dequeue();

                //process the event
                if (dictionary.ContainsKey(eventData.EventCategoryType))
                    // List<EventHandlerDelegate> list = dictionary[eventData.EventCategoryType];
                    foreach (EventHandlerDelegate del in dictionary[eventData.EventCategoryType])
                        del(eventData); //notifying the original subscribing function/method
                                        // del.Invoke(eventData);

                //remove from sentinel set
                sentinelSet.Remove(eventData);
            }
        }
        #endregion Update & Notify
    }
}