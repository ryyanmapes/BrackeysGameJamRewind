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

        public OverlayEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
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

        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {

        }

        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
