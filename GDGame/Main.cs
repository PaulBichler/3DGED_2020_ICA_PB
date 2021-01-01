#define DEMO

using GDGame.Controllers;
using GDGame.MyGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Containers;
using GDLibrary.Controllers;
using GDLibrary.Debug;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using GDLibrary.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using GDGame.MyGame.Actors;
using GDGame.MyGame.Controllers;
using GDLibrary;
using GDLibrary.Core.Managers.State;

namespace GDGame
{
    public class Main : Game
    {
        #region Fields

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private CameraManager<Camera3D> cameraManager;
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private RenderManager renderManager;
        private UIManager uiManager;
        private MyMenuManager menuManager;
        private SoundManager soundManager;
        private MyGameStateManager gameStateManager;
        private TweeningManager tweeningManager;
        private TimeManager timeManager;

        //used to process and deliver events received from publishers
        private EventDispatcher eventDispatcher;

        //store useful game resources (e.g. effects, models, rails and curves)
        private Dictionary<string, BasicEffect> effectDictionary;

        //use ContentDictionary to store assets (i.e. file content) that need the Content.Load() method to be called
        private ContentDictionary<Texture2D> textureDictionary;

        private ContentDictionary<SpriteFont> fontDictionary;
        private ContentDictionary<Model> modelDictionary;

        //use normal Dictionary to store objects that do NOT need the Content.Load() method to be called (i.e. the object is not based on an asset file)
        private Dictionary<string, Transform3DCurve> transform3DCurveDictionary;

        //stores the rails used by the camera
        private Dictionary<string, RailParameters> railDictionary;

        //stores the archetypal primitive objects (used in Main and LevelLoader)
        private Dictionary<string, PrimitiveObject> archetypeDictionary;

        //defines centre point for the mouse i.e. (w/2, h/2)
        private Vector2 screenCentre;

        #endregion Fields

        #region Constructors

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion Constructors

        #region Debug
#if DEBUG

        private void InitDebug()
        {
            InitDebugInfo(true);
        }

        private void InitDebugInfo(bool bEnable)
        {
            if (bEnable)
            {
                //create the debug drawer to draw debug info
                DebugDrawer debugInfoDrawer = new DebugDrawer(this, _spriteBatch,
                    Content.Load<SpriteFont>("Assets/Fonts/debug"),
                    cameraManager, objectManager);

                //set the debug drawer to be drawn AFTER the object manager to the screen
                debugInfoDrawer.DrawOrder = 2;

                //add the debug drawer to the component list so that it will have its Update/Draw methods called each cycle.
                Components.Add(debugInfoDrawer);
            }
        }

#endif
        #endregion Debug

        #region Load - Assets

        private void LoadSounds()
        {
            soundManager.Add(new GDLibrary.Managers.Cue("smokealarm",
                Content.Load<SoundEffect>("Assets/Audio/Effects/smokealarm1"), SoundCategoryType.Alarm, new Vector3(1, 0, 0), false));

            //to do..add more sounds
        }

        private void LoadEffects()
        {
            //to do...
            BasicEffect effect = null;

            //used for unlit primitives with a texture (e.g. textured quad of skybox)
            effect = new BasicEffect(_graphics.GraphicsDevice);
            effect.VertexColorEnabled = true; //otherwise we wont see RGB
            effect.TextureEnabled = true;
            effectDictionary.Add(GameConstants.Effect_UnlitTextured, effect);

            //used for wireframe primitives with no lighting and no texture (e.g. origin helper)
            effect = new BasicEffect(_graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effectDictionary.Add(GameConstants.Effect_UnlitWireframe, effect);

            //to do...add a new effect to draw a lit textured surface (e.g. a lit pyramid)
            effect = new BasicEffect(_graphics.GraphicsDevice);
            effect.TextureEnabled = true;
            effect.LightingEnabled = true; //redundant?
            effect.PreferPerPixelLighting = true; //cost GPU cycles
            effect.EnableDefaultLighting();
            //change lighting position, direction and color

            effectDictionary.Add("lit textured", effect);
        }

        private void LoadTextures()
        {
            //level 1 where each image 1_1, 1_2 is a different Y-axis height specificied when we use the level loader
            textureDictionary.Load("Assets/Textures/Level/level1_1");
            textureDictionary.Load("Assets/Textures/Level/level1_2");
            textureDictionary.Load("Assets/Textures/Level/level_1");
            textureDictionary.Load("Assets/Textures/Level/level_1_2");
            //add more levels here...

            //sky
            textureDictionary.Load("Assets/Textures/Skybox/back");
            textureDictionary.Load("Assets/Textures/Skybox/left");
            textureDictionary.Load("Assets/Textures/Skybox/right");
            textureDictionary.Load("Assets/Textures/Skybox/front");
            textureDictionary.Load("Assets/Textures/Skybox/sky");
            textureDictionary.Load("Assets/Textures/Foliage/Ground/grass1");

            //demo
            textureDictionary.Load("Assets/Demo/Textures/checkerboard");

            //ui
            textureDictionary.Load("Assets/Textures/UI/Controls/progress_white");

            //props
            textureDictionary.Load("Assets/Textures/Props/Crates/crate1");

            //menu
            textureDictionary.Load("Assets/Textures/UI/Controls/genericbtn");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/mainmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/audiomenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/controlsmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/exitmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/exitmenuwithtrans");

            //ui
            textureDictionary.Load("Assets/Textures/UI/Controls/reticuleDefault");

            //add more...
            textureDictionary.Load("Assets/Textures/GameObjects/Grass");
            textureDictionary.Load("Assets/Textures/GameObjects/Water");
            textureDictionary.Load("Assets/Textures/GameObjects/Road");
        }

        private void LoadFonts()
        {
            fontDictionary.Load("Assets/Fonts/debug");
            fontDictionary.Load("Assets/Fonts/menu");
            fontDictionary.Load("Assets/Fonts/ui");
        }

        #endregion Load - Assets

        #region Initialization - Graphics, Managers, Dictionaries, Cameras, Menu, UI

        protected override void Initialize()
        {
            float worldScale = 1;
            //set game title
            Window.Title = "My Amazing Game";

            //graphic settings - see https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            InitGraphics(1024, 768);

            //note that we moved this from LoadContent to allow InitDebug to be called in Initialize
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //create event dispatcher
            InitEventDispatcher();

            //managers
            InitManagers();

            //dictionaries
            InitDictionaries();

            //load from file or initialize assets, effects and vertices
            LoadEffects();
            LoadTextures();
            LoadFonts();
            LoadSounds();

            //ui
            InitUI();
            InitMenu();

            //add archetypes that can be cloned
            InitArchetypes();

            //drawn content (collidable and noncollidable together - its simpler)
            InitLevel(worldScale);

            //curves and rails used by cameras
            InitCurves();
            InitRails();

            //cameras - notice we moved the camera creation BELOW where we created the drawn content - see DriveController
            InitCameras3D();

            #region Debug
#if DEBUG
            //debug info
            InitDebug();
#endif
            #endregion Debug

            base.Initialize();
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            _graphics.ApplyChanges();

            //set screen centre based on resolution
            screenCentre = new Vector2(width / 2, height / 2);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            _graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            //set blending
            _graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //set screen centre for use when centering mouse
            screenCentre = new Vector2(width / 2, height / 2);
        }

        private void InitUI()
        {
            Transform2D transform2D = null;
            Texture2D texture = null;
            SpriteFont spriteFont = null;

            #region Mouse Reticule & Text
            texture = textureDictionary["reticuleDefault"];

            transform2D = new Transform2D(
                new Vector2(512, 384), //this value doesnt matter since we will recentre in UIMouseObject::Update()
                0,
                 Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(45, 46)); //read directly from the PNG file dimensions

            UIMouseObject uiMouseObject = new UIMouseObject("reticule", ActorType.UIMouse,
                StatusType.Update | StatusType.Drawn, transform2D, Color.White,
                SpriteEffects.None, fontDictionary["menu"],
                "Hello there!",
                new Vector2(0, -40),
                Color.Yellow,
                0.75f * Vector2.One,
                0,
                texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height), //how much of source image do we want to draw?
                mouseManager);

            uiManager.Add(uiMouseObject);
            #endregion Mouse Reticule & Text

            #region Progress Control Left
            texture = textureDictionary["progress_white"];

            transform2D = new Transform2D(new Vector2(512, 20),
                0,
                 Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(100, 100));

            UITextureObject uiTextureObject = new UITextureObject("progress 1", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.Yellow, 0, SpriteEffects.None,
                texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

            //uiTextureObject.ControllerList.Add(new UIRotationController("rc1", ControllerType.RotationOverTime));

            //uiTextureObject.ControllerList.Add(new UIColorLerpController("clc1", ControllerType.ColorLerpOverTime,
            //    Color.White, Color.Black));

            //uiTextureObject.ControllerList.Add(new UIMouseController("moc1", ControllerType.MouseOver,
            //    this.mouseManager));

            uiTextureObject.ControllerList.Add(new UIProgressController("pc1", ControllerType.Progress, 0, 10));

            uiManager.Add(uiTextureObject);
            #endregion Progress Control Left

            #region Text Object
            spriteFont = Content.Load<SpriteFont>("Assets/Fonts/debug");

            //calculate how big the text is in (w,h)
            string text = "Hello World!!!";
            Vector2 originalDimensions = spriteFont.MeasureString(text);

            transform2D = new Transform2D(new Vector2(512, 768 - (originalDimensions.Y * 4)),
                0,
                4 * Vector2.One,
                new Vector2(originalDimensions.X / 2, originalDimensions.Y / 2), //this is text???
                new Integer2(originalDimensions)); //accurate original dimensions

            UITextObject uiTextObject = new UITextObject("hello", ActorType.UIText,
                StatusType.Update | StatusType.Drawn, transform2D, new Color(0.1f, 0, 0, 1),
                0, SpriteEffects.None, text, spriteFont);

            uiTextObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                 mouseManager, Color.Red, Color.White));

            uiManager.Add(uiTextObject);
            #endregion Text Object
        }

        private void InitMenu()
        {
            Texture2D texture = null;
            Transform2D transform2D = null;
            DrawnActor2D uiObject = null;
            Vector2 fullScreenScaleFactor = Vector2.One;

            #region All Menu Background Images
            //background main
            texture = textureDictionary["exitmenuwithtrans"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);

            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("main_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.LightGreen, 1, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("main", uiObject);

            //background audio
            texture = textureDictionary["audiomenu"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);
            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("audio_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("audio", uiObject);

            //background controls
            texture = textureDictionary["controlsmenu"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);
            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("controls_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("controls", uiObject);

            //background exit
            texture = textureDictionary["exitmenuwithtrans"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);
            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("exit_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("exit", uiObject);

            //background lose Screen
            texture = textureDictionary["exitmenuwithtrans"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);
            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("lose_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("lose", uiObject);

            //background win Screen
            texture = textureDictionary["exitmenuwithtrans"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);
            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("lose_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("win", uiObject);
            #endregion All Menu Background Images

            #region Main Menu Buttons
            //main menu buttons
            texture = textureDictionary["genericbtn"];

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Integer2 imageDimensions = new Integer2(texture.Width, texture.Height);

            //play
            transform2D = new Transform2D(screenCentre - new Vector2(0, 50), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("play", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height),
                "Play",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));
            menuManager.Add("main", uiObject);

            //exit
            transform2D = new Transform2D(screenCentre + new Vector2(0, 50), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("exit", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
             transform2D, Color.White, 1, SpriteEffects.None, texture,
             new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height),
             "Exit",
             fontDictionary["menu"],
             new Vector2(1, 1),
             Color.Blue,
             new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                 mouseManager, Color.Red, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
              mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("main", uiObject); 
            #endregion

            //finally dont forget to SetScene to say which menu should be drawn/updated!
            menuManager.SetScene("main");



        }

        private void InitEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(this);
            Components.Add(eventDispatcher);
        }

        private void InitCurves()
        {
            //create the camera curve to be applied to the track controller
            Transform3DCurve curveA = new Transform3DCurve(CurveLoopType.Oscillate); //experiment with other CurveLoopTypes
            curveA.Add(new Vector3(0, 5, 100), -Vector3.UnitZ, Vector3.UnitY, 0); //start
            curveA.Add(new Vector3(0, 5, 80), new Vector3(1, 0, -1), Vector3.UnitY, 1000); //start position
            curveA.Add(new Vector3(0, 5, 50), -Vector3.UnitZ, Vector3.UnitY, 3000); //start position
            curveA.Add(new Vector3(0, 5, 20), new Vector3(-1, 0, -1), Vector3.UnitY, 4000); //start position
            curveA.Add(new Vector3(0, 5, 10), -Vector3.UnitZ, Vector3.UnitY, 6000); //start position

            //add to the dictionary
            transform3DCurveDictionary.Add("headshake1", curveA);
        }

        private void InitRails()
        {
            //create the track to be applied to the non-collidable track camera 1
            railDictionary.Add("rail1", new RailParameters("rail1 - parallel to z-axis", new Vector3(20, 10, 50), new Vector3(20, 10, -50)));
        }

        private void InitDictionaries()
        {
            //stores effects
            effectDictionary = new Dictionary<string, BasicEffect>();

            //stores textures, fonts & models
            modelDictionary = new ContentDictionary<Model>("models", Content);
            textureDictionary = new ContentDictionary<Texture2D>("textures", Content);
            fontDictionary = new ContentDictionary<SpriteFont>("fonts", Content);

            //curves - notice we use a basic Dictionary and not a ContentDictionary since curves and rails are NOT media content
            transform3DCurveDictionary = new Dictionary<string, Transform3DCurve>();

            //rails - store rails used by cameras
            railDictionary = new Dictionary<string, RailParameters>();

            //used to store archetypes for primitives in the game
            archetypeDictionary = new Dictionary<string, PrimitiveObject>();
        }

        private void InitManagers()
        {
            //physics and CD-CR (moved to top because MouseManager is dependent)
            //to do - replace with simplified CDCR

            //camera
            cameraManager = new CameraManager<Camera3D>(this, StatusType.Off);
            Components.Add(cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(keyboardManager);

            //mouse
            mouseManager = new MouseManager(this, true, screenCentre);
            Components.Add(mouseManager);

            //object
            objectManager = new ObjectManager(this, StatusType.Off, 6, 10);
            Components.Add(objectManager);

            //render
            renderManager = new RenderManager(this, StatusType.Drawn, ScreenLayoutType.Single,
                objectManager, cameraManager);
            Components.Add(renderManager);

            //add in-game ui
            uiManager = new UIManager(this, StatusType.Off, _spriteBatch, 10);
            uiManager.DrawOrder = 4;
            Components.Add(uiManager);

            //add menu
            menuManager = new MyMenuManager(this, StatusType.Update | StatusType.Drawn, _spriteBatch,
                mouseManager, keyboardManager);
            menuManager.DrawOrder = 5; //highest number of all drawable managers since we want it drawn on top!
            Components.Add(menuManager);

            //sound
            soundManager = new SoundManager(this, StatusType.Update);
            Components.Add(soundManager);

            //Game State
            gameStateManager = new MyGameStateManager(this, StatusType.Update, cameraManager);
            Components.Add(gameStateManager);

            //Tweening
            tweeningManager = new TweeningManager(this, StatusType.Update);
            Components.Add(tweeningManager);

            //Time Manager (provides timed callbacks)
            timeManager = new TimeManager(this, StatusType.Off);
            Components.Add(timeManager);
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;
            Viewport viewPort = new Viewport(0, 0, 1024, 768);

            #region Noncollidable Camera - Flight

            transform3D = new Transform3D(new Vector3(10, 10, 20),
                new Vector3(0, 0, -1), Vector3.UnitY);

            camera3D = new Camera3D(GameConstants.Camera_NonCollidableFlight,
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen, new Viewport(0, 0, 1024, 768));

            //attach a controller
            camera3D.ControllerList.Add(new FlightCameraController(
                GameConstants.Controllers_NonCollidableFlight, ControllerType.FlightCamera,
                keyboardManager, mouseManager, null,
                GameConstants.CameraMoveKeys,
                10 * GameConstants.moveSpeed,
                10 * GameConstants.strafeSpeed,
                GameConstants.rotateSpeed));
            cameraManager.Add(camera3D);

            #endregion Noncollidable Camera - Flight

            #region Noncollidable Camera - Curve3D

            //notice that it doesnt matter what translation, look, and up are since curve will set these
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero);

            camera3D = new Camera3D(GameConstants.Camera_NonCollidableCurveMainArena,
              ActorType.Camera3D, StatusType.Update, transform3D,
                        ProjectionParameters.StandardDeepSixteenTen, viewPort);

            camera3D.ControllerList.Add(
                new Curve3DController(GameConstants.Controllers_NonCollidableCurveMainArena,
                ControllerType.Curve,
                        transform3DCurveDictionary["headshake1"])); //use the curve dictionary to retrieve a transform3DCurve by id

            cameraManager.Add(camera3D);

            #endregion Noncollidable Camera - Curve3D

            #region Player Follow Camera
            transform3D = new Transform3D(Vector3.Zero, -Vector3.Forward, Vector3.Up);
            camera3D = new Camera3D(GameConstants.Camera_PlayerFollowCamera,
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen, viewPort);

            camera3D.ControllerList.Add(new PlayerFollowCameraController(
                GameConstants.Controllers_CameraFollowPlayer,
                ControllerType.FollowCamera,
                null, GameConstants.PlayerFollowCamera_ElevationAngle, 
                GameConstants.PlayerFollowCamera_DistanceToPlayer));

            cameraManager.Add(camera3D); 
            #endregion

            cameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        #endregion Initialization - Graphics, Managers, Dictionaries, Cameras, Menu, UI

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        /// <summary>
        /// Creates archetypes used in the game.
        ///
        /// What are the steps required to add a new primitive?
        ///    1. In the VertexFactory add a function to return Vertices[]
        ///    2. Add a new BasicEffect IFF this primitive cannot use existing effects(e.g.wireframe, unlit textured)
        ///    3. Add the new effect to effectDictionary
        ///    4. Create archetypal PrimitiveObject.
        ///    5. Add archetypal object to archetypeDictionary
        ///    6. Clone archetype, change its properties (transform, texture, color, alpha, ID) and add manually to the objectmanager or you can use LevelLoader.
        /// </summary>
        private void InitArchetypes() //formerly InitTexturedQuad
        {
            Transform3D transform3D = null;
            EffectParameters effectParameters = null;
            IVertexData vertexData = null;
            PrimitiveType primitiveType;
            int primitiveCount;

            #region Lit Textured Pyramid

            /*********** Transform, Vertices and VertexData ***********/
            //lit pyramid
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                 Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);

            VertexPositionNormalTexture[] vertices
                = VertexFactory.GetVerticesPositionNormalTexturedPyramid(out primitiveType,
                out primitiveCount);

            //analog of the Model class in G-CA (i.e. it holdes vertices and type, count)
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices,
                primitiveType, primitiveCount);

            /*********** PrimitiveObject ***********/
            //now we use the "FBX" file (our vertexdata) and make a PrimitiveObject
            PrimitiveObject primitiveObject = new PrimitiveObject(
                GameConstants.Primitive_LitTexturedPyramid,
                ActorType.Decorator, //we could specify any time e.g. Pickup
                StatusType.Drawn,
                transform3D, effectParameters,
                vertexData);

            /*********** Controllers (optional) ***********/
            //we could add controllers to the archetype and then all clones would have cloned controllers
            //  drawnActor3D.ControllerList.Add(
            //new RotationController("rot controller1", ControllerType.RotationOverTime,
            //1, new Vector3(0, 1, 0)));

            //to do...add demos of controllers on archetypes
            //ensure that the Clone() method of PrimitiveObject will Clone() all controllers

            archetypeDictionary.Add(primitiveObject.ID, primitiveObject);
            #endregion Lit Textured Pyramid

            #region Unlit Textured Quad
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                  Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            effectParameters = new EffectParameters(
                effectDictionary[GameConstants.Effect_UnlitTextured],
                textureDictionary["grass1"], Color.White, 1);

            vertexData = new VertexData<VertexPositionColorTexture>(
                VertexFactory.GetTextureQuadVertices(out primitiveType, out primitiveCount),
                primitiveType, primitiveCount);

            archetypeDictionary.Add(GameConstants.Primitive_UnlitTexturedQuad,
                new PrimitiveObject(GameConstants.Primitive_UnlitTexturedQuad,
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform3D, effectParameters, vertexData));
            #endregion Unlit Textured Quad

            #region Unlit Origin Helper
            transform3D = new Transform3D(new Vector3(0, 20, 0),
                     Vector3.Zero, new Vector3(10, 10, 10),
                     Vector3.UnitZ, Vector3.UnitY);

            effectParameters = new EffectParameters(
                effectDictionary[GameConstants.Effect_UnlitWireframe],
                null, Color.White, 1);

            vertexData = new VertexData<VertexPositionColor>(VertexFactory.GetVerticesPositionColorOriginHelper(
                                    out primitiveType, out primitiveCount),
                                    primitiveType, primitiveCount);

            archetypeDictionary.Add(GameConstants.Primitive_WireframeOriginHelper,
                new PrimitiveObject(GameConstants.Primitive_WireframeOriginHelper,
                ActorType.Helper,
                StatusType.Update | StatusType.Drawn,
                transform3D, effectParameters, vertexData));

            #endregion Unlit Origin Helper

            //add more archetypes here...

            #region Lit Textured Octahedron
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);

            vertices = VertexFactory.GetVerticesPositionNormalTexturedOctahedron(out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);

            primitiveObject = new PrimitiveObject(
                GameConstants.Primitive_LitTexturedOctahedron,
                ActorType.Decorator,
                StatusType.Drawn,
                transform3D, effectParameters,
                vertexData);

            archetypeDictionary.Add(primitiveObject.ID, primitiveObject);
            #endregion

            #region Lit Textured Spiked Cube
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);

            vertices = VertexFactory.GetVerticesPositionNormalTexturedSpikedCube(out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            primitiveObject = new PrimitiveObject(
                GameConstants.Primitive_LitTexturedTest,
                ActorType.Decorator, //we could specify any time e.g. Pickup
                StatusType.Drawn,
                transform3D, effectParameters,
                vertexData);

            archetypeDictionary.Add(primitiveObject.ID, primitiveObject);
            #endregion


            /*---------------Collidables-------------*/

            CollidablePrimitiveObject collidable;

            #region Player
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    new Vector3(0.8f, 0.8f, 0.8f), Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            CollidablePlayerObject player = new CollidablePlayerObject(
                GameConstants.Player, ActorType.PC,
                StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData, 
                new BoxCollisionPrimitive(transform3D, Vector3.One / 2), objectManager,
                new[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right }, keyboardManager);
            archetypeDictionary.Add(player.ID, player);
            #endregion

            #region Grass Tile
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["Grass"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                GameConstants.Grass,
                ActorType.GrassTile,
                StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters,
                vertexData, new BoxCollisionPrimitive(transform3D), objectManager);
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region Road Tile
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["Road"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                GameConstants.Road,
                ActorType.Decorator,
                StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters,
                vertexData, new BoxCollisionPrimitive(transform3D), objectManager);
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region Water Tile
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    new Vector3(1f, 0.5f, 1f), Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["Water"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                GameConstants.Water,
                ActorType.WaterTile,
                StatusType.Drawn,
                transform3D, effectParameters,
                vertexData, new BoxCollisionPrimitive(transform3D), objectManager);
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region ObstacleSpawner
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            MovingObstacleSpawner obstacleSpawner = new MovingObstacleSpawner(
                "Obstacle Spawner", ActorType.MovingObstacleSpawner,
                StatusType.Drawn | StatusType.Update, transform3D, 
                effectParameters, vertexData, new BoxCollisionPrimitive(transform3D), objectManager);

            archetypeDictionary.Add(obstacleSpawner.ID, obstacleSpawner);
            #endregion

            #region Obstacle
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One / 2, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                "Obstacle",
                ActorType.Obstacle, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager
            );
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region Water Platform
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    new Vector3(.9f, .9f, .9f), Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                "Water Platform",
                ActorType.WaterPlatform, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager
            );
            archetypeDictionary.Add(collidable.ID, collidable); 
            #endregion
        }

        private void InitLevel(float worldScale)//, List<string> levelNames)
        {
            //remove any old content (e.g. on restart or next level)
            objectManager.Clear();

            /************ Non-collidable ************/
            //adds origin helper etc
            InitHelpers();

            //add skybox
            InitSkybox(worldScale);

            //add grass plane
            InitGround(worldScale);

            //pyramids
            InitDecorators();

            /************ Collidable ************/

            Transform3D transform3D = new Transform3D(new Vector3(0, 5, 0), Vector3.UnitZ, Vector3.UnitY);
            BoxCollisionPrimitive boxPrim = new BoxCollisionPrimitive(transform3D);

            //CollidablePrimitiveObject collPrimObj = new CollidablePrimitiveObject("id",
            //    ActorType.CollidableDecorator, StatusType.Drawn, transform3D,
            //    effectParameters, vertexData, boxPrim, this.objectManager);

            /************ Level-loader (can be collidable or non-collidable) ************/

            LevelLoader<PrimitiveObject> levelLoader = new LevelLoader<PrimitiveObject>(
                this.archetypeDictionary, this.textureDictionary);
            List<DrawnActor3D> actorList = null;

            //add level1_1 contents
            actorList = levelLoader.Load(
                this.textureDictionary["level_1"],
                                1,     //number of in-world x-units represented by 1 pixel in image
                                1,     //number of in-world z-units represented by 1 pixel in image
                                1,     //y-axis height offset
                                new Vector3(0, 0, 0) //offset to move all new objects by
                                );
            this.objectManager.Add(actorList);

            //clear the list otherwise when we add level1_2 we would re-add level1_1 objects to object manager
            actorList.Clear();

            //add level1_2 contents
            actorList = levelLoader.Load(
             this.textureDictionary["level_1_2"],
                             1,     //number of in-world x-units represented by 1 pixel in image
                             1,     //number of in-world z-units represented by 1 pixel in image
                             2,     //y-axis height offset
                             new Vector3(0, 0, 0) //offset to move all new objects by
                             );
            this.objectManager.Add(actorList);
        }

        /// <summary>
        /// Demos how we can clone an archetype and manually add to the object manager.
        /// </summary>
        private void InitDecorators()
        {
            //clone the archetypal pyramid
            PrimitiveObject drawnActor3D
                = archetypeDictionary[GameConstants.Primitive_LitTexturedPyramid].Clone() as PrimitiveObject;

            //change it a bit
            drawnActor3D.ID = "pyramid1";
            drawnActor3D.Transform3D.Scale = 10 * new Vector3(1, 1, 1);
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, 0, 0);
            drawnActor3D.Transform3D.Translation = new Vector3(0, 10, 0);
            drawnActor3D.EffectParameters.Alpha = 0.5f;

            //lets add a rotation controller so we can see all sides easily
            drawnActor3D.ControllerList.Add(
                new RotationController("rot controller1", ControllerType.RotationOverTime,
                1, new Vector3(0, 1, 0)));

            //drawnActor3D.ControllerList.Add(
            //   new RotationController("rot controller2", ControllerType.RotationOverTime,
            //   2, new Vector3(1, 0, 0)));

            //finally add it into the objectmanager after SIX(!) steps
            objectManager.Add(drawnActor3D);


            drawnActor3D = archetypeDictionary[GameConstants.Primitive_LitTexturedOctahedron].Clone() as PrimitiveObject;
            drawnActor3D.Transform3D.Scale = 10 * new Vector3(1, 1, 1);
            drawnActor3D.Transform3D.Translation = new Vector3(10, 8, 0);
            objectManager.Add(drawnActor3D);

            drawnActor3D = archetypeDictionary[GameConstants.Primitive_LitTexturedTest].Clone() as PrimitiveObject;
            drawnActor3D.Transform3D.Scale = 10 * new Vector3(1, 1, 1);
            drawnActor3D.Transform3D.Translation = new Vector3(40, 8, 0);
            objectManager.Add(drawnActor3D);
        }

        private void InitHelpers()
        {
            //clone the archetype
            PrimitiveObject originHelper = archetypeDictionary[GameConstants.Primitive_WireframeOriginHelper].Clone() as PrimitiveObject;
            //add to the dictionary
            objectManager.Add(originHelper);
        }

        private void InitGround(float worldScale)
        {
            PrimitiveObject drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Ground;
            drawnActor3D.EffectParameters.Texture = textureDictionary["grass1"];
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(-90, 0, 0);
            drawnActor3D.Transform3D.Scale = worldScale * Vector3.One;
            objectManager.Add(drawnActor3D);
        }

        private void InitSkybox(float worldScale)
        {
            PrimitiveObject drawnActor3D = null;

            //back
            drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Sky;

            //  primitiveObject.StatusType = StatusType.Off; //Experiment of the effect of StatusType
            drawnActor3D.ID = "sky back";
            drawnActor3D.EffectParameters.Texture = textureDictionary["back"]; ;
            drawnActor3D.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            drawnActor3D.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            objectManager.Add(drawnActor3D);

            //left
            drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Sky;
            drawnActor3D.ID = "left back";
            drawnActor3D.EffectParameters.Texture = textureDictionary["left"]; ;
            drawnActor3D.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
            drawnActor3D.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            objectManager.Add(drawnActor3D);

            //right
            drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Sky;
            drawnActor3D.ID = "sky right";
            drawnActor3D.EffectParameters.Texture = textureDictionary["right"];
            drawnActor3D.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
            drawnActor3D.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            objectManager.Add(drawnActor3D);

            //top
            drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Sky;
            drawnActor3D.ID = "sky top";
            drawnActor3D.EffectParameters.Texture = textureDictionary["sky"];
            drawnActor3D.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(90, -90, 0);
            drawnActor3D.Transform3D.Translation = new Vector3(0, worldScale / 2.0f, 0);
            objectManager.Add(drawnActor3D);

            //front
            drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Sky;
            drawnActor3D.ID = "sky front";
            drawnActor3D.EffectParameters.Texture = textureDictionary["front"];
            drawnActor3D.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            drawnActor3D.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            objectManager.Add(drawnActor3D);
        }

        #endregion Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        #region Load & Unload Game Assets

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            //housekeeping - unload content
            textureDictionary.Dispose();
            modelDictionary.Dispose();
            fontDictionary.Dispose();
            modelDictionary.Dispose();
            soundManager.Dispose();

            base.UnloadContent();
        }

        #endregion Load & Unload Game Assets

        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(Keys.Escape))
            {
                Exit();
            }
            #region Demo
#if DEMO

            #region Object Manager
            if (keyboardManager.IsFirstKeyPress(Keys.R))
            {
                EventDispatcher.Publish(new EventData(
                EventCategoryType.Object,
                EventActionType.OnApplyActionToFirstMatchActor,
                (actor) => actor.StatusType = StatusType.Drawn | StatusType.Update, //Action
                (actor) => actor.ActorType == ActorType.Decorator
                && actor.ID.Equals("pyramid1"), //Predicate
                null //parameters
                ));
            }
            #endregion Object Manager

            #region Sound Demos
            if (keyboardManager.IsFirstKeyPress(Keys.F1))
            {
                // soundManager.Play2D("smokealarm");

                object[] parameters = { "smokealarm" };
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnPlay2D, parameters));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F2))
            {
                soundManager.Pause("smokealarm");

                object[] parameters = { "smokealarm" };
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnPause, parameters));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F3))
            {
                soundManager.Stop("smokealarm");

                object[] parameters = { "smokealarm" };
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnStop, parameters));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F4))
            {
                soundManager.SetMasterVolume(0);
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F5))
            {
                soundManager.SetMasterVolume(0.5f);
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F6))
            {
                AudioListener listener = new AudioListener();
                listener.Position = new Vector3(0, 5, 50);
                listener.Forward = -Vector3.UnitZ;
                listener.Up = Vector3.UnitY;

                AudioEmitter emitter = new AudioEmitter();
                emitter.DopplerScale = 1;
                emitter.Position = new Vector3(0, 5, 0);
                emitter.Forward = Vector3.UnitZ;
                emitter.Up = Vector3.UnitY;

                object[] parameters = { "smokealarm", listener, emitter };
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound,
                    EventActionType.OnPlay3D, parameters));
            }
            #endregion Sound Demos

            #region Menu & UI Demos
            if (keyboardManager.IsFirstKeyPress(Keys.F9))
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F10))
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
            }

            if (keyboardManager.IsFirstKeyPress(Keys.Up))
            {
                object[] parameters = { 1 }; //will increase the progress by 1 to its max of 10 (see InitUI)
                EventDispatcher.Publish(new EventData(EventCategoryType.UI, EventActionType.OnHealthDelta, parameters));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.Down))
            {
                object[] parameters = { -1 }; //will decrease the progress by 1 to its min of 0 (see InitUI)
                EventDispatcher.Publish(new EventData(EventCategoryType.UI, EventActionType.OnHealthDelta, parameters));
            }

            if (keyboardManager.IsFirstKeyPress(Keys.F5)) //game -> menu
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay, null));
            }
            else if (keyboardManager.IsFirstKeyPress(Keys.F6)) //menu -> game
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause, null));
            }
            #endregion Menu & UI Demos

            #region Camera
            if (keyboardManager.IsFirstKeyPress(Keys.C))
            {
                cameraManager.CycleActiveCamera();
                EventDispatcher.Publish(new EventData(EventCategoryType.Camera,
                    EventActionType.OnCameraCycle, null));
            }
            #endregion Camera

#endif
            #endregion Demo

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion Update & Draw
    }
}