using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;
using RewindGame.Game.Sound;
using System;
using RewindGame.Game.Solids;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Solids
{

    public class Warp : CollisionObject
    {

        protected AnimationChoice anim_idle = new AnimationChoice("idle", "warp/timeriploop", 6, 2, true);
        protected AnimationChoice anim_close = new AnimationChoice("close", "warp/timerip", 4, 6, true);

        protected bool isActivated;

        public static Warp Make(Level level, Vector2 starting_pos)
        {
            var tile = new Warp();
            tile.Initialize(level, starting_pos);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos)
        {
            collisionType = CollisionType.death;

            collisionSize = new Vector2(100,100);

            AnimationChooser anims = new AnimationChooser( new AnimationChoice[] { anim_idle, anim_close }, level.Content);
            anims.changeAnimation("idle");
            renderer = anims;

            base.Initialize(level, starting_pos);

            collisionType = CollisionType.warp_trigger;
        }

        public override void Update(StateData state)
        {
        }

        public void TriggerActivation()
        {
            isActivated = true;
            ((AnimationChooser)renderer).changeAnimation("close");
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            renderer.Draw(state, sprite_batch, position + new Vector2(0,state.time_data.getFloaty(0,false)), SpriteEffects.None);
        }

        public override void Reset()
        {
            isActivated = false;
            ((AnimationChooser)renderer).changeAnimation("idle");
            base.Reset();
        }

    }
}
