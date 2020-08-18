using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;

namespace RewindGame.Game.Solids
{
    public abstract class Platform : Solid
    {

        public Vector2 velocity;
        public Vector2 startingPosition;

        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, int tilelen)
        {
            velocity = velocity_;

            doWallCarry = false;

            collisionSize = new Vector2(tilelen * GameUtils.TILE_WORLD_SIZE, GameUtils.SEMISOLID_THICKNESS);
            startingPosition = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            Move(new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime()));
        }

        public override void LoadContent(){ }

        public override void Reset()
        {
            position = startingPosition;
            base.Reset();
        }
    }
}
