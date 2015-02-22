using System;
using System.Collections.Generic;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockRLH.Components.Screens
{
    /// <summary>
    /// A gamescreen that handles screen controls 
    /// </summary>
    public class ControlHandler : GameScreen
    {
        #region Fields

        public event EventHandler<MouseEventArgs> OnMouseClick;
        public event EventHandler OnMouseEntered;
        public event EventHandler OnMouseExit;
        public event EventHandler OnScreenExit;

        private bool hasMouseEntered = false;

        private MouseState oldState, currentState;

        private List<ScreenControl> controls;

        #endregion

        /// <summary>
        /// Initializes a new instance of Control handler
        /// </summary>
        /// <param name="gc"></param>
        public ControlHandler(ScreenComponentManager gc) : base(gc) 
        {
            currentState = Mouse.GetState();
            controls = new List<ScreenControl>(4);
            this.IsEnabled = true;
        }

        #region Methods

        /// <summary>
        /// Adds a control to the list
        /// </summary>
        /// <param name="item"></param>
        public void AddControl(ScreenControl item)
        {
            if (controls.Contains(item))
                return;
            this.controls.Add(item);
        }

        /// <summary>
        /// Removes a control from the list
        /// </summary>
        /// <param name="item">The control to remove</param>
        public void RemoveControl(ScreenControl item)
        {
            this.controls.Remove(item);
        }

        /// <summary>
        /// Updates controller logic
        /// </summary>
        public override void Update(GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gt, otherScreenHasFocus, coveredByOtherScreen);

            Rectangle rect = new Rectangle();
            rect.Width = this.Manager.ScreenDimensions.X;
            rect.Height = this.Manager.ScreenDimensions.Y;

            this.Rectangle = rect;

            if (!IsEnabled)
                return;

            foreach (var item in controls)
                item.Update(gt);

            if (TransitionOff <= TimeSpan.Zero)
                if (this.OnScreenExit != null)
                    this.OnScreenExit(this, new EventArgs());

            if(this.ScreenAnimation != null)
                this.ScreenAnimation.Update(gt);
        }

        /// <summary>
        /// Draws this control and it's components
        /// </summary>
        public override void Draw(GameTime gt, bool coveredByOtherScreen)
        {
            if(!coveredByOtherScreen || IsPopup)
                foreach (var item in controls)
                    item.Draw(gt);
        }

        /// <summary>
        /// Handles Input for this control
        /// </summary>
        public override void HandleInput(InputManager input)
        {
#if WINDOWS
            oldState = currentState;
            currentState = Mouse.GetState();

            if (MouseInputManager.IsMouseOver(this.Rectangle) && this.hasMouseEntered)
            {
                this.hasMouseEntered = true;

                if (OnMouseEntered != null)
                    OnMouseEntered(this, new EventArgs());
            }

            if (MouseInputManager.IsMouseClicked(oldState, currentState) && this.hasMouseEntered)
            {
                MouseInputManager.MousePresses mp = MouseInputManager.GetMouseClicks(oldState, currentState);

                if (OnMouseClick != null)
                    OnMouseClick(this, new MouseEventArgs(mp.LeftClicked, mp.RightClicked, mp.MiddleClicked));
            }

            if (!MouseInputManager.IsMouseOver(this.Rectangle) && hasMouseEntered)
            {
                this.hasMouseEntered = false;

                if (OnMouseExit != null)
                    OnMouseExit(this, new EventArgs());
            }
#endif
        }

        /// <summary>
        /// Returns a copy of the elements this control owns
        /// </summary>
        /// <returns>a collection of ScreenControls</returns>
        public ScreenControl[] GetControls()
        {
            return this.controls.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// If this control is enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The bounding rectangle for this control
        /// </summary>
        public Rectangle Rectangle { get; protected set; }

        /// <summary>
        /// An animator for this screen
        /// </summary>
        public ScreenAnimation ScreenAnimation { get; set; }

        /// <summary>
        /// The controls that this screen owns
        /// </summary>
        protected List<ScreenControl> ScreenControls { get { return controls; } private set { controls = value; } }

        /// <summary>
        /// This screen's offset from the top right
        /// </summary>
        public Vector2 Offset { get; set; }

        #endregion
    }

    public static class ControlUtils
    {
        public static Vector2 ScreenCenter(this ControlHandler screen)
        {
            var size = screen.Manager.GraphicsDevice.Viewport;
            return new Vector2(size.Width / 2, size.Height / 2);
        }

        public static Vector2 ScreenDimensions(this ControlHandler screen)
        {
            var size = screen.Manager.GraphicsDevice.Viewport;
            return new Vector2(size.Width, size.Height);
        }
    }
}
