using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace RewindGame.Game
{

    enum PrimaryCollisionType
    {
        none,
        normal,
        death
    }
    enum SecondaryCollisionType
    {
        none,
        squish
    }

    class CollisionReturn
    {
        public CollisionReturn(PrimaryCollisionType type_, CollisionObject collisionee_)
        {
            type = type_;
            collisionee = collisionee_;
        }

        public PrimaryCollisionType type = PrimaryCollisionType.none;
        public CollisionObject collisionee;

        public static CollisionReturn None()
        {
            return new CollisionReturn(PrimaryCollisionType.none, null);
        }
    }

    abstract class Entity : CollisionObject
    {
        protected Vector2 moveRemainder;

        protected Vector2 velocity;
        protected CollisionObject riddenObject;

        // Code inspired by https://medium.com/@MattThorson/celeste-and-towerfall-physics-d24bd2ae0fc5
        public void moveX(float amount, SecondaryCollisionType onSecondCollision)
        {
            float real_amount = amount + moveRemainder.X;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.X = real_amount - move;

            int sign = move > 0 ? 1 : -1;

            while (move != 0)
            {
                Vector2 new_position = position + new Vector2(sign, 0);

                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position));

                switch (collision.type)
                {
                    case PrimaryCollisionType.normal:
                        if (onSecondCollision == SecondaryCollisionType.squish)
                        {
                            //todo die
                        }
                        else
                        {
                            velocity.X = 0;
                            // do collision- TODO walljumping here
                        }
                        return;
                    case PrimaryCollisionType.death:
                        // todo die
                        return;
                    case PrimaryCollisionType.none:
                        position = new_position;
                        move -= sign;
                        break;
                }
            }

        }

        public void moveY(float amount, SecondaryCollisionType onSecondCollision)
        {
            float real_amount = amount + moveRemainder.Y;
            int move = (int)Math.Floor(real_amount);
            moveRemainder.Y = real_amount - move;

            int sign = move > 0 ? 1 : -1;

            while (move != 0)
            {

                riddenObject = null;
                Vector2 new_position = position + new Vector2(0, sign);

                CollisionReturn collision = localLevel.getSolidCollisionAt(this.getCollisionBoxAt(new_position));

                switch ( collision.type )
                {
                    case PrimaryCollisionType.normal:
                        if (onSecondCollision == SecondaryCollisionType.squish)
                        {
                            //todo die
                        }
                        else
                        {
                            velocity.Y = 0;
                            riddenObject = collision.collisionee;
                        }
                        return;
                    case PrimaryCollisionType.death:
                        // todo die
                        return;
                    case PrimaryCollisionType.none:
                        position = new_position;
                        move -= sign;
                        break;
                }
            }

        }


        public bool isRiding(CollisionObject obj)
        {
            return obj == riddenObject;
        }


    }
}
