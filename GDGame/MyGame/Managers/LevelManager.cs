using System;
using System.Collections.Generic;
using GDLibrary.Actors;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using GDLibrary.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.MyGame.Managers
{
    public struct LevelInfo
    {
        public string Name;
        public List<Texture2D> LevelLayerTextures;
        public Transform3DCurve StartCameraCurve;
        public float xScale, zScale, LayerHeightOffset;
        public Vector3 Offset;
    }

    /// <summary>
    /// The Level Manager stores all of the levels with an ID and provides functionality to load them by their ID
    /// </summary>
    public class LevelManager
    {
        #region Fields
        private Main main;
        private ObjectManager objectManager;
        private Dictionary<string, LevelInfo> levels;
        #endregion

        #region Properties
        public LevelInfo CurrentLevel { get; private set; }
        public LevelLoader<PrimitiveObject> LevelLoader { get; set; }
        #endregion

        #region Constructor & Core
        public LevelManager(Main game, ObjectManager objectManager)
        {
            this.main = game;
            this.objectManager = objectManager;
            levels = new Dictionary<string, LevelInfo>();
        }

        /// <summary>
        /// Add a level to the game
        /// </summary>
        /// <param name="ID">The ID the level will be called by</param>
        /// <param name="level">The level information</param>
        public void AddLevel(string ID, LevelInfo level)
        {
            if (!levels.ContainsKey(ID))
                levels.Add(ID, level);
        }

        /// <summary>
        /// Load a previously added level by ID
        /// </summary>
        /// <param name="ID">The ID of the level to load</param>
        public void LoadLevel(string ID)
        {
            if (!levels.ContainsKey(ID))
                throw new ArgumentException("No Level with ID: " + ID + " found!");

            LoadLevel(levels[ID]);
        }

        private void LoadLevel(LevelInfo level)
        {
            // clear the scene and draw the skybox and ground
            objectManager.Clear();
            main.InitSkybox(1000);
            main.InitGround(1000);

            // load all of the level layers in the level information
            float heightOffset = 0;
            foreach (Texture2D layerTexture in level.LevelLayerTextures)
            {
                List<DrawnActor3D> actorList = LevelLoader.Load(
                    layerTexture,
                    level.xScale,               //number of in-world x-units represented by 1 pixel in image
                    level.zScale,               //number of in-world z-units represented by 1 pixel in image
                    heightOffset,               //y-axis height offset
                    level.Offset                //offset to move all new objects by
                );

                this.objectManager.Add(actorList);
                heightOffset += level.LayerHeightOffset;
            }

            CurrentLevel = level;
        }

        public void Dispose()
        {
            levels.Clear();
        }
        #endregion
    }
}
