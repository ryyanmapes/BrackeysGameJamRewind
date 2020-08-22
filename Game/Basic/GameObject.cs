using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Graphics;

namespace RewindGame.Game
{
    public abstract class GameObject : IGameObject
    {
        protected IRenderMethod renderer;
        protected SpriteEffects spriteEffects = SpriteEffects.None;

        protected Level localLevel;
        public Vector2 position;

        public bool isHidden
        {
            get => renderer.isHidden;
            set => renderer.isHidden = value;
        }

        public GameObject() { }

        // BEFORE you call this, every non-abstract object must first set up it's renderer!
        // If you are erroring out here, it's probably because you forgot to set renderer to something
        public virtual void Initialize(Level level, Vector2 starting_pos)
        {
            localLevel = level;
            position = starting_pos;
            renderer.LoadContent(level.Content);
        }

        public abstract void Update(StateData state);

        public virtual void Draw(StateData state, SpriteBatch sprite_batch)
        {
            // todo this should error if renderer is null!
            renderer.Draw(state, sprite_batch, position, spriteEffects);
        }

        public virtual void Reset() { }

        public virtual void SetInactive() 
        {
            renderer.isHidden = true;
        }

        public virtual void SetActive()
        {
            renderer.isHidden = false;
        }

    }
}
