
using BlockRLH.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace BlockRLH.Components.Screens
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    public class LoadingScreen : ControlHandler
    {

        #region Fields

        private bool screenStackEmpty;
        private GameScreen[] screensToLoad;
        private Texture2D background;

        #endregion

        private LoadingScreen(ScreenComponentManager manager, GameScreen[] screensToLoad, Texture2D backgrounds)
            : base(manager)
        {
            this.screensToLoad = screensToLoad;
            this.background = manager.Game.Content.Load<Texture2D>(@"Load Menu/load");
        }

        public static void Load(ScreenComponentManager manager, GameScreen[] screensToLoad, PlayerIndex? controllingPlayer, Texture2D backgrounds)
        {
            foreach (var item in manager.GetScreens())
                item.ExitScreen();

            LoadingScreen screen = new LoadingScreen(manager, screensToLoad, backgrounds);
            manager.AddScreen(screen, controllingPlayer);
        }

        #region Methods

        public override void Update(GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gt, otherScreenHasFocus, coveredByOtherScreen);

            if (screenStackEmpty)
            {
                this.Manager.RemoveScreen(this);

                foreach (var item in screensToLoad)
                {
                    if(item != null)
                        this.Manager.AddScreen(item, this.ControllingPlayer);
                }

                this.Manager.Game.ResetElapsedTime();
            }

        }

        public override void Draw(GameTime gt, bool coveredByOtherScreen)
        {
            this.Manager.SpriteBatch.Begin();

            this.Manager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);

            this.Manager.SpriteBatch.End();

            base.Draw(gt, coveredByOtherScreen);
            if (background != null)
            {
                this.Manager.SpriteBatch.Begin();
                this.Manager.SpriteBatch.Draw(this.background, new Rectangle(0, 0, this.Manager.ScreenDimensions.X, this.Manager.ScreenDimensions.Y), Color.White);
                this.Manager.SpriteBatch.End();
            }

            if ((this.State == ScreenState.Active) && (this.Manager.GetScreens().Length == 1))
                this.screenStackEmpty = true;

        }
        #endregion
    }
}
