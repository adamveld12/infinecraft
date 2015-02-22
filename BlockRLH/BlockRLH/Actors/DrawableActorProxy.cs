using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework;

namespace BlockRLH.Actors
{
    /// <summary>
    /// An actor that has a mesh component and a collision mesh
    /// </summary>
    public class DrawableActorProxy
    {
        public readonly Actor actor;
        public readonly RenderComponent mesh;
        public readonly CollisionComponent collision;

        public DrawableActorProxy(Actor actor, RenderComponent mesh, CollisionComponent collision)
        {
            this.actor = actor;
            this.mesh = mesh;
            this.collision = collision;
        }

        /// <summary>
        /// Gets the location of this actor
        /// </summary>
        public Vector3 Location
        { get { return this.actor.Location; } }

        /// <summary>
        /// Gets if this actor should be rendered
        /// </summary>
        public bool Visible
        { get { return mesh.bIsVisible; } }

        /// <summary>
        /// Special Id for this actor
        /// </summary>
        public int ID
        { get { return actor.ID; } }
    }

}
