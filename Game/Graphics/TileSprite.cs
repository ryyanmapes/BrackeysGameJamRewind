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
    class TileSprite : IRenderMethod
    {
        public bool isHidden { get; set; } = false;

        protected TileSpriteInfo sprite;
        protected Color color = Color.White;

        protected Level parentLevel;

        public TileSprite(TileSpriteInfo sprite, Level parentLevel)
        {
            this.sprite = sprite;
            this.color = Color.White;
            this.parentLevel = parentLevel;
        }

        public TileSprite(TileSpriteInfo sprite, Level parentLevel, Color color)
        {
            this.sprite = sprite;
            this.color = color;
            this.parentLevel = parentLevel;
        }

        // This does nothing because RewindGame already loaded all the tilesheets
        public void LoadContent(ContentManager Content) { }


        // For these draw functions: SpriteEffects does not work as is, and neither does using a custom rect
        // (plus no scale, no offset)
        public void Draw(StateData state, SpriteBatch sprite_batch, Vector2 position, SpriteEffects effect)
        {
            if (isHidden) return;
            parentLevel.DrawTile(sprite, position, sprite_batch);
        }

        public void Draw(StateData state, SpriteBatch sprite_batch, Rectangle rect, SpriteEffects effect)
        {
            Draw(state, sprite_batch, new Vector2(rect.X, rect.Y), effect);
        }

    }
}
