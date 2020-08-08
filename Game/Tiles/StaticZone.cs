using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class StaticZone : SolidTile
    {
        public new static StaticZone Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new StaticZone();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            collisionType = CollisionType.timestop;
            collisionPriority = 1;
            base.Initialize(level, starting_pos, tile_sprite_);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }
    }
}
