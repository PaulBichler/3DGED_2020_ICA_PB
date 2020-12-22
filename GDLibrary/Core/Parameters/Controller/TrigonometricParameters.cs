using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the trig specific parameters (e.g. y = A*Sin(w*T+phi) for use with a IController object
    /// </summary>
    /// <see cref="GDLibrary.Controllers.PanController"/>
    public class TrigonometricParameters : ICloneable
    {
        #region Fields

        private float maxAmplitude, angularSpeed, phaseAngleInDegrees;

        #endregion Fields

        #region Properties
        public float MaxAmplitude { get => maxAmplitude; set => maxAmplitude = value; }
        public float AngularSpeed { get => angularSpeed; set => angularSpeed = value; }
        public float PhaseAngleInDegrees { get => phaseAngleInDegrees; set => phaseAngleInDegrees = value; }
        #endregion Properties

        #region Constructors & Core

        public TrigonometricParameters(float maxAmplitude, float angularSpeed, float phaseAngleInDegrees)
        {
            MaxAmplitude = maxAmplitude;
            AngularSpeed = angularSpeed;
            PhaseAngleInDegrees = phaseAngleInDegrees;
        }

        public object Clone()
        {
            return new TrigonometricParameters(MaxAmplitude, AngularSpeed, PhaseAngleInDegrees);
        }

        #endregion Constructors & Core
    }
}