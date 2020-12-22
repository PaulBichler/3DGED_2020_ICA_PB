
**For a markdown cheat sheet see [Markdown Cheat Sheet](https://www.markdownguide.org/cheat-sheet/)**

## 3D Game Engine Development - [ICA_GDGame + GDLibrary](https://github.com/nmcguinness/ICA_GDGame.git)

### Further Reading
- Common screen [resolutions](https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg)
- Using the [out](https://www.c-sharpcorner.com/article/out-parameter-in-c-sharp-7/) keyword
- Using a [Predicate](https://www.tutorialsteacher.com/csharp/csharp-predicate) and [Delegate](https://www.tutorialsteacher.com/csharp/csharp-delegates)
- Using a [pragma](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-pragma) 
- Added [IEnumerable](https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/make-class-foreach-statement) support to a class with a container to support foreach()
- Iterating through a [dictionary](https://robertgreiner.com/iterating-through-a-dictionary-in-csharp/) used in SoundManager\:\:Dispose() and with simple/crude implementation in ContentDictionary\:\:Dispose()
- Creating a [sealed](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed) class in Cue used by SoundManager
- Understanding [Platonic Solids](http://www.technologyuk.net/mathematics/geometry/platonic-solids.shtml)
![sdfsdf](http://www.technologyuk.net/mathematics/geometry/images/geometry_0185.gif)

### Bugs
- [x] Debug text in top LHC not drawing correctly
- [x] Skybox will not drawn when debug text is shown
- [x] Apparent z-fighting with ground plane texture in new collidable object version
- [x] Check blend color in UITextObject
- [x] Boxes and spheres were not being properly rendered - CullMode issue in RenderManager
- [x] Exit button was hidden but we were still testing for mouse click - see MyMenuManager::HandleInput - solved by Antanas
- [?] Are transparent objects being drawn correctly i.e. sorted by camera distance?

### Code to Explain
- [x] How to integrate your existing code with the new Week 5 codebase
- [x] How to add your **content** to the new project
- [x] Re-factor your code to take account for any changes to method signatures
- [x] Where to add any **game specific** classes
- [x] Changes: IActor::Draw, I3DDrawable, Controller, ControllerType, ControllerList
- [x] Additions: MoveParameters, TrigonometricParameters, RailParameters, Curve1D, Curve2D, Curve3D, Transform3DCurve 
- [x] Using breakpoints with conditions and action to debug PanController on car
- [x] IController and the classes which implement it.
- [x] Create a simple enum using 2^N values on the enum values and demonstrate bitwise operators.
- [x] ModelObject
- [x] Input manager classes (MouseManager, KeyboardManager, GamePadManager)
- [x] DrawOrder on DrawableGameComponents in Main
- [x] VertexFactory
- [ ] Effect of SamplerState on on-screen aliasing

### Refactor for Efficiency
- [x] ProjectionParameters::Projection property by adding isDirty flag
- [x] Transform3D::World property by adding isDirty flag
- [ ] Camera3D::View property by adding isDirty flag
- [ ] Add validation to appropriate set properties in core classes

### Tasks - Week 2 
- [x] Introduces view, projection, effect, and VertexPositionColor concepts to render a wireframe triangle to the screen.
- [x] Add a VertexData class to draw VertexPositionColor vertex types.

### Tasks - Week 3
- [x] Add ProjectionParamters to encapsulate projection matrix.
- [x] Add assets to the Content.mgcb file. See [MonoGame Tutorial: Textures and SpriteBatch](https://gamefromscratch.com/monogame-tutorial-textures-and-spritebatch/)
- [x] Rename default namespace to GDLibrary
- [x] Add generic class for VertexData
- [x] Add folder system and organised existing files
- [x] Add EffectParameters
- [x] Add PrimitiveObject
- [x] Add SkyBox
- [x] Add Transform3D
- [x] Add IActor
- [x] Add Actor3D
- [x] Add Camera3D 
- [x] Add PrimitiveObject
- [x] Organise new classes into folder structure

### Tasks - Week 4
- [x] Add new enums: ActorType and StatusType
- [x] Add Actor::Description, Actor::ActorType, and Actor::StatusType
- [x] Add Clone, GetHashCode, Equals to classes in IActor hierarchy
- [x] Add ObjectManager and created lists using DrawnActor3D
- [x] Remove unnecessary GetAlpha() etc from IActor after change to ObjectManager list from IActor to DrawnActor3D
- [x] Add CameraManager and make a GameComponent
- [x] Add use of input managers (mouse, keyboard)
- [x] Re-factor IActor::Draw and ObjectManager to use CameraManager
- [x] Use StatusType in ObjectManager Update and Draw
- [x] Add subfolders to Actor folder for drawn and camera actors
- [x] Add ContentDictionary
- [x] Add IController and ControllerList to Actor
- [x] Add 1st, Flight, Pan (Security)
- [x] Add ModelObject
- [x] Add GDConstants and move "magic-number" hard-coded values

### Tasks - Reading Week 
- [x] Create separate *GDLibrary* project for core game engine code
- [x] Create *GDGame* for student game code and folder *Game* for all student code particular to their project
- [x] Rename GDLibrary/Debug to GDDebug to allow upload to GitHub as .gitignore was preventing upload
- [x] Move all enums to a single file EnumTypes and added ControllerType
- [x] Add parent class Controller for all controllers which adds fields ControllerType and ID
- [x] Add MoveParameters and TrigonometricParameters to simplify controller constructor parameters
- [x] Add ControllerList which inherits from List and adds Transform and Update methods
- [x] Add CollectionUtility which adds Transform method to apply a transform to a collection based on a predicate
- [x] Add namespaces to subdivide main namespace 
- [x] Add and initialized screenCentre in Main but not yet using this variable
- [x] Remove Draw() from IActor and created I3DDrawable which is implmented in first drawn 3D class i.e. DrawnActor3D

### Tasks - Week 5
- [x] Add Curve, Curve2D, Curve3D to support CurveController
- [x] Add 3rd party camera controller
- [x] Add rail controller
- [x] Add curve controllers

### Tasks - Week 6
- [x] Implement EventDispatcher with a dictionary
- [x] Add HashSet to EventDispatcher
- [x] Create demo project to discuss Delegate, Func, Action, and Predicate
- [x] Critique EventDispatcher and re-factor using Delegates into EventDispatcherV2
- [x] Add IEventListener and implement this interface
- [x] Add EventHandlerList to Actor

### Tasks - Week 7
- [x] Re-factor Camera and Object managers to support pausing
- [x] Finish ObjectManager::RemoveFirstIf and RemoveAll methods
- [x] Complete RenderManager and group individual actor Draw() methods in RenderManager
- [x] Add Viewport to Camera3D to enable multiple camera views
- [x] Add DEBUG and DEMO [pragmas](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-pragma) to Main
- [x] Add CollidableFirstPersonController
- [x] Add PhysicsDebugDrawer
- [x] Disable demo and debug code in Main using pragma
- [x] Added JibLibX shared library project
- [x] Add support for diffuse color and alpha to draw
- [x] Renamed Debug to Demo in MGCB to prevent gitignore blocking upload of assets

### Tasks - Week 8
- [x] Add UIManager
- [x] Add UI objects & UI controllers
- [x] Add Equals, GetHashCode and Clone to the following: Actor2D, DrawnActor2D, UITextureObject, UITextObject, UIButtonObject, and all UI controllers
- [x] Add MenuManager & MenuScene

### Tasks - Week 9
- [x] Move setting initial mouse position in screen centre to MouseManager
- [x] Move Main::InitGraphics() to start of initialize in case any dependent code needed new resolution settings e.g. setting background texture scale for menu
- [x] Ask if MenuScene is really necessary?
- [x] Ask if UIManager and MenuManager are essentially the same class?
- [x] Add dynamic calculation of textOrigin in UIButtonObject to centre text of any font format and content
- [x] Add UIScaleLerpController to Exit button
- [x] Implement Equals, GetHashCode and Clone in Actor and children
- [x] Add batch remove to PhysicsManager and ObjectManager
- [x] Add support for opaque to transparent changes in DrawnActor3D to ObjectManager
- [x] Add setting rasterizer states for transparent and opaque objects in RenderManager
- [x] Change SoundManager to PausableGameComponent to fit into SubscribeToEvents type component
- [x] Add SoundManager and support for 2D and 3D sounds
- [x] Add PickingManager with demo for pick and remove, pick and show info on left and right mouse click

### Tasks - Week 10
- [x] Add bug fix to MouseManager::GetMouseRayDirection() that was not allowing you to pick from a user-specified screenPosition
- [x] Remove unused GraphicsDevice parameter in Draw() calls for I3DDrawable, DrawnActor3D, ModelObject and PrimitiveObject


### Tasks - Week 11
- [x] Add a LIT pyramid in VertexFactory to demonstrate use of factory, archetype dictionary and to practice creating vertices in 3D space
- [ ] Add user-friendly symbolic names for strings used by cameras, controllers, effects and primitive objects in the archetype dictionaries (i.e. replace "textured quad" with GameConstants.TexturedQuad)
- [x] Add Action<IActor> and Predicate<IActor> to EventData to allow us to pass an event with an action function that will change the state of a target actor defined by the predicate
- [x] Add UIManager::ApplyActionToActor() to allow us to target and modify a specific actor in the ui list using an event
- [x] Add EventActionType.OnResetStart listener to HandleEvent in EventScheduler to allow the scheduler to restart itself by adding last event in the scheduler of type EventActionType.OnResetStart
- [x] Add checking of targetID (taken from Parameters[0]) in EventScheduler to allow us to target a specific event scheduler 
- [x] Add IEventHandler to formalize methods implemented by event handlers 
- [x] Add LevelLoader
- [x] Add BufferedVertexData class
- [x] Create ICA_GDGame based on GCA codebase but with JibLibX and ModelObject removed
- [x] Add ICollisionPrimitive and concrete child classes
- [x] Add CollidablePrimitiveObject, ColliableZoneObject, CollidablePickupObject 
- [x] Add CollidablePlayerObject

### Tasks - Outstanding
- [ ] Add support for SoundCategoryType in SoundManager
- [ ] Add CycleCamera event listener code in CameraManager
- [ ] Static variables/enum to represent symbolic name for resolution ResolutionType.SVGA
- [ ] Add gamepad thumbsticks to GamePadManager (map from (-1,1) to (0,0) -> (w,h))
- [ ] Add vibration support to GamePadManager
- [ ] Introduce in-built Monogame effects in demo project
- [ ] Add Mouse picking to MouseManager
- [ ] Remove hard-coded (512, 384) in Controller and replace with screenCentre
- [ ] Add tiling functionality (see grass plane)