using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{

    class LimboPlatform : Platform
    {

        protected AnimationPlayer renderLongDown = new AnimationPlayer("limbo/platformlongdown", 6, 4, true, 1, new Vector2(0, -GameUtils.TILE_WORLD_SIZE - 8));
        protected AnimationPlayer renderLongUp = new AnimationPlayer("limbo/platformlongup", 6, 4, true, 1, Vector2.Zero);
        protected AnimationPlayer renderDown = new AnimationPlayer("limbo/platformdown", 6, 4, true, 1, new Vector2(0, -GameUtils.TILE_WORLD_SIZE - 8));
        protected AnimationPlayer renderUp = new AnimationPlayer("limbo/platformup", 6, 4, true, 1, Vector2.Zero);

        public static LimboPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new LimboPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {

            // I know this looks like an awful mess, but trust me it's better than the ten lines I had before
            renderer = is_long ? ((velocity_.Y < 0) ? renderLongDown : renderLongUp) : ((velocity_.Y < 0) ? renderDown : renderUp);

            base.Initialize(level, starting_pos, velocity_, is_long? 4 : 2);
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
