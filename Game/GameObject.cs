using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    abstract class GameObject
    {
        protected String texturePath = "UNDEFINED";
        protected Color textureColor = Color.White;
    

        protected Level localLevel;
        protected Vector2 position;

        protected Texture2D texture;

        public GameObject()
        {
        }

        public GameObject(Level level, Vector2 starting_pos)
        {
            Initialize(level, starting_pos);
        }

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

        public abstract void Update(GameTime game_time);

        public virtual void Draw(GameTime game_time, SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(texture, position, textureColor);
        }

        


    }
}
