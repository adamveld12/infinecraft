using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RLH.Components;
using Microsoft.Xna.Framework.GamerServices;
using BlockRLH.Components;
using Core.Data;
using Core.Input;
using System.IO;
using BlockRLH.Components.Screens;
using System;
using Core.Management;

namespace BlockRLH
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BlockRLHGame : EngineContext
    {
        ScreenComponentManager manager;
        private AudioManager AM;

        public BlockRLHGame()
        {
            Content.RootDirectory = "Content";
            manager = new ScreenComponentManager(this);
            this.Components.Add(manager);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.IsFixedTimeStep = true;
            this.InactiveSleepTime = TimeSpan.FromMilliseconds(16);
            this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;

            manager.AddFont(this.Content.Load<SpriteFont>(@"Fonts\MenuFont"));

            this.AM = new AudioManager(this);

            GraphicsDeviceManager.PreferredBackBufferWidth = 1024;
            GraphicsDeviceManager.PreferredBackBufferHeight = 768;

            GraphicsDeviceManager.PreferMultiSampling = true;

            // FSAA
            GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
           GraphicsDeviceManager.ToggleFullScreen();

            GraphicsDeviceManager.ApplyChanges();

            ///*
            // * Now we use a game state manager to control what gets updated and when, the code below shows how to begin a game
            // * with the new WorldGenerationParams object and the new BlockFactory design. If you look at IFactory, you will
            // * see that we have a basic interface defined, and within the GenerateWorld method you would create the blocks you
            // * need to form the level the player will eventually play on. If you notice, the GenerateWorld method also returns
            // * a block, this will be used as the objective for the player to get to, if this is null then we will use our standard
            // * random objective block code to find a suitable objective. Just replace the EricLevelFactory code with your own custom level
            // * factory object
            // **/
            //var pp = new WorldGenerationParams(){ BlockFactory = new EricLevelFactory(), WorldDimension = new Vector3(400, 400, 1)};
            //LoadingScreen.Load(this.manager, new[] { new GameplayScreen(this.manager, pp) }, null, null);

            this.manager.AddScreen(new TitleScreen(this.manager), null);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            AM.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);

            base.Draw(gameTime);
        }
    }
}
