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
        public Texture2D eternalBarFourth;

        public float currentBarSize = GameUtils.TIMELINE_BAR_SIZE_FULL;
        public float barDistanceFromTop = 16 * 2;

        protected Texture2D currentBar;

        public TimelineBarGUI(RewindGame parent_game)
        {
            Content = parent_game.Content;
            parentGame = parent_game;
            limboBar1 = Content.Load<Texture2D>("gui/barlimbo0");
            limboBarHalf = Content.Load<Texture2D>("gui/barlimbo1");
            limboBarFourth = Content.Load<Texture2D>("gui/barlimbo2");

            cottonBar1 = Content.Load<Texture2D>("gui/barcottonwood");
            cottonBarHalf = Content.Load<Texture2D>("gui/barcottonwoodhalf");
            cottonBarFourth = Content.Load<Texture2D>("gui/barcottonwoodquarter");

            eternalBar1 = Content.Load<Texture2D>("gui/barterrarium");
            eternalBarHalf = Content.Load<Texture2D>("gui/barlimbo1");
            eternalBarFourth = Content.Load<Texture2D>("gui/barlimbo2");

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
                sprite_batch.Draw(currentBar, state.camera_position + new Vector2(-GameUtils.LEVEL_SIZE_X / 2, -GameUtils.LEVEL_SIZE_Y/2), Color.White);
                // todo draw indicator
                float time_progress = (float)(state.time_data.time_moment) /(float)(parentGame.timeBound.length());
                sprite_batch.Draw(indicator, state.camera_position + new Vector2(-indicator.Width*1.25f + (time_progress * currentBarSize * 2) , -GameUtils.LEVEL_SIZE_Y / 2 + barDistanceFromTop), Color.White);
            }
        }

    }
}
