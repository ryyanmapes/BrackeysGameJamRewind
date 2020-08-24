using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{
    public abstract class SimpleSolid : Solid
    {

        public Vector2 targetDisplacement;
        public Vector2 startingPosition;

        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, Vector2 size)
        {
            targetDisplacement = velocity_;
            collisionSize = size;
            startingPosition = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            Vector2 true_velocity = targetDisplacement / state.time_bound.max * state.getTimeSign();
            Move(true_velocity);
        }

        public override void Reset()
        {
            position = startingPosition;
            base.Reset();
        }
    }
}
