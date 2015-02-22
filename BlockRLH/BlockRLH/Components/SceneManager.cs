using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLH.Components;
using Microsoft.Xna.Framework;
using BlockRLH.Actors;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework.Graphics;
using BlockRLH.Actors.GameActors;

namespace BlockRLH.Components
{
    /// <summary>
    /// Manages updating and drawing actors in the scene
    /// </summary>
    public class SceneManager
    {
        private WorldInfo worldInfo;
        private List<Actor> actorsInWorld;

        private BasicEffect basicEffect;
        private Texture2D texture;
        private StaticVertexBufferRenderer renderer;

        /// <summary>
        /// Initializes a new instance of SceneManager
        /// </summary>
        public SceneManager(WorldInfo worldInfo)
        {
            this.worldInfo = worldInfo;
            actorsInWorld = new List<Actor>(2048);
            GraphicsDevice = worldInfo.Engine.GraphicsDevice;

            texture = worldInfo.Engine.Content.Load<Texture2D>("terrain");

            basicEffect = new BasicEffect(GraphicsDevice);
        }

        public void LoadContent()
        {
            renderer = new StaticVertexBufferRenderer(GraphicsDevice, actorsInWorld.Where(actor => actor is Block).Select(actor => actor as Block).ToArray<Block>(), texture, worldInfo.Engine.Content.Load<Effect>(@"Effects\StaticVertexBufferEffect"));
        }

        #region CRUD Actor

        /// <summary>
        /// Adds an actor to the scene
        /// </summary>
        public void AddActor(Actor actor)
        {
            this.actorsInWorld.Add(actor);
        }

        /// <summary>
        /// Removes an actor from the scene
        /// </summary>
        public void RemoveActor(Actor actor)
        { this.actorsInWorld.Remove(actor); }

        /// <summary>
        /// Gets all the actors
        /// </summary>
        public Actor[] GetActors()
        { return actorsInWorld.ToArray(); }

        #endregion

        #region Update Draw

        /// <summary>
        /// Ticks each actor in the scene
        /// </summary>
        public void Update(GameTime gameTime)
        {
            PlayerController player = worldInfo.GameInfo.GetPlayer(PlayerIndex.One);
            player.Update(gameTime);

            for (int i = actorsInWorld.Count - 1; i >= 0; i--)
            {

                var actor = actorsInWorld[i];
                
                if (actor.bEnabled)
                {
                    if(!(actor is PlayerController)){
                        actor.Update(gameTime);
                    }

                    if (actor is Block)
                    {
                        float distance = Vector3.Distance(actor.Location, player.Location);

                        if (distance < 90f)
                        {

                            if ((actor.GetComponent("Collision") as CubeCollisionComponent).Intersecting(player.BoundingBox))
                                player.CameraCollision(actor as Block);
                        }
                    }

                    if (actor is WaterPlane)
                    {
                        if ((actor as WaterPlane).Location.Y > player.Location.Y)
                        {
                            player.Controllable = false;
                            player.Location = new Vector3(0, player.Location.Y, 0);
                            this.worldInfo.PlayerBelowWater();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the scene
        /// </summary>
        public void Draw(GameTime gameTime, Camera camera)
        {
            renderer.Render(camera);
            if(worldInfo.debug) DrawBoundingBoxes(camera);
        }

        /// <summary>
        /// Draws the scene
        /// </summary>
        public void Draw(GameTime gameTime, Matrix camera)
        {

            renderer.Render(camera);
        }


        // Someone got a litle comment happy
        // V This is a method V 
        //If you don't want it. Don't call it L-O-L
        public void DrawBoundingBoxes(Camera camera)
        {
            foreach (Actor a in GetActors())
            {
                CubeCollisionComponent cc = a.GetComponent("Collision") as CubeCollisionComponent;
                if (cc != null)
                {
                    BoundingBoxRenderer.Render(cc.GetBoundingBox(), worldInfo.Engine.GraphicsDevice, camera.View, camera.Projection, Color.Red);
                }
            }

            BoundingBoxRenderer.Render(worldInfo.GameInfo.GetPlayer(PlayerIndex.One).BoundingBox, worldInfo.Engine.GraphicsDevice, camera.View, camera.Projection, Color.Blue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of actors in this scene
        /// </summary>
        public int Count
        { get { return 0; } }


        /// <summary>
        /// Get or set the graphics device to render to 
        /// </summary>
        public GraphicsDevice GraphicsDevice
        { get; private set; }
        #endregion

        internal void Dispose()
        {
            this.renderer.Dispose();
        }
    }
}
