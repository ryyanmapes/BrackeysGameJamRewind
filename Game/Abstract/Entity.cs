using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace RewindGame.Game
{

    public enum PrimaryCollisionType
    {
        harmless,
        normal,
        death,
        refresh_jump,
        timestop
    }
    public enum SecondaryCollisionType
    {
        none,
        squish
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
        public CollisionReturn(PrimaryCollisionType type_, CollisionObject collisionee_, int priority_)
        {
            type = type_;
            collisionee = collisionee_;
            priority = priority_;
        }

        public PrimaryCollisionType type = PrimaryCollisionType.harmless;
        public CollisionObject collisionee;
        public int priority;

        public static CollisionReturn None()
        {
            return new CollisionReturn(PrimaryCollisionType.harmless, null, 0);
        }
    }

    public abstract class Entity : CollisionObject
    {
        protected Vector2 moveRemainder;

        protected Vector2 velocity;
        protected CollisionObject riddenObject;
        protected CollisionObject hungObject;
        protected HangDirection hangDirection = HangDirection.None;

        // Code inspired by https://medium.com/@MattThorson/celeste-and-towerfall-physics-d24bd2ae0fc5
        public void moveX(float amount, SecondaryCollisionType onSecondCollision)
        {
            float real_amount = amount + moveRemainder.X;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.X = real_amount - move;

            int sign = move > 0 ? 1 : -1;
            MoveDirection dir = move > 0 ? MoveDirection.right : MoveDirection.left;

            while (move != 0)
            {
                hungObject = null;
                hangDirection = HangDirection.None;

                Vector2 new_position = position + new Vector2(sign, 0);

                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position), dir);

                switch (collision.type)
                {
                    case PrimaryCollisionType.normal:
                        if (onSecondCollision == SecondaryCollisionType.squish)
                        {
                            Die();
                        }
                        else
                        {
                            velocity.X = 0;
                            hungObject = collision.collisionee;
                            hangDirection = sign > 0 ? HangDirection.Right : HangDirection.Left;
                        }
                        return;
                    case PrimaryCollisionType.death:
                        Die();
                        break;
                }

                position = new_position;
                move -= sign;
            }

        }

        public void moveY(float amount, SecondaryCollisionType onSecondCollision)
        {
            float real_amount = amount + moveRemainder.Y;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.Y = real_amount - move;

            int sign = move > 0 ? 1 : -1;
            MoveDirection dir = move > 0 ? MoveDirection.down : MoveDirection.up;

            while (move != 0)
            {
                riddenObject = null;

                Vector2 new_position = position + new Vector2(0, sign);

                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position), dir);

                switch ( collision.type )
                {
                    case PrimaryCollisionType.normal:
                        if (onSecondCollision == SecondaryCollisionType.squish)
                        {
                            Die();
                        }
                        else
                        {
                            velocity.Y = 0;
                            riddenObject = collision.collisionee;
                        }
                        return;
                    case PrimaryCollisionType.death:
                        Die();
                        break;
                    case PrimaryCollisionType.refresh_jump:
                        RefreshJump();
                        break;
                }

                position = new_position;
                move -= sign;
            }

        }


        public virtual void Die() { }

        public virtual void RefreshJump() { }


        public bool isRiding(CollisionObject obj)
        {
            return obj == riddenObject;
        }


    }
}
