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
        public Int16 paralaxGlobal = 1;
        public Int16 paralax0 = 1;
        public Int16 paralax1 = 1;
        public Int16 paralax2 = 1;
        private Texture2D raindrop1;
        private Texture2D limboStatic;
        private Texture2D limboVertical0;
        private Texture2D limboVertical1;
        private Texture2D limboVertical2;
        private Texture2D limboHorizontal0;
        private Texture2D limboHorizontal1;
        private Texture2D limboHorizontal2;
        private Color textureColor = Color.White;

        public LimboEffects(RewindGame parent_game, ContentManager content)
        {
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            raindrop1 = Content.Load<Texture2D>("effects/raindrop1");
            limboStatic = Content.Load<Texture2D>("backgrounds/limbo/limbostatic");
            limboHorizontal0 = Content.Load<Texture2D>("backgrounds/limbo/limbohorizontal0");
            limboHorizontal1 = Content.Load<Texture2D>("backgrounds/limbo/limbohorizontal1");
            limboHorizontal2 = Content.Load<Texture2D>("backgrounds/limbo/limbohorizontal2");
            limboVertical0 = Content.Load<Texture2D>("backgrounds/limbo/limbovertical0");
            limboVertical1 = Content.Load<Texture2D>("backgrounds/limbo/limbovertical1");
            limboVertical2 = Content.Load<Texture2D>("backgrounds/limbo/limbovertical2");

        }
        //parentgame.activelevel.screenhori
        //parentgame.activelevel.screenvirti
        //if both 1 static image if not use bigest one
        public void Update(StateData state)
        {


        }

        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            // remember to use state.levelcenter and state.camera_offset
            // state.levelcenter- the center of the current level, where you should be drawing the background
            // state.camera_position- the offset of the camera from the levelcenter (for levels larger than one screen)
            // background
            sprite_batch.Draw(limboStatic, Vector2.Zero, textureColor);
        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(raindrop1, new Vector2(0, 0), textureColor);
        }

        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
