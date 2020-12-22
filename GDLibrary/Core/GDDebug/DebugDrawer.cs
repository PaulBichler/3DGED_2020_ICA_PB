using GDLibrary.Actors;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BlendState = Microsoft.Xna.Framework.Graphics.BlendState;

namespace GDLibrary.Debug
{
    /// <summary>
    /// Draws debug information to the screen using a spriteBatch
    /// </summary>
    public class DebugDrawer : DrawableGameComponent
    {
        #region Fields

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private CameraManager<Camera3D> cameraManager;
        private ObjectManager objectManager;

        #endregion Fields

        #region Constructors & Core

        public DebugDrawer(Game game,
            SpriteBatch spriteBatch, SpriteFont spriteFont,
            CameraManager<Camera3D> cameraManager, ObjectManager objectManager) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.cameraManager = cameraManager;
            this.objectManager = objectManager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //start the sprite batch draw and set any initial modes
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend/*bug fix black background on drawn text*/, null, null, null);

            spriteBatch.DrawString(spriteFont, "Camera ID:" + cameraManager.ActiveCamera.ID, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(spriteFont, "Camera[translation]:" + cameraManager.ActiveCamera.Transform3D.Translation, new Vector2(10, 30), Color.White);

            //end the sprite batch draw
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion Constructors & Core
    }
}