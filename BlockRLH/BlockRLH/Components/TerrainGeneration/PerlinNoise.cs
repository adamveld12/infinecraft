using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.TerrainGeneration
{
    public static class CreateTexture
    {
        public static Color[] CreatefBmHeightMap(int width, int height)
        {
            Color[] data = new Color[width * height];
            float tempColor;

            float x = 0, y = 0, z = 0;
            for (int v = 0; v < width; v++)
            {
                x += 0.5f;
                y = 0;
                for (int u = 0; u < height; u++)
                {
                    tempColor = (float)PerlinNoise.fBm(x, y, z, 8, .45f);
                    data[v * width + u] = new Color(tempColor / 16, tempColor, tempColor, 1);

                    y += 0.5f;
                }
            }

            return data;
        }

        public static Color[] CreateRidgeHeightMap(int width, int height, out int max, out int min, int divisor = 32)
        {
            min = Int32.MaxValue;
            max = Int32.MinValue;
            Color[] data = new Color[width * height];
            float tempColor;

            float x = 0, y = 0, z = 0;
            for (int v = 0; v < width; v++)
            {
                x += 0.5f;
                y = 0;
                for (int u = 0; u < height; u++)
                {
                    tempColor = (float)PerlinNoise.RidgedMF(x, y, z, 12, .1f, .9f, .9f) / divisor;
                    data[v * width + u] = new Color(tempColor, tempColor, tempColor, 1);
                    //Console.WriteLine(tempColor+" "+max);
                    if (data[v * width + u].R < min)
                        min = (int)data[v * width + u].R;
                    else if (data[v * width + u].R > max)
                        max = (int)data[v * width + u].R;
                    y += 0.5f;
                }
            }

            return data;
        }

    }



    public static class PerlinNoise
    {
        private static Random random;

        private static int[] permutationTable = new int[512];

        public static int Seed { get; private set; }

        public static void RebuildPermutation(int seed)
        {
            random = new Random(seed);

            // Use the bitwise left shift operator to make sure the array of permutation values is a multiple of 2
            int nbVals = (1 << 8); // result is 256
            int[] tempPermutationTable = new int[nbVals];

            // set values in temp perm array as "unused", denoted by -1
            for (int i = 0; i < nbVals; i++)
            {
                tempPermutationTable[i] = -1;
            }

            for (int i = 0; i < nbVals; i++)
            {
                // for each value, find an empty spot, and place it in it
                while (true)
                {
                    // generate rand # with max a nbvals
                    int tempIndex = random.Next() % nbVals;
                    if (tempPermutationTable[tempIndex] == -1)
                    {
                        tempPermutationTable[tempIndex] = i;
                        break;
                    }
                }
            }

            for (int i = 0; i < nbVals; i++)
            {
                permutationTable[nbVals + i] = permutationTable[i] = tempPermutationTable[i];
            }
            Seed = seed;
        }

        private static double Noise(double x, double y, double z)
        {
            int X = (int)x;
            int Y = (int)y;
            int Z = (int)z;

            x -= Math.Floor(x);
            y -= Math.Floor(y);
            z -= Math.Floor(z);

            double u = fade(x);
            double v = fade(y);
            double w = fade(z);

            int A = permutationTable[X] + Y, AA = permutationTable[A] + Z, AB = permutationTable[A + 1] + Z;
            int B = permutationTable[X + 1] + Y, BA = permutationTable[B] + Z, BB = permutationTable[B + 1] + Z;

            return lerp(w, lerp(v, lerp(u, grad(permutationTable[AA], x, y, z),
                                   grad(permutationTable[BA], x - 1, y, z)),
                           lerp(u, grad(permutationTable[AB], x, y - 1, z),
                                   grad(permutationTable[BB], x - 1, y - 1, z))),
                   lerp(v, lerp(u, grad(permutationTable[AA + 1], x, y, z - 1),
                                   grad(permutationTable[BA + 1], x - 1, y, z - 1)),
                           lerp(u, grad(permutationTable[AB + 1], x, y - 1, z - 1),
                                   grad(permutationTable[BB + 1], x - 1, y - 1, z - 1))));
        }

        public static double fBm(double x, double y, double z, float octaves, float persistance)
        {
            //Set the initial value and initial size
            double value = 0.0f;
            float amplitude = 0.0f;
            float frequency = 0.0f;

            for (int o = 0; o < octaves; o++)
            {
                frequency = (float)Math.Pow(2.0f, o);
                amplitude = (float)Math.Pow(persistance, o);
                value += Noise(x * frequency, y * frequency, z * frequency) * amplitude;
            }

            //Return the result over the initial size
            return value;
        }

        public static double RidgedMF(double x, double y, double z, int octaves, float lacunarity, float gain, float offset)
        {
            double sum = 0;
            float amplitude = 0.5f;
            float frequency = 1.0f;
            double prev = 1.0f;

            for (int i = 0; i < octaves; i++)
            {
                double n = ridge(Noise(x * frequency, y * frequency, z * frequency), offset);
                sum += n * amplitude * prev;
                prev = n;
                frequency *= lacunarity;
                amplitude *= gain;
            }

            return sum;
        }

        private static double ridge(double h, float offset)
        {
            h = Math.Abs(h);
            h = offset - h;
            h = h * h;
            return h;
        }

        private static double fade(double t)
        {
            return (t * t * t * (t * (t * 6 - 15) + 10));
        }

        private static double lerp(double t, double a, double b)
        {
            return (a + t * (b - a));
        }

        private static double grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z;

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}
