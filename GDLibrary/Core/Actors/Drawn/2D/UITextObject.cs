using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws text to the screen with a user-defined text string and font. Useful for showing a score, elapsed time, or other game-state related info
    /// </summary>
    public class UITextObject : DrawnActor2D
    {
        #region Fields
        private string text;
        private SpriteFont spriteFont;
        #endregion Fields

        #region Properties
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = (value.Length >= 0) ? value : "Default";
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return spriteFont;
            }
            set
            {
                spriteFont = value;
            }
        }
        #endregion Properties

        #region Constructors & Core
        public UITextObject(string id, ActorType actorType, StatusType statusType,
          Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
          string text, SpriteFont spriteFont)
           : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects)
        {
            SpriteFont = spriteFont;
            Text = text;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, text, Transform2D.Translation, Color,
                Transform2D.RotationInRadians, Transform2D.Origin, Transform2D.Scale,
                SpriteEffects, LayerDepth);

            //base.Draw(gameTime, spriteBatch);
        }

        public override bool Equals(object obj)
        {
            return obj is UITextObject @uiObj &&
                   base.Equals(obj) &&
                   text == @uiObj.text &&
                   EqualityComparer<SpriteFont>.Default.Equals(spriteFont, @uiObj.spriteFont);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(text);
            hash.Add(spriteFont);
            return hash.ToHashCode();
        }

        public new object Clone()
        {
            return new UITextObject(this.ID, this.ActorType, this.StatusType,
                this.Transform2D.Clone() as Transform2D,
                this.Color, this.LayerDepth, this.SpriteEffects, this.text, this.SpriteFont);
        }

        #endregion Constructors & Core
    }
}