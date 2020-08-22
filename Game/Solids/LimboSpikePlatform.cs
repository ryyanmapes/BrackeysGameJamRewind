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

        protected Vector2 velocity;
        protected Vector2 startingPos;

        protected IRenderMethod renderSmall = new AnimationPlayer("limbo/2spikeplatform", 1, 1, true, 1, Vector2.Zero);
        protected IRenderMethod renderLarge = new AnimationPlayer("limbo/4spikeplatform", 1, 1, true, 1, Vector2.Zero);

        public static LimboSpikePlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new LimboSpikePlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {
            collisionType = CollisionType.death;

            renderer = is_long ? renderLarge : renderSmall;

            starting_pos -= new Vector2(0, 0.5f*GameUtils.TILE_WORLD_SIZE);
            collisionOffset = new Vector2(0, 0.25f * GameUtils.TILE_WORLD_SIZE);
            collisionSize = new Vector2(GameUtils.TILE_WORLD_SIZE * (is_long ? 4 : 2), GameUtils.TILE_WORLD_SIZE);
            velocity = velocity_;
            startingPos = starting_pos;

            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            position += new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime());
        }

        public override void Reset()
        {
            position = startingPos;
            base.Reset();
        }

    }
}
