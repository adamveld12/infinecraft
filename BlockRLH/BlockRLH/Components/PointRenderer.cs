using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BlockRLH.Actors.GameActors;

namespace BlockRLH.Components
{
    public static class PointRenderer
    {
        public static void DrawPoints(GraphicsDevice gd, Block b)
        {
            gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, b.GetPoints(), 0, 5);
        }
    }
}
