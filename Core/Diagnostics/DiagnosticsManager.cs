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
using System.Diagnostics;


namespace Core.Diagnostics
{
    /// <summary>
    /// Allows you to grab diagnostic information about your game
    /// </summary>
    public class DiagnosticsManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Dictionary<Type, DiagnosticComponent> services;
        private System.Diagnostics.Process applicationProcess;

        public DiagnosticsManager(Game game)
            : base(game)
        {
            services = new Dictionary<Type, DiagnosticComponent>();
            applicationProcess = Process.GetCurrentProcess();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var components in services.Values)
                components.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var components in services.Values)
                components.Draw(gameTime);
        }

        /// <summary>
        /// Add a profiler to the manager
        /// </summary>
        /// <param name="component"></param>
        public void AddDiagnostic(DiagnosticComponent component)
        { services.Add(component.GetType(), component); }

        /// <summary>
        /// Retrieve a profiler from this component
        /// </summary>
        /// <typeparam name="T">DiagnosticComponent</typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public T GetComponent<T>(Type component) where T : DiagnosticComponent
        { return (services[component.GetType()] as T); }

        /// <summary>
        /// Retrieve a profiler from this component
        /// </summary>
        /// <typeparam name="T">DiagnosticComponent</typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public void RemoveComponent<T>(Type component) where T : DiagnosticComponent
        { services.Remove(component); }

        /// <summary>
        /// The proccess of this application
        /// </summary>
        public Process AppDomainProcess 
        { get { return this.applicationProcess; } }
    }
}
