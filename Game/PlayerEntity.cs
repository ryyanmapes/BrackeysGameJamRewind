using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    class PlayerEntity : PhysicsEntity
    {

        protected float jumpVelocity = -175f;
        protected float moveVelocity = 3750f;

        //todo stuff make this correct
        public PlayerEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";
            collisionSize = new Vector2(20, 20);
            Initialize(level, starting_pos);
        }


        public override void Update(GameTime game_time)
        {
            float elapsed = (float)game_time.ElapsedGameTime.TotalSeconds;

            // todo this is a mess
            InputData input_data = localLevel.parentGame.inputData;

            if (input_data.is_jump_pressed && isGrounded())
            {
                riddenObject = null;
                velocity.Y += jumpVelocity;
            }
            //this code is not broken. even if the values are negative you still move right?
            if (Math.Abs(input_data.horizontal_axis_value) > 0.4f)
            {
                velocity.X = input_data.horizontal_axis_value * moveVelocity * elapsed;
            }

            base.Update(game_time);

        }

    }
}
