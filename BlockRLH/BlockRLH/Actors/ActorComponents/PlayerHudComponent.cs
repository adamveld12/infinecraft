using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BlockRLH.Actors.ActorComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class HudComponent : RenderComponent
    {
        //SpriteFont font;

        public HudComponent(Actor owner) : base(owner)
        {  }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // draw hud here, for now DEBUG INFO:
        }
    }
}
