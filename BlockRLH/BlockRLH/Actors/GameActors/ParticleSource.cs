using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Actors.GameActors
{
    class ParticleSource : Actor
    {
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;

        GraphicsDevice graphicsDevice;

        public ParticleSource(WorldInfo info)
            : base(info)
        {
            graphicsDevice = info.Engine.GraphicsDevice;
            LoadContent(info.Engine.Content);
        }

        private void LoadContent(ContentManager Content)
        {
            explosionTexture = Content.Load<Texture2D>(@"Particle");
            explosionColorsTexture = Content.Load<Texture2D>(@"Gold_T");
            explosionEffect = Content.Load<Effect>(@"Effects\Particle");
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);
        }

        protected override void Tick(GameTime delta)
        {
            if (explosions.Count == 0)
            {
                CreateParticles();
            }
            UpdateExplosions(delta);
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            foreach (ParticleExplosion pe in explosions)
            {
                pe.Draw(camera);
            }
        }

        protected void UpdateExplosions(GameTime gameTime)
        {
            // Loop through and update explosions
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);
                // If explosion is finished, remove it
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }
        }

        private void CreateParticles()
        {
            Random rnd = new Random();
            // Collision! add an explosion.
            explosions.Add(new ParticleExplosion(graphicsDevice,
            Location,
            rnd.Next(
            particleExplosionSettings.minLife,
            particleExplosionSettings.maxLife),
            rnd.Next(
            particleExplosionSettings.minRoundTime,
            particleExplosionSettings.maxRoundTime),
            rnd.Next(
            particleExplosionSettings.minParticlesPerRound,
            particleExplosionSettings.maxParticlesPerRound),
            rnd.Next(
            particleExplosionSettings.minParticles,
            particleExplosionSettings.maxParticles),
            explosionColorsTexture, particleSettings,
            explosionEffect));
        }
    }
}
