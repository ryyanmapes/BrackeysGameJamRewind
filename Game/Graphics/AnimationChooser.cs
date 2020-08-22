using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


namespace RewindGame.Game.Graphics
{
    public class AnimationChoice
    {
        public AnimationChoice(string name, string path, int frametime, int framecount, bool doloop, bool useTimeN = true)
        {
            this.name = name;
            animationPlayer = new AnimationPlayer(new AnimationInfo(path, frametime, framecount, doloop), 1, Vector2.Zero, useTimeN);
        }

        public AnimationChoice(string name, string path, int frametime, int framecount, bool doloop, int scale, Vector2 offset, bool useTimeN = true)
        {
            this.name = name;
            animationPlayer = new AnimationPlayer(new AnimationInfo(path, frametime, framecount, doloop), scale, offset, useTimeN);
        }

        public AnimationChoice(string name, string path, int frametime, int framecount, bool doloop, int scale, Vector2 offset, string prev_anim, string next_anim, bool useTimeN = true)
        {
            this.name = name;
            animationPlayer = new AnimationPlayer(path, frametime, framecount, doloop, scale, offset, useTimeN);
            nextAnimName = next_anim;
            prevAnimName = prev_anim;
        }

        public string name;
        public AnimationPlayer animationPlayer;
        public string nextAnimName;
        public string prevAnimName;
    }

    public class AnimationChooser : IRenderMethod
    {
        Dictionary<string, AnimationChoice> animations = new Dictionary<string, AnimationChoice>();
        public AnimationChoice currentAnimationChoice = null;

        public bool isHidden { get; set; } = false;

        public AnimationChooser() { }

        public AnimationChooser(AnimationChoice[] animations, ContentManager Content) 
        {
            foreach (AnimationChoice anim in animations)
            {
                addAnimation(anim, Content);
            }
        }

        public void addAnimation(AnimationChoice anim, ContentManager Content)
        {
            anim.animationPlayer.LoadContent(Content);
            animations.Add(anim.name, anim);
        }

        public void changeAnimation(string name)
        {
            if (currentAnimationChoice != null && name == currentAnimationChoice.name) return;
            else if (name == "")
            {
                stopAnimation();
                return;
            }

            AnimationChoice animp;
            
            if (animations.TryGetValue(name, out animp))
            {
                currentAnimationChoice = animp;
                animp.animationPlayer.Reset();
            }
            else
            {
                Console.WriteLine("Unable to find animation player of name: {0}", name);
            }
        }

        public void changeAnimationBackwards(string name)
        {
            if (currentAnimationChoice != null && name == currentAnimationChoice.name) return;
            else if (name == "")
            {
                stopAnimation();
                return;
            }

            AnimationChoice animp;
            if (animations.TryGetValue(name, out animp))
            {
                currentAnimationChoice = animp;
                animp.animationPlayer.ResetToEnd();
            }
            else
            {
                Console.WriteLine("Unable to find animation player of name: {0}", name);
            }
        }

        // This puts the animation state back to null so nothing is rendered at all
        public void stopAnimation()
        {
            currentAnimationChoice = null;
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect)
        {
            if (currentAnimationChoice != null && !isHidden)
            {
                currentAnimationChoice.animationPlayer.Draw(state, sprite_batch, position, effect);
                if (!currentAnimationChoice.animationPlayer.animationInfo.do_loop) CheckTransition();
            }
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Rectangle rect, SpriteEffects effect)
        {
            if (currentAnimationChoice != null && !isHidden)
            {
                currentAnimationChoice.animationPlayer.Draw(state, sprite_batch, rect, effect);
                if (!currentAnimationChoice.animationPlayer.animationInfo.do_loop) CheckTransition();
            }
        }



        public void CheckTransition()
        {
            if (currentAnimationChoice.animationPlayer.ExceedsMin() && currentAnimationChoice.prevAnimName != null)
            {
                changeAnimationBackwards(currentAnimationChoice.prevAnimName);
            }
            else if (currentAnimationChoice.animationPlayer.ExceedsMax() && currentAnimationChoice.nextAnimName != null)
            {
                changeAnimation(currentAnimationChoice.nextAnimName);
            }
        }


        public bool isCurrentAnimationDone()
        {
            return (currentAnimationChoice.animationPlayer.ExceedsMax() || currentAnimationChoice.animationPlayer.ExceedsMin())
                && !currentAnimationChoice.animationPlayer.animationInfo.do_loop;
        }


        // Don't use this, it does nothing
        // all content is loaded in addAnimation- should that be changed?
        public void LoadContent(ContentManager Content) { }
    }
}
