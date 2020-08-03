using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    // mostly just copy pasted from PhysicsEntity
    class TimePhysicsEntity : TimeEntity
    {
        protected Vector2 terminalVelocity = new Vector2(3750, 250);
        protected Vector2 aerialDrag = new Vector2(75f, 75f);
        protected float groundedXFriction = 250f;
        protected float gravitationalAcceleration = 400f;

        public override void Update(StateData state)
        {
            float elapsed = (float)state.getSignedDeltaTime();
            // this is the only difference between this and normal physics entity
            // this makes physics go backwards when timestate is backwards

            if (isGrounded())
            {
                velocity.X = PhysicsEntity.addMagnitude(velocity.X, -groundedXFriction * elapsed);
            }
            else
            {
                velocity.X = PhysicsEntity.addMagnitude(velocity.X, -aerialDrag.X * elapsed);
                velocity.Y = PhysicsEntity.addMagnitude(velocity.Y, -aerialDrag.Y * elapsed);
            }

            velocity.Y += gravitationalAcceleration * elapsed;

            velocity = new Vector2(PhysicsEntity.capMagnitude(velocity.X, terminalVelocity.X), PhysicsEntity.capMagnitude(velocity.Y, terminalVelocity.Y));

            moveX(velocity.X * elapsed, SecondaryCollisionType.none);
            moveY(velocity.Y * elapsed, SecondaryCollisionType.none);
        }

        public bool isGrounded()
        {
            return riddenObject != null;
        }


    }
}
