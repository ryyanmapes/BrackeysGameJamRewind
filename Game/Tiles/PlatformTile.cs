using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    class PlatformTile : SolidTile
    {
        public new static PlatformTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new PlatformTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            collisionDirection = MoveDirection.down;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.Y = Level.SEMISOLID_THICKNESS;
        }

        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (rect.Y + rect.Height > position.Y + Level.SEMISOLID_THICKNESS) return false;
            return base.isThisOverlapping(rect, direction);
        }

    }
}
