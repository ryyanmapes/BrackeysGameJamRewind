using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public class PlayerEntity : PhysicsEntity
    {
        public static int playerAnimScale = 1;
        public static Vector2 playerAnimationOffset = new Vector2(-18, -28);

        protected AnimationChooser animator = new AnimationChooser(playerAnimScale, playerAnimationOffset);
        protected Animation idleAnim = new Animation("faux/fauxidle", 6, 8, true);
        protected Animation walkAnim = new Animation("faux/fauxwalk", 4, 12, true);
        protected Animation fallAnim = new Animation("faux/fauxfallnormal", 1, 1, true);
        protected Animation jumpRewindAnim = new Animation("faux/fauxjumprewind", 3, 6, true);
        protected Animation fallRewindAnim = new Animation("faux/fauxfallrewind", 3, 6, true);


        public bool facingRight = true;

        protected float jumpInitialVelocity = -470f;
        protected float moveVelocity = 2500f;
        protected float heldJumpVelocity = -1500f;
        protected float heldJumpVelocityModifier = 0f;
        protected float maxJumpHoldTime = 0.1f;

        protected float jumpHeldTime = -1f;
        public bool isRewinding = false;

        //todo stuff make this correct
        public PlayerEntity(Level level, Vector2 starting_pos)
        {
            texturePath = "debug/square";

            animator.addAnimaton(idleAnim, "idle",  level.parentGame.Content);
            animator.addAnimaton(walkAnim, "walk",  level.parentGame.Content);
            animator.addAnimaton(fallAnim, "fall", level.parentGame.Content);
            animator.addAnimaton(jumpRewindAnim, "rewind_jump",  level.parentGame.Content);
            animator.addAnimaton(fallRewindAnim, "rewind_fall",  level.parentGame.Content);
            animator.changeAnimation("idle");

            collisionSize = new Vector2(56, 56);
            Initialize(level, starting_pos);
            maxVelocityMagnitudeX = 200f;
        }


        public override void Update(StateData state)
        {
            UpdateAnimations();

            float elapsed = (float)state.getDeltaTime();

            // todo this is a mess
            InputData input_data = state.input_data;

            if (input_data.is_jump_held && isGrounded())
            {
                riddenObject = null;
                velocity.Y += jumpInitialVelocity;
                jumpHeldTime = 0f;
                isRewinding = true;
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

            if (isGrounded()) isRewinding = false;

            if (Math.Abs(input_data.horizontal_axis_value) > 0.4f)
            {
                velocity.X += input_data.horizontal_axis_value * moveVelocity * elapsed;
            }

            if (velocity.X > 0) facingRight = true;
            else if (velocity.X < 0) facingRight = false;

            base.Update(state);

        }

        public void UpdateAnimations()
        {
            if (isGrounded())
            {
                if (Math.Abs(velocity.X) > 100)
                {
                    animator.changeAnimation("walk");
                }
                else
                {
                    animator.changeAnimation("idle");
                }
            }
            else
            {
                if (velocity.Y < 0)
                {
                    animator.changeAnimation("rewind_jump");
                }
                else
                {
                    if (isRewinding)
                    {
                        animator.changeAnimation("rewind_fall");
                    }
                    else
                    {
                        animator.changeAnimation("fall");
                    }
                }
            }
        }


        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            animator.Draw(state, sprite_batch, position, facingRight? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }

    }
}
