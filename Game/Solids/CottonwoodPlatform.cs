using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class CottonwoodPlatform : Platform
    {

        protected  AnimationPlayer renderLongDown = new AnimationPlayer("cottonwood/cottonwoodplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderLongUp = new AnimationPlayer("cottonwood/cottonwoodplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderDown = new AnimationPlayer("cottonwood/cottonwoodplatformdown", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderUp = new AnimationPlayer("cottonwood/cottonwoodplatformup", 1, 1, true, 1, Vector2.Zero);

        public  static CottonwoodPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new CottonwoodPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }
        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {

            // I know this looks like an awful mess, but trust me it's better than the ten lines I had before
            renderer = is_long ? ((velocity_.Y < 0) ? renderLongDown : renderLongUp) : ((velocity_.Y < 0) ? renderDown : renderUp);

            base.Initialize(level, starting_pos, velocity_, is_long ? 4 : 2);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (targetDisplacement.X < 0) spriteEffects = SpriteEffects.FlipHorizontally;
            else spriteEffects = SpriteEffects.None;

            base.Draw(state, sprite_batch);

            ((AnimationPlayer)renderer).UpdateAnimation(state, state.getTimeSign());
        }
    }
}
