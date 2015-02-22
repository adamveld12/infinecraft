using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.Screens
{
    public class GameOverScreen : MenuScreenControlHandler
    {
        public GameOverScreen(ScreenComponentManager gc, string message, float time) : base(gc)
        {
            var messageText = new MenuItemControl(0, this);
            var timeText = new MenuItemControl(0, this);
            var @continue = new MenuItemControl(0, this);

            messageText.AddString(message);
            timeText.AddString("Time: " + time + " seconds");
            @continue.AddString("Continue");


            var screencenter = this.ScreenCenter();
            messageText.Location = new Point((int)screencenter.X - messageText.Right / 2, 128);
            timeText.Location = new Point((int)screencenter.X - timeText.Right / 2, 200);
            @continue.Location = new Point((int)screencenter.X - @continue.Right / 2, 384);

            @continue.Index = 1;

            this.AddControl(messageText);
            this.AddControl(timeText);
            this.AddControl(@continue);

            @continue.OnMouseClick += new EventHandler<MouseEventArgs>(continue_OnMouseClick);
        }

        void continue_OnMouseClick(object sender, MouseEventArgs e)
        {
            LoadingScreen.Load(this.Manager, new[] { new TitleScreen(this.Manager) }, null, null);
        }
    }
}
