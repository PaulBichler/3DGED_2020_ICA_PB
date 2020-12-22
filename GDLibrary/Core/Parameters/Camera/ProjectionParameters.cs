using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the projection matrix specific parameters for the camera class
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    public class ProjectionParameters : ICloneable
    {
        #region Statics

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardDeepFourThree property gets an instance of a ProjectionParameters object (4.0f / 3, 0.1f, 2500)
        /// </value>
        public static ProjectionParameters StandardDeepFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver4, 4.0f / 3, 1, 2500);
            }
        }

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardDeepSixteenTen property gets an instance of a ProjectionParameters object (16.0f / 10, 1, 2500)
        /// </value>
        public static ProjectionParameters StandardDeepSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver4, 16.0f / 10, 0.1f, 10000);
            }
        }

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardMediumFourThree property gets an instance of a ProjectionParameters object (4.0f / 3, 1, 1000)
        /// </value>
        public static ProjectionParameters StandardMediumFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 1, 1000);
            }
        }

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardMediumSixteenTen property gets an instance of a ProjectionParameters object (16.0f / 10, 0.1f, 1000)
        /// </value>
        public static ProjectionParameters StandardMediumSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 1000);
            }
        }

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardShallowFourThree property gets an instance of a ProjectionParameters object (4.0f / 3, 0.1f, 500)
        /// </value>
        public static ProjectionParameters StandardShallowFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 500);
            }
        }

        /// <summary>
        /// Creates a deep-copy instance of a ProjectionParameters object to be used by the camera
        /// </summary>
        /// <value>
        /// StandardShallowSixteenTen property gets an instance of a ProjectionParameters object (16.0f / 10, 0.1f, 500)
        /// </value>
        public static ProjectionParameters StandardShallowSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 500);
            }
        }

        #endregion Statics

        #region Fields

        //used by perspective projections
        private float fieldOfView, aspectRatio, nearClipPlane, farClipPlane;

        //used by both
        private Matrix projection;
        private bool isDirty = true;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Represents the Field-of-View (FOV) property for a Camera3D instance
        /// </summary>
        /// <value>
        /// FOV gets/sets the value of the FOV field
        /// </value>
        public float FOV
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Represents the aspect ratio property for a Camera3D instance
        /// </summary>
        /// <value>
        /// AspectRatio gets/sets the value of the aspectRatio field
        /// </value>
        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Represents the nearClipPlane property for a Camera3D instance
        /// </summary>
        /// <value>
        /// NearClipPlane gets/sets the value of the nearClipPlane field
        /// </value>
        public float NearClipPlane
        {
            get
            {
                return nearClipPlane;
            }
            set
            {
                nearClipPlane = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Represents the farClipPlane property for a Camera3D instance
        /// </summary>
        /// <value>
        /// FarClipPlane gets/sets the value of the farClipPlane field
        /// </value>
        public float FarClipPlane
        {
            get
            {
                return farClipPlane;
            }
            set
            {
                farClipPlane = value; //validation
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Generates the Projection matrix for a Camera3D instance using a specific ProjectionParameter object
        /// </summary>
        /// <value>
        /// Returns the latest projection matrix based on any changes to the contributing fields (e.g. aspect ratio)
        /// </value>
        public Matrix Projection
        {
            get
            {
                if (this.isDirty)
                {
                    projection = Matrix.CreatePerspectiveFieldOfView(
                           fieldOfView, aspectRatio,
                           nearClipPlane, farClipPlane);

                    this.isDirty = false;
                }
                return projection;
            }
        }

        #endregion Properties

        #region Constructors & Core

        /// <summary>
        /// Constructor for the ProjectionParameters object used by the Camera3D
        /// </summary>
        /// <param name="fieldOfView">Field-of-view, normally expressed in radians</param>
        /// <param name="aspectRatio">Aspect ratio - this value normally is directly proportionate to the WxH resolution of the game...</param>
        /// <param name="nearClipPlane">Distance of the near clipping plane from the camera</param>
        /// <param name="farClipPlane">Distance of the far clipping plane from the camera</param>
        public ProjectionParameters(float fieldOfView, float aspectRatio,
            float nearClipPlane, float farClipPlane)
        {
            FOV = fieldOfView;
            AspectRatio = aspectRatio;
            NearClipPlane = nearClipPlane;
            FarClipPlane = farClipPlane;
        }

        public object Clone()
        {
            //deep-copy - can use MemberwiseClone since all fields are value types
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return obj is ProjectionParameters parameters &&
                   fieldOfView == parameters.fieldOfView &&
                   aspectRatio == parameters.aspectRatio &&
                   nearClipPlane == parameters.nearClipPlane &&
                   farClipPlane == parameters.farClipPlane;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
        }

        #endregion Constructors & Core
    }
}