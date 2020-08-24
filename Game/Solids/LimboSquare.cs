using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{
    class LimboSquare : SimpleSolid
    {

        public static LimboSquare Make(Level level, Vector2 starting_pos, Vector2 vel)
        {
            var tile = new LimboSquare();
            tile.Initialize(level, starting_pos, vel);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 vel)
        {
            renderer = new BasicSprite("debug/square");
            renderWithCollisionBox = true;
            base.Initialize(level, starting_pos, vel, new Vector2(GameUtils.TILE_WORLD_SIZE*3));
        }

    }
}
