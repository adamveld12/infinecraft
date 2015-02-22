using Core.Diagnostics.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Core.Console
{
    /// <summary>
    /// A simple Gui console window that the user can enter commands into.
    /// </summary>
    public class ConsoleToolWindow : XGPanel
    {
        private Queue<string> cachedCommands;
        private XGListBox listBox;
        private XGTextBox textfield;
        private XGButton button;

        /// <summary>
        /// Initializes a new instance of ConsoleToolWindow
        /// </summary>
        /// <param name="y">y location</param>
        /// <param name="x">x location</param>
        /// <param name="width">width of the panel</param>
        public ConsoleToolWindow(int x, int y, int width) : base(Rectangle.Empty, true)
        {
            cachedCommands = new Queue<String>(5);

            Build(x, y, width);
        }

        /// <summary>
        /// Rebuilds the Console Window
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        public void Build(int x, int y, int width)
        {
            this.Children.Clear();

            this.Rectangle = new Rectangle(x, y, width, 60);

            button = new XGButton(new Rectangle(0, 40, 50, 20), "Enter");
            button.Visible = true;
            button.Alignment = GUIAlignment.HCenter | GUIAlignment.VCenter;

            textfield = new XGTextBox(new Rectangle(50, 40, width, 20));
            textfield.BorderThickness = 1;
            textfield.Visible = true;

            var panel = new XGPanel(new Rectangle(0, 0, width, 40));
            listBox = new XGListBox(new Rectangle(0, 0, width, 40));

            panel.Children.Add(listBox);
            this.Children.Add(panel);
            this.Children.Add(button);
            this.Children.Add(textfield);
        }

        public void ClearText()
        {
            this.textfield.Text = String.Empty;
        }
        /// <summary>
        /// Adds a command onto the cached commands list, if this list is full, 
        /// then it removes the oldest one and places this one 
        /// </summary>
        /// <param name="command"></param>
        public void PushHistory(string command)
        {
            if (listBox.Items.Count >= 5)
            {
                listBox.Items.RemoveAt(0);
            }

            listBox.Items.Add(new XGListBoxItem(command));
        }

        public void GetEventForButton(XGClickedEvent del)
        {
            button.ClickedHandler += del;
        }
        /// <summary>
        /// Get the text field info
        /// </summary>
        public string Text
        {
            get
            {
                return textfield.Text;
            }
        }
    }
}
