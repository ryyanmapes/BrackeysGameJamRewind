using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    // mostly just copy pasted from PhysicsEntity
    public abstract class TimePhysicsEntity : TimeEntity
    {
        protected Vector2 terminalVelocity = new Vector2(3750, 250);
        protected Vector2 aerialDrag = new Vector2(75f, 75f);
        protected float groundedXFriction = 250f;
        protected float gravitationalAcceleration = 400f;

        public override void Update(StateData state)
        {
            float elapsed_signed = (float)state.getSignedDeltaTime();
            //float elapsed = (float)state.getDeltaTime();
            // this is the only difference between this and normal physics entity
            // this makes physics go backwards when timestate is backwards

            
            if (isGrounded())
            {
                velocity.X = PhysicsEntity.addMagnitude(velocity.X, -groundedXFriction * elapsed_signed);
            }
            else
            {
                velocity.X = PhysicsEntity.addMagnitude(velocity.X, -aerialDrag.X * elapsed_signed);
                velocity.Y = PhysicsEntity.addMagnitude(velocity.Y, -aerialDrag.Y * elapsed_signed);
            }

            velocity.Y = Math.Max(velocity.Y + gravitationalAcceleration * elapsed_signed, 0);

            velocity = new Vector2(PhysicsEntity.minMagnitude(velocity.X, terminalVelocity.X), PhysicsEntity.minMagnitude(velocity.Y, terminalVelocity.Y));

            moveX(velocity.X * elapsed_signed);
            moveY(velocity.Y * elapsed_signed);
        }

        public bool isGrounded()
        {
            return riddenObject != null;
        }


    }
}
