using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BlockRLH.Actors.ActorComponents;
using BlockRLH.Actors;
using RLH.Components;


namespace Grid
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Skybox : Actor
    {
        Vector3 ModelPosition;
        Model SkySphere;
        Effect SkySphereEffect;
        Camera camera;

        public Skybox(WorldInfo info, ref Camera camera) : base(info)
        {
            this.camera = camera;
            LoadContent();
        }

        protected void LoadContent()
        {
            var content = WorldInfo.Engine.Content;

            ModelPosition = Vector3.Zero;

            SkySphereEffect = content.Load<Effect>(@"Skybox\SkySphere");

            var SkyboxTexture = content.Load<Texture2D>(@"Skybox\muchbetterskybox");

            SkySphere = content.Load<Model>(@"Skybox\SphereHighPoly");

            SkySphereEffect.Parameters["ViewMatrix"].SetValue(
                camera.View);
            SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(
                camera.Projection);
            SkySphereEffect.Parameters["SkyboxTexture"].SetValue(
                SkyboxTexture);

            foreach (ModelMesh mesh in SkySphere.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = SkySphereEffect;
        }

        public  void Draw(GameTime gameTime)
        {
            // Set the View and Projection matrix for the effect
            SkySphereEffect.Parameters["ViewMatrix"].SetValue(
                camera.View);
            SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(
                camera.Projection);
            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in SkySphere.Meshes)
                mesh.Draw();
        }
    }
}
