using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;

namespace RewindGame.Game.Debug
{
    public abstract class Platform : Solid
    {

        public Vector2 velocity;

        public void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, int tilelen)
        {
            velocity = velocity_;

            collisionSize = new Vector2(tilelen * Level.TILE_WORLD_SIZE, Level.SEMISOLID_THICKNESS);
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            Move(new Vector2(velocity.X * state.getSignedDeltaTime(), velocity.Y * state.getSignedDeltaTime()));
        }

    }
}
