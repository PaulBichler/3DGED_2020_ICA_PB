using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all drawn 2D actors used in the engine. This class adds color, layerDepth, spriteEffects fields.
    /// </summary>
    public class DrawnActor2D : Actor2D
    {
        #region Fields
        private Color color;
        private float layerDepth;
        private SpriteEffects spriteEffects;
        #endregion Fields

        #region Properties

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public float LayerDepth
        {
            get
            {
                return layerDepth;
            }
            set
            {
                layerDepth = (value >= 0 && value <= 1) ? value : 0;
            }
        }

        public SpriteEffects SpriteEffects
        {
            get
            {
                return spriteEffects;
            }
            set
            {
                spriteEffects = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public DrawnActor2D(string id, ActorType actorType, StatusType statusType, Transform2D transform2D,
        Color color, float layerDepth, SpriteEffects spriteEffects)
        : base(id, actorType, statusType, transform2D)
        {
            Color = color;
            LayerDepth = layerDepth;
            SpriteEffects = spriteEffects;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is DrawnActor2D d &&
                   base.Equals(obj) &&
                   color.Equals(d.color) &&
                   layerDepth == d.layerDepth &&
                   spriteEffects == d.spriteEffects &&
                   Color.Equals(d.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), color, layerDepth, spriteEffects, Color);
        }

        public new object Clone()
        {
            Actor2D clonedActor = new DrawnActor2D("clone - " + ID, ActorType, StatusType,
                 Transform2D.Clone() as Transform2D, //shallow if we write this.Transform2D but deep with Clone()
                 color,
                 layerDepth,
                 spriteEffects); //deep

            clonedActor.ControllerList.AddRange(GetControllerListClone());
            return clonedActor;
        }

        #endregion Constructors & Core
    }
}