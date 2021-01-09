using System;
using System.Collections.Generic;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Managers
{
    /// <summary>
    /// Provides functionality to make timed callbacks. Timers can be paused, resumed and removed.
    /// </summary>
    public class TimeManager : PausableGameComponent
    {
        #region Nested classes
        private sealed class Timer
        {
            #region Fields
            public bool Pause;

            private float currentSeconds;
            private float seconds;
            private Action callback;
            #endregion

            #region Constructor & Update
            public Timer(Action callback, float seconds)
            {
                this.callback = callback;
                this.seconds = seconds;
                currentSeconds = 0;
            }

            /// <summary>
            /// //Update the time of the timer and invoke the callback if the timer reached 0.
            /// </summary>
            /// <param name="gameTime">Used to get time passed since last update</param>
            /// <returns>true if the timer reached 0, false otherwise</returns>
            public bool Update(GameTime gameTime)
            {
                if (Pause) return false;

                currentSeconds += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (currentSeconds >= seconds)
                {
                    callback.Invoke();
                    return true;
                }

                return false;
            }
            #endregion
        } 
        #endregion

        #region Fields
        private static Dictionary<string, Timer> _currentTimers = new Dictionary<string, Timer>();
        private static Dictionary<string, Timer> _timersToAdd = new Dictionary<string, Timer>();
        private static List<string> _timersToRemove = new List<string>();
        #endregion

        #region Constructor & Core
        public TimeManager(Game game, StatusType statusType) : base(game, statusType)
        {
        }

        public static void ExecuteInSeconds(string referenceId, float seconds, Action callback)
        {
            //create a timer with the provided callback
            if (!_currentTimers.ContainsKey(referenceId))
                _timersToAdd.Add(referenceId, new Timer(callback, seconds));
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //Batch remove
            if (_timersToRemove.Count > 0)
            {
                foreach (string id in _timersToRemove)
                    _currentTimers.Remove(id);

                _timersToRemove.Clear();
            }

            //Batch add
            if (_timersToAdd.Count > 0)
            {
                foreach (KeyValuePair<string, Timer> timer in _timersToAdd)
                    _currentTimers.Add(timer.Key, timer.Value);

                _timersToAdd.Clear();
            }

            //Update all of the timers and remove them if they're done
            foreach (KeyValuePair<string, Timer> pair in _currentTimers)
                if (pair.Value.Update(gameTime))
                    _timersToRemove.Add(pair.Key);
        }

        //Pause a timer by its reference id
        public static void PauseTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _currentTimers[referenceId].Pause = true;
        }

        //Remove a timer by its reference id
        public static void RemoveTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _timersToRemove.Add(referenceId);
        }

        //Resume a timer by it reference id
        public static void ResumeTimer(string referenceId)
        {
            if (_currentTimers.ContainsKey(referenceId))
                _currentTimers[referenceId].Pause = false;
        }

        public new void Dispose()
        {
            _currentTimers.Clear();
            _timersToAdd.Clear();
            _timersToRemove.Clear();
        }
        #endregion
    }
}
