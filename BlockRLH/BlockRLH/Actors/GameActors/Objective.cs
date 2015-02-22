using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RLH.Components;
using Microsoft.Xna.Framework;
using BlockRLH.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace BlockRLH.Actors.GameActors
{
    public class Objective : Block
    {
        //Create a custom texture to a "floating" box
        //Shader flash effect to make it "stand" out

        //BasicEffect basicEffect;
        Effect effect;
        Camera camera;
        public readonly Block block;
        GraphicsDevice graphicsDevice;


        #region Particles
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;
        #endregion

        public Objective(WorldInfo info, ref Camera cam, ref Block block)
            : base(info)
        {
            //this.block = block;
            this.Type = BlockType.Goal;
            camera = cam;
            graphicsDevice = info.Engine.GraphicsDevice;

            Location = block.Location;

            LoadContent(info.Engine.Content);
            //basicEffect = new BasicEffect(info.Engine.GraphicsDevice);
           
        }

        public void LoadContent(ContentManager Content)
        {
            effect = Content.Load<Effect>(@"Effects\Red");

            explosionTexture = Content.Load<Texture2D>(@"Particle");
            explosionColorsTexture = Content.Load<Texture2D>(@"ParticleColors");
            explosionEffect = Content.Load<Effect>(@"Effects\Particle");
            // Set effect parameters that don't change per particle
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);
        }

        public void Draw()
        {
            //basicEffect.View = camera.View;
            //basicEffect.Projection = camera.Projection;
            //basicEffect.World = Matrix.CreateWorld(Location, Vector3.Forward, Vector3.Up);

            //effect.CurrentTechnique = effect.Techniques["Technique1"];
            //effect.Parameters["World"].SetValue(Matrix.CreateWorld(Location, Vector3.Forward, Vector3.Up));
            //effect.Parameters["View"].SetValue(camera.View);
            //effect.Parameters["Projection"].SetValue(camera.Projection);

            //short[] indi = InstancedBoxRenderer.Indices;
     
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, InstancedBoxRenderer.CubeVerts, 0, 24, indi, 0, 36 / 3);
            //}

            // Loop through and draw each particle explosion
            //Console.WriteLine(explosions.Count);
            if (explosions.Count == 0)
            {
                CreateParticles();
            }
            foreach (ParticleExplosion pe in explosions)
            {
                pe.Draw(camera);
            }
        }

        protected override void Tick(GameTime delta)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                CreateParticles();
            }
            UpdateExplosions(delta);
            base.Tick(delta);
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

        #region Event
        public delegate void EnteredHandler(object sender, EventArgs e);
        public event EnteredHandler Entered;

        public void Entered_EventHandler(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
