using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using Microsoft.Xna.Framework;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Abstract
{
    interface ISolidTile : ITile 
    {
        public abstract bool isThisOverlapping(CollisionObject obj);

        public abstract bool isThisOverlapping(Rectangle rect);

        public abstract bool isThisOverlapping(CollisionObject obj, MoveDirection direction);

        public abstract bool isThisOverlapping(Rectangle rect, MoveDirection direction);

        public abstract PrimaryCollisionType getCollisionType();
    }
}
