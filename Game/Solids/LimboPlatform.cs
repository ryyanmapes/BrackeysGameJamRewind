using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class LimboPlatform : Platform
    {

        protected AnimationPlayer anims;

        public static LimboPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new LimboPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {
            Animation anim;
            if (is_long)
            {
                if (velocity_.Y < 0)
                {
                    anim = new Animation("limbo/platformlongdown", 6, 4, true);
                    collisionOffset = new Vector2(0, Level.TILE_WORLD_SIZE + 16);
                }
                else
                {
                    anim = new Animation("limbo/platformlongup", 6, 4, true);
                }
            }
            else
            {
                if (velocity_.Y > 0)
                {
                    anim = new Animation("limbo/platformdown", 6, 4, true);
                    collisionOffset = new Vector2(0, Level.TILE_WORLD_SIZE + 8);
                }
                else
                {
                    anim = new Animation("limbo/platformup", 6, 4, true);
                }
            }

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, level.Content);

            doLeftCollision = false;
            doRightCollision = false;
            doDownCollision = false;

            base.Initialize(level, starting_pos, velocity_, is_long? 4 : 2);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            anims.Draw(state, sprite_batch, position, velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, state.getTimeN());
        }

        
        public override CollisionReturn getCollision(FRectangle rect, MoveDirection direction)
        {
            var return_c = base.getCollision(rect, MoveDirection.none);
            if (return_c.priority == 0) return return_c;
            if (direction == MoveDirection.up && rect.Y + rect.Height < position.Y + Level.SEMISOLID_THICKNESS_WINDOW + collisionOffset.Y)
                return new CollisionReturn(CollisionType.refresh_jump, this, 2);
            if (direction != MoveDirection.down || rect.Y + rect.Height > position.Y + Level.SEMISOLID_THICKNESS + collisionOffset.Y)
                return CollisionReturn.None();
            return return_c;
        }
        
        public override Vector2 getEntityOverlap(Entity entity)
        {
            var rect = entity.getCollisionBox();
            if (rect.Y + rect.Height > position.Y + Level.SEMISOLID_THICKNESS + collisionOffset.Y)
                return Vector2.Zero;
            var overlap = entity.getOverlap(this);
            return entity.getOverlap(this);
        }

    }
}
