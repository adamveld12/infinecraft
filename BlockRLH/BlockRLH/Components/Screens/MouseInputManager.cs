using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sigma.User_Interface;

namespace BlockRLH.Components.Screens
{

    public static class MouseInputManager
    {
        #region Methods

        /// <summary>
        /// Holds information about the different states of commonly used mouse buttons
        /// </summary>
        public struct MousePresses
        {
            /// <summary>
            /// Gets the middle mouse button state
            /// </summary>
            public bool MiddleClicked { get; private set; }

            /// <summary>
            /// Gets the left mouse button state
            /// </summary>
            public bool LeftClicked { get; private set; }

            /// <summary>
            /// Gets the rigt mouse button click
            /// </summary>
            public bool RightClicked { get; private set; }

            /// <summary>
            /// Creates a new instance of an object of type MousePresses
            /// </summary>
            public MousePresses(bool leftClicked, bool rightClicked, bool middleClicked)
                : this()
            {
                LeftClicked = leftClicked;
                MiddleClicked = middleClicked;
                RightClicked = rightClicked;
            }
        }

        /// <summary>
        /// Checks if the mouse is over a rectangle
        /// Uses ref's to avoid stack copies
        /// </summary>
        /// <param name="control">The rectangle to check</param>
        /// <returns>If the mouse intersects with this control</returns>
        public static bool IsMouseOver(ref Rectangle control)
        {
            MouseState ms = Mouse.GetState();
            return new Rectangle(ms.X, ms.Y, 1, 1).Intersects(control);
        }

        /// <summary>
        /// Checks if the mouse is over a rectangle
        /// </summary>
        /// <param name="control">The rectangle to check</param>
        /// <returns>If the mouse intersects with this control</returns>
        public static bool IsMouseOver(Rectangle control)
        {
            MouseState ms = Mouse.GetState();
            return new Rectangle(ms.X, ms.Y, 1, 1).Intersects(control);
        }

        /// <summary>
        /// Returns an object containing booleans indicating which buttons were pressed
        /// </summary>
        /// <param name="oldState">The previous state of the mouse</param>
        /// <param name="newState">The current state of the mouse</param>
        /// <returns>If any of the buttons have been clicked between the two states</returns>
        public static MousePresses GetMouseClicks(MouseState oldState, MouseState newState)
        {            
            return new MousePresses
                (
                 (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released),
                 (oldState.RightButton == ButtonState.Pressed && newState.RightButton == ButtonState.Released),
                 (oldState.MiddleButton == ButtonState.Pressed && newState.MiddleButton == ButtonState.Released)
                );
        }

        /// <summary>
        /// Returns true if any of the mouse buttons are clicked
        /// </summary>
        /// <param name="oldState">The previous state of the mouse</param>
        /// <param name="newState">The current state of the mouse</param>
        /// <returns>If any of the buttons have been clicked between the two states</returns>
        public static bool IsMouseClicked(MouseState oldState, MouseState newState)
        {
            return (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released)
                || (oldState.RightButton == ButtonState.Pressed && newState.RightButton == ButtonState.Released)
                || (oldState.MiddleButton == ButtonState.Pressed && newState.MiddleButton == ButtonState.Released);
        }

        #endregion
    }
}