using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Graphics
{
    public class Animation
    {
        public Animation(String path, int frametime, int framecount,bool doloop)  {
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


    public class AnimationPlayer
    {
        public Animation animation;
        public int callCount = 0;
        public Texture2D texture;
        protected int scale;
        protected int texture_width;
        protected Vector2 offset;

        public AnimationPlayer(Animation anim, int scale_, Vector2 offset_, ContentManager Content)
        {
            offset = offset_;
            animation = anim;
            scale = scale_;

            texture = Content.Load<Texture2D>(animation.asset_path);

            texture_width = (int)texture.Width / animation.frame_count;
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect, int timeN)
        {
            int frame = (int) Math.Floor((double)callCount / animation.change_frame_every) % animation.frame_count;
            Rectangle source_rect = new Rectangle(texture_width * frame, 0, texture_width, texture.Height);
            Rectangle destination_rect = new Rectangle((int)(position.X + offset.X), (int)(position.Y + offset.Y), texture_width * scale, texture.Height * scale);

            if (!(!animation.do_loop && callCount > animation.frame_count))
                sprite_batch.Draw(texture, destination_rect, source_rect, Color.White, 0.0f, Vector2.Zero, effect, 0);
            
            callCount += timeN;
        }

        public void Reset()
        {
            callCount = 0;
        }


    }

}
