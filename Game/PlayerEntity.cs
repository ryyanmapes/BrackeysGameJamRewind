using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    class PlayerEntity : PhysicsEntity
    {

        protected float jumpVelocity = -150f;
        protected float moveVelocity = 3750f;
        protected float heldJump = -20f;
        protected float heldTime = -1f;

        //todo stuff make this correct
        public PlayerEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(20, 20);
            Initialize(level, starting_pos);
            terminalVelocity.X = 75f;
        }


        public override void Update(StateData state)
        {
            float elapsed = (float)state.game_time.ElapsedGameTime.TotalSeconds;

            // todo this is a mess
            InputData input_data = state.input_data;

            if (input_data.is_jump_held && isGrounded())
            {
                riddenObject = null;
                velocity.Y += jumpVelocity;
            } else if(input_data.is_jump_held && heldTime < 2f) {
                velocity.Y += heldJump;
                heldTime += 0.75f;
            } else if(isGrounded())
            {
                heldTime = -1f;
            }
            //this code is not broken. even if the values are negative you still move right?
            if (Math.Abs(input_data.horizontal_axis_value) > 0.4f)
            {
                velocity.X += input_data.horizontal_axis_value * moveVelocity * elapsed;
            }

            base.Update(state);

        }

    }
}
