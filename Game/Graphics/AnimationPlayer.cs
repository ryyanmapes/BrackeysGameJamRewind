using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Graphics
{
    public class AnimationInfo
    {
        public AnimationInfo(string path, int frametime, int framecount, bool doloop)  {
            asset_path = path;
            frame_count = framecount;
            change_frame_every = frametime;
            do_loop = doloop;
        }

        public int frame_count;
        public bool do_loop;
        public int change_frame_every;
        public string asset_path;

    }


    public class AnimationPlayer : IRenderMethod
    {
        public bool isHidden
        {
            get;
            set;
        }

        public AnimationInfo animationInfo;
        public int tick = 0;
        public Texture2D texture;
        // scale is only used when a custom rect is not fed into Draw!
        public int scale = 1;
        protected Vector2 offset;

        protected int textureWidth;
        protected int maxTicks;

        public bool useTimeN = true;

        public AnimationPlayer(string path, int frametime, int framecount, bool doloop, int scale_, Vector2 offset_, bool useTimeN = true)
        {
            offset = offset_;
            animationInfo = new AnimationInfo(path, frametime, framecount, doloop);
            scale = scale_;

            this.useTimeN = useTimeN;
        }

        public AnimationPlayer(AnimationInfo anim, int scale_, Vector2 offset_, bool useTimeN = true)
        {
            offset = offset_;
            animationInfo = anim;
            scale = scale_;

            this.useTimeN = useTimeN;
        }

        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(animationInfo.asset_path);

            textureWidth = (int)texture.Width / animationInfo.frame_count;
            maxTicks = animationInfo.change_frame_every * animationInfo.frame_count - 1;
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect)
        {
            Draw(state, sprite_batch, new Rectangle((int)position.X, (int)position.Y, textureWidth * scale, texture.Height * scale), effect);
        }

        // Note that texture.Width gives you the length of the whole sheet and textureWidth gives you the width per frame
        public void Draw(StateData state, SpriteBatch sprite_batch, Rectangle rect, SpriteEffects effect)
        {
            if (!animationInfo.do_loop) tick = Math.Clamp(tick, 0, maxTicks);

            int frame = (int) Math.Floor((double)tick / animationInfo.change_frame_every) % animationInfo.frame_count;
            Rectangle source_rect = new Rectangle(textureWidth * frame, 0, textureWidth, texture.Height);
            Rectangle destination_rect = new Rectangle((int)(rect.X + offset.X), (int)(rect.Y + offset.Y), rect.Width, rect.Height);

            sprite_batch.Draw(texture, destination_rect, source_rect, Color.White, 0.0f, Vector2.Zero, effect, 0);

            // if you aren't using timeN, the animation just goes forward no matter what
            UpdateAnimation(state, useTimeN ? state.getTimeSign() : 1);
        }

        public void UpdateAnimation(StateData state, int time_n)
        {
            tick += time_n;
        }

        public void Reset()
        {
            tick = 0;
        }

        public void ResetToEnd()
        {
            tick = maxTicks;
        }

        public bool ExceedsMin() { return tick < 0 && !animationInfo.do_loop; }

        public bool ExceedsMax() { return tick > maxTicks && !animationInfo.do_loop; }

    }

}
