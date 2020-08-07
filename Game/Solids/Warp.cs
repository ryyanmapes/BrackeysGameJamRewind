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

        protected Animation anim_idle = new Animation("warp/timeriploop", 4, 2, true);
        protected Animation anim_close = new Animation("warp/timerip", 4, 6, false);

        protected AnimationChooser anims;
        protected bool isActivated;

        public static Warp Make(Level level, Vector2 starting_pos)
        {
            var tile = new Warp();
            tile.Initialize(level, starting_pos);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos)
        {
            collisionType = CollisionType.death;

            collisionSize = new Vector2(100,100);

            anims = new AnimationChooser(1, Vector2.Zero);
            anims.addAnimaton(anim_idle, "idle", level.Content);
            anims.addAnimaton(anim_idle, "close", level.Content);
            anims.changeAnimation("idle");

            base.Initialize(level, starting_pos);

            collisionType = CollisionType.warp_trigger;
        }

        public override void Update(StateData state)
        {
        }

        public override void LoadContent() { }

        public void TriggerActivation()
        {
            isActivated = true;
            anims.changeAnimation("close");
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            //base.Draw(state, sprite_batch);
            anims.Draw(state, sprite_batch, position + new Vector2(0,state.time_data.getFloaty(0,false)), SpriteEffects.None, -1);
        }

        public override void Reset()
        {
            isActivated = false;
            anims.changeAnimation("idle");
            base.Reset();
        }

    }
}
