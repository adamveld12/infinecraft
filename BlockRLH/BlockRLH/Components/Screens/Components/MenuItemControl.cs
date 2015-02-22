using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Components.Screens
{
    /// <summary>
    /// Represents an item that can be added onto a screen with text
    /// </summary>
    public class MenuItemControl : ScreenControl
    {
        #region Fields

        protected List<string> stringList;

        /// <summary>
        /// Event raised when the menu entry is selected
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        #endregion

        /// <summary>
        /// Initializes a new instance of MenuItemControl
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        /// <param name="index">Selection index of this item</param>
        public MenuItemControl(int x, int y, int width, int height, int index, int font, ControlHandler parent)
            : this(font, parent)
        {
            this.IsEnabled = true;
            this.Index = index;
            this.Location = new Point(x, y);
            this.Dimensions = new Point(width, height);
        }

        /// <summary>
        /// Initializes a new instance of MenuItemControl
        /// </summary>
        public MenuItemControl(int font, ControlHandler parent) : base(parent)
        {
            this.stringList = new List<string>(2);
            this.Parent = parent;
            this.FontIndex = font;
            this.OnMouseClick += new EventHandler<MouseEventArgs>(OnSelected);
            this.IsEnabled = true;
        }

        #region Method


        void OnSelected(object sender, MouseEventArgs e)
        {
            if (IsHighlighted)
                if (this.Selected != null)
                    Selected(this, new PlayerIndexEventArgs(Parent.ControllingPlayer));
        }

        /// <summary>
        /// Adds a new string to the list, each element in the list,
        /// makes a new line.
        /// </summary>
        /// <param name="k"></param>
        public void AddString(string k)
        { this.stringList.Add(k); var dim = this.Parent.Manager.GetFont(this.FontIndex).MeasureString(k); this.Dimensions = new Point((int)dim.X, (int)dim.Y * stringList.Count); }

        /// <summary>
        /// Sets the string at the specific index to the replacement
        /// </summary>
        public void SetString(string replacement, int index)
        { this.stringList[index] = replacement; }

        /// <summary>
        /// Gets a string based on the index
        /// </summary>
        public string GetString(int line)
        { return this.stringList[line]; }

        
        /// <summary>
        /// Updates menu item logic
        /// </summary>
        public override void Update(GameTime gt)
        {
            if (IsEnabled)
            {

                // When the menu selection changes, entries gradually fade between
                // their selected and deselected appearance, rather than instantly
                // popping to the new state.
                float fadeSpeed = (float)gt.ElapsedGameTime.TotalSeconds * 4;

                if (IsHighlighted)
                    SelectionFade = Math.Min(SelectionFade + fadeSpeed, 1);
                else
                    SelectionFade = Math.Max(SelectionFade - fadeSpeed, 0);
            }

            base.Update(gt);
        }

        /// <summary>
        /// Draws the text for this menu item
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            if (IsEnabled)
                CurrentColor = IsHighlighted ? Color.White : Color.Yellow;

            CurrentColor = new Color( CurrentColor.R, CurrentColor.G, CurrentColor.B,
            this.Parent.TransitionAlpha);

            SpriteBatch sb = this.Parent.Manager.SpriteBatch;

            sb.Begin();

            foreach (string item in this.stringList)
            {
                sb.DrawString(
                                this.Parent.Manager.GetFont(this.FontIndex), 
                                item, new Vector2(this.Location.X, this.Location.Y), 
                                CurrentColor
                            );
            }

            sb.End();
        }

        /// <summary>
        /// Helper for firing the Selected event in derived classes
        /// </summary>
        public void FireSelected(object sender, PlayerIndex index)
        {
            if (Selected != null)
                Selected(sender, new PlayerIndexEventArgs(index));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The selection index of this menu item
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// If this item is selected
        /// </summary>
        public bool IsHighlighted { get; set; }

        /// <summary>
        /// The fade speed of this item when selected or deselected
        /// </summary>
        public float SelectionFade { get; set; }

        /// <summary>
        /// Horizontal(Left and Right) padding of the text
        /// </summary>
        public int HorizontalPadding { get; set; }

        /// <summary>
        /// Vertical(Top and Bottom) padding of the text
        /// </summary>
        public int VerticalPadding { get; set; }

        /// <summary>
        /// Gets or sets the current color of the menu control
        /// </summary>
        public Color CurrentColor { get; set; }

        /// <summary>
        /// The index of this item's font in the font cache
        /// </summary>
        public int FontIndex { get; set; }


        #endregion
    }
}
