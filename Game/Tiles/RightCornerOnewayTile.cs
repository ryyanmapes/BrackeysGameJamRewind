using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    class RightCornerOnewayTile : CollisionTile
    {

        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            if (collisionDirection == MoveDirection.none || direction == MoveDirection.right)
            {
                if (rect.X + rect.Width > position.X + Level.TILE_WORLD_SIZE - Level.SEMISOLID_THICKNESS) return false;
                FRectangle rect1 = getCollisionBox();
                rect1.Width = (int)Level.SEMISOLID_THICKNESS;
                rect1.X = (int)(rect1.X + Level.TILE_WORLD_SIZE - Level.SEMISOLID_THICKNESS);
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
