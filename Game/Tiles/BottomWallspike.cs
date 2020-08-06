using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class BottomWallspike : SolidTile
    {

        public new static BottomWallspike Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new BottomWallspike();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            collisionType = CollisionType.death;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.Y = Level.WALLSPIKE_THICKNESS;
            collisionOffset.Y = Level.TILE_WORLD_SIZE - Level.WALLSPIKE_THICKNESS;
        }

    }
}
