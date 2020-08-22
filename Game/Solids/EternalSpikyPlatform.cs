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

    class EternalSpikePlatform : LimboSpikePlatform
    {

        protected IRenderMethod renderSmall = new AnimationPlayer("eternal/terrariumspikeplatform", 2, 2, true, 1, Vector2.Zero);
        protected IRenderMethod renderLarge = new AnimationPlayer("eternal/terrariumspikeplatformlong", 2, 2, true, 1, Vector2.Zero);

        public new static EternalSpikePlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new EternalSpikePlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public override void Update(StateData state)
        {
            int sign = state.time_data.time_moment < 0 ? -1 : 1;

            position = position += new Vector2(velocity.X * state.getTimeDependentDeltaTime() * sign, velocity.Y * state.getTimeDependentDeltaTime() * sign);
        }

    }
}
