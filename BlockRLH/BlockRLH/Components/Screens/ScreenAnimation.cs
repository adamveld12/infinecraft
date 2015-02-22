using System;

using Microsoft.Xna.Framework;


namespace BlockRLH.Components.Screens
{
    /// <summary>
    /// Animates screens and allows you to make customized animations
    /// </summary>
    public abstract class ScreenAnimation
    {
        /// <summary>
        /// Initializes a new instance of Screen Animation
        /// </summary>
        /// <param name="gc">The component to animate</param>
        public ScreenAnimation(ControlHandler gc)
        {
            this.Screen = gc;
        }

        #region Methods

        /// <summary>
        /// Runs various animations based on the current transition state of the 
        /// screen
        /// </summary>
        /// <param name="gameTime">Snapshot object of timing values</param>
        public void Update(GameTime gameTime)
        {

            if (Screen.State == ScreenState.TransitionOff)
                this.AnimateTransitionOff();
            else if (Screen.State == ScreenState.TransitionOn)
                this.AnimateTransitionOn();
            else if (Screen.State == ScreenState.Active)
                this.AnimateActive();
        }

        /// <summary>
        /// Called when the screen is transitioning off
        /// </summary>
        public abstract void AnimateTransitionOff();

        /// <summary>
        /// Called when the screen is transitioning into view
        /// </summary>
        public abstract void AnimateTransitionOn();

        /// <summary>
        /// Called while the screen is active and fully transitioned
        /// </summary>
        public abstract void AnimateActive();

        #endregion

        #region Properties

        /// <summary>
        /// The screen this animator manipulates
        /// </summary>
        private ControlHandler Screen { get; set; }

        /// <summary>
        /// Gets the screen's current position in transition, ranging from 1 (fully transitioned off) to 0 (fully transitioned on)
        /// </summary>
        public float TransitionPosition
        { get { return this.Screen.TransitionPosition; } }

        /// <summary>
        /// The time it takes for the screen to transition on
        /// </summary>
        public TimeSpan TransitionOn
        { get { return this.Screen.TransitionOn; } set { this.Screen.TransitionOn = value; } }

        /// <summary>
        /// The time it takes for the screen to transition off
        /// </summary>
        public TimeSpan TransitionOff
        { get { return this.Screen.TransitionOff; } set { this.Screen.TransitionOff = value; } }

        /// <summary>
        /// The transition alpha of the screen based on it's current transition time
        /// helper method
        /// </summary>
        public byte TransitionAlpha
        { get { return Screen.TransitionAlpha; } }
        
        #endregion
    }
}
