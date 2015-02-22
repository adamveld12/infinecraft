using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockRLH.Actors.GameActors
{
    class ParticleSettings
    {
        // Max Size of Particle
        public int maxSize = 2;
    }

    class ParticleExplosionSettings
    {
        // Life of particles
        public int minLife = 3000;
        public int maxLife = 3000;
        // Particles per round
        public int minParticlesPerRound = 100;
        public int maxParticlesPerRound = 600;
        // Round time
        public int minRoundTime = 116;
        public int maxRoundTime = 150;
        // Number of particles
        public int minParticles = 1000;
        public int maxParticles = 2000;
    }
}
