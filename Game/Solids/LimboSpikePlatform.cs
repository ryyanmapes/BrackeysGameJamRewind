using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class LimboSpikePlatform : Platform
    {

        protected AnimationPlayer anims;

        public static LimboSpikePlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new LimboSpikePlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {
            collisionType = CollisionType.death;
            Animation anim;
            if (is_long)
            {

                anim = new Animation("limbo/4spikeplatform", 1, 1, true);

            }
            else
            {
                anim = new Animation("limbo/2spikeplatform", 1, 1, true);
            }

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, level.Content);

            starting_pos -= new Vector2(0, 0.5f*Level.TILE_WORLD_SIZE);
            collisionOffset = new Vector2(0, 0.5f * Level.TILE_WORLD_SIZE);

            base.Initialize(level, starting_pos, velocity_, is_long? 4 : 2);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            anims.Draw(state, sprite_batch, position, velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, state.getTimeN());
        }

    }
}
