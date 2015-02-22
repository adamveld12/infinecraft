using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework;
using BlockRLH.Components;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Actors.GameActors
{
    /// <summary>
    /// A cube used to build terrain
    /// </summary>
    public class Block : Actor
    {
        public const float SIZE = 10;
        public BlockType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of Block
        /// </summary>
        /// <param name="info"></param>
        public Block(WorldInfo info) : this(info,BlockType.Grass)
        {
        }

        public Block(WorldInfo info, BlockType blockType) : base(info)
        {
            var collisionMesh = new CubeCollisionComponent(this, InstancedBoxRenderer.SIZE);
            var meshComponent = new InstancedBoxComponent(this);
            this.AddComponent("Collision", collisionMesh);
            this.AddComponent("Mesh", meshComponent);
            Type = blockType;
        }

        public void PlaceInWorld(Vector3 position)
        {
            PlaceInWorld(position.X, position.Y, position.Z);
        }

        public virtual void PlaceInWorld(float x, float y, float z)
        {
            Location = new Vector3() { X = x * InstancedBoxRenderer.SIZE, Y = z * InstancedBoxRenderer.SIZE, Z = y * InstancedBoxRenderer.SIZE };
        }

        public void InitCollision()
        {
            (GetComponent("Collision") as CubeCollisionComponent).UpdateCollisionPrimitive();
        }

        public static readonly VertexPositionNormalTexture[] CubeVerts;
        public static readonly short[] Indices;

        static Block()
        {
            CubeVerts = new VertexPositionNormalTexture[24];
            Vector3 shapePosition = Vector3.Zero;

            Vector3 topLeftFront =
                new Vector3(-1.0f, 1.0f, -1.0f) * SIZE / 2;
            Vector3 bottomLeftFront =
                new Vector3(-1.0f, -1.0f, -1.0f) * SIZE / 2;
            Vector3 topRightFront =
                new Vector3(1.0f, 1.0f, -1.0f) * SIZE / 2;
            Vector3 bottomRightFront =
                new Vector3(1.0f, -1.0f, -1.0f) * SIZE / 2;
            Vector3 topLeftBack =
                new Vector3(-1.0f, 1.0f, 1.0f) * SIZE / 2;
            Vector3 topRightBack = shapePosition +
                new Vector3(1.0f, 1.0f, 1.0f) * SIZE / 2;
            Vector3 bottomLeftBack =
                new Vector3(-1.0f, -1.0f, 1.0f) * SIZE / 2;
            Vector3 bottomRightBack =
                new Vector3(1.0f, -1.0f, 1.0f) * SIZE / 2;

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f) * SIZE / 2;
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f) * SIZE / 2;
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f) * SIZE / 2;
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f) * SIZE / 2;
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f) * SIZE / 2;
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f) * SIZE / 2;

            Vector2 textureTopLeft = new Vector2(1f , 0.0f);
            Vector2 textureTopRight = new Vector2(0.0f , 0.0f );
            Vector2 textureBottomLeft = new Vector2(1f , 1f );
            Vector2 textureBottomRight = new Vector2(0.0f, 1f );

            //            _________________________
            //           /4_____________________ 5/|
            //          / / ___________________/ / |
            //         / / /| |               / /  |
            //        / / / | |              / / . |
            //       / / /| | |             / / /| |
            //      / / / | | |            / / / | |
            //     / / /  | | |           / / /| | |
            //    / /_/__________________/ / / | | |
            //   /________________________/ /  | | |
            //   |0______________________1| |  | | |
            //   | | |    | | |_________| | |__| | |
            //   | | |    | |___________| | |____| |
            //   | | |   / /6___________| | |_ 7/ /
            //   | | |  / / /           | | |/ / /
            //   | | | / / /            | | | / /
            //   | | |/ / /             | | |/ /
            //   | | | / /              | | ' /
            //   | | |/_/_______________| |  /
            //   | |____________________| | /
            //   |2______________________3|/
            //    Front face.

            CubeVerts[0] = new VertexPositionNormalTexture(
                topLeftFront, frontNormal, textureTopLeft);
            CubeVerts[1] = new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            CubeVerts[2] = new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);
            CubeVerts[3] = new VertexPositionNormalTexture(
                bottomRightFront, frontNormal, textureBottomRight);

            // Back face.
            CubeVerts[4] = new VertexPositionNormalTexture(
                topLeftBack, backNormal, textureTopLeft);
            CubeVerts[5] = new VertexPositionNormalTexture(
                bottomLeftBack, backNormal, textureBottomLeft);
            CubeVerts[6] = new VertexPositionNormalTexture(
                topRightBack, backNormal, textureTopRight);
            CubeVerts[7] = new VertexPositionNormalTexture(
                bottomRightBack, backNormal, textureBottomRight);

            // Top face.
            CubeVerts[8] = new VertexPositionNormalTexture(
                topLeftBack, topNormal, textureTopLeft);
            CubeVerts[9] = new VertexPositionNormalTexture(
                topLeftFront, topNormal, textureBottomLeft);
            CubeVerts[10] = new VertexPositionNormalTexture(
                topRightBack, topNormal, textureTopRight);
            CubeVerts[11] = new VertexPositionNormalTexture(
                topRightFront, topNormal, textureBottomRight);

            // Bottom face. 
            CubeVerts[12] = new VertexPositionNormalTexture(
                bottomLeftBack, bottomNormal, textureTopLeft);
            CubeVerts[13] = new VertexPositionNormalTexture(
                bottomLeftFront, bottomNormal, textureBottomLeft);
            CubeVerts[14] = new VertexPositionNormalTexture(
                bottomRightBack, bottomNormal, textureTopRight);
            CubeVerts[15] = new VertexPositionNormalTexture(
                bottomRightFront, bottomNormal, textureBottomRight);

            // Left face.
            CubeVerts[16] = new VertexPositionNormalTexture(
                topLeftBack, leftNormal, textureTopLeft);
            CubeVerts[17] = new VertexPositionNormalTexture(
                bottomLeftBack, leftNormal, textureBottomLeft);
            CubeVerts[18] = new VertexPositionNormalTexture(
                topLeftFront, leftNormal, textureTopRight);
            CubeVerts[19] = new VertexPositionNormalTexture(
                bottomLeftFront, leftNormal, textureBottomRight);

            // Right face. 
            CubeVerts[20] = new VertexPositionNormalTexture(
                topRightBack, rightNormal, textureTopLeft);
            CubeVerts[21] = new VertexPositionNormalTexture(
                bottomRightBack, rightNormal, textureBottomLeft);
            CubeVerts[22] = new VertexPositionNormalTexture(
                topRightFront, rightNormal, textureTopRight);
            CubeVerts[23] = new VertexPositionNormalTexture(
                bottomRightFront, rightNormal, textureBottomRight);

            Indices = new short[36];

            // Front
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 1;
            Indices[4] = 3;
            Indices[5] = 2;

            // Back
            Indices[6] = 4;
            Indices[7] = 6;
            Indices[8] = 5;
            Indices[9] = 5;
            Indices[10] = 6;
            Indices[11] = 7;

            // Top
            Indices[12] = 8;
            Indices[13] = 9;
            Indices[14] = 10;
            Indices[15] = 9;
            Indices[16] = 11;
            Indices[17] = 10;

            // Bottom
            Indices[18] = 13;
            Indices[19] = 12;
            Indices[20] = 14;
            Indices[21] = 13;
            Indices[22] = 14;
            Indices[23] = 15;

            // Left
            Indices[24] = 18;
            Indices[25] = 17;
            Indices[26] = 19;
            Indices[27] = 16;
            Indices[28] = 17;
            Indices[29] = 18;

            // Right
            Indices[30] = 22;
            Indices[31] = 23;
            Indices[32] = 21;
            Indices[33] = 20;
            Indices[34] = 22;
            Indices[35] = 21;
        }

        public VertexPositionColor[] GetPoints()
        {
            VertexPositionColor[] points = new VertexPositionColor[6];
            CubeCollisionComponent cc = GetComponent("Collision") as CubeCollisionComponent;

            points[0] = new VertexPositionColor(cc.topFace.Center, Color.White);
            points[1] = new VertexPositionColor(cc.botFace.Center, Color.White);
            points[2] = new VertexPositionColor(cc.leftFace.Center, Color.White);
            points[3] = new VertexPositionColor(cc.rightFace.Center, Color.White);
            points[4] = new VertexPositionColor(cc.frontFace.Center, Color.White);
            points[5] = new VertexPositionColor(cc.backFace.Center, Color.White);

            return points;
        }

        
    }

    public enum BlockType
    {
        Grass, Sand, Concrete, Wall, Wood, WoodFloor, Brick, Goal
    }
}
