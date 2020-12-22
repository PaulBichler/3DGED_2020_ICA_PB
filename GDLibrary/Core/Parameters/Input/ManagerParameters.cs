using GDLibrary.Managers;

namespace GDLibrary
{
    /// <summary>
    /// Encapsulates manager parameters for those classes that need access to a large number of managers.
    /// </summary>
    public class ManagerParameters
    {
        #region Fields
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        #endregion Fields

        #region Properties
        //only getters since we would rarely want to re-define a manager during gameplay
        public MouseManager MouseManager
        {
            get
            {
                return this.mouseManager;
            }
        }
        public KeyboardManager KeyboardManager
        {
            get
            {
                return this.keyboardManager;
            }
        }
        public GamePadManager GamePadManager
        {
            get
            {
                return this.gamePadManager;
            }
        }
        #endregion Properties

        //useful for objects that need access to ALL managers
        public ManagerParameters(MouseManager mouseManager, KeyboardManager keyboardManager, GamePadManager gamePadManager)
        {
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
        }
    }
}