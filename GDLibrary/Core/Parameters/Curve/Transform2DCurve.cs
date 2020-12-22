using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Allow the developer to pass in offsets for a 2D curve so that platforms can use the same curve but operate "out of sync" by the offsets specified.
    /// </summary>
    public class Transform2DCurveOffsets : ICloneable
    {
        #region Statics

        public static Transform2DCurveOffsets Zero = new Transform2DCurveOffsets(Vector2.Zero, Vector2.One, 0, 0);

        #endregion Statics

        #region Fields

        private Vector2 translation, scale;
        private float rotation;
        private int timeInMS;

        #endregion Fields

        #region Properties

        public Vector2 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
            }
        }

        public Vector2 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public int TimeInMS
        {
            get
            {
                return timeInMS;
            }
            set
            {
                timeInMS = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public Transform2DCurveOffsets(Vector2 translation, Vector2 scale, float rotation, int timeInMS)
        {
            this.translation = translation;
            this.scale = scale;
            this.rotation = rotation;
            this.timeInMS = timeInMS;
        }

        public object Clone()
        {
            return MemberwiseClone(); //simple C# or XNA types so use MemberwiseClone()
        }

        #endregion Constructors & Core
    }

    /// <summary>
    /// Represents a 2D point on a curve (i.e. position, rotation, and scale) at a specified time in seconds
    /// </summary>
    public class Transform2DCurve
    {
        #region Fields

        private Curve1D rotationCurve;
        private Curve2D translationCurve, scaleCurve;

        #endregion Fields

        #region Constructors & Core

        public Transform2DCurve(CurveLoopType curveLoopType)
        {
            translationCurve = new Curve2D(curveLoopType);
            scaleCurve = new Curve2D(curveLoopType);
            rotationCurve = new Curve1D(curveLoopType);
        }

        public void Add(Vector2 translation, Vector2 scale, float rotation,
            int timeInMS)
        {
            translationCurve.Add(translation, timeInMS);
            scaleCurve.Add(scale, timeInMS);
            rotationCurve.Add(rotation, timeInMS);
        }

        public void Clear()
        {
            translationCurve.Clear();
            scaleCurve.Clear();
            rotationCurve.Clear();
        }

        //See https://msdn.microsoft.com/en-us/library/t3c3bfhx.aspx for information on using the out keyword
        public void Evalulate(double timeInMS, int precision, out Vector2 translation, out Vector2 scale, out float rotation)
        {
            translation = translationCurve.Evaluate(timeInMS, precision);
            scale = scaleCurve.Evaluate(timeInMS, precision);
            rotation = rotationCurve.Evaluate(timeInMS, precision);
        }

        #endregion Constructors & Core

        //Add Equals, Clone, ToString, GetHashCode...
    }
}