using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Renders all the objects in the ObjectManager lists
    /// </summary>
    public class RenderManager : PausableDrawableGameComponent
    {
        private ScreenLayoutType screenLayoutType;
        private ObjectManager objectManager;
        private CameraManager<Camera3D> cameraManager;
        private RasterizerState rasterizerStateTransparent;
        private RasterizerState rasterizerStateOpaque;

        public RenderManager(Game game, StatusType statusType,
            ScreenLayoutType screenLayoutType,
            ObjectManager objectManager,
            CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.screenLayoutType = screenLayoutType;
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;

            InitializeGraphics();
        }

        /// <summary>
        /// Called to draw the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Draw(GameTime)"/>
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.screenLayoutType == ScreenLayoutType.Single)
                DrawSingle(gameTime, cameraManager.ActiveCamera);
            else
                DrawMulti(gameTime);

            // base.ApplyDraw(gameTime);
        }

        private void DrawMulti(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawSingle(gameTime, cameraManager[i]);
            }
        }

        private void DrawSingle(GameTime gameTime, Camera3D activeCamera)
        {
            this.GraphicsDevice.Viewport = activeCamera.Viewport;

            SetGraphicsStateObjects(true);
            foreach (DrawnActor3D actor in objectManager.OpaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, activeCamera);
            }

            //sort the transparent objects so that those closest to the camera are the LAST drawn
            objectManager.TransparentList.Sort((a, b) => sortComparator(a, b, activeCamera));

            SetGraphicsStateObjects(false);
            foreach (DrawnActor3D actor in objectManager.TransparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, activeCamera);
            }
        }

        private int sortComparator(Actor3D a, Actor3D b, Camera3D camera)
        {
            float distAToCamera = Vector3.Distance(a.Transform3D.Translation, camera.Transform3D.Translation);
            float distBToCamera = Vector3.Distance(b.Transform3D.Translation, camera.Transform3D.Translation);
            return distBToCamera.CompareTo(distAToCamera);
        }

        private void InitializeGraphics()
        {
            //set the graphics card to repeat the end pixel value for any UV value outside 0-1
            //See http://what-when-how.com/xna-game-studio-4-0-programmingdeveloping-for-windows-phone-7-and-xbox-360/samplerstates-xna-game-studio-4-0-programming/
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Mirror;
            samplerState.AddressV = TextureAddressMode.Mirror;
            Game.GraphicsDevice.SamplerStates[0] = samplerState;

            //opaque objects
            this.rasterizerStateOpaque = new RasterizerState();
            this.rasterizerStateOpaque.CullMode = CullMode.CullCounterClockwiseFace;

            //transparent objects
            this.rasterizerStateTransparent = new RasterizerState();
            this.rasterizerStateTransparent.CullMode = CullMode.None;
        }

        private void SetGraphicsStateObjects(bool isOpaque)
        {
            //Remember this code from our initial aliasing problems with the Sky box?
            //enable anti-aliasing along the edges of the quad i.e. to remove jagged edges to the primitive
            this.Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            if (isOpaque)
            {
                //set the appropriate state for opaque objects
                Game.GraphicsDevice.RasterizerState = this.rasterizerStateOpaque;

                //disable to see what happens when we disable depth buffering - look at the boxes
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            else
            {
                //set the appropriate state for transparent objects
                Game.GraphicsDevice.RasterizerState = this.rasterizerStateTransparent;

                //enable alpha blending for transparent objects i.e. trees
                Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                //disable to see what happens when we disable depth buffering - look at the boxes
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }
        }
    }
}