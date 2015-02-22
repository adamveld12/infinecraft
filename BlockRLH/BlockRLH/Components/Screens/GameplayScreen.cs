using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Core;
using Core.Data;
using Core.Input;
using BlockRLH.Actors.GameActors;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.Screens
{
    public class GameplayScreen : ControlHandler
    {
        WorldInfo info;
        WorldGenerationParams param;

        public GameplayScreen(ScreenComponentManager gc, WorldGenerationParams @params) : base(gc)
        {
            this.param = @params;
            info = new WorldInfo(gc.Game as EngineContext, @params.WorldDimension);
            info.GameOver += new WorldInfo.LevelOverDelegate(info_GameOver);
        }

        void info_GameOver(bool won, string message, float time)
        {
            this.ExitScreen();
            this.Manager.AddScreen(new GameOverScreen(this.Manager, message, time), null);

            info.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();

            var repo = this.Manager.Game.Services.GetService(typeof(EngineConfigurationRepository)) as EngineConfigurationRepository;

            var input = this.Manager.Game.Services.GetService(typeof(InputManager)) as InputManager;

            input.LoadMappingsFromIniFiles(repo.GetConfiguration("RLHControlBindings"));

            var block = param.BlockFactory.GenerateWorld(info);

            info.IntializeWorld(block);

            if (!param.SpawnLocationSet)
            {
                var objLoc = info.GetActors().ElementAt(new Random().Next(info.GetActors().Length)) as Block;
                info.GameInfo.GetPlayer(Microsoft.Xna.Framework.PlayerIndex.One).SetPosition(objLoc.Location += new Vector3(0,60,0));
            }
            else
            {
                info.GameInfo.GetPlayer(Microsoft.Xna.Framework.PlayerIndex.One).SetPosition(param.SpawnLocation);
            }
            Console.WriteLine(info.GameInfo.GetPlayer(Microsoft.Xna.Framework.PlayerIndex.One).Location);
            

            info.WaterPlane.Y_INCREASE *= param.WaterSpeedMultiplier;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gt, otherScreenHasFocus, coveredByOtherScreen);

            info.Update(gt);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gt, bool coveredByOtherScreen)
        {
            base.Draw(gt, coveredByOtherScreen);

            info.Draw(gt);
        }
    }
}
