using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class LeftOnewayTile : SolidTile
    {
        public new static LeftOnewayTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new LeftOnewayTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            collisionDirection = MoveDirection.right;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.X = Level.SEMISOLID_THICKNESS;
        }
        
        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (rect.X + rect.Width > position.X + Level.SEMISOLID_THICKNESS) return false;
            return base.isThisOverlapping(rect, direction);
        }

    }
}
