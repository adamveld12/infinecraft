using System;
using System.Collections.Generic;
using BlockRLH.Components.Screens;
using Core;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Components
{
    /// <summary>
    /// A GameComponent that manages screen states
    /// </summary>
    public sealed class ScreenComponentManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields

        private List<GameScreen> screens, screensToUpdate;
        private List<SpriteFont> fonts;

        #endregion

        /// <summary>
        /// Initializes a new instance of ScreenComponentManager
        /// </summary>
        public ScreenComponentManager(Game game)
            : base(game)
        {
            if (game == null)
                throw new ArgumentNullException("game");
            this.InputManager = (game as EngineContext).Services.GetService(typeof(InputManager)) as InputManager;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            this.screens = new List<GameScreen>();
            this.screensToUpdate = new List<GameScreen>();
            this.fonts = new List<SpriteFont>();
            base.Initialize();
        }

        /// <summary>
        /// Loads unmanaged content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.GraphicsManager = (this.Game as EngineContext).GraphicsDeviceManager;
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        /// <summary>
        /// Disposes of umanaged resources
        /// </summary>
        protected override void UnloadContent()
        {
            this.SpriteBatch.Dispose();

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            foreach (GameScreen s in screens)
                screensToUpdate.Add(s);
            // Throws exception if context is initialized without a gamerservices FIX
            bool otherScreenHasFocus = !Game.IsActive;// || Guide.IsVisible;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


                if (screen.State == ScreenState.TransitionOn || screen.State == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(InputManager);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen s in screens)
            {
                if (s.State == ScreenState.Hidden)
                    continue;

                s.Draw(gameTime, false);
            }

            base.Draw(gameTime);
        }
    
        /// <summary>
        /// Removes a screen from the stack
        /// </summary>
        /// <param name="gs"></param>
        public void RemoveScreen(GameScreen gs)
        {
            screens.Remove(gs);
            screensToUpdate.Remove(gs);
        }

        /// <summary>
        /// Adds a gamescreen to the stack, this also initializes the screen
        /// </summary>
        /// <param name="gs">the gamescreen to add</param>
        /// <param name="controllingPlayer">The player who has control of this screen</param>
        public void AddScreen(GameScreen gs, PlayerIndex? controllingPlayer)
        {
            this.screens.Add(gs);
            gs.Initialize();
            gs.ControllingPlayer = controllingPlayer;
            gs.IsExiting = false;
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        /// <param name="alpha">The alpha channel of the background</param>
        /// <param name="texture">The texture to use for fading to black</param>
        public void FadeBackBufferToBlack(int alpha, Texture2D texture, Effect effect)
        {
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);

            SpriteBatch.Draw(texture,
                             new Rectangle(0, 0, (int)ScreenDimensions.X, (int)ScreenDimensions.Y),
                             new Color(0, 0, 0, (byte)alpha));

            this.SpriteBatch.End();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        /// <param name="alpha">The alpha channel of the background</param>
        /// <param name="texture">The texture to use for fading to black</param>
        public void FadeBackBufferToBlack(int alpha, Texture2D texture, Rectangle bounds, Effect effect)
        {
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);

            SpriteBatch.Draw(texture,
                             bounds,
                             new Color(0, 0, 0, (byte)alpha));

            this.SpriteBatch.End();
        }

        /// <summary>
        /// Gets a copy of all the screens
        /// </summary>
        /// <returns></returns>
        public GameScreen[] GetScreens()
        {
            return this.screens.ToArray();
        }

        /// <summary>
        /// Adds a font to the font cache
        /// </summary>
        public void AddFont(SpriteFont font)
        {
            this.fonts.Add(font);
        }

        /// <summary>
        /// Gets a font from the font cache
        /// </summary>
        public SpriteFont GetFont(int index)
        {
            return this.fonts[index];
        }

        #region Properties

        /// <summary>
        /// Gets or sets the screen dimensions for the manager
        /// </summary>
        public Point ScreenDimensions { get; set; }

        /// <summary>
        /// Gets the spritebatch used by this manager
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets the GraphicsDeviceManager for this manager
        /// </summary>
        public GraphicsDeviceManager GraphicsManager { get; private set; }

        /// <summary>
        /// Gets the current profile
        /// </summary>
        //public GlobalGameSettings Profile { get; private set; }

        #endregion


        public InputManager InputManager { get; private set; }
    }
}
