using BlockRLH.Actors;
using BlockRLH.Components;

using Core;
using Core.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.Management;
using BlockRLH.Actors.GameActors;
using Microsoft.Xna.Framework.GamerServices;
using BlockRLH.Actors.ActorComponents;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Grid;
using System;

namespace RLH.Components
{
    /// <summary>
    /// Defines the level bounds and manages the scenegraph and collision components
    /// </summary>
    public class WorldInfo : IDisposable
    {
        public delegate void LevelOverDelegate(bool won, string message, float time);

        public const int BOX_SIZE = 10;

        /// <summary>
        /// Maximum dimensions of the world
        /// </summary>
        public readonly Vector3 MAX_DIMENSION = new Vector3(400, 400, 80);

        /// <summary>
        /// Minimum dimensions of the world
        /// </summary>readonly
        public readonly Vector3 MIN_DIMENSION = new Vector3(10, 10, 10);

        /// <summary>
        /// World dimension for our worldInfo
        /// </summary>
        public readonly Vector3 WorldDimension;

        private EngineContext context;
        private InputManager inputManager;
        private GraphicsAdapterManager gad;
        

        private GameInfo gameInfo;
        private SceneManager sceneGraph;

        private Viewport viewport;

        private Skybox sky;
        private WaterPlane water;

        public Block obj;

        public SpriteBatch sb;
        public SpriteFont sf;

        private bool gameOver = false;
        public bool debug = false;

        public event LevelOverDelegate GameOver;

        private ParticleSource particles;

        /// <summary>
        /// Initializes a new instance of worldInfo
        /// </summary>
        public WorldInfo(EngineContext context, Vector3 worldDimensions)
        {
            this.context = context;
            this.inputManager = context.Services.GetService(typeof(InputManager)) as InputManager;
            this.gad = context.Services.GetService(typeof(GraphicsAdapterManager)) as GraphicsAdapterManager;
            this.viewport = gad.GraphicsDevice.Viewport;

            this.WorldDimension = Vector3.Clamp(worldDimensions, MIN_DIMENSION, MAX_DIMENSION);
            this.sceneGraph = new SceneManager(this);
            this.gameInfo = new GameInfo();
            this.sb = new SpriteBatch(gad.GraphicsDevice);
            this.sf = context.Content.Load<SpriteFont>(@"Fonts/HUD Font");
        }

        /// <summary>
        /// Initializes the world with blocks
        /// </summary>
        public void IntializeWorld(Block objLoc)
        {
            this.gad.PendingResolutionChange += (o, args) =>
            { this.viewport = new Viewport(0, 0, args.NewResolution.X, args.NewResolution.Y); };

            if (objLoc == null)
            {
                objLoc = sceneGraph.GetActors().ElementAt(new Random().Next(sceneGraph.GetActors().Length)) as Block;
            }

            objLoc.Type = BlockType.Goal;
            obj = objLoc;
            sceneGraph.LoadContent();

            gameInfo.BeginLocalGame(this);
            //find how many blocks we'll need to cover the entire area with 1 block height starting at 0 in y
            gameInfo.GetPlayer(PlayerIndex.One).SetPosition(new Vector3(0, 60, 0));
            var camera = gameInfo.GetPlayer(PlayerIndex.One).Camera;
            
            sky = new Skybox(this, ref camera);
            sceneGraph.AddActor(sky);
            water = new WaterPlane(this);
            sceneGraph.AddActor(water);
            particles = new ParticleSource(this);
            sceneGraph.AddActor(particles);
            particles.Location = obj.Location;
        }

        /// <summary>
        /// Ticks each actor and other World components
        /// </summary>
        public void Update(GameTime gameTime)
        { 
            if (!gameOver) 
            {
                sceneGraph.Update(gameTime);
            }

            if (water.Location.Y > obj.Location.Y) 
            {
                gameOver = true;
                inputManager.ClearInputChannel(PlayerIndex.One);
                if (GameOver != null)
                    GameOver(false, "Water covered up the objective!", gameInfo.GetPlayer(PlayerIndex.One).GetTime());
            }

            particles.Update(gameTime);
        }

        /// <summary>
        /// Pretty self explanatory
        /// </summary>
        public void PlayerTouchedObjective()
        {
            gameOver = true;
            inputManager.ClearInputChannel(PlayerIndex.One);
            if (GameOver != null)
                GameOver(true, "You Won!", gameInfo.GetPlayer(PlayerIndex.One).GetTime());
        }

        /// <summary>
        /// Pretty self explanatory
        /// </summary>
        public void PlayerBelowWater()
        {
            gameOver = true;
            inputManager.ClearInputChannel(PlayerIndex.One);

            if (GameOver != null)
            {
                gameInfo.GetPlayer(PlayerIndex.One).Splash.Play();
                GameOver(false, "You drowned!", gameInfo.GetPlayer(PlayerIndex.One).GetTime());
            }
        }

        /// <summary>
        /// Draws the actors
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            sky.Draw(gameTime);
           
            sceneGraph.Draw(gameTime, gameInfo.GetPlayer(PlayerIndex.One).Camera);
            water.Draw(gameTime);
            particles.Draw(gameTime, gameInfo.GetPlayer(PlayerIndex.One).Camera);
            gameInfo.GetPlayer(PlayerIndex.One).DrawPlayerHUD();
        }

        public void SaveBlocks(Stream stream)
        {
            XDocument document = new XDocument();
            XElement root = new XElement("Blocks");
            XAttribute XDim = new XAttribute("X", WorldDimension.X);
            XAttribute YDim = new XAttribute("Y", WorldDimension.Y);
            XAttribute ZDim = new XAttribute("Z", WorldDimension.Z);
            root.Add(XDim, YDim, ZDim);
            foreach (Actor actor in this.sceneGraph.GetActors())
            {
                if (actor is Block)
                {
                    XElement actorElement = new XElement("Block");
                    XAttribute X = new XAttribute("X", actor.Location.X);
                    XAttribute Y = new XAttribute("Y", actor.Location.Y);
                    XAttribute Z = new XAttribute("Z", actor.Location.Z);
                    XAttribute type = new XAttribute("Type", ((Block)actor).Type);
                    actorElement.Add(X, Y, Z);
                    root.Add(actorElement);
                }
            }
            document.Add(root);
            document.Save(stream);
        }

        public static WorldInfo LoadBlocks(EngineContext context, Stream stream)
        {

            XDocument document = XDocument.Load(stream);
            XElement root = document.Root;
            Vector3 dim = new Vector3(float.Parse(root.Attribute("X").Value), float.Parse(root.Attribute("Y").Value), float.Parse(root.Attribute("Z").Value));
            WorldInfo info = new WorldInfo(context, dim);
            foreach (XElement blockElement in root.Elements())
            {
                var block = new Block(info);

                block.Location = new Vector3(float.Parse(blockElement.Attribute("X").Value), float.Parse(blockElement.Attribute("Y").Value), float.Parse(blockElement.Attribute("Z").Value));
                block.Type = (BlockType)Enum.Parse(typeof(BlockType), blockElement.Attribute("Type").Value);
                (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();

                info.AddActor(block);
            }
            return info;
        }

        #region ActorCRUD
        /// <summary>
        /// Adds an actor to the scene
        /// </summary>
        public void AddActor(Actor actor)
        { this.sceneGraph.AddActor(actor); }

        /// <summary>
        /// Removes an actor from the scene
        /// </summary>
        public void RemoveActor(Actor actor)
        { this.sceneGraph.RemoveActor(actor); }

        /// <summary>
        /// Gets all the actors
        /// </summary>
        public Actor[] GetActors()
        { return sceneGraph.GetActors(); }

        #endregion

        #region Properties

        public bool IsGameOver
        { get { return gameOver; } set { gameOver = value; } }

        /// <summary>
        /// Get the SceneManager for this world
        /// </summary>
        public SceneManager SceneManager
        { get { return sceneGraph; } }

        /// <summary>
        /// Get the input manager for the game
        /// </summary>
        public InputManager InputManager
        { get { return this.inputManager; } }

        /// <summary>
        /// Get the viewport 
        /// </summary>
        public Viewport Viewport
        { get { return viewport; } }

        /// <summary>
        /// Get the world's gameInfo
        /// </summary>
        public GameInfo GameInfo
        { get { return this.gameInfo; } }

        /// <summary>
        /// Get the game engine
        /// </summary>
        public EngineContext Engine
        { get { return this.context; } }

        /// <summary>
        /// Get the water plane
        /// </summary>
        public WaterPlane WaterPlane
        { get { return this.water; } }

        /// <summary>
        /// Objective block
        /// </summary>
        public Block Objective
        { get { return obj; } }

        #endregion

        public void Dispose()
        {
            this.SceneManager.Dispose();
        }
    }
}
