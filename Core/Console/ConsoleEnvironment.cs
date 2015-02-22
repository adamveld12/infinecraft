using Core.Diagnostics.Interface;
using Microsoft.Xna.Framework;
using Core.Input;

namespace Core.Console
{
    /// <summary>
    /// This is a SIEngine implementation of a console environment
    /// <remarks>
    /// When the user types a command and hits the send button a chain of events occur before the actual command gets executed.
    /// First, it gets sent to the Client as a string and then gets processed. Second, the string gets turned into a commandlet by
    /// matching the beginning of the string to the commandlet name. Finally, the actual command gets sent to the commandlet and it gets executed there.
    /// </remarks>
    /// </summary>
    public class ConsoleEnvironment : Microsoft.Xna.Framework.DrawableGameComponent
    {
        ConsoleToolWindow inputArea;
        InputManager inputs;

        /// <summary>
        /// Initializes a new instance of ConsoleEnvironment
        /// </summary>
        /// <param name="game"></param>
        public ConsoleEnvironment(EngineContext game)
            : base(game)
        {
            this.Engine = game;
            this.Game.Components.Add(this);
            this.ConsoleLocation = ConsoleLocation.Bottom;

            // we want this component to be drawn last, that way it appears above any components or post processing effects
            this.DrawOrder = int.MaxValue;
        }

        /// <summary>
        /// Handling graphics related initialization
        /// </summary>
        protected override void LoadContent()
        {
            var gdm = Engine.GraphicsDeviceManager;
            Point consoleLocation = new Point(0, 0);

            // create the XGControl and attach a method to the device.reset to allow resizing the control during resolution changes
            if (this.ConsoleLocation == ConsoleLocation.Top)
                consoleLocation.Y = 0;
            else if (this.ConsoleLocation == ConsoleLocation.Bottom)
                consoleLocation.Y = GraphicsDevice.Viewport.Height - 60;

            inputArea = new ConsoleToolWindow(0, consoleLocation.Y, GraphicsDevice.Viewport.Width);

            gdm.DeviceReset += (o, e) =>
                {
                    var view = Game.GraphicsDevice.Viewport;
                    inputArea.Build(0, ConsoleLocation == Console.ConsoleLocation.Top ? 0 : view.Height - 60, view.Width);
                };

            XnaGUIManager.Controls.Add(inputArea);
            inputArea.Visible = false;

            this.inputArea.GetEventForButton(CommandSent);
        }

        /// <summary>
        /// event handler for commmands
        /// </summary>
        /// <param name="sender"></param>
        void CommandSent(XGControl sender)
        {
            var text = this.inputArea.Text;
            this.inputArea.ClearText();

            if (!string.IsNullOrWhiteSpace(text))
                this.inputArea.PushHistory(text);
            
            //new Interpreter(text, new[] { "::" }).ParseCommand();
        }

        /// <summary>
        /// Toggles the console's window
        /// </summary>
        public void ToggleConsoleVisibility()
        {
            inputArea.Visible = !inputArea.Visible;
        }

        /// <summary>
        /// Shortcut for getting the SIEngineContext
        /// </summary>
        public EngineContext Engine { get; private set; }

        /// <summary>
        /// Where the console gets rendered
        /// </summary>
        public ConsoleLocation ConsoleLocation { get; set; }

        /// <summary>
        /// Get if the console is currently visible
        /// </summary>
        public bool IsConsoleVisible { get { return inputArea.Visible; } }
    }

    /// <summary>
    /// Defines different console locations
    /// </summary>
    public enum ConsoleLocation
    {
        // Console appears at the top of the window
        Top = 0,
        // Console appears at the bottom of the window
        Bottom = 1
    }
}
