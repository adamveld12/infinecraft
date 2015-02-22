using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework;
using BlockRLH.Actors.GameActors;
using BlockRLH.Components.TerrainGeneration;

namespace BlockRLH.Components
{
    /// <summary>
    /// Defines a basic interface for generating block worlds/levels
    /// </summary>
    public interface IFactory
    {
        Block GenerateWorld(WorldInfo info);
    }

    public class EricLevelFactory : IFactory
    {
        public Block GenerateWorld(WorldInfo info)
        {
            // First we find the grid size of the level
            var width = (int)(info.WorldDimension.X / InstancedBoxRenderer.SIZE);
            var height = (int)(info.WorldDimension.Y / InstancedBoxRenderer.SIZE);


            WorldEditor.world = info;

            //next we generate cubes along the width and height
            for (int x = -width / 2; x < width / 2; x++)
            {
                for (int y = -height / 2; y < height / 2; y++)
                {
                    for (int z = 0; z < 1; z++)
                    {
                        var block = new Block(info);

                        block.Location = new Vector3() { X = x * InstancedBoxRenderer.SIZE, Y = z * 1 * InstancedBoxRenderer.SIZE, Z = y * InstancedBoxRenderer.SIZE };

                        (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();

                        info.AddActor(block);
                    }
                }
            }

            return CreateTestLevel(info);
        }

        private Block CreateTestLevel(WorldInfo info)
        {
            //tower 1
            CreatePlatform(3, 3, 1, 1, -9, BlockType.Wall);
            CreatePlatform(3, 3, 2, 1, -9, BlockType.Wall);
            CreatePlatform(3, 3, 3, 1, -9, BlockType.Wall);

            //tower 2
            CreatePlatform(3, 3, 1, 10, -9, BlockType.Wall);
            CreatePlatform(3, 3, 2, 10, -9, BlockType.Wall);
            CreatePlatform(3, 3, 3, 10, -9, BlockType.Wall);

            //stairs
            CreatePlatform(3, 3, 1, 10, -7, BlockType.Wall);
            CreatePlatform(3, 2, 2, 10, -7, BlockType.Wall);

            //bridge between towers
            CreatePlatform(2, 1, 3, 6, -8);

            //little platforms
            CreatePlatform(1, 1, 3, 1, -13);
            CreatePlatform(1, 1, 3, 3, -17);

            //back platform
            CreatePlatform(3, 12, 3, -1, -31, BlockType.Concrete);

            for (int i = -13; i <= 2; i++)
            {
                CreatePlatform(1, 1, i, 0, -28, BlockType.Wall);
                CreatePlatform(1, 1, i, 0, -27, BlockType.Wall);
            }
            

            //little platforms pt 2
            CreatePlatform(1, 1, 3, 6, -28);
            CreatePlatform(1, 1, 3, 10, -28);
            CreatePlatform(1, 1, 4, 13, -28);

            //large tower 1
            for (int i = -13; i < 18; i++)
            {
                CreatePlatform(5, 5, i, 10, -35, BlockType.Concrete);
            }

            for (int i = 20; i < 24; i++)
            {
                CreatePlatform(5, 5, i, 10, -35, BlockType.Concrete);
            }

            //back wall
            CreatePlatform(2, 1, 18, 10, -35, BlockType.Concrete);
            CreatePlatform(2, 1, 19, 10, -35, BlockType.Concrete);

            //front wall
            CreatePlatform(3, 1, 18, 11, -31, BlockType.Concrete);
            CreatePlatform(3, 1, 19, 11, -31, BlockType.Concrete);

            //left wall
            CreatePlatform(1, 4, 18, 10, -34, BlockType.Concrete);
            CreatePlatform(1, 4, 19, 10, -34, BlockType.Concrete);

            //right wall
            CreatePlatform(1, 5, 18, 14, -35, BlockType.Concrete);
            CreatePlatform(1, 5, 19, 14, -35, BlockType.Concrete);

            //side platform 1
            for (int i = -30; i > -33; i--)
            {
                CreatePlatform(1, 1, 5, 15, i, BlockType.Wall);
            }

            CreatePlatform(1, 1, 6, 15, -34, BlockType.Wall);

            //side platform 2
            CreatePlatform(6, 1, 7, 10, -36, BlockType.Wall);

            //large tower 2
            for (int i = -13; i < 8; i++)
            {
                CreatePlatform(5, 5, i, 10, -44, BlockType.Concrete);
            }

            for (int i = 1; i < 16; i++)
            {
                CreatePlatform(5, 5, i + 9, 10, -44, BlockType.Concrete);
            }

            //back wall
            CreatePlatform(2, 1, 8, 10, -44, BlockType.Concrete);
            CreatePlatform(2, 1, 9, 10, -44, BlockType.Concrete);

            //front wall
            CreatePlatform(2, 1, 8, 13, -40, BlockType.Concrete);
            CreatePlatform(2, 1, 9, 13, -40, BlockType.Concrete);

            //left wall
            CreatePlatform(1, 4, 8, 10, -43, BlockType.Concrete);
            CreatePlatform(1, 4, 9, 10, -43, BlockType.Concrete);

            //right wall
            CreatePlatform(1, 5, 8, 14, -44, BlockType.Concrete);
            CreatePlatform(1, 5, 9, 14, -44, BlockType.Concrete);

            //arrow up
            for (int i = 0; i < 6; i++)
                CreatePlatform(1, 1, 11 + i, 12, -39, BlockType.Sand);

            CreatePlatform(1, 1, 15, 11, -39, BlockType.Sand);
            CreatePlatform(1, 1, 15, 13, -39, BlockType.Sand);
            CreatePlatform(1, 1, 14, 10, -39, BlockType.Sand);
            CreatePlatform(1, 1, 14, 14, -39, BlockType.Sand);

            //steps up the back of tower 2
            CreatePlatform(1, 1, 7, 13, -45, BlockType.Wood);
            CreatePlatform(1, 1, 8, 15, -45, BlockType.Wood);
            CreatePlatform(1, 1, 9, 15, -43, BlockType.Wood);
            CreatePlatform(1, 1, 10, 15, -41, BlockType.Wood);
            CreatePlatform(1, 1, 11, 15, -39, BlockType.Wood);
            CreatePlatform(1, 1, 12, 15, -37, BlockType.Wood);
            CreatePlatform(1, 1, 13, 15, -35, BlockType.Wood);
            CreatePlatform(1, 1, 14, 15, -33, BlockType.Wood);
            CreatePlatform(1, 1, 15, 15, -31, BlockType.Wood);

            //objective
            return WorldEditor.CreateObjective(new Vector3(12, -33, 18));
        }

        public void CreatePlatform(int length, int width, int height, int x, int y)
        {
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    WorldEditor.CreateCube(new Vector3(i + x, j + y, height));
                }
            }
        }

        public void CreatePlatform(int length, int width, int height, int x, int y, BlockType type)
        {
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    WorldEditor.CreateCube(new Vector3(i + x, j + y, height), type);
                }
            }
        }
    }

    public class BarryLevelFactory : IFactory
    {
        public Block GenerateWorld(WorldInfo info)
        {
            WorldEditor.world = info;
            WorldEditor.CreatePlane(new Vector3(-5, -5, 0), new Vector3(10, 10, 1), BlockType.Concrete);
            WorldEditor.CreatePlane(new Vector3(-5, -5, 5), new Vector3(5, 10, 1), BlockType.Concrete);
            WorldEditor.CreatePlane(new Vector3(0, -5, 5), new Vector3(5, 10, 1), BlockType.Concrete);
            WorldEditor.CreatePyramid(new Vector3(-5, -5, 5), new Point(10, 10), BlockType.Wood);
            WorldEditor.CreateWall(new Vector3(0, 0, 10), new Point(30, 1), WallDirection.NegativeY, BlockType.Brick);
            WorldEditor.CreateWall(new Vector3(-1, 0, 10), new Point(30, 1), WallDirection.NegativeY, BlockType.Brick);
            WorldEditor.CreateWall(new Vector3(-5, -5, 0), new Point(10, 2), WallDirection.PositiveX, BlockType.Concrete);
            WorldEditor.CreateWall(new Vector3(-5, -5, 0), new Point(10, 5), WallDirection.PositiveY, BlockType.Concrete);
            WorldEditor.CreateWall(new Vector3(5, -5, 0), new Point(10, 5), WallDirection.PositiveY, BlockType.Concrete);
            WorldEditor.CreateWall(new Vector3(-5, 5, 0), new Point(10, 5), WallDirection.PositiveX, BlockType.Concrete);

            return WorldEditor.CreateCube(new Vector3(0, -20, 5), BlockType.Goal);
        }
    }

    public class JSwagLevelFactoryUsingNoiseWithPlane : IFactory
    {
        public Block GenerateWorld(WorldInfo info)
        {
            // First we find the grid size of the level
            var width = (int)(info.WorldDimension.X / 10 / InstancedBoxRenderer.SIZE);
            var height = (int)(info.WorldDimension.Y / 10 / InstancedBoxRenderer.SIZE);
            Random gen = new Random(PerlinNoise.Seed);

            int overallMax = 0;
            int overallMin = 0;
            Color[] overallMap = CreateTexture.CreateRidgeHeightMap(width, height, out overallMax, out overallMin, 64);
            List<Block> topBlocks = new List<Block>();

            for (int overallX = -width / 2, overallIndexX = 0; overallX < width / 2; overallX++, overallIndexX++)
            {
                for (int overallY = -height / 2, overallIndexY = 0; overallY < height / 2; overallY++, overallIndexY++)
                {
                    int fineMax = 0;
                    int fineMin = 0;
                    
                    Color[] fineMap = CreateTexture.CreateRidgeHeightMap(10, 10, out fineMax, out fineMin, 32);
                    int average = (fineMin + fineMax + overallMax + overallMin) / 4;
                    for (int x = 0, indexX = 0; x < 10; x++, indexX++)
                    {
                        for (int y = 0, indexY = 0; y < 10; y++, indexY++)
                        {
                            int overallHeight = overallMax - overallMap[overallIndexX * width + overallIndexY].R;
                            int fineHeight = fineMax - fineMap[indexX * 10 + indexY].R;
                            int cubeHeight = overallHeight + fineHeight;
                            for (int z = overallHeight; z < fineHeight; z++)
                            {
                                var block = new Block(info);

                                block.Location = new Vector3() { X = (overallX * 10 + x) * InstancedBoxRenderer.SIZE, Y = z * InstancedBoxRenderer.SIZE, Z = (overallY * 10 + y) * InstancedBoxRenderer.SIZE };
                                (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();
                                if (z > fineHeight / 2)
                                    block.Type = BlockType.Wall;
                                else
                                    block.Type = BlockType.Grass;

                                info.AddActor(block);
                                
                                if (z == fineHeight - 1)
                                {
                                    topBlocks.Add(block);
                                }
                            }
                        }
                    }
                }
            }

            return topBlocks[gen.Next(topBlocks.Count)];
        }
    }

    class JSwagFactoryUsingNoise : IFactory
    {

        public Block GenerateWorld(WorldInfo info)
        {
            // First we find the grid size of the level
            var width = (int)(info.WorldDimension.X / 10 / InstancedBoxRenderer.SIZE);
            var height = (int)(info.WorldDimension.Y / 10 / InstancedBoxRenderer.SIZE);
            Random gen = new Random(PerlinNoise.Seed);
            int overallMax = 0;
            int overallMin = 0;
            Color[] overallMap = CreateTexture.CreateRidgeHeightMap(width, height, out overallMax, out overallMin, 64);

            for (int overallX = -width / 2, overallIndexX = 0; overallX < width / 2; overallX++, overallIndexX++)
            {
                for (int overallY = -height / 2, overallIndexY = 0; overallY < height / 2; overallY++, overallIndexY++)
                {
                    int fineMax = 0;
                    int fineMin = 0;
                    Color[] fineMap = CreateTexture.CreateRidgeHeightMap(10, 10, out fineMax, out fineMin, 32);
                    for (int x = 0, indexX = 0; x < 10; x++, indexX++)
                    {
                        for (int y = 0, indexY = 0; y < 10; y++, indexY++)
                        {
                            int overallHeight = overallMax - overallMap[overallIndexX * width + overallIndexY].R;
                            int fineHeight = fineMax - fineMap[indexX * 10 + indexY].R;
                            int cubeHeight = overallHeight + fineHeight;
                            var block = new Block(info);
                            block.Location = new Vector3() { X = (overallX * 10 + x) * InstancedBoxRenderer.SIZE, Y = cubeHeight * InstancedBoxRenderer.SIZE, Z = (overallY * 10 + y) * InstancedBoxRenderer.SIZE };
                            (block.Components[0] as CubeCollisionComponent).UpdateCollisionPrimitive();

                            info.AddActor(block);
                        }
                    }
                }
            }
            return null;
        }
    }

}
