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
    public class OverlayEffects
    {
        public ContentManager Content;
        public RewindGame parentGame;

        private Texture2D deathSquare;
        private Texture2D star0;
        private Texture2D star1;
        private Texture2D star2;
        private Texture2D star3;
        private Texture2D star4;
        private Texture2D star5;
        private float fadeValue = 0f;
        private bool showCube = false;
        private Color cubeColor = Color.Black;
        private float stall = 1.75f;
        private bool fadeOut = false;
        private float starFade = 0f;
        private bool showStars = false;
        private Random rnd = new Random();

        public OverlayEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
            deathSquare = Content.Load<Texture2D>("debug/square");
            star0 = Content.Load<Texture2D>("effects/star0");
            star1 = Content.Load<Texture2D>("effects/star1");
            star2 = Content.Load<Texture2D>("effects/star2");
            star3 = Content.Load<Texture2D>("effects/star3");
            star4 = Content.Load<Texture2D>("effects/star4");
            star5 = Content.Load<Texture2D>("effects/star5");
        }



        public void TriggerDeath()
        {
            //when death runstate is runstate.platerdead get with parentgame.runstate
            //parentgame.statetimer tells how long will stay in state
            //rewindgame.playerdeathtime is max death time (ticks down to 0(when player respawns))
            //
            //render a big black squear on entire screen (content/debug) if turned to white can be any color with draw function
            //fade into black starting at about halfway through platyerdeath time (when playerdeath time hits 50% fade to black) fade out to black when it is no longer runstate.playerdead
        }

        public void Update(StateData state)
        {
            if (parentGame.runState == RunState.playerdead)
            {
                if (parentGame.stateTimer <= 0.25)
                {
                    fadeValue += .1f;
                    showCube = true;
                }
            }
            else if (showCube == true)
            {
                if (fadeValue < stall && fadeOut == false)
                {
                    fadeValue += .1f;
                }
                else if (fadeValue >= stall && fadeOut == false)
                {
                    fadeValue -= .05f;
                    fadeOut = true;
                }
                else if (fadeOut == true)
                {
                    fadeValue -= .05f;
                }
                if(fadeValue <= 0.01f)
                {
                    showCube = false;
                    fadeValue = 0.00f;
                    fadeOut = false;
                }
            }
            if (showStars == false && state.getTimeDependentDeltaTime() == 0)
            {

            }
        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            if (showCube)
            {
                sprite_batch.Draw(deathSquare, CameraPosReal, new Rectangle((int)CameraPosReal.X, (int)CameraPosReal.Y, (int)RewindGame.LEVEL_SIZE_X, (int)RewindGame.LEVEL_SIZE_Y), new Color(cubeColor, fadeValue), 0f, Vector2.Zero, new Vector2(RewindGame.LEVEL_SIZE_X, RewindGame.LEVEL_SIZE_Y), SpriteEffects.None, 0f);
            }
            if (showStars == true)
            {
                sprite_batch.Draw(star0, state.camera_position, Color.White);
            }
        }
        public void MakeStasisParticles(StateData state)
        {

        }
        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
