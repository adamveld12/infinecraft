using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Core.Data;
using Core.Input;
using BlockRLH.Components.TerrainGeneration;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Components.Screens
{
    public class TitleScreen : MenuScreenControlHandler
    {
        private Texture2D Background;
        private Texture2D BlockLayer3;
        private Texture2D BlockLayer2;
        private Texture2D BlockLayer1;
        private Texture2D CloudLayer3;
        private Texture2D CloudLayer2;
        private Texture2D CloudLayer1;
        private Texture2D Sun;
        private Texture2D Text;

        private int BL3X = 0;
        private int BL2X = 0;
        private int BL1X = 0;
        private int CL3X = 0;
        private int CL2X = 0;
        private int CL1X = 0;

        private const int BlockDelayMax = 2;
        private int BlockDelay = BlockDelayMax;

        private const int CloudDelayMax = 4;
        private int CloudDelay = CloudDelayMax;

        public TitleScreen(ScreenComponentManager gc)
            : base(gc)
        {
            Background = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/background");
            BlockLayer3 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/blocklayer3");
            BlockLayer2 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/blocklayer2");
            BlockLayer1 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/blocklayer1");
            CloudLayer3 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/cloudlayer3");
            CloudLayer2 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/cloudlayer2");
            CloudLayer1 = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/cloudlayer1");
            Sun = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/sun");
            Text = this.Manager.Game.Content.Load<Texture2D>(@"Main Menu/text");

            var ericLevel = new MenuItemControl(0, this);
            var jswag = new MenuItemControl(0, this);
            var jswag2 = new MenuItemControl(0, this);
            var barryLevel = new MenuItemControl(0, this);
            var exit = new MenuItemControl(0, this);

            ericLevel.AddString("Towers of Babel");
            jswag.AddString("JSwag's Perlin Noise Using Time Seed");
            jswag2.AddString("JSwag's Perlin Noise With Stacking");
            barryLevel.AddString("Tease");
            exit.AddString("Exit");

            var screenCenter = this.ScreenCenter();

            ericLevel.Location = new Point((int)screenCenter.X - ericLevel.Right / 2, 128);
            jswag.Location = new Point((int)screenCenter.X - jswag.Right / 2, 256);
            jswag2.Location = new Point((int)screenCenter.X - jswag2.Right / 2, 384);
            barryLevel.Location = new Point((int)screenCenter.X - barryLevel.Right / 2, 512);
            exit.Location = new Point((int)screenCenter.X - exit.Right / 2, 666);

            ericLevel.Index = 0;
            jswag.Index = 1;
            jswag2.Index = 2;
            barryLevel.Index = 3;
            exit.Index = 4;

            this.AddControl(ericLevel);
            this.AddControl(jswag);
            this.AddControl(jswag2);
            this.AddControl(barryLevel);
            this.AddControl(exit);

            ericLevel.OnMouseClick += new EventHandler<MouseEventArgs>(ericLevel_OnMouseClick);
            jswag.OnMouseClick += new EventHandler<MouseEventArgs>(jswag_OnMouseClick);
            jswag2.OnMouseClick += new EventHandler<MouseEventArgs>(jswag2_OnMouseClick);
            barryLevel.OnMouseClick += new EventHandler<MouseEventArgs>(barryLevel_OnMouseClick);
            exit.OnMouseClick += new EventHandler<MouseEventArgs>(exit_OnMouseClick);
        }

        public override void Draw(GameTime gt, bool coveredByOtherScreen)
        {
            this.Manager.SpriteBatch.Begin();

            this.Manager.SpriteBatch.Draw(Background, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(Sun, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(BlockLayer3, new Vector2(BL3X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(BlockLayer3, new Vector2(BL3X + BlockLayer3.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(BlockLayer2, new Vector2(BL2X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(BlockLayer2, new Vector2(BL2X + BlockLayer2.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(BlockLayer1, new Vector2(BL1X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(BlockLayer1, new Vector2(BL1X + BlockLayer1.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(CloudLayer3, new Vector2(CL3X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(CloudLayer3, new Vector2(CL3X + CloudLayer3.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(CloudLayer2, new Vector2(CL2X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(CloudLayer2, new Vector2(CL2X + CloudLayer2.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(CloudLayer1, new Vector2(CL1X, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);
            this.Manager.SpriteBatch.Draw(CloudLayer1, new Vector2(CL1X + CloudLayer1.Width / 2, 0), null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.Draw(Text, new Vector2(0, 0), new Rectangle(0, 0, 2048, 600), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 1);

            this.Manager.SpriteBatch.End();

            base.Draw(gt, coveredByOtherScreen);
        }

        public override void Update(GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            BlockDelay--;
            CloudDelay--;

            if (BlockDelay <= 0)
            {
                BL3X -= 1;
                BL2X -= 2;
                BL1X -= 3;

                BlockDelay = BlockDelayMax;
            }

            if (CloudDelay <= 0)
            {
                CL3X -= 1;
                CL2X -= 2;
                CL1X -= 3;

                CloudDelay = CloudDelayMax;
            }

            if (BL1X <= -1024)
                BL1X += 1024;
            if (BL2X <= -1024)
                BL2X += 1024;
            if (BL3X <= -1024)
                BL3X += 1024;

            if (CL1X <= -1024)
                CL1X += 1024;
            if (CL2X <= -1024)
                CL2X += 1024;
            if (CL3X <= -1024)
                CL3X += 1024;

            base.Update(gt, otherScreenHasFocus, coveredByOtherScreen);
        }

        void barryLevel_OnMouseClick(object sender, MouseEventArgs e)
        {
            var pp = new WorldGenerationParams() { BlockFactory = new BarryLevelFactory(), WaterSpeedMultiplier = 1, SpawnLocation = new Vector3(0, 10, 0) };
            LoadingScreen.Load(this.Manager, new[] { new GameplayScreen(this.Manager, pp) }, null, null);
        }

        void jswag2_OnMouseClick(object sender, MouseEventArgs e)
        {
            PerlinNoise.RebuildPermutation(Environment.TickCount);
            var pp = new WorldGenerationParams() { BlockFactory = new JSwagLevelFactoryUsingNoiseWithPlane(), WorldDimension = new Vector3(400, 400, 2) };
            LoadingScreen.Load(this.Manager, new[] { new GameplayScreen(this.Manager, pp) }, null, null);
        }

        void exit_OnMouseClick(object sender, MouseEventArgs e)
        {
            this.Manager.Game.Exit();
        }

        void jswag_OnMouseClick(object sender, MouseEventArgs e)
        {
            PerlinNoise.RebuildPermutation(Environment.TickCount);
            var pp = new WorldGenerationParams() { BlockFactory = new JSwagFactoryUsingNoise(), WorldDimension = new Vector3(400, 400, 4), WaterSpeedMultiplier = 2.33f };
            LoadingScreen.Load(this.Manager, new[] { new GameplayScreen(this.Manager, pp) }, null, null);

        }

        void ericLevel_OnMouseClick(object sender, MouseEventArgs e)
        {
            /*
             * Now we use a game state manager to control what gets updated and when, the code below shows how to begin a game
             * with the new WorldGenerationParams object and the new BlockFactory design. If you look at IFactory, you will
             * see that we have a basic interface defined, and within the GenerateWorld method you would create the blocks you
             * need to form the level the player will eventually play on. If you notice, the GenerateWorld method also returns
             * a block, this will be used as the objective for the player to get to, if this is null then we will use our standard
             * random objective block code to find a suitable objective. Just replace the EricLevelFactory code with your own custom level
             * factory object
             **/
            var pp = new WorldGenerationParams() { BlockFactory = new EricLevelFactory(), WorldDimension = new Vector3(400, 400, 1), SpawnLocation = new Vector3(0, 60, 0) };
            LoadingScreen.Load(this.Manager, new[] { new GameplayScreen(this.Manager, pp) }, null, null);
        }
    }
}
