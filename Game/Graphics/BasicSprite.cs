using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace RewindGame.Game.Graphics
{
    class BasicSprite : IRenderMethod
    {
        public bool isHidden { get; set; } = false;

        protected string path = "";
        public Color color = Color.White;
        // scale is only used when a custom rect is not fed into Draw!
        protected int scale = 1;
        protected Vector2 offset;

        protected Texture2D texture;

        public BasicSprite(Texture2D texture)
        {
            this.texture = texture;
            this.path = "";
        }

        public BasicSprite(string path)
        {
            this.path = path;
        }

        public BasicSprite(string path, Color color)
        {
            this.path = path;
            this.color = color;
        }

        public BasicSprite(string path, int scale, Vector2 offset, Color color)
        {
            this.path = path;
            this.scale = scale;
            this.offset = offset;
            this.color = color;
        }

        public void LoadContent(ContentManager Content)
        {
            if (path != "")
                texture = Content.Load<Texture2D>(path);
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect)
        {
            if (isHidden) return;
            sprite_batch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, texture.Height * scale, texture.Width * scale), null, color, 0, Vector2.Zero, effect, 0);
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Rectangle rect, SpriteEffects effect)
        {
            if (isHidden) return;
            sprite_batch.Draw(texture, new Rectangle(rect.X + (int)offset.X, rect.Y + (int)offset.Y, rect.Width, rect.Height), null, color, 0, Vector2.Zero, effect, 0);
        }

    }
}
