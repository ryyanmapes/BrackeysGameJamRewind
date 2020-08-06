using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public abstract class PhysicsEntity : Entity
    {
        //protected Vector2 velocity;
        // should entity really have a velocity?

        protected float maxVelocityY = 800f; // downward
        protected float minVelocityY = -2000f; // upward
        protected float maxVelocityMagnitudeX = 4600f;
        protected Vector2 aerialDrag = new Vector2(500f,100f);
        protected float groundedXFriction = 1000f;
        protected float gravitationalAcceleration = 1250f;

        protected bool is_grounded = false;

        public override void Update(StateData state)
        {


            float elapsed = (float)state.getDeltaTime();

            moveX(velocity.X * elapsed);
            moveY(velocity.Y * elapsed);

            // our main mechanic will have to do something about this
            // will this actually work just as expected if elapsed is negative?!

            if (is_grounded)
            {
                velocity.X = addMagnitude(velocity.X, -groundedXFriction*elapsed);
            }
            else
            {
                velocity.X = addMagnitude(velocity.X, -aerialDrag.X*elapsed);
                velocity.Y = addMagnitude(velocity.Y, -aerialDrag.Y*elapsed);
            }

            velocity.Y += gravitationalAcceleration* elapsed;

            velocity = new Vector2(minMagnitude(velocity.X, maxVelocityMagnitudeX), Math.Clamp(velocity.Y, minVelocityY, maxVelocityY));

            is_grounded = isGrounded();
        }

        public bool isGrounded()
        {
            var box = getCollisionBox();
            box.Y += 1;
            if (localLevel.getSolidCollisionAt(box, MoveDirection.down).type == CollisionType.normal)
            {
                return true;
            }
            return false;
        }

        public static float addMagnitude(float i, float addend)
        {
            if (i > 0) return Math.Max(0, i + addend);
            if (i < 0) return Math.Min(0, i - addend);
            return 0;
        }
        public static float minMagnitude(float i, float cap)
        {
            if (i > 0) return Math.Min(i, cap);
            if (i < 0) return Math.Max(i, -cap);
            return i;
        }

        public static float maxMagnitude(float i, float cap)
        {
            if (i > 0) return Math.Max(i, cap);
            if (i < 0) return Math.Min(i, -cap);
            return i;
        }
    }
}
