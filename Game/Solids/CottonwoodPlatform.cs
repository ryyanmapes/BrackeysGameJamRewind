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

        protected new AnimationPlayer renderLongDown = new AnimationPlayer("cottonwood/cottonwoodplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderLongUp = new AnimationPlayer("cottonwood/cottonwoodplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderDown = new AnimationPlayer("cottonwood/cottonwoodplatformdown", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderUp = new AnimationPlayer("cottonwood/cottonwoodplatformup", 1, 1, true, 1, Vector2.Zero);

        public new static CottonwoodPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new CottonwoodPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

    }
}
