﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class RightWallspike : RenderedSolidTile
    {

        public new static RightWallspike Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new RightWallspike();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            collisionType = CollisionType.death;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.X = GameUtils.WALLSPIKE_THICKNESS;
            collisionOffset.X = GameUtils.TILE_WORLD_SIZE - GameUtils.WALLSPIKE_THICKNESS;
        }

    }
}
