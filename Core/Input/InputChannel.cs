using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Input
{
    /// <summary>
    /// A handle to one of the player inputs
    /// </summary>
    public class InputChannel
    {
        private KeyboardState oldKeyState, newKeyState;
        private GamePadState oldGamePadState, newGamePadState;

        private PlayerIndex playerIndex;

        private bool bcontrollerConnected;
        private bool bcontrollerWasConnected;

        private float elapsedTimeSinceRumble;
        private bool brumbling;

        /// <summary>
        /// Initializes a new instance of InputChannel
        /// </summary>
        /// <param name="index"></param>
        public InputChannel(PlayerIndex index)
        {
            this.playerIndex = index;
            brumbling = false;
            elapsedTimeSinceRumble = 0;
        }
        
        #region Update and Utility

        /// <summary>
        /// Update channel logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            oldGamePadState = newGamePadState;
            oldKeyState = newKeyState;

            newKeyState = Keyboard.GetState(playerIndex);

            newGamePadState = GamePad.GetState(playerIndex);

            CheckForControllerAbsence();

            TickRumbleLogic(gameTime);

        }

        private void TickRumbleLogic(GameTime gameTime)
        {
            if (brumbling)
            {
                elapsedTimeSinceRumble -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedTimeSinceRumble <= 0)
                {
                    brumbling = false;
                    elapsedTimeSinceRumble = 0;
                    GamePad.SetVibration(playerIndex, 0, 0);
                }
            }
        }

        private void CheckForControllerAbsence()
        {
            bool controllerIn = GamePad.GetCapabilities(playerIndex).IsConnected;

            // if the controller is currently plugged in..
            if (controllerIn)
            {
                //we check to see if the state of this channel says plugged in, if not then...
                if (!bcontrollerConnected)
                {
                    // we set controllerconnected and wasconnected to true
                    bcontrollerConnected = bcontrollerWasConnected = controllerIn;
                }
            }
            // else the controller is not unplugged, so...
            else
            {
                // if the state of this channel says plugged in
                if (bcontrollerConnected)
                {
                    // then we set it to the current controllerIn state
                    bcontrollerConnected = controllerIn;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if there is a button pressed 
        /// </summary>
        public bool DebouncedButtonPress(ControlMapping mapping)
        {
            bool keyboardPressed = false;
            bool gamepadPressed = false;
#if WINDOWS
            if (mapping.KeyboardPress.HasValue)
            {
                keyboardPressed = newKeyState.IsKeyDown(mapping.KeyboardPress.Value) && oldKeyState.IsKeyUp(mapping.KeyboardPress.Value);
            }

#endif
            if (mapping.ButtonPress.HasValue)
            {
                gamepadPressed = newGamePadState.IsButtonDown(mapping.ButtonPress.Value) && oldGamePadState.IsButtonUp(mapping.ButtonPress.Value);
            }

            return keyboardPressed || gamepadPressed;
        }

        /// <summary>
        /// Checks if there is a constantly held button press
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public bool ConstantButtonPress(ControlMapping mapping)
        {
            bool keyboardPressed = false;
            bool gamepadPressed = false;
#if WINDOWS
            if (mapping.KeyboardPress.HasValue)
            {
                keyboardPressed = newKeyState.IsKeyDown(mapping.KeyboardPress.Value);
            }

#endif
            if (mapping.ButtonPress.HasValue)
            {
                gamepadPressed = newGamePadState.IsButtonDown(mapping.ButtonPress.Value);
            }

            return keyboardPressed || gamepadPressed;
        }

        /// <summary>
        /// Gets the raw input from the Left Thumbstick
        /// </summary>
        /// <returns></returns>
        public Vector2 GetLeftStickAngle()
        {
            return newGamePadState.ThumbSticks.Left;
        }

        /// <summary>
        /// Gets the raw input from the Right Thumbstick
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRightStickAngle()
        {
            return newGamePadState.ThumbSticks.Right;
        }

        /// <summary>
        /// Gets raw input from the left trigger
        /// </summary>
        /// <returns></returns>
        public float GetLeftTrigger()
        {
            return newGamePadState.Triggers.Left;
        }

        /// <summary>
        /// Gets raw input from the right trigger
        /// </summary>
        /// <returns></returns>
        public float GetRightTrigger()
        {
            return newGamePadState.Triggers.Right;
        }

        /// <summary>
        /// Rumbles the players gamepad
        /// </summary>
        /// <param name="elapsed">how long to rumble for</param>
        /// <param name="smallMotorIntensity">intensity of the small motor</param>
        /// <param name="largeMotorIntensity">intensity of the large motor</param>
        public void RumblePad(float elapsed, float smallMotorIntensity, float largeMotorIntensity)
        {
            // set the gamepad's vibration and then start a timer for how long it should last.. negative values dont cause any vibration
            if (smallMotorIntensity > 0 || largeMotorIntensity > 0)
            {
                elapsedTimeSinceRumble = elapsed;
                brumbling = true;
                GamePad.SetVibration(playerIndex, MathHelper.Clamp(smallMotorIntensity, 0, 1), MathHelper.Clamp(largeMotorIntensity, 0, 1));
            }
        }
        #endregion
        /// <summary>
        /// Get or set the player index
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        /// <summary>
        /// get if the controller is currently connected
        /// </summary>
        public bool bControllerConnected
        {
            get { return bcontrollerConnected; }
        }

        /// <summary>
        /// get if the controller was ever connected at some point
        /// </summary>
        public bool bControllerWasConnected
        {
            get { return bcontrollerWasConnected; }
        }

        /// <summary>
        /// If the controller is currently rumbling
        /// </summary>
        public bool bRumbling
        {
            get { return brumbling; }
        }

    }
}
