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

        protected AnimationChoice anim_idle_pink = new AnimationChoice("idle", "cottonwood/jumppoofpink", 1, 1, true, 1, Vector2.Zero);
        protected AnimationChoice anim_idle_green = new AnimationChoice("idle", "cottonwood/jumppoofgreen", 1, 1, true, 1, Vector2.Zero);
        protected AnimationChoice anim_poof_pink = new AnimationChoice("pop", "cottonwood/jumppoofpinkpop", 1, 1, false, 1, Vector2.Zero, "idle", "");
        protected AnimationChoice anim_poof_green = new AnimationChoice("pop", "cottonwood/jumppoofgreenpop", 1, 1, false, 1, Vector2.Zero, "idle", "");

        public static Floof Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isForwards)
        {
            var tile = new Floof();
            tile.Initialize(level, starting_pos, velocity_, isForwards);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_forwards)
        {
            collisionSize.X = GameUtils.LARGE_TILE_WORLD_SIZE;
            collisionSize.Y = GameUtils.SEMISOLID_THICKNESS_WINDOW;

            isForwards = is_forwards;
            velocity = velocity_;

            if (isForwards)
            {
                renderer = new AnimationChooser(new AnimationChoice[] { anim_idle_pink, anim_poof_pink }, localLevel.Content);
            }
            else
            {
                renderer = new AnimationChooser(new AnimationChoice[] { anim_idle_green, anim_poof_green }, localLevel.Content);
            }
            ((AnimationChooser)renderer).changeAnimation("idle");

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
                ((AnimationChooser)renderer).changeAnimationBackwards("poof");
            }
        }

        public override void Reset()
        {
            position = startingPos;
            isActive = true;
            ((AnimationChooser)renderer).changeAnimation("idle");
            base.Reset();
        }

        public void Consume(StateData state)
        {
            if (state.time_data.time_status == TimeState.still) return;
            timeMomentConsumedOn = state.time_data.time_moment - 2;
            isActive = false;
            ((AnimationChooser)renderer).changeAnimation("poof");
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
