using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// Provides methods to determine the state of gamepad buttons and sticks.
    /// </summary>
    public class GamePadManager : GameComponent
    {
        #region Fields
        //xna uses special PlayerIndex variable to refer to controller number not simple 1-4
        private static readonly PlayerIndex[] playerIndices = { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
        //similar to keyboard and mouse except we can have as many as 4 states (i.e. 4 connected controllers)
        protected GamePadState[] newState, oldState;
        //how many players
        private int numberOfConnectedPlayers;
        #endregion Fields

        #region Properties
        public int NumberOfConnectedPlayers
        {
            get
            {
                return numberOfConnectedPlayers;
            }
            set
            {
                //max number of 4 connected players with an XBox controller
                numberOfConnectedPlayers = (value > 0 && value <= 4) ? value : 1;
                //a new and old state for each of the 1-4 controllers
                newState = new GamePadState[numberOfConnectedPlayers];
                oldState = new GamePadState[numberOfConnectedPlayers];
            }
        }
        #endregion Properties

        public GamePadManager(Game game, int numberOfConnectedPlayers)
            : base(game)
        {
            NumberOfConnectedPlayers = numberOfConnectedPlayers;
        }

        public override void Update(GameTime gameTime)
        {
            //store the old states
            for (int i = 0; i < numberOfConnectedPlayers; i++)
            {
                oldState[i] = newState[i];
            }

            //update the new states
            for (int i = 0; i < numberOfConnectedPlayers; i++)
            {
                newState[i] = GamePad.GetState(playerIndices[i]);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Checks if a player index for a controller with a valid playerInde (i.e. 1-4) is connected
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>True if connected, otherwise false</returns>
        public bool IsPlayerConnected(PlayerIndex playerIndex)
        {
            return newState[(int)playerIndex].IsConnected;
        }

        /// <summary>
        /// Check if a specific button pressed on the gamepad for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <param name="button">Button to be checked</param>
        /// <returns>True if pressed, otherwise false</returns>
        public bool IsButtonPressed(PlayerIndex playerIndex, Buttons button)
        {
            return IsPlayerConnected(playerIndex) ?
                newState[(int)playerIndex].IsButtonDown(button) : false;
        }

        /// <summary>
        /// Check if a specific button pressed now that was not pressed in the last update for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <param name="button">Button to be checked</param>
        /// <returns>True if pressed for first time, otherwise false</returns>
        public bool IsFirstButtonPress(PlayerIndex playerIndex, Buttons button)
        {
            return IsPlayerConnected(playerIndex) ?
            newState[(int)playerIndex].IsButtonDown(button) && oldState[(int)playerIndex].IsButtonUp(button) : false;
        }

        /// <summary>
        /// Check if the gamepad state changed since the last update for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>True if changed, otherwise false</returns>
        public bool IsStateChanged(PlayerIndex playerIndex)
        {
            return IsPlayerConnected(playerIndex) ?
                !(newState[(int)playerIndex].Equals(oldState[(int)playerIndex])) : false;
        }

        /// <summary>
        /// Returns the position of the thumbsticks for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>GamePadThumbSticks object containing position of L & R thumbsticks<returns>
        public GamePadThumbSticks GetThumbSticks(PlayerIndex playerIndex)
        {
            return IsPlayerConnected(playerIndex) ?
               newState[(int)playerIndex].ThumbSticks : default;
        }

        /// <summary>
        /// Returns the state of the triggers (i.e. front of controller) for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>GamePadTriggers object containing trigger states</returns>
        public GamePadTriggers GetTriggers(PlayerIndex playerIndex)
        {
            return IsPlayerConnected(playerIndex) ?
                newState[(int)playerIndex].Triggers : default;
        }

        /// <summary>
        /// Returns the state of the DPad (i.e. front of controller) for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>GamePadDPad object containing DPad states</returns>
        public GamePadDPad GetDPad(PlayerIndex playerIndex)
        {
            return IsPlayerConnected(playerIndex) ?
                newState[(int)playerIndex].DPad : default;
        }

        /// <summary>
        /// Returns the state of the buttons for a specific connected player
        /// </summary>
        /// <param name="playerIndex">Integer representing number of connected gamepad (e.g. 1- 4)</param>
        /// <returns>GamePadButtons object containing states</returns>
        public GamePadButtons GetButtons(PlayerIndex playerIndex)
        {
            return IsPlayerConnected(playerIndex) ?
                newState[(int)playerIndex].Buttons : default;
        }

        //to do...add vibration
    }
}