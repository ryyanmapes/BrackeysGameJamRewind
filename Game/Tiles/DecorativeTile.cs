using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    class DecorativeTile : GameObject, ITile
    {
        public TileSprite tile_sprite { get; set; }

        public DecorativeTile(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            tile_sprite = tile_sprite_;
            Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state) { }

        public virtual void Draw(StateData state, SpriteBatch sprite_batch)
        {
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }

    }
}
