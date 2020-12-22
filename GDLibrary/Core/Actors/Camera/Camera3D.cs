using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all 3D cameras used in the engine. This class adds a ProjectionParameters field.
    /// </summary>
    public class Camera3D : Actor3D
    {
        #region Fields

        //name, active, Transform3D::Reset??
        private ProjectionParameters projectionParameters;

        private Viewport viewPort;
        private Matrix view;
        // private bool isDirty;

        #endregion Fields

        public Viewport Viewport
        {
            get
            {
                return viewPort;
            }
            protected set
            {
                viewPort = value;
            }
        }

        #region Properties

        public Matrix Projection
        {
            get
            {
                return projectionParameters.Projection;
            }
        }

        //add a clean/dirty flag later
        public Matrix View
        {
            get
            {
                //   if (this.isDirty)
                //  {
                view = Matrix.CreateLookAt(Transform3D.Translation,
                    Transform3D.Translation + Transform3D.Look,
                    Transform3D.Up);
                //     this.isDirty = true;
                //  }
                return view;
            }
        }

        #endregion Properties

        #region Constructors

        public Camera3D(string id, ActorType actorType, StatusType statusType,
            Transform3D transform3D, ProjectionParameters projectionParameters, Viewport viewPort)
            : base(id, actorType, statusType, transform3D)
        {
            this.projectionParameters = projectionParameters;
            this.viewPort = viewPort;
        }

        #endregion Constructors

        public override void Update(GameTime gameTime)
        {
            //check for keyboard input?
            //if input, then modify transform
            //  this.controller.Update(gameTime, this);

            base.Update(gameTime);
        }

        public new object Clone()
        {
            return new Camera3D(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                projectionParameters.Clone() as ProjectionParameters, viewPort);
        }

        public override bool Equals(object obj)
        {
            return obj is Camera3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<ProjectionParameters>.Default.Equals(projectionParameters, d.projectionParameters);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(projectionParameters);
            return hash.ToHashCode();
        }
    }
}