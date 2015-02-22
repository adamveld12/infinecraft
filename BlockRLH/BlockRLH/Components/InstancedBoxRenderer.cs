using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BlockRLH.Actors;

namespace BlockRLH.Components
{
    public class InstancedBoxRenderer
    {
        public const float SIZE = 10;

        public static Matrix LightDirection = Matrix.Identity;
         // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        public static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public static readonly VertexPositionNormalTexture[] CubeVerts;
        public static readonly short[] Indices;

        static InstancedBoxRenderer()
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

            Vector2 textureTopLeft = new Vector2(0.5f * SIZE / 2, 0.0f * SIZE / 2);
            Vector2 textureTopRight = new Vector2(0.0f * SIZE / 2, 0.0f * SIZE / 2);
            Vector2 textureBottomLeft = new Vector2(0.5f * SIZE / 2, 0.5f * SIZE / 2);
            Vector2 textureBottomRight = new Vector2(0.0f * SIZE / 2, 0.5f * SIZE / 2);
            //        /4_____________________ 5/|
            //       / / ___________________/ / |
            //      / / /| |               / /  |
            //     / / / | |              / / . |
            //    / / /| | |             / / /| |
            //   / / / | | |            / / / | |
            //  / / /  | | |           / / /| | |
            // / /_/__________________/ / / | | |
            ///________________________/ /  | | |
            //|0______________________1| |  | | |
            //| | |    | | |_________| | |__| | |
            //| | |    | |___________| | |____| |
            //| | |   / /6___________| | |_ 7/ /
            //| | |  / / /           | | |/ / /
            //| | | / / /            | | | / /
            //| | |/ / /             | | |/ /
            //| | | / /              | | ' /
            //| | |/_/_______________| |  /
            //| |____________________| | /
            //|2______________________3|/
            // Front face.
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

        DynamicVertexBuffer locationBuffer;
        VertexBuffer cubeMesh;
        IndexBuffer indices;

        private GraphicsDevice gd;
        private Effect effect;
        private float rotation = 0;

        public GraphicsDevice GraphicsDevice
        {
            get { return gd; }
            set { gd = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of InstancedBoxRenderer
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public InstancedBoxRenderer(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.gd = graphicsDevice;
            effect = content.Load<Effect>(@"Effects/Instanced");
            effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

            cubeMesh = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, CubeVerts.Length, BufferUsage.WriteOnly);
            cubeMesh.SetData(CubeVerts);

            indices = new IndexBuffer(graphicsDevice, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            indices.SetData(Indices);
        }

        public virtual void Draw(Matrix[] locations, Texture2D textureAtlas, Camera camera)
        {
            rotation += .06f;
            if (locations.Length > 0)
            {
                LightDirection = Matrix.CreateRotationY(rotation);
                // set up our location buffer
                if (locationBuffer == null || locationBuffer.VertexCount != locations.Length)
                {
                    if (locationBuffer != null)
                        locationBuffer.Dispose();
                    this.locationBuffer = new DynamicVertexBuffer(this.GraphicsDevice, instanceVertexDeclaration, locations.Length, BufferUsage.WriteOnly);
                }

                locationBuffer.SetData(locations, 0, locations.Length, SetDataOptions.Discard);

                GraphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(cubeMesh, 0, 0),
                    new VertexBufferBinding(locationBuffer, 0, 1)
                    );

                GraphicsDevice.Indices = indices;

                effect.Parameters["ProjectionView"].SetValue(camera.View * camera.Projection);
                effect.Parameters["World"].SetValue(Matrix.Identity);
                effect.Parameters["LightingDirection"].SetValue(LightDirection);
                effect.Parameters["Texture"].SetValue(textureAtlas);

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, Indices.Length / 3, locations.Length);
                }
            }
        }
    }
}
