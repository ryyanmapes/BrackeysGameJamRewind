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
        private float fadeValue = 0f;
        private bool showCube = false;
        private Color cubeColor = Color.Black;

        public OverlayEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
            deathSquare = Content.Load<Texture2D>("debug/square");
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
                if(fadeValue > 0.01f)
                {
                    fadeValue -= .1f;
                }
                else if(fadeValue <= 0.01f)
                {
                    showCube = false;
                }
            }
        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (showCube)
            {
                sprite_batch.Draw(deathSquare, Vector2.Zero, new Rectangle(0, 0, (int)RewindGame.LEVEL_SIZE_X, (int)RewindGame.LEVEL_SIZE_Y), new Color(cubeColor, fadeValue), 0f, Vector2.Zero, new Vector2(RewindGame.LEVEL_SIZE_X, RewindGame.LEVEL_SIZE_Y), SpriteEffects.None, 0f);
            }
        }

        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
