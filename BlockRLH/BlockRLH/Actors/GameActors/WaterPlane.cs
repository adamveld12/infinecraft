using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockRLH.Components;

namespace BlockRLH.Actors.GameActors
{
    public class WaterPlane : Actor
    {
        Texture2D waterBumpMap;

        public float Y_INCREASE = .2f, INCREASE_TICK = 100;

        private VertexPositionNormalTexture[] vertices;
        private Texture2D texture;
        private float currentY;
        private Effect effect;
        private double nextTimeIncrease;

        public WaterPlane(WorldInfo info)
            : base(info)
        {
            LoadContent();
        }

        protected void LoadContent()
        {
            var content = WorldInfo.Engine.Content;
            texture = content.Load<Texture2D>(@"Water/water");
            waterBumpMap = content.Load<Texture2D>(@"Water/waterbump");
            effect = content.Load<Effect>(@"Effects/Water");

            currentY = -Block.SIZE * 10;
            RebuildPlane();
        }

        protected override void Tick(GameTime delta)
        {
            if (delta.TotalGameTime.TotalMilliseconds >= nextTimeIncrease)
            {
                nextTimeIncrease = delta.TotalGameTime.TotalMilliseconds + INCREASE_TICK;
                currentY += Y_INCREASE;
                RebuildPlane();
            }
        }

        private void RebuildPlane()
        {
            var width = (int)(WorldInfo.WorldDimension.X) * 10;
            var height = (int)(WorldInfo.WorldDimension.Y) * 10;
            location = new Vector3(-width / 2, currentY, -height / 2);
            vertices = new VertexPositionNormalTexture[6];
            Vector3 topLeft = new Vector3(-width / 2, currentY, -height / 2);
            Vector3 topRight = new Vector3(width / 2, currentY, -height / 2);
            Vector3 bottomLeft = new Vector3(-width / 2, currentY, height / 2);
            Vector3 bottomRight = new Vector3(width / 2, currentY, height / 2);
            float tile = 40.0f;
            vertices[0] = new VertexPositionNormalTexture(topLeft, Vector3.Up, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(topRight, Vector3.Up, new Vector2(1*tile, 0));
            vertices[2] = new VertexPositionNormalTexture(bottomLeft, Vector3.Up, new Vector2(0, 1 * tile));
            vertices[3] = new VertexPositionNormalTexture(bottomLeft, Vector3.Up, new Vector2(0, 1 * tile));
            vertices[4] = new VertexPositionNormalTexture(topRight, Vector3.Up, new Vector2(1 * tile, 0));
            vertices[5] = new VertexPositionNormalTexture(bottomRight, Vector3.Up, new Vector2(1 * tile, 1 * tile));
        }

        public void Draw(GameTime time)
        {
            Camera camera = WorldInfo.GameInfo.GetPlayer(PlayerIndex.One).Camera;
            effect.CurrentTechnique = effect.Techniques["Water"];
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["waveLength"].SetValue(0.1f);
            effect.Parameters["waveHeight"].SetValue(0.3f);
            effect.Parameters["texture1"].SetValue(texture);
            effect.Parameters["waterBumpMap"].SetValue(waterBumpMap);
            effect.Parameters["time"].SetValue((float)time.TotalGameTime.TotalSeconds);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                WorldInfo.Engine.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, 2);
            }
        }
    }
}
