using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class LeftWallspike : RenderedSolidTile
    {
        public new static LeftWallspike Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new LeftWallspike();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            collisionType = CollisionType.death;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.X = GameUtils.WALLSPIKE_THICKNESS;
        }

    }
}
