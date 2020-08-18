using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public abstract class GameObject : IGameObject
    {
        protected String texturePath = "UNDEFINED";
        protected Color textureColor = Color.White;

        public bool hidden = false;
    

        protected Level localLevel;
        public Vector2 position;

        protected Texture2D texture;

        public GameObject() { }

        public virtual void Initialize(Level level, Vector2 starting_pos)
        {
            localLevel = level;
            position = starting_pos;
            LoadContent();
        }

        public virtual void LoadContent()
        {
            texture = localLevel.Content.Load<Texture2D>( texturePath );
        }

        public abstract void Update(StateData state);

        public virtual void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            sprite_batch.Draw(texture, position, textureColor);
        }

        public virtual void Reset() { }

        public virtual void SetInactive() 
        {
            hidden = true;
        }

        public virtual void SetActive()
        {
            hidden = false;
        }

    }
}
