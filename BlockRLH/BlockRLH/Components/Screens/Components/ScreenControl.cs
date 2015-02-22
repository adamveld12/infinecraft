using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace BlockRLH.Components.Screens
{
    /// <summary>
    /// The base class for interactive controls that can be added to screens
    /// </summary>
    public abstract class ScreenControl
    {
        #region Fields

        private MouseState currentState, oldState;

        private Rectangle bounds;

        public event EventHandler<MouseEventArgs> OnMouseClick;
        public event EventHandler OnMouseEntered;

        #endregion

        /// <summary>
        /// Creates a new instance of an object of type ScreenControl
        /// </summary>
        /// <param name="x">The X location</param>
        /// <param name="y">The Y location</param>
        /// <param name="width">This control's width</param>
        /// <param name="height">This control's height</param>
        public ScreenControl( int x, int y, int width, int height, ControlHandler parent) : this(parent)
        { this.bounds = new Rectangle(x, y, width, height); }

        /// <summary>
        /// Creates a new instance of an object of type ScreenControl
        /// </summary>
        public ScreenControl(ControlHandler parent)
        {
            currentState = Mouse.GetState();
            bounds = new Rectangle(); 
        }

        #region Methods

        /// <summary>
        /// Updates the logic for this control
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values from updates</param>
        public virtual void Update(GameTime gameTime)
        {
            if (IsEnabled)
            {
                oldState = currentState;
                currentState = Mouse.GetState();

                if (MouseInputManager.IsMouseOver(this.Rectangle))
                {
                    if (OnMouseEntered != null)
                        OnMouseEntered(this, new EventArgs());

                    if (MouseInputManager.IsMouseClicked(oldState, currentState))
                    {
                        MouseInputManager.MousePresses mp = MouseInputManager.GetMouseClicks(oldState, currentState);

                        if (OnMouseClick != null)
                            OnMouseClick(this, new MouseEventArgs(mp.LeftClicked, mp.RightClicked, mp.MiddleClicked));
                    }
                }
            }
        }

        /// <summary>
        /// Implement the draw code for your control here
        /// </summary>
        public abstract void Draw(GameTime gameTime);

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the X and Y location of the control
        /// </summary>
        public Point Location 
        { get { return this.bounds.Location; } set { this.bounds.Location = value; } }

        /// <summary>
        /// Get or set the dimensions of the control
        /// </summary>
        public Point Dimensions 
        { get { return new Point(this.bounds.Width, this.bounds.Height); } set { this.bounds.Width = value.X; this.bounds.Height = value.Y; } }

        /// <summary>
        /// The y location of the control
        /// </summary>
        public int Top
        { get { return this.bounds.Top; } }

        /// <summary>
        /// The x location of the control
        /// </summary>
        public int Left
        { get { return this.bounds.Left; } }

        /// <summary>
        /// The location of the right side of the control
        /// </summary>
        public int Right
        { get { return this.bounds.Right; } }

        /// <summary>
        /// The location of the bottom side of the control
        /// </summary>
        public int Bottom 
        { get { return this.bounds.Bottom; } }

        /// <summary>
        /// Gets the rectangle that represents the area and location of this
        /// control
        /// </summary>
        public Rectangle Rectangle
        { get { return this.bounds; } }

        /// <summary>
        /// If this control is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The player that has control of this screen
        /// </summary>
        public PlayerIndex? ControllingPlayer { get; set; }

        /// <summary>
        /// The screen or component that is holding this control
        /// </summary>
        protected ControlHandler Parent { get; set; }

        #endregion
    }

    /// <summary>
    /// Event argument class for mouse states
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        public bool MiddleClicked { get; private set;}
        public bool LeftClicked { get; private set; }
        public bool RightClicked { get; private set; }

        /// <summary>
        /// Initializes a new instance of MouseEventArgs
        /// </summary>
        public MouseEventArgs(bool leftClicked, bool rightClicked, bool middleClicked)
        {
            this.LeftClicked = leftClicked;
            this.RightClicked = rightClicked;
            this.MiddleClicked = middleClicked;
        }
    }
}
