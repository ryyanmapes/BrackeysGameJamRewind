using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class PlatformTile : SolidTile
    {
        public new static PlatformTile Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new PlatformTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            //collisionDirection = MoveDirection.down;
            base.Initialize(level, starting_pos, tile_sprite_);
            collisionSize.Y = GameUtils.SEMISOLID_THICKNESS;
        }

        public override CollisionReturn getCollision(FRectangle rect, MoveDirection direction)
        {
            var return_c = base.getCollision(rect, MoveDirection.none);
            if (return_c.priority == 0) return return_c;
            if (direction == MoveDirection.up && rect.Y + rect.Height < position.Y + GameUtils.SEMISOLID_THICKNESS_WINDOW)
                return new CollisionReturn(CollisionType.refresh_jump, this, 2);
            if (direction != MoveDirection.down || rect.Y + rect.Height > position.Y + GameUtils.SEMISOLID_THICKNESS)
                return CollisionReturn.None();
            return return_c;
        }

    }
}
