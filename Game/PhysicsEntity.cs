using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    class PhysicsEntity : Entity
    {
        //protected Vector2 velocity;
        // should entity really have a velocity?

        protected Vector2 terminalVelocity = new Vector2(3750, 250);
        protected Vector2 aerialDrag = new Vector2(75f,75f);
        protected float groundedXFriction = 250f;
        protected float gravitationalAcceleration = 400f;

        public override void Update(StateData state)
        {
            float elapsed = (float)state.getDeltaTime();
            // our main mechanic will have to do something about this
            // will this actually work just as expected if elapsed is negative?!

            if (isGrounded())
            {
                velocity.X = addMagnitude(velocity.X, -groundedXFriction*elapsed);
            }
            else
            {
                velocity.X = addMagnitude(velocity.X, -aerialDrag.X*elapsed);
                velocity.Y = addMagnitude(velocity.Y, -aerialDrag.Y*elapsed);
            }

            velocity.Y += gravitationalAcceleration* elapsed;

            velocity = new Vector2(capMagnitude(velocity.X, terminalVelocity.X), capMagnitude(velocity.Y, terminalVelocity.Y));

            moveX(velocity.X * elapsed, SecondaryCollisionType.none);
            moveY(velocity.Y * elapsed, SecondaryCollisionType.none);
        }

        public bool isGrounded()
        {
            return riddenObject != null;
        }

        public static float addMagnitude(float i, float addend)
        {
            if (i > 0) return Math.Max(0, i + addend);
            if (i < 0) return Math.Min(0, i - addend);
            return i;
        }
        public static float capMagnitude(float i, float cap)
        {
            if (i > 0) return Math.Min(i, cap);
            if (i < 0) return Math.Max(i, -cap);
            return i;
        }
    }
}
