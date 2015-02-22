
using System;
using Core.Console;
using Core.Data;
using Core.Diagnostics;
using Core.Diagnostics.Interface;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Core.Management;

namespace Core
{
    /// <summary>
    /// Initializes the SIEngine
    /// </summary>
    public class EngineContext : Game
    {
        GraphicsDeviceManager GDM;
        private ContentManager engineContent;

        /// <summary>
        /// Initializes a new instance of EngineContext
        /// </summary>
        public EngineContext() : base()
        {
            this.Window.Title = "Infinecraft";
            IsMouseVisible = true;
            GDM = new GraphicsDeviceManager(this);
            engineContent = new ContentManager(this.Services, "Engine_Content");

            var gsc = new GamerServicesComponent(this);
            //Components.Add(gsc);
            gsc.Enabled = false;
            // add some services
            Services.AddService(typeof(EngineConfigurationRepository), new EngineConfigurationRepository(this));
            //Services.AddService(typeof(GraphicsDeviceManager), GDM);
            Services.AddService(typeof(InputManager), new InputManager(this));
            //Services.AddService(typeof(GamerServicesComponent), gsc);
            Services.AddService(typeof(GraphicsAdapterManager), new GraphicsAdapterManager(this));

#if WINDOWS
            Services.AddService(typeof(LogManager), new LogManager(this));
            Services.AddService(typeof(DiagnosticsManager), new DiagnosticsManager(this));
            Services.AddService(typeof(ConsoleEnvironment), new ConsoleEnvironment(this));
#endif
        }

        /// <summary>
        /// Initializes the engine's subcomponents
        /// </summary>
        protected override void Initialize()
        {            
            base.Initialize();
            // grab the key bindings from our EngineConfigurationRepository and load them into the InputManager
            var engineConfig = (Services.GetService(typeof(EngineConfigurationRepository)) as EngineConfigurationRepository);
            var inputManager = (Services.GetService(typeof(InputManager)) as InputManager);
            var controls = engineConfig.GetConfiguration("SIControlBindings");

            inputManager.LoadMappingsFromIniFiles(controls);

            //Bind up the engine's ConsoleEnvironment with our input manager's control system
            inputManager.ListenToInputChannel(ReceiveInputs, PlayerIndex.One);
            XnaGUIManager.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        /// <param name="delta"></param>
        void ReceiveInputs(object sender, InputFiredArgs arguments, float delta)
        {
            if (arguments.InputType == InputType.Debounced)
            {
                if (arguments.Command.Equals("Console", StringComparison.OrdinalIgnoreCase))
                {
                    var consoleEnvironment = Services.GetService(typeof(ConsoleEnvironment)) as ConsoleEnvironment;
                    var input = Services.GetService(typeof(InputManager)) as InputManager;

                    consoleEnvironment.ToggleConsoleVisibility();
                    input.bEnteringConsoleInput = consoleEnvironment.IsConsoleVisible;
                }
            }
        }

        /// <summary>
        /// Updates the main loop
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Tick the gui manager
            XnaGUIManager.Update(gameTime);
        }

        /// <summary>
        /// Draws the engine
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            XnaGUIManager.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Get the GraphicsDeviceManager
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        { get { return GDM; } }

        /// <summary>
        /// Get the Engine's content manager. Has debug assets and other things the engine needs
        /// </summary>
        public ContentManager EngineContent
        { get { return this.engineContent; } }
    }
}
