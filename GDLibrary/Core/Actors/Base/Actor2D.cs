using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all drawn and undrawn 2D actors used in the engine. This class adds a Transform2D field.
    /// </summary>
    public class Actor2D : Actor
    {
        #region Fields
        private Transform2D transform2D;
        #endregion Fields

        #region Properties

        public Transform2D Transform2D
        {
            get => transform2D; set => transform2D = value;
        }

        #endregion Properties

        #region Constructors & Core

        public Actor2D(string id, ActorType actorType, StatusType statusType, Transform2D transform2D) : base(id, actorType, statusType)
        {
            Transform2D = transform2D;
        }

        public override bool Equals(object obj)
        {
            return obj is Actor2D d &&
                   base.Equals(obj) &&
                   EqualityComparer<EventHandlerList>.Default.Equals(EventHandlerList, d.EventHandlerList) &&
                   EqualityComparer<ControllerList>.Default.Equals(ControllerList, d.ControllerList) &&
                   ID == d.ID &&
                   Description == d.Description &&
                   ActorType == d.ActorType &&
                   StatusType == d.StatusType &&
                   EqualityComparer<Transform2D>.Default.Equals(Transform2D, d.Transform2D);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), EventHandlerList, ControllerList, ID, Description, ActorType, StatusType, Transform2D);
        }

        public new object Clone()
        {
            //deep-copy
            return new Actor2D(ID, //value type so this is deep copy
                ActorType, // value type so this is deep copy
                StatusType, // value type so this is deep copy
                transform2D.Clone() as Transform2D); // reference type so we call its Clone to get a deep copy
        }

        #endregion Constructors & Core
    }
}