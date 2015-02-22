using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Core.Data;
using Core.Initialization.INi;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Management
{
    /// <summary>
    /// event arguments for device resets
    /// </summary>
    public class DeviceResetArgs : EventArgs
    {
        public readonly Point NewResolution;
        public readonly Point OldResolution;

        public DeviceResetArgs (Point newRes, Point oldRes)
	    {
            this.NewResolution = newRes;
            this.OldResolution = oldRes;
	    }
    }

    /// <summary>
    /// Offers higher level functions for manipulating graphics device properties
    /// </summary>
    public class GraphicsAdapterManager : DrawableGameComponent
    {
        /// <summary>
        /// A lookup table for resolutions
        /// </summary>
        private Dictionary<string, Point> resolutionLUT;
        private List<Point> supportedDisplayModes;

        private string currentResolution = "";

        /// <summary>
        /// Fired when the resolution is about to be changed
        /// </summary>
        public event EventHandler<DeviceResetArgs> PendingResolutionChange;

        /// <summary>
        /// Initializes a new instance of GraphicsAdapterManager
        /// </summary>
        public GraphicsAdapterManager(EngineContext game) : base(game)
        {
            game.Components.Add(this);

            this.GraphicsDeviceManager = game.GraphicsDeviceManager;
            this.resolutionLUT = new Dictionary<string, Point>();
        }

        /// <summary>
        /// Initializes graphics settings
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            var graphicsIni = (Game.Services.GetService(typeof(EngineConfigurationRepository)) as EngineConfigurationRepository).GetConfiguration("SIGraphicsDefaults");

            InitializeRenderingSettings(graphicsIni["Rendering"]);
            InitializeResolutionLookUps(graphicsIni["Resolutions"]);

            this.GraphicsDeviceManager.ApplyChanges();

        }

        /// <summary>
        /// Initializes the resolution lookup table
        /// </summary>
        /// <param name="iniFileSection"></param>
        private void InitializeResolutionLookUps(IniFileSection iniFileSection)
        {
            foreach (var resolutionData in iniFileSection.elements)
            {
                var splitUp = resolutionData.Line.Split('=');
                var dim = splitUp[1].Split('x');
                var size = new Point(int.Parse(dim[0]), int.Parse(dim[1]));
                this.resolutionLUT.Add(splitUp[0], size);
            }
        }

        /// <summary>
        /// Initializes the graphics device with settings from an inifile
        /// </summary>
        /// <param name="section"></param>
        private void InitializeRenderingSettings(IniFileSection section)
        {
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = bool.Parse(section["VSync"]);

            GraphicsDeviceManager.PreferMultiSampling = true;

            var pp = GraphicsDeviceManager.GraphicsDevice.PresentationParameters;

            pp.MultiSampleCount = int.Parse(section["MultiSampleCount"]);

            Game.IsFixedTimeStep = bool.Parse(section["UseFixedTimeTicks"]);
            Game.TargetElapsedTime = TimeSpan.FromMilliseconds(double.Parse(section["TargetTickTime"]));

           
        }


        /// <summary>
        /// Get the current resolution setting
        /// </summary>
        public Point Resolution
        { get { return this.resolutionLUT[currentResolution]; } }

        /// <summary>
        /// Get or set the GraphicsDeviceManager for your game
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        { get; private set; }
    }
}
