using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;

namespace GDLibrary
{
    /// <summary>
    /// Creates a parent target controller which causes the parent actor (to which the controller is attached) to follow a target e.g. ThirdPersonController or RailController
    /// </summary>
    public class TargetController : Controller
    {
        #region Fields
        private IActor targetActor;
        #endregion Fields

        #region Properties
        public IActor TargetActor
        {
            get
            {
                return targetActor;
            }
            set
            {
                targetActor = value;
            }
        }

        #endregion Properties

        #region Constructors & Core
        public TargetController(string id, ControllerType controllerType, IActor targetActor)
            : base(id, controllerType)
        {
            this.targetActor = targetActor;
        }
        #endregion Constructors & Core

        //to do...add Clone, Equals, GetHashCode
    }
}