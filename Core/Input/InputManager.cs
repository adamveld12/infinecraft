using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Core.Initialization.INi;


namespace Core.Input
{
    public enum InputType
    {
        /// <summary>
        /// Only allows for a singular button press, won't cause a fire over multiple frames
        /// </summary>
        Debounced = 0,

        /// <summary>
        /// If this input is fired from a button being continuously held down, will fire over multiple frames
        /// </summary>
        Continuous = 1
    }

    /// <summary>
    /// Event argument for controller disconnect
    /// </summary>
    public class ControllerDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the player who's controller is disconnected
        /// </summary>
        public readonly PlayerIndex PlayerIndex;

        /// <summary>
        /// Initializes a new instance of ControllerDisconnectedEventArgs
        /// </summary>
        /// <param name="index"></param>
        public ControllerDisconnectedEventArgs(PlayerIndex index)
        {
            PlayerIndex = index;
        }
    }

    /// <summary>
    /// Event argument for Input fire
    /// </summary>
    public class InputFiredArgs : EventArgs
    {
        /// <summary>
        /// What kind of input generated this event
        /// </summary>
        public readonly InputType InputType;

        /// <summary>
        /// The name of the mapping
        /// </summary>
        public readonly string Command;

        /// <summary>
        /// Initializes a new instance of InputFired
        /// </summary>
        /// <param name="keypress"></param>
        /// <param name="buttonpress"></param>
        public InputFiredArgs(string name, InputType inputKind)
        {
            Command = name;
            InputType = inputKind;
        }
    }

    /// <summary>
    /// Contains information about key mappings
    /// </summary>
    public struct ControlMapping
    {
        /// <summary>
        /// The name of this mapping
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The Keyboard key for this control
        /// </summary>
        public readonly Keys? KeyboardPress;
        /// <summary>
        /// The Button press for this control
        /// </summary>
        public readonly Buttons? ButtonPress;

        /// <summary>
        /// What type of input this mapping contains
        /// </summary>
        public readonly InputType InputType;

        /// <summary>
        /// Initializes a new instance of ControlMapping
        /// </summary>
        /// <param name="name"></param>
        /// <param name="KeyboardPress"></param>
        /// <param name="ButtonPress"></param>
        public ControlMapping(string name, Keys? KeyboardPress, Buttons? ButtonPress, InputType type)
        {
            this.Name = name;
            this.KeyboardPress = KeyboardPress;
            this.ButtonPress = ButtonPress;
            this.InputType = type;
        }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        private bool benteringConsoleInput = false;
        private InputChannel[] inputChannels;
        private Dictionary<string, ControlMapping> controls;

        public delegate void InputFired(object sender, InputFiredArgs eventArgs, float delta);

        /// <summary>
        /// Fired when an input was captured
        /// </summary>
        private InputFired[] inputsCaptured = new InputFired[4];

        /// <summary>
        /// Fired when a controller that was ever connected to the engine becomes disconnected
        /// </summary>
        public event EventHandler<ControllerDisconnectedEventArgs> ControllerDisconnected;

        /// <summary>
        /// Initializes a new instance of InputManager
        /// </summary>
        /// <param name="game"></param>
        public InputManager(EngineContext game)
            : base(game)
        {
            this.Engine = game;
            game.Components.Add(this);

            // Update us first, since we want all input using objects to have the latest data
            this.UpdateOrder = int.MinValue;
        }

        #region Initalize and Update

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            inputChannels = new InputChannel[4];
            controls = new Dictionary<string, ControlMapping>();

            for (int i = 0; i < inputChannels.Length; i++)
            {
                inputChannels[i] = new InputChannel((PlayerIndex)Enum.Parse(typeof(PlayerIndex), i.ToString(), true));
            }
        }

        public void LoadMappingsFromIniFiles(IniFile file)
        {
            foreach (var unparsedData in file.sections)
            {
                foreach (var element in unparsedData.elements)
                {
                    var splitup = element.ToString().Split(new string[] { "|", "{", "}", " =" }, System.StringSplitOptions.RemoveEmptyEntries);

                    Keys? keyboardPress;
                    Buttons? buttonPress;
                    InputType type = default(InputType);

                    if (splitup[2] == "null")
                        keyboardPress = null;
                    else
                        keyboardPress = (Keys)Enum.Parse(typeof(Keys), splitup[2]);

                    if (splitup[3] == "null")
                        buttonPress = null;
                    else
                        buttonPress = (Buttons)Enum.Parse(typeof(Buttons), splitup[3]);

                    if (unparsedData.Name ==InputType.Debounced.ToString())
                        type = InputType.Debounced;
                    else if(unparsedData.Name == InputType.Continuous.ToString())
                        type = InputType.Continuous;

                    AddControlMapping(splitup[0].Substring(8), buttonPress, keyboardPress, type);
                }
            }
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < inputChannels.Length; i++)
            {
                var input = inputChannels[i];

                input.Update(gameTime);

                foreach (var mapping in controls)
                {
                    if (mapping.Value.InputType == InputType.Debounced && input.DebouncedButtonPress(mapping.Value))
                        FireInputCaptured(input.PlayerIndex, mapping.Value, InputType.Debounced, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    else if (mapping.Value.InputType == InputType.Continuous && input.ConstantButtonPress(mapping.Value))
                        FireInputCaptured(input.PlayerIndex, mapping.Value, InputType.Continuous, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                // fire an event when the controller becomes disconnected
                if (!input.bControllerConnected && input.bControllerWasConnected)
                {
                    if (ControllerDisconnected != null)
                        ControllerDisconnected(this, new ControllerDisconnectedEventArgs(input.PlayerIndex));
                }
            }
        }

        #endregion

        private void FireInputCaptured(PlayerIndex index, ControlMapping mapping, InputType inputType, float delta)
        {
            InputFired inputEvent = null;
            if (benteringConsoleInput && mapping.KeyboardPress.Value == Keys.OemTilde && index == PlayerIndex.One)
                inputEvent = inputsCaptured[0];
            else
            {
                switch (index)
                {
                    case PlayerIndex.Four:
                        inputEvent = inputsCaptured[3];
                        break;
                    case PlayerIndex.One:
                        inputEvent = inputsCaptured[0];
                        break;
                    case PlayerIndex.Three:
                        inputEvent = inputsCaptured[2];
                        break;
                    case PlayerIndex.Two:
                        inputEvent = inputsCaptured[1];
                        break;
                    default:
                        break;
                }
            }

            if (inputEvent != null)
                inputEvent(this, new InputFiredArgs(mapping.Name, inputType), delta);
        }

        /// <summary>
        /// Add a new target for input fire events
        /// </summary>
        /// <param name="methodTarget"></param>
        /// <param name="index"></param>
        public void ListenToInputChannel(InputFired methodTarget, PlayerIndex index)
        {
            this.inputsCaptured[(int)index] += methodTarget;
        }

        /// <summary>
        /// Removes all delegates from the event handlers
        /// </summary>
        public void ClearInputChannel(PlayerIndex index)
        {
            this.inputsCaptured[(int)index] = null;
        }

        /// <summary>
        /// Adds a control mapping to the input manager's check list
        /// </summary>
        public void AddControlMapping(string key, Buttons? buttonpress, Keys? keypress, InputType type)
        {
            var mapping = new ControlMapping(key, keypress, buttonpress, type);

            this.controls[key] = mapping;
        }

        /// <summary>
        /// Returns an input channel
        /// </summary>
        public InputChannel GetChannel(PlayerIndex index)
        {
            return inputChannels[(int)index];
        }

        /// <summary>
        /// Reference to Sigma Engine
        /// </summary>
        public EngineContext Engine { get; private set; }

        /// <summary>
        /// Get or set if the player is entering console input
        /// </summary>
        public bool bEnteringConsoleInput
        {
            get { return benteringConsoleInput; }
            set { benteringConsoleInput = value; }
        }
    }
}
