
using Core.Input;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.Screens
{
    public class MenuScreenControlHandler : ControlHandler
    {
        #region Fields 

        /// <summary>
        /// The currently selected index on this menu.
        /// Returns -1 if nothing is selected
        /// </summary>
        public int SelectedIndex { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of MenuScreenControl
        /// </summary>
        public MenuScreenControlHandler(ScreenComponentManager gc)
            : base(gc)
        {
            SelectedIndex = 1;
        }

        #region Methods

        /// <summary>
        /// Updates this screen's logic and menu items
        /// </summary>
        public override void Update(GameTime gt, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gt, otherScreenHasFocus, coveredByOtherScreen);
            
            if (!IsEnabled)
                return;

            foreach (var item in this.ScreenControls)
            {
                if (item is MenuItemControl)
                    (item as MenuItemControl).IsHighlighted = (item as MenuItemControl).Index == this.SelectedIndex;
            }
        }
        
        /// <summary>
        /// Handles the input for this screen
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            
            //var gb = GlobalGameSettings.UserControlMappings;

            //PlayerIndex index;

            //if (input.IsNewButtonPress(gb.GetButton(1337), this.ControllingPlayer, out index))
            //    SelectedIndex = (SelectedIndex + 1) % this.ScreenControls.Count;

            //else if (input.IsNewButtonPress(gb.GetButton(1338), this.ControllingPlayer, out index))
            //    SelectedIndex = ((SelectedIndex - 1) <= -1) ? this.ScreenControls.Count - 1 : SelectedIndex--;

            //if (input.IsNewButtonPress(gb.GetButton(1339), this.ControllingPlayer, out index))
            //    foreach (var item in this.ScreenControls)
            //        if ((item as MenuItemControl).Index == this.SelectedIndex)
            //            (item as MenuItemControl).FireSelected(this, this.ControllingPlayer ?? PlayerIndex.One);

#if WINDOWS
            foreach (var item in this.ScreenControls)
            {
                if (MouseInputManager.IsMouseOver(item.Rectangle))
                    this.SelectedIndex = (item as MenuItemControl).Index;
            }
#endif
        }

        /// <summary>
        /// Adds a control to this screen
        /// </summary>
        public new void AddControl(ScreenControl item)
        {
            if (this.ScreenControls.Contains(item) || !(item is MenuItemControl))
                return;
            this.ScreenControls.Add(item);
            item.OnMouseEntered += (o, e) => { this.SelectedIndex = ((MenuItemControl)o).Index; };
        }
        
        #endregion

    }
}
