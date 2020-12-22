using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace GDLibrary.Events
{
    /// <summary>
    /// Encapsulates the fields of an event within the game
    /// </summary>
    /// <see cref="GDLibrary.Events.EventDispatcher"/>
    public class EventData : ICloneable
    {
        #region Fields
        private EventCategoryType eventCategoryType;
        private EventActionType eventActionType;
        private Action<Actor> action;
        private Predicate<Actor> predicate;
        private object[] parameters;
        #endregion Fields

        #region Properties
        public EventCategoryType EventCategoryType { get => eventCategoryType; set => eventCategoryType = value; }
        public EventActionType EventActionType { get => eventActionType; set => eventActionType = value; }
        public object[] Parameters { get => parameters; set => parameters = value; }
        public Action<Actor> Action { get => action; set => action = value; }
        public Predicate<Actor> Predicate { get => predicate; set => predicate = value; }
        #endregion Properties

        #region Constructors & Core

        /// <summary>
        /// Used for events where we want to apply an action (e.g. transform/change actor state) using the action, predicate and parameters passed
        /// </summary>
        /// <param name="eventCategoryType"></param>
        /// <param name="eventActionType"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        public EventData(EventCategoryType eventCategoryType,
           EventActionType eventActionType, Action<Actor> action,
           Predicate<Actor> predicate, object[] parameters)
        {
            EventCategoryType = eventCategoryType;
            EventActionType = eventActionType;
            Action = action;
            Predicate = predicate;
            Parameters = parameters;
        }

        /// <summary>
        /// Used for events with associated parameters (e.g. sound events where params=["boing", 1, true]
        /// </summary>
        /// <param name="eventCategoryType"></param>
        /// <param name="eventActionType"></param>
        /// <param name="parameters"></param>
        public EventData(EventCategoryType eventCategoryType,
            EventActionType eventActionType, object[] parameters)
            : this(eventCategoryType, eventActionType, null, null, parameters)
        {
        }

        public override bool Equals(object obj)
        {
            //  if(parameters != null)
            return obj is EventData data &&
                   eventCategoryType == data.eventCategoryType &&
                   eventActionType == data.eventActionType &&
                   parameters != null
                   ? EqualityComparer<object[]>.Default.Equals(parameters, data.parameters) : false;
        }

        public override int GetHashCode()
        {
            int hashCode = HashCode.Combine(eventCategoryType, eventActionType);

            if (parameters != null)
                HashCode.Combine(hashCode, parameters);

            return hashCode;
        }

        public override string ToString()
        {
            if (parameters == null)
            {
                return eventCategoryType + "," + eventActionType + ", [no params]";
            }
            else
            {
                string parametersAsString = String.Join(",", Array.ConvertAll(parameters, item => item.ToString()));
                return eventCategoryType + "," + eventActionType + "," + parametersAsString;
            }
        }

        public object Clone()
        {
            return new EventData(eventCategoryType, eventActionType,
                parameters);
        }

        #endregion Constructors & Core
    }
}