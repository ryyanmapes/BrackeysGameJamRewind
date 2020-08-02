using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{
    class DebugEntity : Entity
    {

        public DebugEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(20, 20);
            Initialize(level, starting_pos);
        }

        public override void Update(GameTime game_time)
        {
            moveY(1.0f, SecondaryCollisionType.none);
        }

    }
}
