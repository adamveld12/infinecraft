using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Core.Console
{
    /// <summary>
    /// Defines a command that can be sent to the console and executed by it's receiver
    /// </summary>
    public abstract class Commandlet
    {
        private Game game;

        /// <summary>
        /// Initializes a new instance of commandlet
        /// </summary>
        /// <param name="game"></param>
        public Commandlet(Game game)
        {
            this.game = game;
        }
    }
}
