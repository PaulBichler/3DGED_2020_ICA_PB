using GDLibrary.Actors;
using GDLibrary.Containers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using GDGame;
using GDGame.MyGame.Controllers;
using GDGame.MyGame.Managers;
using GDLibrary.Enums;

namespace GDLibrary.Utilities
{
    /// <summary>
    /// Use the level loader to instanciate 3D drawn actors within your level from a PNG file.
    ///
    /// Usage:
    ///    LevelLoader levelLoader = new LevelLoader(this.objectArchetypeDictionary,
    ///    this.textureDictionary);
    ///     List<DrawnActor3D> actorList = levelLoader.Load(this.textureDictionary[fileName],
    ///           scaleX, scaleZ, height, offset);
    ///     this.object3DManager.Add(actorList);
    ///
    /// </summary>
    public class LevelLoader<T> where T : DrawnActor3D
    {
        private static readonly Color ColorLevelLoaderIgnore = Color.White;

        private Dictionary<string, T> archetypeDictionary;
        private ContentDictionary<Texture2D> textureDictionary;

        public LevelLoader(Dictionary<string, T> archetypeDictionary,
            ContentDictionary<Texture2D> textureDictionary)
        {
            this.archetypeDictionary = archetypeDictionary;
            this.textureDictionary = textureDictionary;
        }

        public List<DrawnActor3D> Load(Texture2D texture,
            float scaleX, float scaleZ, float height, Vector3 offset)
        {
            List<DrawnActor3D> list = new List<DrawnActor3D>();
            Color[] colorData = new Color[texture.Height * texture.Width];
            texture.GetData<Color>(colorData);

            Color color;
            Vector3 translation;
            DrawnActor3D actor;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    color = colorData[x + y * texture.Width];

                    if (!color.Equals(ColorLevelLoaderIgnore))
                    {
                        //scale allows us to increase the separation between objects in the XZ plane
                        translation = new Vector3(x * scaleX, height, y * scaleZ);

                        //the offset allows us to shift the whole set of objects in X, Y, and Z
                        translation += offset;

                        actor = GetObjectFromColor(color, translation, texture, x);

                        if (actor != null)
                        {
                            list.Add(actor);
                        }
                    }
                } //end for x
            } //end for y
            return list;
        }

        private Random rand = new Random();

        private int count = 1;

        private DrawnActor3D GetObjectFromColor(Color color, Vector3 translation, Texture2D texture, int x)
        {
            //if the pixel is red then draw a tall (stretched collidable unlit cube)
            if (color.Equals(new Color(255, 0, 0)))
            {
                #region Player
                CollidablePlayerObject archetype = archetypeDictionary[GameConstants.Player] as CollidablePlayerObject;
                CollidablePlayerObject drawnActor3D = archetype.Clone() as CollidablePlayerObject;

                //change it a bit
                drawnActor3D.ID = "Player " + count++;
                //drawnActor3D.EffectParameters.DiffuseColor = Color.Blue;
                //drawnActor3D.EffectParameters.Alpha = 0.5f;
                drawnActor3D.Transform3D.Translation = translation;
                drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                return drawnActor3D; 
                #endregion
            }
            
            if (color.Equals(new Color(0, 0, 255)))
            {
                #region Water Tile
                //Water Tile
                CollidablePrimitiveObject archetype = archetypeDictionary[GameConstants.Water] as CollidablePrimitiveObject;
                CollidablePrimitiveObject drawnActor3D = archetype.Clone() as CollidablePrimitiveObject;

                drawnActor3D.ID = "Water " + count++;
                drawnActor3D.Transform3D.Translation = translation;
                return drawnActor3D; 
                #endregion
            }
            
            if (color.Equals(new Color(0, 255, 0)))
            {
                #region Grass Tile
                //Grass Tile
                CollidablePrimitiveObject archetype = archetypeDictionary[GameConstants.Grass] as CollidablePrimitiveObject;
                CollidablePrimitiveObject drawnActor3D = archetype.Clone() as CollidablePrimitiveObject;

                drawnActor3D.ID = "Grass " + count++;
                drawnActor3D.Transform3D.Translation = translation;
                drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
                return drawnActor3D; 
                #endregion
            }

            if (color.Equals(new Color(0, 0, 0)))
            {
                #region Road Tile
                //Road Tile
                CollidablePrimitiveObject archetype = archetypeDictionary[GameConstants.Road] as CollidablePrimitiveObject;
                CollidablePrimitiveObject drawnActor3D = archetype.Clone() as CollidablePrimitiveObject;

                drawnActor3D.ID = "Road " + count++;
                drawnActor3D.Transform3D.Translation = translation;
                drawnActor3D.Transform3D.RotationInDegrees = Vector3.Zero;
                return drawnActor3D; 
                #endregion
            }

            if (color.Equals(new Color(255, 255, 0)))
            {
                #region Obstacle Spawner
                //Obstacle Spawner
                PrimitiveObject archetype = archetypeDictionary["Obstacle Spawner"] as PrimitiveObject;
                PrimitiveObject drawnActor3D = archetype.Clone() as PrimitiveObject;

                drawnActor3D.ID = "Obstacle Spawner " + count++;
                drawnActor3D.Transform3D.Translation = translation;
                drawnActor3D.Transform3D.RotationInDegrees = Vector3.Zero;

                Vector3 moveDir = (x < texture.Width / 2) ? Vector3.UnitX : -Vector3.UnitX;
                ObstacleSpawnController test = new ObstacleSpawnController(
                    "Obstacle Spawn Controller", ControllerType.ObstacleSpawner,
                    archetypeDictionary["Obstacle"] as CollidablePrimitiveObject,
                    moveDir, texture.Width, new Vector3(0, -0.25f, 0));

                drawnActor3D.ControllerList.Add(test);

                TimeManager.ExecuteInSeconds("test " + count, 3f, () => test.SpawnObstacle());
                return drawnActor3D; 
                #endregion
            }

            if (color.Equals(new Color(100, 100, 100)))
            {
                #region Water Platform Spawner
                //Water Platform Spawner
                PrimitiveObject archetype = archetypeDictionary["Obstacle Spawner"] as PrimitiveObject;
                PrimitiveObject drawnActor3D = archetype.Clone() as PrimitiveObject;

                drawnActor3D.ID = "Water Platform Spawner " + count++;
                drawnActor3D.Transform3D.Translation = translation;

                Vector3 moveDir = (x < texture.Width / 2) ? Vector3.UnitX : -Vector3.UnitX;
                ObstacleSpawnController spawnController = new ObstacleSpawnController(
                    "Water Platform Controller", ControllerType.WaterPlatformSpawner,
                    archetypeDictionary["Water Platform"] as CollidablePrimitiveObject,
                    moveDir, texture.Width, new Vector3(0, -1f, 0));

                drawnActor3D.ControllerList.Add(spawnController);

                TimeManager.ExecuteInSeconds("test " + count, 3f, () => spawnController.SpawnObstacle());
                return drawnActor3D; 
                #endregion
            }

            //add an else if for each type of object that you want to load...

            return null;
        }
    }
}