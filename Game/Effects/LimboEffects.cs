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
    public class LimboEffects : ILevelEffect
    {
        public ContentManager Content;
        public RewindGame parentGame;
        private Texture2D limboStatic;
        private Texture2D limboHorizontal0;
        private Texture2D limboHorizontal1;
        private Texture2D limboHorizontal2;
        private Texture2D limboVertical0;
        private Texture2D limboVertical1;
        private Texture2D limboVertical2;
        public LimboEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
            limboStatic = Content.Load<Texture2D>("effects/backgrounds/limbo/limbostatic");
            limboHorizontal0 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbohorizontal0");
            limboHorizontal1 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbohorizontal1");
            limboHorizontal2 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbohorizontal2");
            limboVertical0 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbovertical0");
            limboVertical1 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbovertical1");
            limboVertical2 = Content.Load<Texture2D>("effects/backgrounds/limbo/limbovertical2");
        }

        public void Update(StateData state)
        {
            // use this only for time-changing stuff, like falling the rain
            // DrawBackground(state, sprite_batch);

        }

        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X/2, RewindGame.LEVEL_SIZE_Y/2);
            // remember to use state.levelcenter and state.camera_offset
            // state.levelcenter- the center of the current level, where you should be drawing the background
            // state.camera_position- the offset of the camera from the levelcenter (for levels larger than one screen)
            // background
            //sprite_batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            //sprite_batch.Draw(limboStatic, CameraPosReal, Color.White);
            sprite_batch.Draw(limboHorizontal0, CameraPosReal,  Color.White);
            sprite_batch.Draw(limboHorizontal1, CameraPosReal,  Color.White);
            sprite_batch.Draw(limboHorizontal2, CameraPosReal,  Color.White);


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
