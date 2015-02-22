using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BlockRLH.Actors.GameActors;

namespace BlockRLH.Components
{
    public class CollisionSolver
    {
        private float distance, blockDistance, topDistance, yDistance;
        private Vector3 P1, P2, P3, P4, P5, P6, P7, P8, location;
        private Face frontFace, backFace, leftFace, rightFace, topFace, botFace;
        private bool topCollision, botCollision;

        public bool TopCollision { get { return topCollision; } set { topCollision = value; } }
        public bool BotCollision { get { return botCollision; } set { botCollision = value; } }
        public bool LeftCollision { get; set; }
        public bool RightCollision { get; set; }
        public bool FrontCollision { get; set; }
        public bool BackCollision { get; set; }

        public float TopDistance { get { return topDistance; } }
        public float Distance { get { return distance; } }

        public int TopCollisions { get; set; }
        public int LeftCollisions { get; set; }
        public int RightCollisions { get; set; }
        public int FrontCollisions { get; set; }
        public int BackCollisions { get; set; }
        public float ScaledDistance { get { return distance; } }

        public bool Debug { get; set; }

        #region tweaks
        public float scale;
        private float minimumtopdistance, minimumsidedistance, mindistance;
        #endregion

        public CollisionSolver()
        {
            distance = InstancedBoxRenderer.SIZE;
            blockDistance = 0.0f;
            topDistance = 0.0f;
            topCollision = false;
            scale = 1.0f;
            minimumtopdistance = 5f;
            minimumsidedistance = 1f;
            Debug = false;

            ResetLeftRight();
            ResetSolver();
            ResetCollisions();
        }

        public void SetScale(float toScale)
        {
            scale = toScale;
            distance = InstancedBoxRenderer.SIZE / scale;
            minimumtopdistance = distance / 2;
            minimumsidedistance = 1.0001f * scale;
        }


        public void UpdateFaces(BoundingBox bbox)
        {
            P1 = bbox.Min;
            P2 = bbox.Max;
            P3 = new Vector3(P1.X + distance, P1.Y, P1.Z);
            P4 = new Vector3(P1.X, P1.Y, P1.Z + distance);
            P5 = new Vector3(P2.X, P2.Y - distance, P2.Z);
            P6 = new Vector3(P1.X, P1.Y + distance, P1.Z);
            P7 = new Vector3(P2.X, P2.Y, P2.Z - distance);
            P8 = new Vector3(P2.X - distance, P2.Y, P2.Z);

            frontFace = new Face(P1, P3, P6, P7);
            backFace = new Face(P2, P4, P5, P8);
            leftFace = new Face(P1, P4, P6, P8);
            rightFace = new Face(P2, P3, P5, P7);
            topFace = new Face(P2, P6, P7, P8);
            botFace = new Face(P1, P3, P4, P5);
        }

        public Vector3 Solve(Block b)
        {
            CubeCollisionComponent col = b.GetComponent("Collision") as CubeCollisionComponent;
            location = Vector3.Zero;

            SolveTop(col.topFace, botFace);
            SolveBot(topFace, col.botFace);
            SolveBack(backFace, col.frontFace);
            SolveFront(col.backFace, frontFace);
            SolveRight(col.leftFace, rightFace);
            SolveLeft(col.rightFace, leftFace);

            return location;
        }

        private void SolveTop(Face F1, Face F2)
        {
            topDistance = (float)Math.Round(F1.Center.Y - F2.Center.Y, 5);
            mindistance = topDistance + 5;

            if (topDistance < 0) topDistance *= -1;

            if (F1.Center.Y > F2.Center.Y)
            {
                if (topDistance < minimumtopdistance && topDistance > 0)
                {
                    location = new Vector3(location.X, location.Y + topDistance, location.Z);
                    topCollision = true;
                    ++TopCollisions;
                }
            }
        }

        private void SolveBot(Face F1, Face F2)
        {
            blockDistance = F1.Center.Y - F2.Center.Y;
            blockDistance = (float)Math.Round(blockDistance,5);

            if (blockDistance < 0) blockDistance *= -1;

            if (F1.Center.Y > F2.Center.Y)
            {
                if (blockDistance < minimumtopdistance && blockDistance > 0)
                {
                    location = new Vector3(location.X, location.Y - blockDistance, location.Z);
                    FrontCollision = true;
                    botCollision = true;
                }
            }
        }

        private void SolveBack(Face F1, Face F2)
        {
            blockDistance = F1.Center.Z - F2.Center.Z;
            blockDistance = (float)Math.Round(blockDistance, 5);
            yDistance = (F1.Center.Y - F2.Center.Y);

            if (F2.Center.Z < F1.Center.Z)
            {
                if (yDistance < distance)
                {
                    if (blockDistance < minimumsidedistance)
                    {
                        if (mindistance > distance)
                        {
                            location = new Vector3(location.X, location.Y, location.Z - blockDistance - .1f);
                            Console.WriteLine("Back: {0}", blockDistance);
                            BackCollision = true;
                            ++BackCollisions;
                        }
                    }
                }
            }
        }

        private void SolveFront(Face F1, Face F2)
        {
            blockDistance = F1.Center.Z - F2.Center.Z;
            blockDistance = (float)Math.Round(blockDistance, 5);
            yDistance = (F2.Center.Y - F1.Center.Y);

            if (F1.Center.Z > F2.Center.Z)
            {
                if (yDistance < distance)
                {
                    if (blockDistance < minimumsidedistance)
                    {
                        if (mindistance > distance)
                        {
                            FrontCollision = true;
                            Console.WriteLine("Front: {0}", blockDistance);
                            location = new Vector3(location.X, location.Y, location.Z + blockDistance + .1f);
                            ++FrontCollisions;
                        }
                    }
                }
            }
        }

        private void SolveRight(Face F1, Face F2)
        {
            blockDistance = F2.Center.X - F1.Center.X;
            blockDistance = (float)Math.Round(blockDistance, 5);
            yDistance = (F2.Center.Y - F1.Center.Y);

            if (F1.Center.X < F2.Center.X)
            {
                if (yDistance < distance)
                {
                    if (blockDistance < minimumsidedistance)
                    {
                        if (mindistance > distance)
                        {
                            RightCollision = true;
                            location = new Vector3(location.X - blockDistance - .1f, location.Y, location.Z);
                            ++RightCollisions;
                        }
                    }
                }
            }
        }

        private void SolveLeft(Face F1, Face F2)
        {
            blockDistance = F1.Center.X - F2.Center.X;
            blockDistance = (float)Math.Round(blockDistance, 5);
            yDistance = (F2.Center.Y - F1.Center.Y);

            if (F1.Center.X > F2.Center.X)
            {
                if (yDistance < distance)
                {
                    if (blockDistance < minimumsidedistance)
                    {
                        if (mindistance > distance)
                        {
                            LeftCollision = true;
                            location = new Vector3(location.X + blockDistance + .1f, location.Y, location.Z);
                            ++LeftCollisions;
                        }
                    }
                }
            }
        }

        public void NormalizeCollisions()
        {
            if (TopCollisions == 0) TopCollisions = 1;
            if (FrontCollisions == 0) FrontCollisions = 1;
            if (BackCollisions == 0) BackCollisions = 1;
            if (LeftCollisions == 0) LeftCollisions = 1;
            if (RightCollisions == 0) RightCollisions = 1;
        }

        public void ResetCollisions()
        {
            TopCollisions = 0;
            LeftCollisions = 0;
            RightCollisions = 0;
            FrontCollisions = 0;
            BackCollisions = 0;
        }

        public void ResetSolver()
        {
            topCollision = false;
            botCollision = false;
        }

        public void ResetLeftRight()
        {
            LeftCollision = false;
            RightCollision = false;
        }
    }

    public struct Face
    {
        public Vector3 Point1, Point2, Point3, Point4, Center;

        public Face(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
            Point4 = p4;

            Vector3 max = Vector3.Max(Vector3.Max(p1, p2), Vector3.Max(p3, p4));
            Vector3 min = Vector3.Min(Vector3.Min(p1, p2), Vector3.Min(p3, p4));

            Center = Vector3.Divide(Vector3.Add(max, min), new Vector3(2, 2, 2));

            if (Center.X == 0.0f) Center.X = max.X;
            if (Center.Y == 0.0f) Center.Y = max.Y;
            if (Center.Z == 0.0f) Center.Z = max.Z;
        }
    }

}
