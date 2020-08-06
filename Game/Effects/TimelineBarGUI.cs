using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace RewindGame.Game.Effects
{
    public class TimelineBarGUI
    {

        public ContentManager Content;
        public RewindGame parentGame;

        public Texture2D limboBar1;
        public Texture2D limboBarHalf;
        public Texture2D limboBarFourth;

        public TimelineBarGUI(RewindGame parent_game, ContentManager content)
        {
            Content = content;
            parentGame = parent_game;
            limboBar1 = Content.Load<Texture2D>("gui/barlimbo0");
            limboBarHalf = Content.Load<Texture2D>("gui/barlimbo1");
            limboBarFourth = Content.Load<Texture2D>("gui/barlimbo2");

            //texturename = Content.Load<Texture2D>( texturePath );
        }

        public void Update(StateData state)
        {
            //update indicator on timeline bar
        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            Texture2D bar_texture = null;
            bar_texture = limboBar1;
            if (bar_texture != null)
            {
                sprite_batch.Draw(bar_texture, Vector2.Zero, Color.White);
            }
        }

    }
}
