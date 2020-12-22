using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all actors used in the engine. This class contains fields to uniquely identify
    /// an actor, its ActorType and its StatusType.
    /// </summary>
    public class Actor : IActor
    {
        #region Fields

        private string id, description;
        private ActorType actorType;
        private StatusType statusType;
        private ControllerList controllerList = new ControllerList();
        private EventHandlerList eventHandlerList = new EventHandlerList();

        #endregion Fields

        #region Properties

        public EventHandlerList EventHandlerList
        {
            get
            {
                return eventHandlerList;
            }
            //no reason to allow this list to be directly set externally
            protected set
            {
                eventHandlerList = value;
            }
        }

        public ControllerList ControllerList
        {
            get
            {
                return controllerList;
            }
            //no reason to allow this list to be directly set externally
            protected set
            {
                controllerList = value;
            }
        }

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                //remove whitespace on LHS or RHS of a user-defined string
                id = value.Trim();
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value.Trim();
            }
        }

        public ActorType ActorType
        {
            get
            {
                return actorType;
            }
            set
            {
                actorType = value;
            }
        }

        public StatusType StatusType
        {
            get
            {
                return statusType;
            }
            set
            {
                statusType = value;
            }
        }

        #endregion Properties

        #region Constructors

        public Actor(string id, ActorType actorType, StatusType statusType)
        {
            this.id = id;
            this.actorType = actorType;
            this.statusType = statusType;
        }

        #endregion Constructors

        public virtual void Update(GameTime gameTime)
        {
            //calls update on any attached controllers
            controllerList.Update(gameTime, this);
        }

        public override bool Equals(object obj)
        {
            return obj is Actor actor &&
                   id == actor.id &&
                   description == actor.description &&
                   actorType == actor.actorType &&
                   statusType == actor.statusType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, description, actorType, statusType);
        }

        public object Clone()
        {
            //to do...are we also cloning controllers and event handlers???

            //deep-copy or shallow-copy
            //value types - byte, sbyte, double, boolean, string, struct (e.g. Vector3), enums (e.g. PrimitiveType)
            //reference types - user-defined classes, MonoGame classes, array

            Actor clonedActor = new Actor(id, actorType, statusType);  //deep
            clonedActor.ControllerList.AddRange(GetControllerListClone());
            return clonedActor;
        }

        protected List<IController> GetControllerListClone()
        {
            List<IController> list = new List<IController>();
            foreach (IController controller in this.controllerList)
            {
                list.Add(controller.Clone() as IController);
            }

            return list;
        }
    }
}