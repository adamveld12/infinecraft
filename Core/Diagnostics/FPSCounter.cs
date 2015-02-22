using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Core.Diagnostics;


namespace Core.Diagnostics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FPSCounter : DiagnosticComponent
    {
        const int NUM_SECONDS = 1;
        int framesDrawn = 0;

        float delta = 0;
        int framesPerSecond = 0;
        float elapsedSinceLastUpdate = 0;

        public FPSCounter(DiagnosticsManager diagnostics) : base(diagnostics)
        {

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            elapsedSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedSinceLastUpdate >= NUM_SECONDS)
            {
                elapsedSinceLastUpdate = 0;

                framesPerSecond = framesDrawn;
            }
        }

        /// <summary>
        /// Draws the game component
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            framesDrawn++;
        }

        public override string ComponentID
        {
            get { return "FPS Display"; }
        }

        public override string[] GetValues()
        {
            return new string[] { this.framesPerSecond.ToString() };
        }
    }
}
