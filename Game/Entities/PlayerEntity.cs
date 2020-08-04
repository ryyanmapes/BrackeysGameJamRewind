using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public class PlayerEntity : PhysicsEntity
    {

        protected float jumpInitialVelocity = -470f;
        protected float moveVelocity = 2500f;
        protected float heldJumpVelocity = -1500f;
        protected float heldJumpVelocityModifier = 0f;
        protected float maxJumpHoldTime = 0.1f;

        protected float jumpHeldTime = -1f;

        //todo stuff make this correct
        public PlayerEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(40, 56);
            Initialize(level, starting_pos);
            maxVelocityMagnitudeX = 200f;
        }


        public override void Update(StateData state)
        {
            float elapsed = (float)state.getDeltaTime();

            // todo this is a mess
            InputData input_data = state.input_data;

            if (input_data.is_jump_held && isGrounded())
            {
                riddenObject = null;
                velocity.Y += jumpInitialVelocity;
                jumpHeldTime = 0f;
            } else if(input_data.is_jump_held && jumpHeldTime != -1 && jumpHeldTime <= maxJumpHoldTime && velocity.Y != 0) {
                velocity.Y += (heldJumpVelocity  - jumpHeldTime*heldJumpVelocityModifier  ) * elapsed;
                jumpHeldTime += elapsed;
            } else if (!input_data.is_jump_held && jumpHeldTime != -1) {
                velocity.Y *= 0.5f;
                jumpHeldTime = -1f;
            }
            else if (jumpHeldTime != -1) {
                jumpHeldTime = -1f;
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
