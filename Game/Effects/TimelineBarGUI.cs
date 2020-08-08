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

        protected Texture2D indicator;

        public Texture2D limboBar1;
        public Texture2D limboBarHalf;
        public Texture2D limboBarFourth;

        public Texture2D cottonBar1;
        public Texture2D cottonBarHalf;
        public Texture2D cottonBarFourth;

        public Texture2D eternalBar1;
        public Texture2D eternalBarHalf;
        public Texture2D eternalFourth;

        public float currentBarSize = 105 * 4;
        public float barDistanceFromTop = 16 * 2;

        protected Texture2D currentBar;

        public TimelineBarGUI(RewindGame parent_game, ContentManager content)
        {
            Content = content;
            parentGame = parent_game;
            limboBar1 = Content.Load<Texture2D>("gui/barlimbo0");
            limboBarHalf = Content.Load<Texture2D>("gui/barlimbo1");
            limboBarFourth = Content.Load<Texture2D>("gui/barlimbo2");

            indicator = Content.Load<Texture2D>("gui/barindicator");

            //texturename = Content.Load<Texture2D>( texturePath );
        }

        public void Update(StateData state)
        {
            //update indicator on timeline bar
        }

        public void SetBar(Texture2D bar)
        {
            currentBar = bar;
        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (currentBar != null)
            {
                sprite_batch.Draw(currentBar, state.camera_position + new Vector2(-RewindGame.LEVEL_SIZE_X / 2, -RewindGame.LEVEL_SIZE_Y/2), Color.White);
                // todo draw indicator
                float time_progress = (float)(state.time_data.time_moment - parentGame.timeNegBound)/(float)(parentGame.timePosBound - parentGame.timeNegBound);
                sprite_batch.Draw(indicator, state.camera_position + new Vector2(-currentBarSize - indicator.Width*1.25f + (time_progress * currentBarSize * 2) , -RewindGame.LEVEL_SIZE_Y / 2 + barDistanceFromTop), Color.White);
            }
        }

    }
}
