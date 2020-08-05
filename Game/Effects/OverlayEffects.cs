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
