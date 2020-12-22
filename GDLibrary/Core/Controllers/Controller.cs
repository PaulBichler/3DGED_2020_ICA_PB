using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Parent class for all controllers in the game which adds the ControllerType and id fields
    /// </summary>
    public class Controller : IController
    {
        #region Fields

        private string id;
        private ControllerType controllerType;
        //we could use this to turn on/off individual controllers (currently unused)
        private StatusType statusType = StatusType.Update;
        #endregion Fields

        #region Properties

        public string ID { get => id; set => id = value.Trim(); }
        public ControllerType ControllerType { get => controllerType; set => controllerType = value; }
        public StatusType StatusType { get => statusType; set => statusType = value; }

        public ControllerType GetControllerType()
        {
            throw new NotImplementedException();
        }

        #endregion Properties

        public Controller(string id, ControllerType controllerType)
        {
            ID = id;
            ControllerType = controllerType;
        }

        public virtual void Update(GameTime gameTime, IActor actor)
        {
            //does nothing - see child classes
        }

        public object Clone()
        {
            return new Controller(id, controllerType);
        }

        public override bool Equals(object obj)
        {
            return obj is Controller controller &&
                   id == controller.id &&
                   controllerType == controller.controllerType &&
                   statusType == controller.statusType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, controllerType, statusType);
        }
    }
}