using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Implements a pan controller which rotates the attached parent about a user-defined axis based on a sine wave generated from user-defined trigonometric parameters.
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    /// <seealso cref="GDLibrary.Parameters.TrigonometricParameters"/>
    public class PanController : Controller
    {
        #region Fields

        private Vector3 rotationAxis;
        private TrigonometricParameters trigonometricParameters;

        #endregion Fields

        #region Constructors & Core

        public PanController(string id, ControllerType controllerType,
            Vector3 rotationAxis, TrigonometricParameters trigonometricParameters) : base(id, controllerType)
        {
            this.rotationAxis = rotationAxis;
            this.trigonometricParameters = trigonometricParameters;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;
            if (parent != null)
            {
                //get the total elapsed time and mod with 360 since a sine wave will only need input from 0-359 degrees
                float time = (float)gameTime.TotalGameTime.TotalSeconds % 360;

                // y = A * Sin(wT + phaseAngle)
                float rotAngle = trigonometricParameters.MaxAmplitude * (float)Math.Sin(
                    MathHelper.ToRadians(trigonometricParameters.AngularSpeed * time + trigonometricParameters.PhaseAngleInDegrees));

                //apply rotation to the parent
                parent.Transform3D.RotateBy(rotationAxis * rotAngle);
            }

            //always check if its necessary to call the base method i.e. does the base method have any code?
            //base.Update(gameTime, actor);
        }

        public new object Clone()
        {
            return new PanController(ID, ControllerType, rotationAxis, trigonometricParameters.Clone() as TrigonometricParameters);
        }

        //to do...add Equals, GetHashCode

        #endregion Constructors & Core
    }
}