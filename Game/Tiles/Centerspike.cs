using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class Centerspike : SolidTile
    {
        public new static Centerspike Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new Centerspike();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            // todo should this hitbox be smaller?
            collisionType = CollisionType.death;
            base.Initialize(level, starting_pos, tile_sprite_);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }
    }
}
