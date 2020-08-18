using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RewindGame.Game
{

    public interface ICollisionObject : IGameObject
    {

        public abstract FRectangle getCollisionBox();

        public abstract FRectangle getCollisionBoxAt(Vector2 new_position);

        public abstract CollisionReturn getCollision(FRectangle rect, MoveDirection direction);

        public abstract bool isThisOverlapping(CollisionObject obj);

        public abstract bool isThisOverlapping(FRectangle rect);

        public abstract bool isThisOverlapping(CollisionObject obj, MoveDirection direction);

        public abstract bool isThisOverlapping(FRectangle rect, MoveDirection direction);

        public abstract Vector2 getOverlap(CollisionObject obj);

        public abstract Vector2 getCenterPosition();

        public abstract CollisionReturn getCollisionReturn();

    }
}
