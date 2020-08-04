using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{
    class DebugPlatform : Solid
    {

        public DebugPlatform(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(200, 200);
            Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            Move(new Vector2((float)(25.0 *state.getSignedDeltaTime()), 0));
        }


    }
}
