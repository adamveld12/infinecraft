using System;
using Core.Input;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.Screens
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden
    }

    abstract public class GameScreen
    {

        #region Fields

        private bool otherScreenHasFocus;
        
        #endregion

        protected GameScreen(ScreenComponentManager manager)
        {
            this.Manager = manager;
            this.IsExiting = false;
            this.TransitionPosition = 1;
            this.State = ScreenState.TransitionOn;
        }

        #region Methods

        /// <summary>
        /// Initializes the gamescreen
        /// </summary>
        public virtual void Initialize()
        {
        }

        public virtual void HandleInput(InputManager input)
        {}

        public abstract void Draw(GameTime gameTime, bool covered);

        /// <summary>
        /// Updates the gamescreen
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="coveredByOtherScreen"></param>
        public virtual void Update(GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                this.State = ScreenState.TransitionOff;

                if (!UpdateTransition(gt, TransitionOff, 1))
                    // When the transition finishes, remove the screen.
                    Manager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gt, TransitionOff, 1))
                    // Still busy transitioning.
                    this.State = ScreenState.TransitionOff;
                else
                    // Transition finished!
                    this.State = ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gt, TransitionOn, -1))
                    // Still busy transitioning.
                    this.State = ScreenState.TransitionOn;
                else
                    // Transition finished!
                    this.State = ScreenState.Active;

            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        /// <param name="direction">0 for transitioning off, 1 for transitioning on</param>
        public bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (TransitionPosition <= 0)) ||
                ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOff <= TimeSpan.Zero)
                // If the screen has a zero transition time, remove it immediately.
                Manager.RemoveScreen(this);
            else
                // Otherwise flag that it should transition off and then exit.
                this.IsExiting = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the current screen's state
        /// </summary>
        public ScreenState State { get; set; }

        /// <summary>
        /// Gets or Sets whether this screen should behave like a popup
        /// </summary>
        public bool IsPopup { get; set; }

        /// <summary>
        /// Gets/Sets how long it takes to transition on screen
        /// </summary>
        public TimeSpan TransitionOn { get; set; }

        /// <summary>
        /// Gets or sets how long it takes to transition off the screen
        /// </summary>
        public TimeSpan TransitionOff { get; set; }

        /// <summary>
        /// Gets or Sets whether this component should exit and be removed
        /// </summary>
        public bool IsExiting { get; set; }

        /// <summary>
        /// Gets or Sets the component manager
        /// </summary>
        public ScreenComponentManager Manager { get; private set; }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public byte TransitionAlpha 
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlphaAsFloat
        {
            get { return (255 - TransitionPosition * 255); }
        }

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition { get; set; }

        /// <summary>
        /// If this component is active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !this.otherScreenHasFocus && (State == ScreenState.TransitionOn ||
                       State == ScreenState.Active);
            }
        }

        /// <summary>
        /// The controlling player of this screen
        /// </summary>
        public PlayerIndex? ControllingPlayer
        { get; set; }

        #endregion
    }
}
