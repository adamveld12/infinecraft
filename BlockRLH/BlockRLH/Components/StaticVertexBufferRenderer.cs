using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockRLH.Actors.GameActors;
using BlockRLH.Actors;

namespace BlockRLH.Components
{
    public class StaticVertexBufferRenderer : IDisposable
    {
        const int VERTS_PER_BATCH = 1500 * 24;
        private GraphicsDevice device;
        private VertexBuffer cubeVertexBuffer;
        private IndexBuffer cubeIndexBuffer;
        private Texture2D cubeTexture;
        private Effect shader;

        public StaticVertexBufferRenderer(GraphicsDevice device, Block[] blocks, Texture2D cubeTexture, Effect shader)
        {
            this.cubeTexture = cubeTexture;
            this.shader = shader;
            this.device = device;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[blocks.Length * 24];
            short[] indices = new short[blocks.Length * 36];

            // Fill in our buffers with data
            for (int i = 0; i < blocks.Length; i++)
            {
                //Vertex buffers
                for (int vertIndex = 0; vertIndex < 24; vertIndex += 4)
                {
                    Vector2 uvTopPosition = Vector2.Zero;
                    Vector2 uvSidePosition = Vector2.Zero;

                    float uvSize = (1f / 16);

                    switch(blocks[i].Type)
                    {
                        case BlockType.Grass:
                            uvTopPosition = Vector2.Multiply(new Vector2(2, 9),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(3, 0), uvSize);
                        break;
                        case BlockType.Sand:
                            uvTopPosition = Vector2.Multiply(new Vector2(2, 1),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(2, 1), uvSize);
                        break;
                        case BlockType.Concrete:
                            uvTopPosition = Vector2.Multiply(new Vector2(0, 0),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(1, 0), uvSize);
                        break;
                        case BlockType.Brick:
                            uvTopPosition = Vector2.Multiply(new Vector2(7, 0),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(7, 0), uvSize);
                        break;
                        case BlockType.Wall:
                            uvTopPosition = Vector2.Multiply(new Vector2(1, 1),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(0, 1), uvSize);
                        break;
                        case BlockType.Wood:
                            uvTopPosition = Vector2.Multiply(new Vector2(5, 1),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(4, 1), uvSize);
                        break;
                        case BlockType.WoodFloor:
                            uvTopPosition = Vector2.Multiply(new Vector2(4, 0),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(4, 0), uvSize);
                        break;
                        case BlockType.Goal:
                            uvTopPosition = Vector2.Multiply(new Vector2(7, 1),uvSize);
                            uvSidePosition = Vector2.Multiply(new Vector2(7, 1), uvSize);
                        break;
                    }
                    if (vertIndex > 7 && vertIndex < 16)
                    {
                        VertexPositionNormalTexture vert = Block.CubeVerts[vertIndex];
                        vert.TextureCoordinate = uvTopPosition;
                        vertices[i * 24 + vertIndex] = vert;
                        vertices[i * 24 + vertIndex].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 1];
                        vert.TextureCoordinate = new Vector2(uvTopPosition.X + uvSize, uvTopPosition.Y);
                        vertices[i * 24 + vertIndex + 1] = vert;
                        vertices[i * 24 + vertIndex + 1].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 2];
                        vert.TextureCoordinate = new Vector2(uvTopPosition.X, uvTopPosition.Y + uvSize);
                        vertices[i * 24 + vertIndex + 2] = vert;
                        vertices[i * 24 + vertIndex + 2].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 3];
                        vert.TextureCoordinate = new Vector2(uvTopPosition.X + uvSize, uvTopPosition.Y + uvSize);
                        vertices[i * 24 + vertIndex + 3] = vert;
                        vertices[i * 24 + vertIndex + 3].Position += blocks[i].Location;
                    }
                    else
                    {
                        VertexPositionNormalTexture vert = Block.CubeVerts[vertIndex];
                        vert.TextureCoordinate = new Vector2(uvSidePosition.X + uvSize, uvSidePosition.Y);
                        vertices[i * 24 + vertIndex] = vert;
                        vertices[i * 24 + vertIndex].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 1];

                        vert.TextureCoordinate = new Vector2(uvSidePosition.X + uvSize, uvSidePosition.Y + uvSize);
                        vertices[i * 24 + vertIndex + 1] = vert;
                        vertices[i * 24 + vertIndex + 1].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 2];
                        vert.TextureCoordinate = uvSidePosition;
                        vertices[i * 24 + vertIndex + 2] = vert;
                        vertices[i * 24 + vertIndex + 2].Position += blocks[i].Location;

                        vert = Block.CubeVerts[vertIndex + 3];

                        vert.TextureCoordinate = new Vector2(uvSidePosition.X, uvSidePosition.Y + uvSize);
                        vertices[i * 24 + vertIndex + 3] = vert;
                        vertices[i * 24 + vertIndex + 3].Position += blocks[i].Location;
                    }


                    //var matrix = Matrix.CreateWorld(blocks[i].Location, Vector3.Forward, Vector3.Up);
                    //vertices[i * 24 + vertIndex].Normal = Vector3.Transform(vertices[i * 24 + vertIndex].Normal, matrix);
                    //vertices[i * 24 + vertIndex + 1].Normal = Vector3.Transform(vertices[i * 24 + vertIndex + 1].Normal, matrix);
                    //vertices[i * 24 + vertIndex + 2].Normal = Vector3.Transform(vertices[i * 24 + vertIndex + 2].Normal, matrix);
                    //vertices[i * 24 + vertIndex + 3].Normal = Vector3.Transform(vertices[i * 24 + vertIndex + 3].Normal, matrix);
                }

                //Index buffers
                for (int indiceIndex = 0; indiceIndex < 36; indiceIndex++)
                {
                    indices[i * 36 + indiceIndex] = (short)(Block.Indices[indiceIndex] +  i * 24);
                }
            }

            cubeVertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), blocks.Length * 24, BufferUsage.WriteOnly);
            cubeIndexBuffer = new IndexBuffer(device, typeof(short), blocks.Length * 36, BufferUsage.WriteOnly);

            cubeVertexBuffer.SetData(vertices);
            cubeIndexBuffer.SetData(indices);

            shader.Parameters["Texture"].SetValue(cubeTexture);
        }

        public void Render(Camera camera)
        {
            device.SetVertexBuffers(new VertexBufferBinding(cubeVertexBuffer, 0, 0));
            device.Indices = cubeIndexBuffer;
            device.DepthStencilState = DepthStencilState.Default;

            shader.Parameters["View"].SetValue(camera.View);
            shader.Parameters["Projection"].SetValue(camera.Projection);

            int startingVert = 0;
            int endVert = startingVert + VERTS_PER_BATCH;

            while (startingVert < cubeVertexBuffer.VertexCount)
            {
                foreach (var pass in shader.CurrentTechnique.Passes)
                {
                    pass.Apply();

                   // if(startingVert + VERTS_PER_BATCH < cubeVertexBuffer.VertexCount)
                        this.device.DrawIndexedPrimitives(PrimitiveType.TriangleList, startingVert % cubeVertexBuffer.VertexCount, 0, cubeVertexBuffer.VertexCount, 0, cubeIndexBuffer.IndexCount / 3);

                }
                startingVert = (startingVert + VERTS_PER_BATCH);
            }
        }

        public void Render(Matrix camera)
        {
            device.SetVertexBuffers(new VertexBufferBinding(cubeVertexBuffer, 0, 0));
            device.Indices = cubeIndexBuffer;
            device.DepthStencilState = DepthStencilState.Default;

            shader.Parameters["ViewProjection"].SetValue(camera);

            int startingVert = 0;
            int endVert = startingVert + VERTS_PER_BATCH;

            while (startingVert < cubeVertexBuffer.VertexCount)
            {
                foreach (var pass in shader.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // if(startingVert + VERTS_PER_BATCH < cubeVertexBuffer.VertexCount)
                    this.device.DrawIndexedPrimitives(PrimitiveType.TriangleList, startingVert % cubeVertexBuffer.VertexCount, 0, cubeVertexBuffer.VertexCount, 0, cubeIndexBuffer.IndexCount / 3);

                }
                startingVert = (startingVert + VERTS_PER_BATCH);
            }
        }

        /// <summary>
        /// Disposes the render
        /// </summary>
        public void Dispose()
        {
            this.cubeIndexBuffer.Dispose();
            this.cubeIndexBuffer.Dispose();
        }
    }
}
