using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using BlockRLH.Actors.GameActors;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.TerrainGeneration
{
    public class SimpleTerrain
    {
        public static int SEED = 42;

        public void build(WorldInfo info)
        {
            var width = (int)(info.WorldDimension.X / InstancedBoxRenderer.SIZE);

            var height = (int)(info.WorldDimension.Y / InstancedBoxRenderer.SIZE);
            var depth = (int)(info.WorldDimension.Z / InstancedBoxRenderer.SIZE);
            for (int x = 0; x < width; x++)
            {
                int worldX = x * (int)InstancedBoxRenderer.SIZE;

                for (int z = 0; z < height; z++)
                {
                    int worldZ = z * (int)InstancedBoxRenderer.SIZE;
                    generateTerrain(info, worldX, worldZ, width, height, depth);
                }
            }

        }

        //protected virtual void generateTerrain(WorldInfo info, float blockXInWorld, float blockZInWorld, int worldWidth, int worldHeight, int worldDepth)
        //{
        //    // The lower ground level is at least this high.
        //    int minimumGroundheight = worldDepth / 4;
        //    int minimumGroundDepth = (int)(worldDepth * 0.75f);

        //    float octave1 = PerlinSimplexNoise.noise(worldWidth * 0.0001f, worldHeight * 0.0001f) * 0.5f;
        //    float octave2 = PerlinSimplexNoise.noise(worldWidth * 0.0005f, worldHeight * 0.0005f) * 0.25f;
        //    float octave3 = PerlinSimplexNoise.noise(worldWidth * 0.005f, worldHeight * 0.005f) * 0.12f;
        //    float octave4 = PerlinSimplexNoise.noise(worldWidth * 0.01f, worldHeight * 0.01f) * 0.12f;
        //    float octave5 = PerlinSimplexNoise.noise(worldWidth * 0.03f, worldHeight * 0.03f) * octave4;
        //    float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;
        //    lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;
        //    for (int y = worldDepth - 1; y >= 0; y--)
        //    {
        //        var block = new Block(info);
        //        block.Location = new Vector3() { X = blockXInWorld, Y = y * InstancedBoxRenderer.SIZE, Z = blockZInWorld };

        //        (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();

        //        info.AddActor(block);
        //        // Debug.WriteLine(string.Format("chunk {0} : ({1},{2},{3})={4}", chunk.Position, blockXInChunk, y, blockZInChunk, blockType));

        //    }
        //}

        protected virtual void generateTerrain(WorldInfo info, int blockX, int blockZ, int worldWidth, int worldHeight, int worldDepth)
        {
            int groundHeight = (int)GetBlockNoise(blockX, blockZ);
            if (groundHeight < 1)
            {
                groundHeight = 1;
            }
            else if (groundHeight > 128)
            {
                groundHeight = 96;
            }

            // Default to sunlit.. for caves
            bool sunlit = true;
            for (int y = worldDepth - 1; y > 0; y--)
            {
                if (y < groundHeight)
                {
                    // Since we are at or below ground height, let's see if we need
                    // to make
                    // a cave
                    int noiseX = (blockX + SEED);
                    float octave1 = PerlinSimplexNoise.noise(noiseX * 0.009f, blockZ * 0.009f, y * 0.009f) * 0.25f;

                    float initialNoise = octave1 + PerlinSimplexNoise.noise(noiseX * 0.04f, blockZ * 0.04f, y * 0.04f) * 0.15f;
                    initialNoise += PerlinSimplexNoise.noise(noiseX * 0.08f, blockZ * 0.08f, y * 0.08f) * 0.05f;
                    if (initialNoise <= 0.2f)
                    {
                        AddBlock(new Vector3(blockX, y * InstancedBoxRenderer.SIZE, blockZ), info);
                    }
                }
            }
        }

        private float GetBlockNoise(int blockX, int blockZ)
        {
            float mediumDetail = PerlinSimplexNoise.noise(blockX / 300.0f, blockZ / 300.0f, 20);
            float fineDetail = PerlinSimplexNoise.noise(blockX / 80.0f, blockZ / 80.0f, 30);
            float bigDetails = PerlinSimplexNoise.noise(blockX / 800.0f, blockZ / 800.0f);
            float noise = bigDetails * 64.0f + mediumDetail * 32.0f + fineDetail * 16.0f; // *(bigDetails
            // *
            // 64.0f);
            return noise + 16;
        }

        public void AddBlock(Vector3 placementVector, WorldInfo info)
        {
            var block = new Block(info);
            block.Location = placementVector;

            (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();

            info.AddActor(block);
            // Debug.WriteLine(string.Format("chunk {0} : ({1},{2},{3})={4}", chunk.Position, blockXInChunk, y, blockZInChunk, blockType));
        }
    }
}
