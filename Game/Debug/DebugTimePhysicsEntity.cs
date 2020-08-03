using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{
    class DebugTimePhysicsEntity : TimePhysicsEntity
    {

        public DebugTimePhysicsEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(40, 40);
            Initialize(level, starting_pos);
        }

    }
}
