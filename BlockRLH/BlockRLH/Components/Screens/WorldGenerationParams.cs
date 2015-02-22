using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components.Screens
{
    public class WorldGenerationParams
    {
        public WorldGenerationParams()
        {
            this.WorldDimension = new Vector3(400, 400, 20);
            WaterSpeedMultiplier = 1;
        }

        public Vector3 WorldDimension { get; set; }
        public IFactory BlockFactory { get; set; }
        public float WaterSpeedMultiplier { get; set; }
        public bool SpawnLocationSet { get; set; }
        private Vector3 spawnLocation;
        public Vector3 SpawnLocation
        {
            get
            {
                return spawnLocation;
            }
            set
            {
                spawnLocation = value;
                SpawnLocationSet = true;
            }
        }
    }
}
