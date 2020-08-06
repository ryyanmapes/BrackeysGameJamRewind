using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


namespace RewindGame.Game.Graphics
{
    public class AnimationChooser
    {
        Dictionary<String, AnimationPlayer> animations = new Dictionary<string, AnimationPlayer>();
        public String currentAnimationName = "";
        AnimationPlayer currentAnimation = null;

        int scale;
        Vector2 offset;

        public AnimationChooser(int scale_, Vector2 offset_)
        {
            scale = scale_;
            offset = offset_;
        }

        public void addAnimaton(Animation anim, String name, ContentManager Content)
        {
            animations.Add(name, new AnimationPlayer(anim, scale, offset, Content));
        }

        public void changeAnimation(String name)
        {
            if (name == currentAnimationName) return;
            AnimationPlayer animp;
            if (animations.TryGetValue(name, out animp))
            {
                currentAnimation = animp;
                currentAnimationName = name;
                animp.Reset();
                
            }
            else
            {
                Console.WriteLine("Unable to find animation player of name: {0}", name);
            }
        }

        public void stopAnimation()
        {
            currentAnimation = null;
            currentAnimationName = "";
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position)
        {
            Draw(state, sprite_batch, position, SpriteEffects.None);
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect)
        {
            currentAnimation.Draw(state, sprite_batch, position, effect, 1);
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect, int timeN)
        {
            currentAnimation.Draw(state, sprite_batch, position, effect, timeN);
        }
    }
}
