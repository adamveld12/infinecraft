using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework;
using BlockRLH.Actors.GameActors;

namespace BlockRLH.Components
{
    public static class WorldEditor
    {
        public static WorldInfo world;

         #region BaseMethods

        /// <summary>
        /// Places a single cube in the world
        /// </summary>
        /// <param name="position">Where you will place the cube in the grid ex. (1,1,1) will place the cube 1 cube on x, 1 cube on y, and 1 cube on z</param>
        public static Block CreateCube(Vector3 position)
        {
            return CreateCube(position, BlockType.Grass);
        }

        /// <summary>
        /// Places a single cube in the world with the specified blocktype
        /// </summary>
        /// <param name="position">Where you want the cube in the grid</param>
        /// <param name="type">The type of cube you want to place</param>
        public static Block CreateCube(Vector3 position, BlockType type)
        {
            Block block = new Block(world);
            block.PlaceInWorld(position);
            block.InitCollision();
            block.Type = type;
            world.AddActor(block);
            return block;
        }

        /// <summary>
        /// Places a single cube in the world with the specified blocktype
        /// </summary>
        /// <param name="position">Where you want the cube in the grid</param>
        /// <param name="type">The type of cube you want to place</param>
        public static Block CreateObjective(Vector3 position)
        {
            Block block = new Block(world);
            block.PlaceInWorld(position);
            block.InitCollision();
            block.Type = BlockType.Goal;
            world.AddActor(block);
            return block;
        }

        /// <summary>
        /// Create a plane in the world
        /// </summary>
        /// <param name="position">Where you want the plane to begin</param>
        /// <param name="dimension">Dimensions of the plane</param>
        public static void CreatePlane(Vector3 position, Vector3 dimension)
        {
            CreatePlane(position, dimension, BlockType.Grass);
        }

        /// <summary>
        /// Create a plane in the world
        /// </summary>
        /// <param name="position">Where you want the plane to begin</param>
        /// <param name="dimension">Dimensions of the plane</param>
        /// <param name="type">The type of the blocks</param>
        public static void CreatePlane(Vector3 position, Vector3 dimension, BlockType type)
        {
            for (int i = 0; i < dimension.X; i++)
            {
                for (int j = 0; j < dimension.Y; j++)
                {
                    for (int k = 0; k < dimension.Z; k++)
                    {
                        CreateCube(Vector3.Add(position, new Vector3(i, j, k)), type);
                    }
                }
            }
        }

        /// <summary>
        /// Create a wall in the world
        /// </summary>
        /// <param name="position">Where you want the plane to start</param>
        /// <param name="dimension">How long and How High you'd like the wall to be</param>
        /// <param name="direction">The direction you'd like the wall to face</param>
        public static void CreateWall(Vector3 position, Point dimension, WallDirection direction)
        {
            CreateWall(position, dimension, direction, BlockType.Grass);
        }

        public static void CreateWall(Vector3 position, Point dimension, WallDirection direction, BlockType type)
        {
            switch (direction)
            {
                case WallDirection.PositiveX:
                    CreateWallOnX(position, dimension, type);
                    break;
                case WallDirection.NegativeX:
                    CreateWallOnX(new Vector3(position.X - dimension.X, position.Y, position.Z), dimension, type);
                    break;
                case WallDirection.PositiveY:
                    CreateWallOnY(position, dimension, type);
                    break;
                case WallDirection.NegativeY:
                    CreateWallOnY(new Vector3(position.X, position.Y - dimension.X, position.Z), dimension, type);
                    break;
            }
        }

        /// <summary>
        /// Create a pyramid in the world
        /// </summary>
        /// <param name="position">Where you want the pyramid to start</param>
        /// <param name="dimension">How wide you'd like the pyramid to be</param>
        public static void CreatePyramid(Vector3 position, Point dimension)
        {
            CreatePyramid(position, dimension, BlockType.Grass);
        }

        /// <summary>
        /// Create a pyramid in the world
        /// </summary>
        /// <param name="position">Where you want the pyramid to start</param>
        /// <param name="dimension">How wide you'd like the pyramid to be</param>
        /// <param name="type">The type of block you want it to be made out of</param>
        public static void CreatePyramid(Vector3 position, Point dimension, BlockType type)
        {
            for (int i = 0; i < dimension.Y; i++)
            {
                CreatePlane(new Vector3(position.X + i, position.Y + i, position.Z + i + 1), new Vector3(dimension.X - (i * 2), dimension.X - (i * 2), 1), type);
            }
        }

        #endregion

        #region HelperMethods

        private static void CreateWallOnX(Vector3 position, Point dimension, BlockType type)
        {
            CreatePlane(new Vector3(position.X, position.Y, position.Z + 1), new Vector3(dimension.X, 1, dimension.Y), type);
        }

        private static void CreateWallOnX(Vector3 position, Point dimension)
        {
            CreateWallOnX(position, dimension, BlockType.Grass);
        }

        private static void CreateWallOnY(Vector3 position, Point dimension, BlockType type)
        {
            CreatePlane(new Vector3(position.X, position.Y, position.Z + 1), new Vector3(1, dimension.X, dimension.Y), type);
        }

        private static void CreateWallOnY(Vector3 position, Point dimension)
        {
            CreateWallOnY(position, dimension, BlockType.Grass);
        }

        #endregion
    }

    public enum WallDirection{ PositiveX,NegativeX,PositiveY,NegativeY}
}
