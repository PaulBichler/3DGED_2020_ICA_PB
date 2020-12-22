using GDLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace GDLibrary.Containers
{
    /// <summary>
    /// Used to store event handlers to be applied to an actor
    /// </summary>
    /// <see cref="GDLibrary.Actors.Actor"/>
    public class AdvancedList : List<IEventHandler>
    {
        #region Constructors & Core

        public virtual bool Remove(Predicate<IEventHandler> predicate)
        {
            int position = this.FindIndex(predicate);
            if (position != -1)
            {
                this.RemoveAt(position);
                return true;
            }
            return false;
        }

        public virtual int Transform(Predicate<IEventHandler> filter,
                                            Action<IEventHandler> transform)
        {
            int count = 0;
            foreach (IEventHandler obj in this)
            {
                if (filter(obj))
                {
                    transform(obj);
                    count++;
                }
            }
            return count;
        }

        #endregion Constructors & Core
    }
}