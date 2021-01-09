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
        private LevelManager levelManager;

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

            //All SFX sounds made by Dustyroom (http://dustyroom.com/free-casual-game-sounds/)
            soundManager.Add(new GDLibrary.Managers.Cue("jump", 
                Content.Load<SoundEffect>("Assets/Audio/Effects/PlayerJump"), SoundCategoryType.Jump, Vector3.One, false));
            soundManager.Add(new GDLibrary.Managers.Cue("shoot", 
                Content.Load<SoundEffect>("Assets/Audio/Effects/Shoot"), SoundCategoryType.Shoot, Vector3.One, false));
            soundManager.Add(new GDLibrary.Managers.Cue("star", 
                Content.Load<SoundEffect>("Assets/Audio/Effects/StarPickup2"), SoundCategoryType.Pickup, Vector3.One, false));
            soundManager.Add(new GDLibrary.Managers.Cue("lose", 
                Content.Load<SoundEffect>("Assets/Audio/Effects/LoseSound"), SoundCategoryType.WinLose, Vector3.One, false));
            soundManager.Add(new GDLibrary.Managers.Cue("win", 
                Content.Load<SoundEffect>("Assets/Audio/Effects/WinSound"), SoundCategoryType.WinLose, Vector3.One, false));

            //Music by Zapsplat (https://www.zapsplat.com/music/game-music-tropical-game-fun-light-hearted-steel-drums-caribbean-percussive/)
            soundManager.Add(new GDLibrary.Managers.Cue("music1", 
                Content.Load<SoundEffect>("Assets/Audio/Music/InGameMusic"), SoundCategoryType.Soundtrack, Vector3.One, true));
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
            textureDictionary.Load("Assets/Textures/Level/level_1_1");
            textureDictionary.Load("Assets/Textures/Level/level_1_2");
            textureDictionary.Load("Assets/Textures/Level/level_2_1");
            textureDictionary.Load("Assets/Textures/Level/level_2_2");

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
            textureDictionary.Load("Assets/Textures/UI/HUD/StarsOutlineHud");
            textureDictionary.Load("Assets/Textures/UI/HUD/StarsFilledHud");

            //props
            textureDictionary.Load("Assets/Textures/Props/Crates/crate1");

            //menu
            textureDictionary.Load("Assets/Textures/UI/Controls/genericbtn");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/mainmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/audiomenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/controlsmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/exitmenu");
            textureDictionary.Load("Assets/Textures/UI/Backgrounds/exitmenuwithtrans");

            textureDictionary.Load("Assets/Textures/UI/Other/MainMenuTitle");
            textureDictionary.Load("Assets/Textures/UI/Other/PauseMenuTitle");
            textureDictionary.Load("Assets/Textures/UI/Other/LoseScreenTitle");
            textureDictionary.Load("Assets/Textures/UI/Other/WinScreenTitle");
            textureDictionary.Load("Assets/Textures/UI/Other/LevelSelectTitle");

            //ui
            textureDictionary.Load("Assets/Textures/UI/Controls/reticuleDefault");

            //add more...
            textureDictionary.Load("Assets/Textures/GameObjects/Grass");
            textureDictionary.Load("Assets/Textures/GameObjects/Water");
            textureDictionary.Load("Assets/Textures/GameObjects/Road");
            textureDictionary.Load("Assets/Textures/GameObjects/Star");
            textureDictionary.Load("Assets/Textures/GameObjects/BlockingObstacle");
            textureDictionary.Load("Assets/Textures/GameObjects/WaterPlatform");
            textureDictionary.Load("Assets/Textures/GameObjects/ObstacleSpawner");
            textureDictionary.Load("Assets/Textures/GameObjects/White");
            textureDictionary.Load("Assets/Textures/GameObjects/Player");
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

            //curves used by cameras
            InitCurves();

            //drawn content (collidable and noncollidable together - its simpler)
            InitLevel(worldScale);

            //cameras - notice we moved the camera creation BELOW where we created the drawn content - see DriveController
            InitCameras3D();

            #region Debug
#if DEBUG
            //debug info
            //InitDebug();
#endif
            #endregion Debug

            //Start the music
            EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay2D, new [] { "music1" }));

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

            #region Star Progress
            //Foreground
            texture = textureDictionary["StarsFilledHud"];
            transform2D = new Transform2D(new Vector2(texture.Width / 2, 30),
                0, Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(texture.Width, texture.Height));

            UITextureObject uiTextureObject = new UITextureObject("star progress", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0, SpriteEffects.None,
                texture, new Rectangle(0, 0, texture.Width, texture.Height));

            uiTextureObject.ControllerList.Add(new UIProgressController("pc1", ControllerType.Progress, 0, 3));
            uiManager.Add(uiTextureObject);

            //Background
            texture = textureDictionary["StarsOutlineHud"];
            transform2D = transform2D.Clone() as Transform2D;

            uiTextureObject = new UITextureObject("star progress background", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0, SpriteEffects.None,
                texture, new Rectangle(0, 0, texture.Width, texture.Height));
            uiManager.Add(uiTextureObject);
            #endregion Star Progress

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
            Integer2 imageDimensions = null;
            Transform2D transform2D = null;
            DrawnActor2D uiObject = null;
            Vector2 fullScreenScaleFactor = Vector2.One;
            Vector2 origin = Vector2.Zero;

            #region All Menu Background Images
            //background main
            texture = textureDictionary["Player"];
            fullScreenScaleFactor = new Vector2((float)_graphics.PreferredBackBufferWidth / texture.Width, (float)_graphics.PreferredBackBufferHeight / texture.Height);

            transform2D = new Transform2D(fullScreenScaleFactor);
            uiObject = new UITextureObject("main_bckgnd", ActorType.UITextureObject, StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("main", uiObject);
            menuManager.Add("pause", uiObject);
            menuManager.Add("lose", uiObject);
            menuManager.Add("win", uiObject);
            menuManager.Add("levelselect", uiObject);

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

            #endregion All Menu Background Images

            #region Main Menu Elements

            #region Buttons
            //main menu buttons
            texture = textureDictionary["genericbtn"];
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            imageDimensions = new Integer2(texture.Width, texture.Height);

            //play
            transform2D = new Transform2D(screenCentre, 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("play", ActorType.UITextureObject, StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Play",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));
            
            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.Green, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("main", uiObject); 

            //exit
            transform2D = new Transform2D(screenCentre + new Vector2(0, 100), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("exit", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
             transform2D, Color.White, 1, SpriteEffects.None, texture,
             new Rectangle(0, 0, texture.Width, texture.Height),
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

            #region Title Logo
            //Logo was generated on https://de.cooltext.com/
            texture = textureDictionary["MainMenuTitle"];
            imageDimensions = new Integer2(texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            transform2D = new Transform2D(screenCentre + new Vector2(0, -200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UITextureObject("Game Title", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn, transform2D,
                Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            uiObject.ControllerList.Add(new UIScaleLerpController("Scale Lerp Controller", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("main", uiObject);
            #endregion

            #endregion

            #region Level Select Elements

            #region Buttons
            //main menu buttons
            texture = textureDictionary["genericbtn"];
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            imageDimensions = new Integer2(texture.Width, texture.Height);

            //level 1 button
            transform2D = new Transform2D(screenCentre, 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("level1", ActorType.UITextureObject, StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Level 1",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.Green, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("levelselect", uiObject);

            //level 2
            transform2D = new Transform2D(screenCentre + new Vector2(0, 100), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("level2", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
             transform2D, Color.White, 1, SpriteEffects.None, texture,
             new Rectangle(0, 0, texture.Width, texture.Height),
             "Level 2",
             fontDictionary["menu"],
             new Vector2(1, 1),
             Color.Blue,
             new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                 mouseManager, Color.Green, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
              mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("levelselect", uiObject);

            //back to menu
            transform2D = new Transform2D(screenCentre + new Vector2(0, 300), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("tomenu", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Back To Menu",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.Red, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("levelselect", uiObject);
            #endregion

            #region Level Select Title
            //Logo was generated on https://de.cooltext.com/
            texture = textureDictionary["LevelSelectTitle"];
            imageDimensions = new Integer2(texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            transform2D = new Transform2D(screenCentre + new Vector2(0, -200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UITextureObject("Level Select Title", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn, transform2D,
                Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            uiObject.ControllerList.Add(new UIScaleLerpController("Scale Lerp Controller", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("levelselect", uiObject);
            #endregion 
            #endregion

            #region Pause Menu Elements

            #region Buttons
            //main menu buttons
            texture = textureDictionary["genericbtn"];
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            imageDimensions = new Integer2(texture.Width, texture.Height);

            //resume
            transform2D = new Transform2D(screenCentre, 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("resume", ActorType.UITextureObject, StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Resume",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.Green, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("pause", uiObject);

            //Back to menu
            transform2D = new Transform2D(screenCentre + new Vector2(0, 100), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("tomenu", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
             transform2D, Color.White, 1, SpriteEffects.None, texture,
             new Rectangle(0, 0, texture.Width, texture.Height),
             "Back To Menu",
             fontDictionary["menu"],
             new Vector2(1, 1),
             Color.Blue,
             new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                 mouseManager, Color.Red, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
              mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("pause", uiObject);

            //Exit
            transform2D = new Transform2D(screenCentre + new Vector2(0, 200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("exit", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Exit",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.DarkRed, Color.Red));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("pause", uiObject);
            #endregion 

            #region Title
            //Logo was generated on https://de.cooltext.com/
            texture = textureDictionary["PauseMenuTitle"];
            imageDimensions = new Integer2(texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            transform2D = new Transform2D(screenCentre + new Vector2(0, -200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UITextureObject("Game Title", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn, transform2D,
                Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            uiObject.ControllerList.Add(new UIScaleLerpController("Scale Lerp Controller", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("pause", uiObject);
            #endregion

            #endregion

            #region Lose/Win Screen Elements

            #region Buttons
            //main menu buttons
            texture = textureDictionary["genericbtn"];
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            imageDimensions = new Integer2(texture.Width, texture.Height);

            //restart
            transform2D = new Transform2D(screenCentre + new Vector2(0, 100), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("restart", ActorType.UITextureObject, StatusType.Drawn | StatusType.Update,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Restart",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.Green, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("lose", uiObject);
            menuManager.Add("win", uiObject);

            //Back to menu
            transform2D = new Transform2D(screenCentre + new Vector2(0, 200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("tomenu", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
             transform2D, Color.White, 1, SpriteEffects.None, texture,
             new Rectangle(0, 0, texture.Width, texture.Height),
             "Back To Menu",
             fontDictionary["menu"],
             new Vector2(1, 1),
             Color.Blue,
             new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                 mouseManager, Color.Red, Color.White));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
              mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("lose", uiObject);
            menuManager.Add("win", uiObject);

            //Exit
            transform2D = new Transform2D(screenCentre + new Vector2(0, 300), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UIButtonObject("exit", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn,
                transform2D, Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                "Exit",
                fontDictionary["menu"],
                new Vector2(1, 1),
                Color.Blue,
                new Vector2(0, 0));

            uiObject.ControllerList.Add(new UIMouseOverController("moc1", ControllerType.MouseOver,
                mouseManager, Color.DarkRed, Color.Red));

            uiObject.ControllerList.Add(new UIScaleLerpController("slc1", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("lose", uiObject);
            menuManager.Add("win", uiObject);
            #endregion 

            #region Lose Title
            //Logo was generated on https://de.cooltext.com/
            texture = textureDictionary["LoseScreenTitle"];
            imageDimensions = new Integer2(texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            transform2D = new Transform2D(screenCentre + new Vector2(0, -200), 0, Vector2.One, origin, imageDimensions);
            uiObject = new UITextureObject("Lose Title", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn, transform2D,
                Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            uiObject.ControllerList.Add(new UIScaleLerpController("Scale Lerp Controller", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("lose", uiObject);
            #endregion

            #region Win Title
            //Logo was generated on https://de.cooltext.com/
            texture = textureDictionary["WinScreenTitle"];
            imageDimensions = new Integer2(texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            transform2D = new Transform2D(screenCentre + new Vector2(0, -200), 0, Vector2.One / 2, origin, imageDimensions);
            uiObject = new UITextureObject("Win Title", ActorType.UITextureObject,
                StatusType.Update | StatusType.Drawn, transform2D,
                Color.White, 1, SpriteEffects.None, texture,
                new Rectangle(0, 0, texture.Width, texture.Height));
            uiObject.ControllerList.Add(new UIScaleLerpController("Scale Lerp Controller", ControllerType.ScaleLerpOverTime,
                mouseManager, new TrigonometricParameters(0.02f, 1, 0)));

            menuManager.Add("win", uiObject);
            #endregion

            #region Star Display
            texture = textureDictionary["StarsFilledHud"];
            transform2D = new Transform2D(screenCentre + new Vector2(0, -50),
                0, Vector2.One * 2,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(texture.Width, texture.Height));

            UITextureObject uiTextureObject = new UITextureObject("star progress end screen", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0, SpriteEffects.None,
                texture, new Rectangle(0, 0, texture.Width, texture.Height));

            uiTextureObject.ControllerList.Add(new UIProgressController("pc1", ControllerType.Progress, 0, 3));
            menuManager.Add("lose", uiTextureObject);
            menuManager.Add("win", uiTextureObject);

            //Background
            texture = textureDictionary["StarsOutlineHud"];
            transform2D = transform2D.Clone() as Transform2D;

            uiTextureObject = new UITextureObject("star progress background", ActorType.UITextureObject,
                StatusType.Drawn | StatusType.Update, transform2D, Color.White, 0, SpriteEffects.None,
                texture, new Rectangle(0, 0, texture.Width, texture.Height));
            menuManager.Add("lose", uiTextureObject);
            menuManager.Add("win", uiTextureObject); 
            #endregion

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
            Transform3DCurve curveA = new Transform3DCurve(CurveLoopType.Oscillate);
            curveA.Add(new Vector3(12.5f, 10, 70), new Vector3(0, -0.6f, -0.8f), new Vector3(0, 0.8f, -0.6f), 0);
            curveA.Add(new Vector3(-10.5f, 10, 34), new Vector3(1f, -0.2f, 0f), new Vector3(0, 1f, 0), 2000);
            curveA.Add(new Vector3(35, 10, 26), new Vector3(-1f, -0.1f, -0.3f), new Vector3(-0.1f, 1f, 0), 4000);
            curveA.Add(new Vector3(-10.5f, 10, 10), new Vector3(1f, -0.1f, -0.2f), new Vector3(0.1f, 1f, 0), 6000);
            curveA.Add(new Vector3(12.5f, 10, -9), new Vector3(0, -0.4f, 1f), new Vector3(0, 0.9f, 0.4f), 7000);
            curveA.Add(new Vector3(12.5f, 10, 70), new Vector3(0, -0.6f, -0.8f), new Vector3(0, 0.8f, -0.6f), 9000);

            //add to the dictionary
            transform3DCurveDictionary.Add("Start Camera Curve", curveA);
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
            menuManager = new MyMenuManager(this, StatusType.Update | StatusType.Drawn, _spriteBatch, mouseManager);
            menuManager.DrawOrder = 5; //highest number of all drawable managers since we want it drawn on top!
            Components.Add(menuManager);

            //sound
            soundManager = new SoundManager(this, StatusType.Update);
            Components.Add(soundManager);

            //Tweening
            tweeningManager = new TweeningManager(this, StatusType.Update);
            Components.Add(tweeningManager);

            //Time Manager (provides timed callbacks)
            timeManager = new TimeManager(this, StatusType.Update);
            Components.Add(timeManager);

            //stores all the levels and loads them when necessary
            levelManager = new LevelManager(this, objectManager);

            //Game State
            gameStateManager = new MyGameStateManager(this, StatusType.Update, cameraManager, levelManager, keyboardManager);
            Components.Add(gameStateManager);
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;
            Viewport viewPort = new Viewport(0, 0, 1024, 768);

            #region Noncollidable Camera - Curve3D

            //notice that it doesnt matter what translation, look, and up are since curve will set these
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero);

            //Curve3DController is added in the game state manager
            camera3D = new Camera3D(GameConstants.Camera_NonCollidableCurveMainArena,
              ActorType.Camera3D, StatusType.Update, transform3D,
                        ProjectionParameters.StandardDeepSixteenTen, viewPort);

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
                textureDictionary["Player"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            CollidablePlayerObject player = new CollidablePlayerObject(
                GameConstants.Player, ActorType.PC, StatusType.Drawn,
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
                textureDictionary["ObstacleSpawner"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            MovingObstacleSpawner obstacleSpawner = new MovingObstacleSpawner(
                "Obstacle Spawner", ActorType.MovingObstacleSpawner,
                StatusType.Drawn | StatusType.Update, transform3D, 
                effectParameters, vertexData, new BoxCollisionPrimitive(transform3D), objectManager);

            archetypeDictionary.Add(obstacleSpawner.ID, obstacleSpawner);
            #endregion

            #region Moving Obstacle
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One / 2, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["White"], Color.White, 1);
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
                textureDictionary["WaterPlatform"], Color.White, 1);
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

            #region Blocking Obstacle
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    new Vector3(1, 0.5f, 1), Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["BlockingObstacle"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                "Blocking Obstacle",
                ActorType.BlockingObstacle, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager
            );
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region Projectile
            transform3D = new Transform3D(Vector3.Zero, new Vector3(90, 0, 0), 
                    Vector3.One / 2, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedPyramid(out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            CollidableProjectile projectile = new CollidableProjectile(
                "Projectile",
                ActorType.Projectile, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager
            );
            archetypeDictionary.Add(projectile.ID, projectile);
            #endregion

            #region Shooter
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["checkerboard"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedOctahedron(out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            collidable = new CollidablePrimitiveObject(
                "Shooter",
                ActorType.Shooter, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager
            );
            archetypeDictionary.Add(collidable.ID, collidable);
            #endregion

            #region Star Pickup
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
                    Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            effectParameters = new EffectParameters(effectDictionary[GameConstants.Effect_LitTextured],
                textureDictionary["Star"], Color.White, 1);
            vertices = VertexFactory.GetVerticesPositionNormalTexturedSpikedCube(out primitiveType, out primitiveCount);
            vertexData = new VertexData<VertexPositionNormalTexture>(vertices, primitiveType, primitiveCount);
            CollidablePickupObject pickup = new CollidablePickupObject(
                "Star Pickup",
                ActorType.CollidablePickup, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, vertexData,
                new BoxCollisionPrimitive(transform3D), objectManager, new PickupParameters("Star Pickup", 1f)
            );
            archetypeDictionary.Add(pickup.ID, pickup); 
            #endregion
        }

        private void InitLevel(float worldScale)//, List<string> levelNames)
        {
            /************ Level-loader (can be collidable or non-collidable) ************/
            levelManager.LevelLoader = new LevelLoader<PrimitiveObject>(
                this.archetypeDictionary, this.textureDictionary);

            #region Level 1
            LevelInfo level = new LevelInfo
            {
                Name = "Level 1",
                LevelLayerTextures = new List<Texture2D> { textureDictionary["level_1_1"], textureDictionary["level_1_2"] },
                StartCameraCurve = transform3DCurveDictionary["Start Camera Curve"],
                xScale = 1,
                zScale = 1,
                LayerHeightOffset = 1,
                Offset = Vector3.Zero
            };
            levelManager.AddLevel("Level 1", level);
            #endregion

            #region Level 2
            level = new LevelInfo
            {
                Name = "Level 2",
                LevelLayerTextures = new List<Texture2D> { textureDictionary["level_2_1"], textureDictionary["level_2_2"] },
                StartCameraCurve = transform3DCurveDictionary["Start Camera Curve"],
                xScale = 1,
                zScale = 1,
                LayerHeightOffset = 1,
                Offset = Vector3.Zero
            };
            levelManager.AddLevel("Level 2", level); 
            #endregion
        }

        private void InitHelpers()
        {
            //clone the archetype
            PrimitiveObject originHelper = archetypeDictionary[GameConstants.Primitive_WireframeOriginHelper].Clone() as PrimitiveObject;
            //add to the dictionary
            objectManager.Add(originHelper);
        }

        public void InitGround(float worldScale)
        {
            PrimitiveObject drawnActor3D = archetypeDictionary[GameConstants.Primitive_UnlitTexturedQuad].Clone() as PrimitiveObject;
            drawnActor3D.ActorType = ActorType.Ground;
            drawnActor3D.EffectParameters.Texture = textureDictionary["White"];
            drawnActor3D.EffectParameters.DiffuseColor = Color.LightBlue;
            drawnActor3D.Transform3D.TranslateBy(new Vector3(0, -5, 0));
            drawnActor3D.Transform3D.RotationInDegrees = new Vector3(-90, 0, 0);
            drawnActor3D.Transform3D.Scale = worldScale * Vector3.One;
            objectManager.Add(drawnActor3D);
        }

        public void InitSkybox(float worldScale)
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

            tweeningManager.Dispose();
            timeManager.Dispose();
            levelManager.Dispose();

            base.UnloadContent();
        }

        #endregion Load & Unload Game Assets

        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            if (keyboardManager.IsFirstKeyPress(GameConstants.SoundControlKeys[2]))
                soundManager.ToggleMute();
            else if(keyboardManager.IsFirstKeyPress(GameConstants.SoundControlKeys[0]))
                soundManager.ChangeMasterVolume(.2f);
            else if(keyboardManager.IsFirstKeyPress(GameConstants.SoundControlKeys[1]))
                soundManager.ChangeMasterVolume(-.2f);

            #region Demo
#if DEMO

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