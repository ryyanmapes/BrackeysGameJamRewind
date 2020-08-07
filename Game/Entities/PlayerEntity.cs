using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;
using RewindGame.Game.Sound;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public enum GroundedReturn
    {
        no,
        grounded,
        floof_forwards,
        floof_backwards
    }

    public class PlayerEntity : PhysicsEntity
    {
        public static int playerAnimScale = 1;
        public static Vector2 playerAnimationOffset = new Vector2(-18, -28);

        protected AnimationChooser animator = new AnimationChooser(playerAnimScale, playerAnimationOffset);
        protected Animation idleAnim = new Animation("faux/fauxidle", 6, 8, true);
        protected Animation walkAnim = new Animation("faux/fauxwalk", 2, 12, true);
        protected Animation fallAnim = new Animation("faux/fauxfallnormal", 1, 1, true);
        protected Animation wallAnim = new Animation("faux/wall", 1, 1, true);
        protected Animation jumpRewindAnim = new Animation("faux/fauxjumprewind", 4, 6, true);
        protected Animation fallRewindAnim = new Animation("faux/fauxfallrewind", 4, 6, true);
        protected Animation deathAnim = new Animation("faux/death", 2, 9, false);

        protected RewindGame parentGame;

        public bool facingRight = true;

        protected float jumpLaunchVelocity = -470f;
        protected float moveVelocity = 2700f;
        protected float heldJumpVelocity = -1500f;
        protected float heldJumpVelocityModifier = 0f;
        protected float maxJumpHoldTime = 0.1f;
        protected float playerMaxMove = 350f;
        protected float wallHangMaxY = 175f;
        protected float wallHangStickX = 4f;
        protected float wallJumpLaunchVelocityY = -400f;
        protected float wallJumpLaunchVelocityX = 300f;
        protected float wallJumpRemainingHoldTime = 0.05f;
        protected float maxNoOppositeTravelTime = 0.4f;

        protected float jumpHeldTime = -1f;
        public bool isRewinding = false;
        public bool temporaryAllowJump = false;
        protected float noOppositeTravelTime = -1f;
        protected HangDirection noOppositeTravelDirection = HangDirection.None;
        public bool wasGroundedLastFrame = true;

        //todo stuff make this correct
        public PlayerEntity(RewindGame parent_game, Vector2 starting_pos)
        {
            texturePath = "debug/square";

            parentGame = parent_game;

            animator.addAnimaton(idleAnim, "idle",  parentGame.Content);
            animator.addAnimaton(walkAnim, "walk",  parentGame.Content);
            animator.addAnimaton(fallAnim, "fall", parentGame.Content);
            animator.addAnimaton(wallAnim, "wallhang", parentGame.Content);
            animator.addAnimaton(jumpRewindAnim, "rewind_jump",  parentGame.Content);
            animator.addAnimaton(fallRewindAnim, "rewind_fall",  parentGame.Content);
            animator.addAnimaton(deathAnim, "death", parentGame.Content);
            animator.changeAnimation("idle");

            collisionSize = new Vector2(35, 56);
            collisionOffset = new Vector2(16, 0);
            Initialize(parentGame.activeLevel, starting_pos);
        }


        public override void Update(StateData state)
        {

            localLevel = parentGame.activeLevel;

            float elapsed = (float)state.getDeltaTime();

            // todo this is a mess
            InputData input_data = state.input_data;
            if (temporaryAllowJump)
            {
                temporaryAllowJump = true;
            }

            if (grounded == GroundedReturn.grounded && velocity.Y >= 0)
            {
                //noOppositeTravelDirection = HangDirection.None;
                noOppositeTravelTime = -1f;
                isRewinding = false;

                if (!wasGroundedLastFrame)
                {
                    parentGame.soundManager.TriggerPlayerLand();
                }

                wasGroundedLastFrame = true;
            }
            else wasGroundedLastFrame = false;

            if (grounded == GroundedReturn.floof_forwards)
            {
                if (velocity.Y <= 0) Jump(false, true);
            }
            else if (grounded == GroundedReturn.floof_backwards)
            {
                if (velocity.Y <= 0) Jump(true, true);
            }
            if (input_data.is_jump_pressed && (grounded == GroundedReturn.grounded || temporaryAllowJump))
            {
                Jump(true);
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

            if (hangDirection != HangDirection.None)
            {
                noOppositeTravelDirection = HangDirection.None;
                velocity.Y = Math.Min(velocity.Y, wallHangMaxY);
                isRewinding = false;
                if (hangDirection == HangDirection.Right)
                {
                    velocity.X = Math.Max(velocity.X, wallHangStickX);
                }
                else
                {
                    velocity.X = Math.Min(velocity.X, -wallHangStickX);
                }

                if (input_data.is_jump_pressed)
                {
                    velocity.Y = wallJumpLaunchVelocityY;
                    velocity.X = wallJumpLaunchVelocityX * (hangDirection == HangDirection.Right ? -1 : 1);
                    jumpHeldTime = maxJumpHoldTime - wallJumpRemainingHoldTime;

                    noOppositeTravelDirection = hangDirection;
                    noOppositeTravelTime = 0;
                    isRewinding = true;
                }

            }

            bool can_move = true;

            
            if (noOppositeTravelTime != -1)
            {
                noOppositeTravelTime += elapsed;

                if (noOppositeTravelTime > maxNoOppositeTravelTime) noOppositeTravelTime = -1;

                if (noOppositeTravelTime != -1 && input_data.horizontal_axis_value < 0 &&
                (noOppositeTravelDirection == HangDirection.Left || noOppositeTravelDirection == HangDirection.None))
                    can_move = false;
                else if (noOppositeTravelTime != -1 && input_data.horizontal_axis_value > 0 &&
                    (noOppositeTravelDirection == HangDirection.Right || noOppositeTravelDirection == HangDirection.None))
                    can_move = false;
            }

            //if ((noOppositeTravelDirection == HangDirection.Left && input_data.horizontal_axis_value < 0)
            //    || (noOppositeTravelDirection == HangDirection.Right && input_data.horizontal_axis_value > 0)) 
            //    can_move = false;

            if (Math.Abs(input_data.horizontal_axis_value) > 0.4f && Math.Abs(velocity.X) < playerMaxMove && can_move)
            {
                velocity.X += input_data.horizontal_axis_value * moveVelocity * elapsed;
            }

            if (velocity.X > 0) facingRight = true;
            else if (velocity.X < 0) facingRight = false;

            UpdateAnimations();

            temporaryAllowJump = false;

            base.Update(state);

        }

        public void Jump(bool is_forwards, bool isFloof = false)
        {
            grounded = GroundedReturn.no;
            velocity.Y += jumpLaunchVelocity;
            velocity.Y = Math.Max(velocity.Y, jumpLaunchVelocity * 1.3f);
            jumpHeldTime = 0f;

            //todo poof sfx
            if (isFloof) parentGame.soundManager.TriggerPlayerJump();
            else parentGame.soundManager.TriggerPlayerJump();
            isRewinding = is_forwards;
        }

        public override void Die()
        {
            velocity = Vector2.Zero;
            parentGame.qued_player_death = true;
            animator.changeAnimation("death");
        }

        public override void RefreshJump() 
        {
            temporaryAllowJump = true;
        }

        public override GroundedReturn getGrounded()
        {
            var box = getCollisionBox();
            box.Y += 1;
            switch (localLevel.getSolidCollisionAt(box, MoveDirection.down).type)
            {
                case CollisionType.normal:
                    return GroundedReturn.grounded;
                case CollisionType.forward_floof:
                    return GroundedReturn.floof_forwards;
                case CollisionType.backward_floof:
                    return GroundedReturn.floof_backwards;
                default:
                    return GroundedReturn.no;
            }
        }


        public void UpdateAnimations()
        {
            if (parentGame.qued_player_death) { 
                animator.changeAnimation("death");
                return;
            }

            if (getGrounded() == GroundedReturn.grounded)
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
                    if (hangDirection != HangDirection.None)
                    {
                        animator.changeAnimation("wallhang");
                    }
                    else if (isRewinding)
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
