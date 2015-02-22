using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace BlockRLH.Actors
{
    class DistanceMeter
    {
        Texture2D redBox;
        Texture2D yellowBox;
        Texture2D greenBox;

        int width = 20;
        int height = 20;

        public DistanceMeter(ContentManager Content)
        {
            redBox = Content.Load<Texture2D>("redbox");
            yellowBox = Content.Load<Texture2D>("yellowbox");
            greenBox = Content.Load<Texture2D>("greenbox");
        }
        
        
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, int distanceFromObjective, string text, Vector2 startingPosition)
        {
            int xCoordMeter = (int)startingPosition.X;
            int yCoordMeter = (int)startingPosition.Y;
            if (spriteFont != null)
            {
                Vector2 position = new Vector2(xCoordMeter, yCoordMeter);
                spriteBatch.DrawString(spriteFont, text, position, Color.White);
                xCoordMeter = xCoordMeter + (int)spriteFont.MeasureString(text).X + 3;
                yCoordMeter = (int)position.Y + (int)spriteFont.MeasureString(text).Y/2;
            }
            Rectangle loc = new Rectangle(xCoordMeter, yCoordMeter, width, height);

            int numberOfBlocks = distanceFromObjective / 5;
            Texture2D currentTexture = null;

            if (distanceFromObjective > 6 && distanceFromObjective < 16)
                currentTexture = yellowBox;
            else if (distanceFromObjective < 7)
                currentTexture = redBox;
            else
                currentTexture = greenBox;

            for (int i = 0; i < numberOfBlocks; i++)
            {
                loc.X = Something(i, xCoordMeter, width);
                spriteBatch.Draw(currentTexture, loc, Color.White);
            }

            if (numberOfBlocks == 0)
            {
                spriteBatch.Draw(currentTexture, loc, Color.White);
            }
        }

        private int Something(int itr, int startLoc, int currentWidth)
        {
            return startLoc + (currentWidth * itr + itr);
        }
    }
}
