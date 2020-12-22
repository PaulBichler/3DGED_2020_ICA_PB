using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Allow the developer to pass in offsets for a 3D curve so that platforms can use the same curve but operate "out of sync" by the offsets specified.
    /// </summary>
    public class Transform3DCurveOffsets : ICloneable
    {
        #region Statics

        public static Transform3DCurveOffsets Zero = new Transform3DCurveOffsets(Vector3.Zero, Vector3.One, 0, 0);

        #endregion Statics

        #region Fields

        private Vector3 position, scale;
        private float rotation;
        private int timeInMS;

        #endregion Fields

        #region Properties

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Vector3 Scale
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

        public float TimeInMs
        {
            get
            {
                return timeInMS;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public Transform3DCurveOffsets(Vector3 position, Vector3 scale, float rotation,
            int timeInMS)
        {
            this.position = position;
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
    /// Represents a 3D point on a camera curve (i.e. position, look, and up) at a specified time in seconds
    /// </summary>
    /// <see cref="GDLibrary.Controllers.Curve3DController"/>
    public class Transform3DCurve
    {
        #region Fields

        private Curve3D translationCurve, lookCurve, upCurve;

        #endregion Fields

        #region Constructors & Core

        public Transform3DCurve(CurveLoopType curveLoopType)
        {
            translationCurve = new Curve3D(curveLoopType);
            lookCurve = new Curve3D(curveLoopType);
            upCurve = new Curve3D(curveLoopType);
        }

        public void Add(Vector3 translation, Vector3 look, Vector3 up, int timeInMS)
        {
            translationCurve.Add(translation, timeInMS);
            lookCurve.Add(look, timeInMS);
            upCurve.Add(up, timeInMS);
        }

        public void Clear()
        {
            translationCurve.Clear();
            lookCurve.Clear();
            upCurve.Clear();
        }

        //See https://msdn.microsoft.com/en-us/library/t3c3bfhx.aspx for information on using the out keyword
        public void Evalulate(double timeInMS, int precision,
            out Vector3 translation, out Vector3 look, out Vector3 up)
        {
            translation = translationCurve.Evaluate(timeInMS, precision);
            look = lookCurve.Evaluate(timeInMS, precision);
            up = upCurve.Evaluate(timeInMS, precision);
        }

        #endregion Constructors & Core

        //Add Equals, Clone, ToString, GetHashCode...
    }
}