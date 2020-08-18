using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class RightOnewayTile : SolidTile
    {

        public new static RightOnewayTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new RightOnewayTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            collisionDirection = MoveDirection.left;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.X = GameUtils.SEMISOLID_THICKNESS;
            collisionOffset.X = GameUtils.TILE_WORLD_SIZE - GameUtils.SEMISOLID_THICKNESS;
        }
        
        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (rect.X < position.X + GameUtils.TILE_WORLD_SIZE - GameUtils.SEMISOLID_THICKNESS) return false;
            return base.isThisOverlapping(rect, direction);
        }

    }
}
