using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using Microsoft.Xna.Framework;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Abstract
{
    public interface ISolidTile : ITile 
    {
        public abstract bool isThisOverlapping(CollisionObject obj);

        public abstract bool isThisOverlapping(FRectangle rect);

        public abstract bool isThisOverlapping(CollisionObject obj, MoveDirection direction);

        public abstract bool isThisOverlapping(FRectangle rect, MoveDirection direction);

        public abstract CollisionType getCollisionType();
    }
}
