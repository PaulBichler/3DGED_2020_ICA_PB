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

    public class LevelManager
    {
        #region Fields
        private ObjectManager objectManager;
        private Dictionary<string, LevelInfo> levels;
        #endregion

        #region Properties
        public LevelInfo currentLevel { get; private set; }
        public LevelLoader<PrimitiveObject> LevelLoader { get; set; }
        #endregion

        #region Constructor & Core
        public LevelManager(ObjectManager objectManager)
        {
            this.objectManager = objectManager;
            levels = new Dictionary<string, LevelInfo>();
        }

        public void AddLevel(string ID, LevelInfo level)
        {
            if (!levels.ContainsKey(ID))
                levels.Add(ID, level);
        }

        public void LoadLevel(string ID)
        {
            if (!levels.ContainsKey(ID))
                throw new ArgumentException("No Level with ID: " + ID + " found!");

            LoadLevel(levels[ID]);
        }

        private void LoadLevel(LevelInfo level)
        {
            objectManager.Clear();

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

            currentLevel = level;
        } 
        #endregion
    }
}
