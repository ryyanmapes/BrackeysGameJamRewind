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

        public GroundedReturn grounded = GroundedReturn.no;

        public CollisionObject hungObject;
        public HangDirection hangDirection = HangDirection.None;

        public override void Update(StateData state)
        {


            float elapsed = (float)state.getDeltaTime();

            base.Update(state);

            // our main mechanic will have to do something about this
            // will this actually work just as expected if elapsed is negative?!

            if (grounded == GroundedReturn.grounded)
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

            UpdateGrounded(state);

            UpdateWallHang(state);
        }

        public virtual void UpdateGrounded(StateData state)
        {
            var box = getCollisionBox();
            box.Y += 1;

            if (localLevel.getSolidCollisionAt(box, MoveDirection.down, linkedSolid).type == CollisionType.normal)
            {
                grounded = GroundedReturn.grounded;
            }
            else
            {
                grounded = GroundedReturn.no;
            }
        }

        public virtual void UpdateWallHang(StateData state)
        {

            if (velocity.X > 0 || hangDirection == HangDirection.Right)
            {
                var box = getCollisionBox();
                box.X += 1;
                var collision = localLevel.getSolidCollisionAt(box, MoveDirection.right, linkedSolid);
                if (collision.type == CollisionType.normal)
                {
                    hungObject = collision.collisionee;
                    hangDirection = HangDirection.Right;
                    return;
                }
            }
            else if (velocity.X < 0 || hangDirection == HangDirection.Left)
            {
                var box = getCollisionBox();
                box.X -= 1;
                var collision = localLevel.getSolidCollisionAt(box, MoveDirection.left, linkedSolid);
                if (collision.type == CollisionType.normal)
                {
                    hungObject = collision.collisionee;
                    hangDirection = HangDirection.Left;
                    return;
                }
            }

            hungObject = null;
            hangDirection = HangDirection.None;

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
