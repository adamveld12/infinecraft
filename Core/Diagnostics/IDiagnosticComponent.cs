using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Core.Diagnostics;

namespace Core.Diagnostics
{

    public abstract class DiagnosticComponent
    {
        public DiagnosticComponent(DiagnosticsManager diagnostics)
        {
            this.DiagnosticManager = diagnostics;
        }

        public DiagnosticsManager DiagnosticManager { get; private set; }

        /// <summary>
        /// A simple ID that you can use to keep track of the component with
        /// </summary>
        public abstract string ComponentID
        {
             get;
        }

        /// <summary>
        /// Updates the component
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Not necessarily for drawing the component out, more for counting the number of draws
        /// or any other draw related profiling
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Draw(GameTime gameTime);


        /// <summary>
        /// Gets an array of values from the profiler
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetValues();
    }
}
