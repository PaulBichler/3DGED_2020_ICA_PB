using Microsoft.Xna.Framework;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Allows the developer to define three curves with three independent values changing over time (e.g. (x,y, z) vs time, (height, width, depth) vs time) and then evaluate the (x,y,z) quantity for any valid time on that curve
    /// </summary>
    /// <see cref="Transform3DCurve"/>
    public class Curve3D
    {
        #region Fields

        private Curve1D xCurve, yCurve, zCurve;
        private CurveLoopType curveLookType;

        #endregion Fields

        #region Properties

        public CurveLoopType CurveLookType
        {
            get
            {
                return curveLookType;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public Curve3D(CurveLoopType curveLoopType)
        {
            curveLookType = curveLoopType;

            xCurve = new Curve1D(curveLoopType);
            yCurve = new Curve1D(curveLoopType);
            zCurve = new Curve1D(curveLoopType);
        }

        public void Add(Vector3 value, int timeInMS)
        {
            xCurve.Add(value.X, timeInMS);
            yCurve.Add(value.Y, timeInMS);
            zCurve.Add(value.Z, timeInMS);
        }

        public void Clear()
        {
            xCurve.Clear();
            yCurve.Clear();
            zCurve.Clear();
        }

        public Vector3 Evaluate(double timeInMS, int decimalPrecision)
        {
            return new Vector3(xCurve.Evaluate(timeInMS, decimalPrecision),
                yCurve.Evaluate(timeInMS, decimalPrecision),
                 zCurve.Evaluate(timeInMS, decimalPrecision));
        }

        #endregion Constructors & Core

        //Add Equals, Clone, ToString, GetHashCode...
    }
}