using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws a texture to the screen. Useful for creating health/ammo UI icons, decals around UI elements, or menu backgrounds
    /// </summary>
    public class UITextureObject : DrawnActor2D
    {
        #region Fields
        private Texture2D texture;
        private Rectangle sourceRectangle, originalSourceRectangle;

        #endregion Fields

        #region Properties
        public Texture2D Texture { get => texture; set => texture = value; }
        public Rectangle SourceRectangle { get => sourceRectangle; set => sourceRectangle = value; }

        public int SourceRectangleWidth
        {
            get
            {
                return sourceRectangle.Width;
            }
            set
            {
                sourceRectangle.Width = value;
            }
        }

        public int SourceRectangleHeight
        {
            get
            {
                return sourceRectangle.Height;
            }
            set
            {
                sourceRectangle.Height = value;
            }
        }

        public Rectangle OriginalSourceRectangle
        {
            get
            {
                return originalSourceRectangle;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public UITextureObject(string id, ActorType actorType, StatusType statusType,
           Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
           Texture2D texture, Rectangle sourceRectangle)
           : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
            //store the original source rectangle in case we change the source rectangle (i.e. UIProgressController)
            originalSourceRectangle = SourceRectangle;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Transform2D.Translation, sourceRectangle,
                Color, Transform2D.RotationInRadians, Transform2D.Origin, Transform2D.Scale,
                SpriteEffects, LayerDepth);

            //base.Draw(gameTime);
        }

        public override bool Equals(object obj)
        {
            return obj is UITextureObject ui &&
                   base.Equals(obj) &&
                   EqualityComparer<Texture2D>.Default.Equals(texture, ui.texture) &&
                   sourceRectangle.Equals(ui.sourceRectangle);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(texture);
            hash.Add(sourceRectangle);
            return hash.ToHashCode();
        }

        public new object Clone()
        {
            return new UITextureObject("clone - " + ID, ActorType,
                StatusType, Transform2D.Clone() as Transform2D,
                Color, LayerDepth, SpriteEffects,
                texture, //shallow - reference
                sourceRectangle);
        }

        #endregion Constructors & Core
    }
}