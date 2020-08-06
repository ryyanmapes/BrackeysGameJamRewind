using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RewindGame.Game
{

    public class FRectangle
    {
        public FRectangle(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Height = h;
            Width = w;
        }

        public Rectangle toRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public float X;
        public float Y;
        public float Height;
        public float Width;
    }


    public abstract class CollisionObject : GameObject
    {
        protected Vector2 collisionSize;
        protected Vector2 collisionOffset = Vector2.Zero;

        protected PrimaryCollisionType collisionType = PrimaryCollisionType.normal;
        protected int collisionPriority = 5;


        // for semisolids: what direction does this exclusively stop?
        // none makes it a normal platform
        protected MoveDirection collisionDirection = MoveDirection.none;

        public FRectangle getCollisionBox()
        {
            return getCollisionBoxAt(position);
        }

        public virtual FRectangle getCollisionBoxAt(Vector2 new_position)
        {
            return new FRectangle(new_position.X + collisionOffset.X, new_position.Y + collisionOffset.Y, collisionSize.X, collisionSize.Y);
        }

        public virtual CollisionReturn getCollision(FRectangle rect, MoveDirection direction)
        {
            if (isThisOverlapping(rect, direction)) return getCollisionReturn();
            return CollisionReturn.None();
        }

        public bool isThisOverlapping(CollisionObject obj)
        {
            return isThisOverlapping(obj.getCollisionBox());
        }

        public virtual bool isThisOverlapping(FRectangle rect)
        {
            return GetIntersectionDepth(this.getCollisionBox(), rect) != Vector2.Zero;
        }

        public bool isThisOverlapping(CollisionObject obj, MoveDirection direction)
        {
            return isThisOverlapping(obj.getCollisionBox(), direction);
        }

        public virtual bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (collisionDirection == MoveDirection.none || direction == collisionDirection)
                return isThisOverlapping(rect);
            return false;
        }

        public Vector2 getOverlap(CollisionObject obj)
        {
            return GetIntersectionDepth(this.getCollisionBox(), obj.getCollisionBox());
        }

        // Code inspired by https://github.com/MonoGame/MonoGame.Samples/blob/develop/Platformer2D/Game/RectangleExtensions.cs
        // Returns the amount of overlap in each component
        public static Vector2 GetIntersectionDepth(FRectangle rectA, FRectangle rectB)
        {
            Vector2 center_a = getRectangleCenter(rectA);
            Vector2 center_b = getRectangleCenter(rectB);

            Vector2 minimum_intersection_distance_components = new Vector2((rectA.Width + rectB.Width) / 2.0f, (rectA.Height + rectB.Height) / 2.0f);

            Vector2 center_distance_components = new Vector2(center_a.X - center_b.X, center_a.Y - center_b.Y);


            if (Math.Abs(center_distance_components.X) >= minimum_intersection_distance_components.X
                || Math.Abs(center_distance_components.Y) >= minimum_intersection_distance_components.Y)
                // We return (0,0) if the rects are not intersecting at all.
                return Vector2.Zero;


            Vector2 depth_components = Vector2.Zero;
            depth_components.X = center_distance_components.X > 0
                ? minimum_intersection_distance_components.X - center_distance_components.X
                : -minimum_intersection_distance_components.X - center_distance_components.X;
            depth_components.Y = center_distance_components.Y > 0
                ? minimum_intersection_distance_components.Y - center_distance_components.Y
                : -minimum_intersection_distance_components.Y - center_distance_components.Y;

            return depth_components;

        }

        public static Vector2 getRectangleCenter(FRectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
        }

        public static Vector2 getRectangleBottomCenter(FRectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f , rect.Y + rect.Height);
        }

        public new virtual void Draw(StateData state, SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(texture, getCollisionBox().toRectangle(), textureColor );
        }


        public virtual PrimaryCollisionType getCollisionType()
        {
            return collisionType;
        }

        public virtual CollisionReturn getCollisionReturn()
        {
            return new CollisionReturn(collisionType, this, collisionPriority);
        }

    }
}
