using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class EternalSpikePlatform : CollisionObject
    {

        protected AnimationPlayer anims;
        protected Vector2 velocity;
        protected Vector2 startingPos;

        private Animation idle = new Animation("eternal/terrariumspikeplatform", 4, 2, true);
        private Animation idleLong = new Animation("eternal/terrariumspikeplatformlong", 4, 2, true);

        public static EternalSpikePlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new EternalSpikePlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {
            collisionType = CollisionType.death;

            anims = new AnimationPlayer(is_long ? idleLong : idle, 1, Vector2.Zero, level.parentGame.Content);

            //starting_pos -= new Vector2(0, 0.5f*Level.TILE_WORLD_SIZE);
            //collisionOffset = new Vector2(0, 0.25f * Level.TILE_WORLD_SIZE);
            collisionSize = new Vector2(Level.TILE_WORLD_SIZE * (is_long ? 4 : 2), Level.TILE_WORLD_SIZE);
            velocity = velocity_;
            startingPos = starting_pos;

            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            int sign = state.time_data.time_moment < 0 ? -1 : 1;

            position = position += new Vector2(velocity.X * state.getTimeDependentDeltaTime() * sign, velocity.Y * state.getTimeDependentDeltaTime() * sign);
        }

        public override void LoadContent() { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            anims.Draw(state, sprite_batch, position, SpriteEffects.None, state.getTimeN());
        }

        public override void Reset()
        {
            position = startingPos;
            base.Reset();
        }

    }
}
