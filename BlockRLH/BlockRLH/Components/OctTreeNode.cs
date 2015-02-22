using System.Collections.Generic;
using BlockRLH.Actors;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Components
{
    /// <summary>
    /// A node in an Octal Tree SceneGraph
    /// </summary>
    public class OctTreeNode
    {
        /// <summary>
        /// Number of meshes that can exist in a voxel before they are distributed
        /// </summary>
        const int MESHES_PER_VOXEL = 45;

        /// <summary>
        /// Smallest size a voxel can be when a parent node decides to distribute to its children
        /// </summary>
        const float MINIMUM_VOXEL_SIZE = 15f;

        public static int modelsDrawn;
        private static int modelsStoredInQuadTree;

        private VertexPositionColor[] verts;

        private Vector3 center;
        private BoundingBox boundingBox;

        private float size;

        List<DrawableActorProxy> drawableActors;
        List<OctTreeNode> children;

        OctTreeNode nodeUFL;
        OctTreeNode nodeUFR;
        OctTreeNode nodeUBL;
        OctTreeNode nodeUBR;

        OctTreeNode nodeDFL;
        OctTreeNode nodeDFR;
        OctTreeNode nodeDBL;
        OctTreeNode nodeDBR;

        /// <summary>
        /// Initializes a new instance of OctTreeNode
        /// </summary>
        public OctTreeNode(Vector3 center, float size)
        {
            this.center = center;
            this.size = size;
            this.drawableActors = new List<DrawableActorProxy>();
            this.children = new List<OctTreeNode>(8);

            Vector3 diagonalVector = new Vector3(size / 2.0f);
            this.boundingBox = new BoundingBox(center - diagonalVector, center + diagonalVector);

            MakeDebugBox();
        }

        /// <summary>
        /// Adds an actor to the tree
        /// </summary>
        /// <param name="actor">The actor to add</param>
        /// <returns>The id of the actor added</returns>
        public int Add(Actor actor)
        {
            RenderComponent meshComponentFound = null;
            CollisionComponent collisionComponentFound = null;

            // first we check if this actor has a mesh component
            foreach (var component in actor.Components)
            {
                if (component is RenderComponent && meshComponentFound == null)
                {
                    meshComponentFound = component as RenderComponent;
                }
                else if (component is CollisionComponent && collisionComponentFound == null)
                {
                    collisionComponentFound = component as CollisionComponent;
                }

                if (meshComponentFound != null && collisionComponentFound != null)
                    break;
            }

            if (meshComponentFound != null && collisionComponentFound != null)
            {
                var drawableActor = new DrawableActorProxy(actor, meshComponentFound, collisionComponentFound);
                modelsStoredInQuadTree++;
                this.AddDrawableActor(drawableActor);
                return drawableActor.ID;
            }
            else 
                return -1;
        }


        //public 

        /// <summary>
        /// Draws all of the actors in this tree to the scene
        /// </summary>
        /// <param name="gameTime">Game timing values</param>
        /// <param name="view">camera look at</param>
        /// <param name="projection">viewport projection</param>
        /// <param name="cameraViewFrustum">camera view frustum</param>
        public void Draw(GameTime gameTime, BoundingFrustum cameraViewFrustum)
        {
            ContainmentType cameraNodeContainment = cameraViewFrustum.Contains(this.boundingBox);

            // check to see if our camera is intersecting with this node's bounding box
            if (cameraNodeContainment != ContainmentType.Disjoint)
            {
                // we do yet another check to make sure we cull out as many as we can
                foreach (var drawable in this.drawableActors)
                    drawable.mesh.Draw(gameTime);

                // now we propagate the draw call down to our child nodes
                foreach (OctTreeNode node in children)
                    node.Draw(gameTime, cameraViewFrustum);
            }
        }

        /// <summary>
        /// Draws a debug view 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="device"></param>
        public void DrawDebug(GameTime gameTime, GraphicsDevice device, BoundingFrustum cameraViewFrustum)
        {
            ContainmentType cameraNodeContainment = cameraViewFrustum.Contains(this.boundingBox);

            // check to see if our camera is intersecting with this node's bounding box
            if (cameraNodeContainment != ContainmentType.Disjoint)
                device.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, verts.Length / 2);
                foreach (var node in children)
                {
                    node.DrawDebug(gameTime, device, cameraViewFrustum);
                }
        }

        /// <summary>
        /// Gets a list of actors that exist in a voxel that the passed in actor currently occupies
        /// </summary>
        /// <param name="actor"></param>
        public DrawableActorProxy[] QueryForLocalActors(Actor actor)
        {
            if (children.Count <= 0)
            {
                return drawableActors.ToArray();
            }
            else
            {
                Vector3 position = actor.Location;

                if (position.Y > center.Y)
                    if (position.Z < center.Z)
                        if (position.X < center.X)
                            return nodeUFL.QueryForLocalActors(actor);
                        else
                            return nodeUFR.QueryForLocalActors(actor);
                    else
                        if (position.X < center.X)
                            return nodeUBL.QueryForLocalActors(actor);
                        else
                            return nodeUBR.QueryForLocalActors(actor);
                else
                    if (position.Z < center.Z)
                        if (position.X < center.X)
                            return nodeDFL.QueryForLocalActors(actor);
                        else
                            return nodeDFR.QueryForLocalActors(actor);
                    else
                        if (position.X < center.X)
                            return nodeDBL.QueryForLocalActors(actor);
                        else
                            return nodeDBR.QueryForLocalActors(actor);
            }
        }


        #region Private Parts

        private void MakeDebugBox()
        {
            var corners = this.boundingBox.GetCorners();

            this.verts = new VertexPositionColor[24];

            verts[0] = new VertexPositionColor(corners[0], Color.Red);
            verts[1] = new VertexPositionColor(corners[1], Color.Blue);


            verts[2] = new VertexPositionColor(corners[1], Color.Cyan);
            verts[3] = new VertexPositionColor(corners[2], Color.Green);


            verts[4] = new VertexPositionColor(corners[2], Color.Yellow);
            verts[5] = new VertexPositionColor(corners[3], Color.Purple);


            verts[6] = new VertexPositionColor(corners[0], Color.Cyan);
            verts[7] = new VertexPositionColor(corners[3], Color.Cyan);


            verts[8] = new VertexPositionColor(corners[0], Color.Cyan);
            verts[9] = new VertexPositionColor(corners[4], Color.Cyan);

            verts[10] = new VertexPositionColor(corners[4], Color.Cyan);
            verts[11] = new VertexPositionColor(corners[5], Color.Cyan);

            verts[12] = new VertexPositionColor(corners[5], Color.Cyan);
            verts[13] = new VertexPositionColor(corners[1], Color.Cyan);


            verts[14] = new VertexPositionColor(corners[5], Color.Cyan);
            verts[15] = new VertexPositionColor(corners[6], Color.Cyan);


            verts[16] = new VertexPositionColor(corners[6], Color.Cyan);
            verts[17] = new VertexPositionColor(corners[7], Color.Cyan);

            verts[18] = new VertexPositionColor(corners[7], Color.Cyan);
            verts[19] = new VertexPositionColor(corners[4], Color.Cyan);

            verts[20] = new VertexPositionColor(corners[6], Color.Cyan);
            verts[21] = new VertexPositionColor(corners[2], Color.Cyan);

            verts[22] = new VertexPositionColor(corners[3], Color.Cyan);
            verts[23] = new VertexPositionColor(corners[7], Color.Cyan);
        }

        /// <summary>
        /// Creates the child nodes for this node
        /// </summary>
        private void CreateChildNodes()
        {
            float sizeOver2 = size / 2.0f;
            float sizeOver4 = size / 4.0f;

            nodeUFR = new OctTreeNode(center + new Vector3(sizeOver4, sizeOver4, -sizeOver4), sizeOver2);
            nodeUFL = new OctTreeNode(center + new Vector3(-sizeOver4, sizeOver4, -sizeOver4), sizeOver2);
            nodeUBR = new OctTreeNode(center + new Vector3(sizeOver4, sizeOver4, sizeOver4), sizeOver2);
            nodeUBL = new OctTreeNode(center + new Vector3(-sizeOver4, sizeOver4, sizeOver4), sizeOver2);

            nodeDFR = new OctTreeNode(center + new Vector3(sizeOver4, -sizeOver4, -sizeOver4), sizeOver2);
            nodeDFL = new OctTreeNode(center + new Vector3(-sizeOver4, -sizeOver4, -sizeOver4), sizeOver2);
            nodeDBR = new OctTreeNode(center + new Vector3(sizeOver4, -sizeOver4, sizeOver4), sizeOver2);
            nodeDBL = new OctTreeNode(center + new Vector3(-sizeOver4, -sizeOver4, sizeOver4), sizeOver2);

            this.children.Add(nodeUFR);
            this.children.Add(nodeUFL);
            this.children.Add(nodeUBR);
            this.children.Add(nodeUBL);

            this.children.Add(nodeDFR);
            this.children.Add(nodeDFL);
            this.children.Add(nodeDBR);
            this.children.Add(nodeDBL);
        }

        /// <summary>
        /// Adds an actor to our octal tree
        /// </summary>
        private void AddDrawableActor(DrawableActorProxy actor)
        {
            // if this node doesnt have any children
            if (children.Count == 0)
            {
                // we add the actor to the list
                this.drawableActors.Add(actor);

                bool maxObjectsReached = (drawableActors.Count > MESHES_PER_VOXEL);
                bool minSizeNotReached = (size > MINIMUM_VOXEL_SIZE);

                // if our constraints have been broken...
                if (maxObjectsReached && minSizeNotReached)
                {
                    // we expand our tree
                    CreateChildNodes();

                    // then we distribute every actor
                    foreach (var CurrentActor in drawableActors)
                        DistributeActor(CurrentActor);

                    // clear our list since all of the actors are now in children nodes
                    drawableActors.Clear();
                }
            }
            else
                DistributeActor(actor);
        }

        /// <summary>
        /// Distributes the drawable actor to one of the nodes
        /// </summary>
        private void DistributeActor(DrawableActorProxy actor)
        {
            Vector3 position = actor.Location;

            if (position.Y > center.Y)
                if (position.Z < center.Z)
                    if (position.X < center.X)
                        nodeUFL.AddDrawableActor(actor);
                    else
                        nodeUFR.AddDrawableActor(actor);
                else
                    if (position.X < center.X)
                        nodeUBL.AddDrawableActor(actor);
                    else
                        nodeUBR.AddDrawableActor(actor);
            else
                if (position.Z < center.Z)
                    if (position.X < center.X)
                        nodeDFL.AddDrawableActor(actor);
                    else
                        nodeDFR.AddDrawableActor(actor);
                else
                    if (position.X < center.X)
                        nodeDBL.AddDrawableActor(actor);
                    else
                        nodeDBR.AddDrawableActor(actor);
        }

        #endregion

    }
}
