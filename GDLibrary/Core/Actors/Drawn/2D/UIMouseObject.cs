using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Actors
{
    public class UIMouseObject : UITextureObject
    {
        #region Fields
        private string text;
        private SpriteFont spriteFont;
        private Color textColor;
        private Vector2 textOffsetPosition, textScale, textOrigin, textDimensions;
        private MouseManager mouseManager;
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
                text = value;
                textDimensions = spriteFont.MeasureString(text);
                textOrigin = new Vector2(textDimensions.X / 2, textDimensions.Y / 2);
            }
        }

        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
            }
        }

        public Vector2 TextScale
        {
            get
            {
                return textScale;
            }
            set
            {
                textScale = value;
            }
        }

        public Vector2 TextOffsetPosition
        {
            get
            {
                return textOffsetPosition;
            }
            set
            {
                textOffsetPosition = value;
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

        public UIMouseObject(string id, ActorType actorType, StatusType statusType, Transform2D transform2D,
            Color color, SpriteEffects spriteEffects, SpriteFont spriteFont,
            string text, Vector2 textOffsetPosition, Color textColor, Vector2 textScale,
            float layerDepth, Texture2D texture, Rectangle sourceRectangle, MouseManager mouseManager)
            : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects, texture, sourceRectangle)
        {
            this.spriteFont = spriteFont;
            Text = text;
            TextOffsetPosition = textOffsetPosition;
            TextColor = textColor;
            TextScale = textScale;

            //used to update pointer position
            this.mouseManager = mouseManager;

            SubscribeToEvents();
        }

        #region Handle Events

        protected void SubscribeToEvents()
        {
            //opacity
            EventDispatcher.Subscribe(EventCategoryType.UIPicking, HandleEvent);
        }

        protected void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnObjectPicked)
            {
                DrawnActor3D drawnActor3D = eventData.Parameters[0] as DrawnActor3D;
                text = drawnActor3D.ID;
            }
            else if (eventData.EventActionType == EventActionType.OnNoObjectPicked)
            {
                text = "No Object Picked";
            }
        }

        #endregion Handle Events

        public override void Update(GameTime gameTime)
        {
            Transform2D.Translation = mouseManager.Position; //set screen centre?
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw mouse reticule
            spriteBatch.Draw(Texture, Transform2D.Translation,
                SourceRectangle, Color,
                MathHelper.ToRadians(Transform2D.RotationInDegrees),
                Transform2D.Origin, //bug fix for off centre rotation - uses explicitly specified origin and not this.Transform.Origin
                Transform2D.Scale, SpriteEffects, LayerDepth);

            //draw any additional text
            if (text != null)
            {
                spriteBatch.DrawString(spriteFont, text,
                    Transform2D.Translation + textOffsetPosition, textColor, 0, textOrigin, textScale, SpriteEffects.None, LayerDepth);
            }
        }
    }
}