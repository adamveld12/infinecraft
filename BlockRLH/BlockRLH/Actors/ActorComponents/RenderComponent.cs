using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RLH.Components;

namespace BlockRLH.Actors.ActorComponents
{
    public abstract class RenderComponent : ActorComponent
    {
        private bool bvisible = true;

        public RenderComponent(Actor owner) : base(owner)
        { }

        /// <summary>
        /// Draws this component
        /// <remarks>
        /// Preferable to use the owner's location and rotation info
        /// </remarks>
        /// </summary>
        public abstract void Draw(GameTime gameTime);

        /// <summary>
        /// Get or set the visibility of this Actor
        /// </summary>
        public bool bIsVisible
        {
            get { return bvisible; }
            set { bvisible = value; }
        }
    }
}
