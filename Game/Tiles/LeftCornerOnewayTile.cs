using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    class LeftCornerOnewayTile : SolidTile
    {

        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (collisionDirection == MoveDirection.none || direction == MoveDirection.left)
            {
                if (rect.X + rect.Width > position.X + Level.SEMISOLID_THICKNESS) return false;
                FRectangle rect1 = getCollisionBox();
                rect1.Width = (int)Level.SEMISOLID_THICKNESS;
                return isThisOverlapping(rect);
            }
            if (collisionDirection == MoveDirection.none || direction == MoveDirection.down)
            {
                if (rect.Y + rect.Height > position.Y + Level.SEMISOLID_THICKNESS) return false;
                FRectangle rect2 = getCollisionBox();
                rect2.Height = (int)Level.SEMISOLID_THICKNESS;
                return isThisOverlapping(rect);
            }
            return false;
        }

    }
}
