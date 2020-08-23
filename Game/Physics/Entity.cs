using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace RewindGame.Game
{

    public enum CollisionType
    {
        harmless,
        normal,
        death,
        refresh_jump,
        timestop,
        forward_floof,
        backward_floof,
        warp_trigger
    }

    public enum MoveDirection
    {
        none,
        up,
        down,
        left,
        right
    }

    public enum HangDirection
    {
        None,
        Left,
        Right
    }

    public class CollisionReturn
    {
        public CollisionReturn(CollisionType type_, CollisionObject collisionee_, int priority_)
        {
            type = type_;
            collisionee = collisionee_;
            priority = priority_;
        }

        public CollisionType type = CollisionType.harmless;
        public CollisionObject collisionee;
        public int priority;

        public static CollisionReturn None()
        {
            return new CollisionReturn(CollisionType.harmless, null, 0);
        }
    }

    public abstract class Entity : CollisionObject
    {
        protected Vector2 moveRemainder;

        public Vector2 startingPosition;
        public Vector2 velocity;

        protected Solid linkedSolid;

        public override void Initialize(Level level, Vector2 starting_pos)
        {
            startingPosition = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            float elapsed = (float)state.getDeltaTime();

            moveX(velocity.X * elapsed);
            moveY(velocity.Y * elapsed);

            if (linkedSolid != null) linkedSolid.MoveTo(position);
        }

            // Code inspired by https://medium.com/@MattThorson/celeste-and-towerfall-physics-d24bd2ae0fc5
        public void moveX(float amount) { moveX(amount, null); }
        public void moveX(float amount, Solid pusher)
        {
            float real_amount = amount + moveRemainder.X;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.X = real_amount - move;

            int sign = move > 0 ? 1 : -1;
            MoveDirection dir = move > 0 ? MoveDirection.right : MoveDirection.left;

            while (move != 0)
            {

                Vector2 new_position = position + new Vector2(sign, 0);

                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position), dir, linkedSolid);

                switch (collision.type)
                {
                    case CollisionType.normal:
                        if (pusher != null)
                        {
                            collision.collisionee = null;
                            Die();
                        }
                        else
                        {
                            velocity.X = 0;
                        }
                        return;
                    case CollisionType.death:
                        Die();
                        break;
                }

                position = new_position;
                move -= sign;
            }

        }

        public void moveY(float amount) { moveY(amount, null); }

        public void moveY(float amount, Solid pusher )
        {
            float real_amount = amount + moveRemainder.Y;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.Y = real_amount - move;

            int sign = move > 0 ? 1 : -1;
            MoveDirection dir = move > 0 ? MoveDirection.down : MoveDirection.up;

            while (move != 0)
            {

                Vector2 new_position = position + new Vector2(0, sign);

                // this used to include 'pusher'
                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position), dir, linkedSolid);

                switch ( collision.type )
                {
                    case CollisionType.normal:
                        if (pusher != null)
                        {
                            Die();
                        }
                        else
                        {
                            velocity.Y = 0;
                        }
                        return;
                    case CollisionType.death:
                        Die();
                        break;
                    case CollisionType.refresh_jump:
                        RefreshJump();
                        break;
                }

                position = new_position;
                move -= sign;
            }

        }


        public virtual void Die() { }

        public virtual void RefreshJump() { }


        public virtual bool isRiding(Solid solid)
        {
            if (solid == linkedSolid) return false;

            var box = getCollisionBox();
            box.Y += 1;
            if (solid.getCollision(box, MoveDirection.down).type == CollisionType.normal) return true;

            return false;
        }


        public override void Reset()
        {
            position = startingPosition;
            base.Reset();
        }

    }
}
