using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Stores the Camera3D objects used within the game and provides methods to toggle/cycle/set active camera
    /// </summary>
    public class CameraManager<T> : PausableGameComponent where T : IActor
    {
        #region Fields

        private List<Camera3D> list;
        private int activeCameraIndex = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Indexer for the camera manager
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Camera3D</returns>
        public Camera3D this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        /// <summary>
        /// Returns the currently active camera
        /// </summary>
        public Camera3D ActiveCamera
        {
            get
            {
                return list[activeCameraIndex];
            }
        }

        /// <summary>
        /// Gets/sets the current active camera index which, in turn, gets/sets the current active camera
        /// </summary>
        public int ActiveCameraIndex
        {
            get
            {
                return activeCameraIndex;
            }
            set
            {
                //in a 3 camera world [0,1,2,3,4,5,...] become [0,1,2,0,1,2,...]
                value = value % list.Count;
                activeCameraIndex = value; //bug!!! [0, list.size()-1]
            }
        }

        #endregion Properties

        #region Constructors & Core

        public CameraManager(Game game, StatusType statusType) : base(game, statusType)
        {
            list = new List<Camera3D>();
        }

        /// <summary>
        /// Adds a 3D camera to the list
        /// </summary>
        /// <param name="camera"></param>
        public void Add(Camera3D camera)
        {
            list.Add(camera);
        }

        /// <summary>
        /// Removes the camera to match the user-defined predicate
        /// </summary>
        /// <param name="predicate">Predicate<Camera></param>
        /// <returns>True if removed, otherwise false</returns>
        public bool RemoveFirstIf(Predicate<Camera3D> predicate)
        {
            int position = list.FindIndex(predicate);

            if (position != -1)
            {
                list.RemoveAt(position);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cycles across the cameras in the list, wraps when > length
        /// </summary>
        public void CycleActiveCamera()
        {
            activeCameraIndex++;
            activeCameraIndex %= list.Count;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (Camera3D camera in list)
            {
                if ((camera.StatusType & StatusType.Update) == StatusType.Update)
                    camera.Update(gameTime);
            }

            // base.ApplyUpdate(gameTime);
        }

        #endregion Constructors & Core
    }
}