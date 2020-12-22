using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Allows us to create a pausable drawable component that stores multiple menu scenes (e.g. main, audio, controls) in a dictionary
    /// and updates/draws the appropriate scene to the scene.
    ///
    /// Do NOT modify this class directly. Instead create your own MyMenuManager that defines how the mouse, keyboard and
    /// gamePad events affect the menu.
    /// </summary>
    /// <see cref="GDGame.MyGame.Managers.MyMenuManager"/>
    public class MenuManager : PausableDrawableGameComponent
    {
        #region Fields

        /// <summary>
        /// Stores mapping of sceneID to list of drawn actors
        /// </summary>
        private Dictionary<string, List<DrawnActor2D>> dictionary;

        /// <summary>
        /// Currently drawn/updated list of drawn actors
        /// </summary>
        private List<DrawnActor2D> activeList;

        private SpriteBatch spriteBatch;
        #endregion Fields

        #region Properties

        protected List<DrawnActor2D> ActiveList
        {
            get
            {
                return this.activeList;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public MenuManager(Game game, StatusType statusType, SpriteBatch spriteBatch)
           : base(game, statusType)
        {
            dictionary = new Dictionary<string, List<DrawnActor2D>>();
            activeList = null;
            this.spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Called when we want to set what MenuScene the MenuManager will draw
        /// </summary>
        /// <param name="sceneID">SceneID (e.g. "main", "controls", "audio")</param>
        /// <returns>True if set, otherwise an exception is thrown</returns>
        public bool SetScene(string sceneID)
        {
            if (!dictionary.ContainsKey(sceneID))
            {
                throw new Exception("Invalid scene ID - " + sceneID);
            }

            //if valid then access the menu scene and set as active
            activeList = dictionary[sceneID];
            return true;
        }

        /// <summary>
        /// Adds a new actor to the list in a MenuScene object
        /// </summary>
        /// <param name="sceneID">SceneID (e.g. "main", "controls", "audio")</param>
        /// <param name="actor">DrawnActor2D (e.g. UITextureObject, UITextObject)</param>
        public void Add(string sceneID, DrawnActor2D actor)
        {
            //if this is the first time we are adding an actor to this new sceneID then make a MenuScene and add to dictionary
            if (!dictionary.ContainsKey(sceneID))
            {
                dictionary.Add(sceneID, new List<DrawnActor2D>());
            }

            //get the menu scene for this sceneID
            List<DrawnActor2D> list = dictionary[sceneID];

            //add the new actor to the scene
            list.Add(actor);
        }

        /// <summary>
        /// Removes the first DrawnActor2D object found in the MenuScene for the sceneID provided
        /// </summary>
        /// <param name="sceneID">SceneID (e.g. "main", "controls", "audio")</param>
        /// <param name="predicate"></param>
        /// <returns>True if set, false if not found, throws exception if sceneID is not found</returns>
        public bool Remove(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            if (!dictionary.ContainsKey(sceneID))
            {
                throw new System.Exception("Invalid scene ID - " + sceneID);
            }

            //access the list inside the MenuScene for this sceneID
            List<DrawnActor2D> list = dictionary[sceneID];

            //remove the DrawnActor2D that matches the predicate in the MenuScene's list
            return list.Remove(list.Find(predicate));
        }

        /// <summary>
        /// Clears the contents of the dictionary (i.e. all menu objects)
        /// </summary>
        public void Clear()
        {
            //get all scene IDs
            List<string> sceneIDList = new List<string>(dictionary.Keys);

            //for each sceneID get the list, then get the list in the MenuScene and clear the list
            foreach (string sceneID in sceneIDList)
            {
                //clear the list for a particular sceneID e.g. "main"
                dictionary[sceneID].Clear();
            }

            //clear the string list
            sceneIDList.Clear();

            //clear the dictionary
            dictionary.Clear();
        }

        /// <summary>
        /// Updates all DrawnActors for the currently active scene
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        protected override void ApplyUpdate(GameTime gameTime)
        {
            //just in case user didn't call SetScene
            if (activeList == null)
            {
                throw new Exception("Did you forget to call SetScene() after initializing the menu in Main?");
            }

            //now that we implemented IEnumerable in MenuScene we can use a foreach() loop
            foreach (DrawnActor2D actor in activeList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all DrawnActors for the currently active scene
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        protected override void ApplyDraw(GameTime gameTime)
        {
            //just in case user didn't call SetScene
            if (activeList == null)
            {
                throw new Exception("Did you forget to call SetScene() after initializing the menu in Main?");
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null);
            foreach (DrawnActor2D actor in activeList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        #endregion Constructors & Core
    }
}