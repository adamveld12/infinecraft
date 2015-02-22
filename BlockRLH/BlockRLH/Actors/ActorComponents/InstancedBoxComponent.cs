using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockRLH.Actors.GameActors;

namespace BlockRLH.Actors.ActorComponents
{
    /// <summary>
    /// A mesh component that holds static collections of matrices that are modifed on the fly
    /// </summary>
    public class InstancedBoxComponent : RenderComponent
    {
        /// <summary>
        /// A dictionary of matrix transforms
        /// </summary>
        public static Dictionary<int, Matrix> instanceTransforms = new Dictionary<int, Matrix>();
        private bool inView = false;
        public InstancedBoxComponent(Actor owner)
            : base(owner)
        { }

        public override void Tick(GameTime delta)
        {
            base.Tick(delta);
            if (inView)
            {
                instanceTransforms.Remove(Owner.ID);
                inView = false;
            }
        }

        /// <summary>
        /// Updates transform info for this block
        /// </summary>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.bEnabled && !inView)
            {
                instanceTransforms[Owner.ID] = Matrix.CreateWorld(this.Owner.Location, Vector3.Forward, Vector3.Up);
                inView = true;
            }
        }
    }
}
