using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
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
    public class Floof : CollisionObject
    {

        public bool isForwards;
        public Vector2 velocity;
        public Vector2 startingPos;
        public int timeMomentConsumedOn = -9999;
        public bool isActive = true;
        protected AnimationChooser anims;

        protected Animation anim_idle_pink = new Animation("cottonwood/jumppoofpink", 1, 1, true);
        protected Animation anim_idle_green = new Animation("cottonwood/jumppoofgreen", 1, 1, true);
        protected Animation anim_poof_pink = new Animation("cottonwood/jumppoofpinkpop", 3, 2, false);
        protected Animation anim_poof_green = new Animation("cottonwood/jumppoofgreenpop", 3, 2, false);

        public static Floof Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isForwards)
        {
            var tile = new Floof();
            tile.Initialize(level, starting_pos, velocity_, isForwards);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_forwards)
        {
            collisionSize.X = Level.LARGE_TILE_WORLD_SIZE;
            collisionSize.Y = Level.SEMISOLID_THICKNESS_WINDOW;
            isForwards = is_forwards;
            velocity = velocity_;
            this.startingPos = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            position += new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime());

            if (timeMomentConsumedOn != -9999 && state.time_data.time_moment < timeMomentConsumedOn)
            {
                isActive = true;
                timeMomentConsumedOn = -9999;
                anims.changeAnimation("idle");
            }
        }

        public override void LoadContent()
        {
            anims = new AnimationChooser(1, Vector2.Zero);

            if (isForwards)
            {
                anims.addAnimaton(anim_idle_pink, "idle", localLevel.Content);
                anims.addAnimaton(anim_idle_pink, "poof", localLevel.Content);
            }
            else
            {
                anims.addAnimaton(anim_idle_green, "idle", localLevel.Content);
                anims.addAnimaton(anim_idle_green, "poof", localLevel.Content);
            }
            anims.changeAnimation("idle");
            //base.LoadContent();
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (!anims.isCurrentAnimationDone())
                anims.Draw(state, sprite_batch, position + new Vector2(0, state.time_data.getFloaty(position.X, true)), SpriteEffects.None, state.getTimeN());
        }

        public override void Reset()
        {
            position = startingPos;
            isActive = true;
            base.Reset();
        }

        public void Consume(StateData state)
        {
            timeMomentConsumedOn = state.time_data.time_moment - 1;
            isActive = false;
            anims.changeAnimation("poof");
        }

        public override CollisionReturn getCollisionReturn()
        {
            if (isActive)
            {
                return new CollisionReturn(isForwards ? CollisionType.forward_floof : CollisionType.backward_floof, this, 3);
            }
            return CollisionReturn.None();
        }
    }
}
