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
    public class LimboEffects : IEffectManager
    {
        public ContentManager Content;
        public RewindGame parentGame;

        public LimboEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
        }

        public void Update(StateData state)
        {
            // use this only for time-changing stuff, like falling the rain
        }

        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            // remember to use state.levelcenter and state.camera_offset
            // state.levelcenter- the center of the current level, where you should be drawing the background
            // state.camera_offset- the offset of the camera from the levelcenter (for levels larger than one screen)
            // background
        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            // foreground
        }

        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
