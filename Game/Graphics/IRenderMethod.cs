using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Graphics
{
    public interface IRenderMethod
    {
        public bool isHidden {
            get;
            set;
        }

        public void LoadContent(ContentManager Content);

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect);

        public void Draw(StateData state, SpriteBatch sprite_batch, Rectangle rect, SpriteEffects effect);

    }
}
