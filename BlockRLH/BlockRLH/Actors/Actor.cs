using System.Collections.Generic;
using System.Linq;
using BlockRLH.Actors.ActorComponents;
using Microsoft.Xna.Framework;
using RLH.Components;

namespace BlockRLH.Actors
{
    /// <summary>
    /// Base class for all placeable objects
    /// </summary>
    public abstract class Actor
    {
        private Dictionary<string, ActorComponent> components;
        private WorldInfo world;

        private static int IDENTIFICATION = 0;

        protected Vector3 location;
        protected Vector3 rotator;

        private bool benabled;
        private int id = IDENTIFICATION++;

        /// <summary>
        /// Initializes a new actor, please note that it adds itself to the scene graph, 
        /// if you add this outside of this constructor then you need to be jerked off with rubbing alcohol
        /// </summary>
        public Actor(WorldInfo worldInfo)
        {
            components = new Dictionary<string, ActorComponent>();
            world = worldInfo;

            rotator = Vector3.Zero;
            location = Vector3.Zero;

            bEnabled = true;
        }

        /// <summary>
        /// Ticks the actor
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Tick(gameTime);

            foreach (var item in components.Values)
            {
                if(item.bEnabled)
                    item.Tick(gameTime);
            }
        }

        /// <summary>
        /// Called every update
        /// </summary>
        /// <param name="delta"></param>
        protected virtual void Tick(GameTime delta) 
        { }

        /// <summary>
        /// Destroys the actor
        /// </summary>
        /// <param name="Instigator">Who did it</param>
        public void Destroy(Actor Instigator)
        { this.world.RemoveActor(this); }

        /// <summary>
        /// Applies damage to this actor
        /// </summary>
        /// <param name="instigator">Who damaged us</param>
        /// <param name="damage"></param>
        public virtual void TakeDamage(Actor instigator, int damage)
        { }

        public virtual ActorComponent GetComponent(string key)
        {
            ActorComponent component = null;
            if(components.TryGetValue(key, out component)) return component;
            return null;
        }

        /// <summary>
        /// Adds an actor component to this actor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="component"></param>
        protected void AddComponent(string type, ActorComponent component)
        { this.components.Add(type, component); }

        /// <summary>
        /// Unique ID for this actor
        /// </summary>
        public int ID
        { get { return id; } }

        /// <summary>
        /// Get or set the actor's rotation
        /// </summary>
        public Vector3 Rotator
        {
            get { return rotator; }
            set { rotator = value; }
        }
        
        /// <summary>
        /// Get or set the actor's location
        /// </summary>
        public Vector3 Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Enable or disable ticking for this actor
        /// </summary>
        public bool bEnabled
        {
            get { return benabled; }
            set { benabled = value; }
        }

        /// <summary>
        /// Gets the components for this actor
        /// </summary>
        public ActorComponent[] Components
        { get { return this.components.Values.ToArray(); } }

        /// <summary>
        /// Get the current WorldInfo for the game
        /// </summary>
        public WorldInfo WorldInfo
        { get { return this.world; } }
    }
}
