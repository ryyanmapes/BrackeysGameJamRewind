using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using Microsoft.Xna.Framework;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Abstract
{
    public interface ITile
    {
        public TileSprite tile_sprite
        {
            get;
            set;
        }


        public abstract void Initialize(Level level, Vector2 starting_pos);

        public abstract void LoadContent();

        public abstract void Update(StateData state);

        public abstract void Draw(StateData state, SpriteBatch sprite_batch);

    }
}
