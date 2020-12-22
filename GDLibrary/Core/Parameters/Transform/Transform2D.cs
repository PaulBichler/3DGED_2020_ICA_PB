using GDLibrary.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the transformation, bounding rectangle, and World matrix specific parameters for any 2D entity that can have a position
    /// </summary>
    public class Transform2D : ICloneable
    {
        #region Fields
        private Vector2 translation, scale;
        private float rotationInDegrees, rotationInRadians;
        private Vector2 origin;
        private Rectangle bounds;
        private Integer2 originalDimensions;
        private Rectangle originalBounds;
        private Matrix world;
        private bool isDirty = true;
        private Vector2 originalTranslation;
        private Vector2 originalScale;
        private float originalRotationInDegrees;
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
                isDirty = true;
            }
        }

        public float RotationInDegrees
        {
            get
            {
                return rotationInDegrees;
            }
            set
            {
                //wraps the value so only ever 0 - 359
                rotationInDegrees = value % 360;
                //automatically generate radians value to save time when we call Draw()
                rotationInRadians = MathHelper.ToRadians(rotationInDegrees);
                isDirty = true;
            }
        }

        public float RotationInRadians
        {
            get
            {
                return rotationInRadians;
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
                //do not allow scale to go to zero
                scale = (value != Vector2.Zero) ? value : Vector2.One;
                isDirty = true;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
                isDirty = true;
            }
        }

        public Matrix World
        {
            get
            {
                if (isDirty)
                {
                    //used to generate World matrix which in turn is used to move bounding box from its start position
                    world = Matrix.CreateTranslation(new Vector3(-origin, 0))
                        * Matrix.CreateScale(new Vector3(scale, 1))
                        * Matrix.CreateRotationZ(rotationInRadians)
                        * Matrix.CreateTranslation(new Vector3(translation, 0));
                }

                return world;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (isDirty)
                {
                    //calculate where the new bounding box is in screen space based on the ISRoT transformation from the World matrix
                    bounds = CollisionUtility.CalculateTransformedBoundingRectangle(originalBounds, World);
                    isDirty = false;
                }

                return bounds;
            }
        }

        public Vector2 OriginalTranslation
        {
            get
            {
                return originalTranslation;
            }
        }

        public float OriginalRotationInDegrees
        {
            get
            {
                return originalRotationInDegrees;
            }
        }

        public float OriginalRotationInRadians
        {
            get
            {
                return MathHelper.ToRadians(originalRotationInDegrees);
            }
        }

        public Vector2 OriginalScale
        {
            get
            {
                return originalScale;
            }
        }

        #endregion Properties

        /// <summary>
        /// Creates the transform2D object specific to an Actor2D
        /// </summary>
        /// <param name="translation">Drawn position of the origin of the Actor2D on screen</param>
        /// <param name="rotationInDegrees">Rotation in degrees around the Z-axis</param>
        /// <param name="scale">Scale (x,y)</param>
        /// <param name="origin">Origin (x,y) normally (0,0) but will need to be set if we want to rotate the Actor2D around its centre</param>
        /// <param name="imageDimensions">Dimensions of the original image used when drawing the actor2D</param>
        public Transform2D(Vector2 translation, float rotationInDegrees, Vector2 scale,
            Vector2 origin, Integer2 imageDimensions)
        {
            Translation = originalTranslation = translation;
            Scale = originalScale = scale;
            RotationInDegrees = originalRotationInDegrees = rotationInDegrees;
            Origin = origin;

            //original bounding box based on the texture source rectangle imageDimensions
            originalBounds = new Rectangle(0, 0, imageDimensions.X, imageDimensions.Y);
            originalDimensions = imageDimensions;
        }

        /// <summary>
        /// Use this version of the constructor to draw background textures that are scaled to fit the screen
        /// </summary>
        /// <param name="scale">Scale factor</param>
        public Transform2D(Vector2 scale) : this(Vector2.Zero, 0, scale, Vector2.Zero, Integer2.One)
        {
        }

        //used internally when creating the originalTransform object
        private Transform2D()
        {
        }

        public object Clone()
        {
            //deep because all variables are either C# types (e.g. primitives, structs, or enums) or  XNA types
            return MemberwiseClone();
        }
    }
}