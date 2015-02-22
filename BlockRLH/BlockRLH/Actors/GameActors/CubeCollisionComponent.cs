using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework;
using BlockRLH.Components;

namespace BlockRLH.Actors.GameActors
{
    /// <summary>
    /// Handles collision primitives for Cubes
    /// </summary>
    public class CubeCollisionComponent : CollisionComponent
    {
        private BoundingBox boundingBox;
        private float faceDim;
        private float distance;
        public Face frontFace, backFace, leftFace, rightFace, topFace, botFace;

        public CubeCollisionComponent(Actor owner, float faceDimension) : base(owner)
        {
            this.faceDim = faceDimension;
            this.distance = faceDim;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override bool Intersecting(BoundingSphere collisionPrimitive)
        { return this.boundingBox.Intersects(collisionPrimitive); }

        public override bool Intersecting(BoundingBox collisionPrimitive)
        { return this.boundingBox.Intersects(collisionPrimitive); }

        public override bool Intersecting(BoundingFrustum collisionPrimitive)
        { return this.boundingBox.Intersects(collisionPrimitive); }

        public override bool Intersecting(Ray collisionPrimitive)
        { return collisionPrimitive.Intersects(boundingBox) == null ? false : true; }

        public override void UpdateCollisionPrimitive()
        {
            distance = InstancedBoxRenderer.SIZE;
            boundingBox.Min = Vector3.Subtract(this.Owner.Location, new Vector3(distance / 2));
            boundingBox.Max = Vector3.Subtract(this.Owner.Location, new Vector3(-distance / 2));

            Vector3 P1 = boundingBox.Min;
            Vector3 P2 = boundingBox.Max;

            Vector3 P3 = new Vector3(P1.X + distance, P1.Y, P1.Z);
            Vector3 P4 = new Vector3(P1.X, P1.Y, P1.Z + distance);
            Vector3 P5 = new Vector3(P2.X, P2.Y - distance, P2.Z);
            Vector3 P6 = new Vector3(P1.X, P1.Y + distance, P1.Z);
            Vector3 P7 = new Vector3(P2.X, P2.Y, P2.Z - distance);
            Vector3 P8 = new Vector3(P2.X - distance, P2.Y, P2.Z);

            frontFace = new Face(P1, P3, P6, P7);
            backFace = new Face(P2, P4, P5, P8);
            leftFace = new Face(P1, P4, P6, P8);
            rightFace = new Face(P2, P3, P5, P7);
            topFace = new Face(P2, P6, P7, P8);
            botFace = new Face(P1, P3, P4, P5);
        }
    }
}
