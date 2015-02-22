using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BlockRLH.Actors.ActorComponents
{
    /// <summary>
    /// A component that can be added to an actor for additional functionality
    /// </summary>
    public abstract class ActorComponent
    {
        private Actor owner;

        /// <summary>
        /// Initializes a new instance of ActorComponent
        /// </summary>
        /// <param name="owner"></param>
        public ActorComponent(Actor owner)
        { this.owner = owner; this.bEnabled = true; }

        /// <summary>
        /// Ticks the component
        /// </summary>
        public virtual void Tick(GameTime delta)
        { }

        /// <summary>
        /// Get the owner of this component
        /// </summary>
        public Actor Owner
        { get { return owner; } }

        /// <summary>
        /// Get or set this actors ability to tick
        /// </summary>
        public bool bEnabled { get; set; }
    }
}
