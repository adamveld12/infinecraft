using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RLH.Components;

namespace BlockRLH.Actors.ActorComponents
{
    /// <summary>
    /// A basic collision component with intersection checks
    /// </summary>
    public abstract class CollisionComponent : ActorComponent
    {
        public CollisionComponent(Actor owner)
            : base(owner)
        { }

        /// <summary>
        /// Check for intersections with a bounding sphere
        /// </summary>
        /// <param name="collisionPrimitive">A collision primitive</param>
        /// <returns>true for intersection or false for none</returns>
        public abstract bool Intersecting(BoundingSphere collisionPrimitive);

        /// <summary>
        /// Check for intersections with a bounding box
        /// </summary>
        /// <param name="collisionPrimitive">A collision primitive</param>
        /// <returns>true for intersection or false for none</returns>
        public abstract bool Intersecting(BoundingBox collisionPrimitive);

        /// <summary>
        /// Check for intersections with a bounding frustum
        /// </summary>
        /// <param name="collisionPrimitive">A collision primitive</param>
        /// <returns>true for intersection or false for none</returns>
        public abstract bool Intersecting(BoundingFrustum collisionPrimitive);

        /// <summary>
        /// Check for intersections with a ray trace
        /// </summary>
        /// <param name="collisionPrimitive">A collision primitive</param>
        /// <returns>true for intersection or false for none</returns>
        public abstract bool Intersecting(Ray collisionPrimitive);

        /// <summary>
        /// Used for updating the collision primitive
        /// </summary>
        public abstract void UpdateCollisionPrimitive();
    }
}
