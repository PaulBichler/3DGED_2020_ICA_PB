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
    /// Base class for all drawn 3D actors used in the engine. This class adds a EffectParameters field.
    /// </summary>
    public class DrawnActor3D : Actor3D, I3DDrawable
    {
        #region Fields

        private EffectParameters effectParameters;

        #endregion Fields

        #region Properties

        public EffectParameters EffectParameters
        {
            get
            {
                return effectParameters;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public DrawnActor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            EffectParameters effectParameters) : base(id, actorType, statusType, transform3D)
        {
            this.effectParameters = effectParameters;
        }

        public virtual void Draw(GameTime gameTime, Camera3D camera)
        {
            //does nothing - see child classes
        }

        public override bool Equals(object obj)
        {
            return obj is DrawnActor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<EffectParameters>.Default.Equals(effectParameters, d.EffectParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), effectParameters);
        }

        public new object Clone()
        {
            return new DrawnActor3D(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                effectParameters.Clone() as EffectParameters);
        }

        #endregion Constructors & Core
    }
}