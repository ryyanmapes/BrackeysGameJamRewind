using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{


    class EternalPlatform : Platform
    {

        protected  AnimationPlayer renderLongDown = new AnimationPlayer("eternal/terrariumplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderLongUp = new AnimationPlayer("eternal/terrariumplatformuplong", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderDown = new AnimationPlayer("eternal/terrariumplatformup", 1, 1, true, 1, Vector2.Zero);
        protected  AnimationPlayer renderUp = new AnimationPlayer("eternal/terrariumplatformdown", 1, 1, true, 1, Vector2.Zero);

        public  static EternalPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new EternalPlatform();
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
