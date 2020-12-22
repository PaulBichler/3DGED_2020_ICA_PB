using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Moves a target actor along a pre-defined curve defined by transformCurve3D
    /// </summary>
    public class Curve3DController : Controller
    {
        #region Statics
        private static int EVALUATE_PRECISION = 3;
        #endregion Statics

        #region Fields
        private Transform3DCurve transform3DCurve;
        private int elapsedTimeInMs = 0;
        #endregion Fields

        #region Constructors & Core
        public Curve3DController(string id, ControllerType controllerType,
         Transform3DCurve transform3DCurve) : base(id, controllerType)
        {
            this.transform3DCurve = transform3DCurve;
        }

        //local
        Vector3 translation, look, up;
        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                elapsedTimeInMs += gameTime.ElapsedGameTime.Milliseconds;
                ///for a discussion of "out" see https://www.c-sharpcorner.com/article/out-parameter-in-c-sharp-7/
                transform3DCurve.Evalulate(elapsedTimeInMs, EVALUATE_PRECISION, out translation, out look, out up);

                parent.Transform3D.Translation = translation;
                parent.Transform3D.Look = look;
                parent.Transform3D.Up = up;
            }

            //does nothing so comment out
            //base.Update(gameTime, actor);
        }

        //to do...add Clone, Equals, GetHashCode

        #endregion Constructors & Core
    }
}