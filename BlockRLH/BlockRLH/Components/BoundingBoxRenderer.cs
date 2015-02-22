using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Components
{
     public static class BoundingBoxRenderer
    {
        #region Fields
 
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static int[] indices = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };
 
        static BasicEffect effect;
        static VertexDeclaration vertDecl;
 
        #endregion
 
        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Render(
            BoundingBox box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
                vertDecl = new VertexDeclaration(VertexPositionColor.VertexDeclaration.GetVertexElements());
            }
 
            Vector3[] corners = box.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }
 
            //graphicsDevice.VertexDeclaration = vertDecl;
 
            effect.View = view;
            effect.Projection = projection;
 
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    verts,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);
            }
        }
    }
}
