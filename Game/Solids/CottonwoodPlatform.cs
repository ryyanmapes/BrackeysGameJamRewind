using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class CottonwoodPlatform : LimboPlatform
    {

        public new static CottonwoodPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new CottonwoodPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {
            Animation anim;
            if (is_long)
            {
                if (velocity_.Y < 0)
                {
                    anim = new Animation("cottonwood/cottonwoodplatformdownlong", 1, 1, true);
                    //collisionOffset = new Vector2(0, GameUtils.TILE_WORLD_SIZE + 8);
                }
                else
                {
                    anim = new Animation("cottonwood/cottonwoodplatformdownlong", 1, 1, true);
                }
            }
            else
            {
                if (velocity_.Y < 0)
                {
                    anim = new Animation("cottonwood/cottonwoodplatformdown", 1, 1, true);
                    //collisionOffset = new Vector2(0, GameUtils.TILE_WORLD_SIZE + 8);
                }
                else
                {
                    anim = new Animation("cottonwood/cottonwoodplatformup", 1, 1, true);
                }
            }

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, level.Content);

            doLeftCollision = false;
            doRightCollision = false;
            doDownCollision = false;

            base.Initialize(level, starting_pos, velocity_, is_long? 4 : 2);
        }

    }
}
