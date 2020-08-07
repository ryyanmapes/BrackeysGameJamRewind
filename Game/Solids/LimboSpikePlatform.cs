using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class LimboSpikePlatform : CollisionObject
    {

        protected AnimationPlayer anims;
        protected Vector2 velocity;

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

            starting_pos -= new Vector2(0, 0.5f*Level.TILE_WORLD_SIZE);
            collisionOffset = new Vector2(0, 0.25f * Level.TILE_WORLD_SIZE);
            collisionSize = new Vector2(Level.TILE_WORLD_SIZE * (is_long ? 4 : 2), Level.TILE_WORLD_SIZE);
            velocity = velocity_;

            base.Initialize(level, starting_pos);

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, localLevel.parentGame.Content);
        }

        public override void Update(StateData state)
        {
            position += new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime());
        }

        public override void LoadContent() { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            anims.Draw(state, sprite_batch, position, velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, state.getTimeN());
        }

    }
}
