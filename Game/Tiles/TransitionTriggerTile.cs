using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{
    class TransitionTriggerTile : SolidTile
    {
        public MoveDirection enterDirection;

        public new static TransitionTriggerTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_, MoveDirection enterDirection)
        {
            var tile = new TransitionTriggerTile();
            tile.Initialize(level, starting_pos, tile_sprite_, enterDirection);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_, MoveDirection enterDirection_)
        {
            enterDirection = enterDirection_;

            switch (enterDirection_)
            {
                case MoveDirection.right:
                    collisionOffset.X += Level.TILE_WORLD_SIZE;
                    break;
                case MoveDirection.left:
                    collisionOffset.X -= Level.TILE_WORLD_SIZE;
                    break;
            }

            collisionType = PrimaryCollisionType.harmless;
            base.Initialize(level, starting_pos, tile_sprite_);
        }

        public override bool isThisOverlapping(FRectangle rect, MoveDirection direction)
        {
            bool isOverlapping = base.isThisOverlapping(rect);
            if (isOverlapping && direction == enterDirection)
            {
                localLevel.TransitionTo(enterDirection);
            }
            return isOverlapping;
        }

    }
}
