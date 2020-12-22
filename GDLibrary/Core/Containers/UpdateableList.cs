using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Containers
{
    /// <summary>
    /// Used to store controllers to be applied to an actor
    /// </summary>
    /// <see cref="GDLibrary.Actors.Actor"/>
    public class UpdateableList : List<IController>
    {
        #region Constructors & Core

        public virtual bool Remove(Predicate<IController> predicate)
        {
            int position = this.FindIndex(predicate);
            if (position != -1)
            {
                this.RemoveAt(position);
                return true;
            }
            return false;
        }

        public virtual int Transform(Predicate<IController> filter,
                                            Action<IController> transform)
        {
            int count = 0;
            foreach (IController obj in this)
            {
                if (filter(obj))
                {
                    transform(obj);
                    count++;
                }
            }
            return count;
        }

        public virtual void Update(GameTime gameTime, IActor parent)
        {
            //calls update on all controllers
            foreach (IController controller in this)
            {
                controller.Update(gameTime, parent);
            }
        }

        #endregion Constructors & Core
    }
}