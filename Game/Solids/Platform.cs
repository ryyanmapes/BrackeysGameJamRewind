using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class Platform : MovingSolid
    {

        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, float tile_len)
        {
            collisionSize = new Vector2(tile_len * GameUtils.TILE_WORLD_SIZE, GameUtils.SEMISOLID_THICKNESS);

            doLeftCollision = false;
            doRightCollision = false;
            doDownCollision = false;

            base.Initialize(level, starting_pos, velocity_, collisionSize);
        }
        
        public override CollisionReturn getCollision(FRectangle rect, MoveDirection direction)
        {
            var return_c = base.getCollision(rect, MoveDirection.none);
            if (return_c.priority == 0) return return_c;
            if (direction == MoveDirection.up && rect.Y + rect.Height < position.Y + GameUtils.SEMISOLID_THICKNESS_WINDOW + collisionOffset.Y)
                return new CollisionReturn(CollisionType.refresh_jump, this, 2);
            if (direction != MoveDirection.down || rect.Y + rect.Height > position.Y + GameUtils.SEMISOLID_THICKNESS + collisionOffset.Y)
                return CollisionReturn.None();
            return return_c;
        }
        
        public override Vector2 getEntityOverlap(Entity entity)
        {
            var rect = entity.getCollisionBox();
            if (rect.Y + rect.Height > position.Y + GameUtils.SEMISOLID_THICKNESS + collisionOffset.Y)
                return Vector2.Zero;
            var overlap = entity.getOverlap(this);
            return entity.getOverlap(this);
        }

    }
}
