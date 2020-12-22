using GDLibrary.Actors;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    public class MenuScene : IEnumerable<DrawnActor2D>
    {
        #region Fields
        private string id; //"main"
        private List<DrawnActor2D> list;
        #endregion Fields

        #region Properties

        public DrawnActor2D this[int index]
        {
            get
            {
                return list[index];
            }
        }
        public List<DrawnActor2D> List
        {
            get
            {
                return this.list;
            }
        }
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        #endregion Properties

        #region Constructors & Core
        public MenuScene(string id)
        {
            this.id = id;
            list = new List<DrawnActor2D>();
        }

        /// <summary>
        /// Returns an enumerator which allows us to enumerate through the MenuScene object using a foreach loop
        /// </summary>
        /// <returns>Enumerator to list of DrawnActor2D objects</returns>

        /// <seealso cref="https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/make-class-foreach-statement"/>
        public IEnumerator<DrawnActor2D> GetEnumerator()
        {
            if (list != null)
            {
                return list.GetEnumerator();
            }
            else
            {
                throw new Exception("List is invalid!");
            }
        }

        /// <summary>
        /// Called by foreach() loop to access the enumerator for the menu scene
        /// </summary>
        /// <returns>IEnumerator</returns>
        /// <see cref="MenuManager.ApplyDraw(Microsoft.Xna.Framework.GameTime)"/>
        /// <seealso cref="MenuManager.ApplyUpdate(Microsoft.Xna.Framework.GameTime)"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is MenuScene scene &&
                   id == scene.id &&
                   EqualityComparer<List<DrawnActor2D>>.Default.Equals(list, scene.list) &&
                   Count == scene.Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, list, Count);
        }

        public object Clone()
        {
            MenuScene clone = new MenuScene("clone - " + id);

            //uses GetEnumerator() replaces for(int i = 0; i < this.list.Count; i++)
            foreach (DrawnActor2D actor in this)
                clone.List.Add(actor.Clone() as DrawnActor2D);

            return clone;
        }

        #endregion Constructors & Core
    }
}