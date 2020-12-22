using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all drawn 3D draw primtive objects used in the engine. This class adds an IVertexData field.
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject"/>
    public class PrimitiveObject : DrawnActor3D
    {
        #region Fields

        private IVertexData vertexData;

        #endregion Fields

        #region Properties

        public IVertexData IVertexData
        {
            get
            {
                return vertexData;
            }
        }

        #endregion Properties

        #region Constructors

        public PrimitiveObject(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            EffectParameters effectParameters, IVertexData vertexData)
                        : base(id, actorType, statusType, transform3D, effectParameters)
        {
            this.vertexData = vertexData;
        }

        #endregion Constructors

        public override void Draw(GameTime gameTime, Camera3D camera)
        {
            EffectParameters.Draw(Transform3D.World, camera);
            IVertexData.Draw(gameTime, EffectParameters.Effect);
        }

        public new object Clone()
        {
            PrimitiveObject clone = new PrimitiveObject(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, vertexData.Clone()
                as IVertexData);

            //if we ever want to clone prims that ALREADY have controllers
            //then we need to add cloning of controllers here
            foreach (Controller controller in this.ControllerList)
            {
                clone.ControllerList.Add(controller.Clone() as Controller);
            }

            return clone;
        }

        public override bool Equals(object obj)
        {
            return obj is PrimitiveObject @object &&
                   base.Equals(obj) &&
                   EqualityComparer<IVertexData>.Default.Equals(vertexData, @object.vertexData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), vertexData);
        }
    }
}