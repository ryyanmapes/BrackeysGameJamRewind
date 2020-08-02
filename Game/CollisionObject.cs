using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RewindGame.Game
{

    abstract class CollisionObject : GameObject
    {
        protected Vector2 collisionSize;

        // for semisolids: what direction does this exclusively stop?
        // none makes it a normal platform
        protected MoveDirection collisionDirection = MoveDirection.none;

        public Rectangle getCollisionBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)collisionSize.X, (int)collisionSize.Y);
        }

        public Rectangle getCollisionBoxAt(Vector2 new_position)
        {
            return new Rectangle((int)new_position.X, (int)new_position.Y, (int)collisionSize.X, (int)collisionSize.Y);
        }

        public bool isThisOverlapping(CollisionObject obj)
        {
            return GetIntersectionDepth(this.getCollisionBox(), obj.getCollisionBox()) == Vector2.Zero;
        }

        public bool isThisOverlapping(Rectangle rect)
        {
            return GetIntersectionDepth(this.getCollisionBox(), rect) != Vector2.Zero;
        }

        public bool isThisOverlapping(CollisionObject obj, MoveDirection direction)
        {
            if (collisionDirection == MoveDirection.none || direction == collisionDirection)
                return isThisOverlapping(obj);
            return false;
        }

        public bool isThisOverlapping(Rectangle rect, MoveDirection direction)
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
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
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

        public static Vector2 getRectangleCenter(Rectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2.0f, rect.Top + rect.Height / 2.0f);
        }

        public static Vector2 getRectangleBottomCenter(Rectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2.0f , rect.Bottom);
        }

        public new virtual void Draw(GameTime game_time, SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(texture, getCollisionBox(), textureColor );
        }

    }
}
