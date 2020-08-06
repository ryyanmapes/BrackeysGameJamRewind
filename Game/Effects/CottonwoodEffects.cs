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
    class CottonwoodEffects : ILevelEffect
    {
        public ContentManager Content;
        public RewindGame parentGame;
        //
        //Textures:
        //
        private Texture2D cottonwood0;
        private Texture2D cottonwood2;
        private Texture2D cottonwoodHorizontal2;
        private Texture2D cottonwoodHorizontalWater0;
        private Texture2D cottonwoodHorizontalWater1;
        private Texture2D cottonwoodHorizontalWater2;
        private Texture2D cottonwoodVertical2;
        private Texture2D cottonwoodWater0;
        private Texture2D cottonwoodWater1;
        private Texture2D cottonwoodWater2;

        public CottonwoodEffects(RewindGame parent_game, ContentManager content)
        {
            Content = content;
            parentGame = parent_game;
            //
            //Texture Init
            //
            cottonwood0 = Content.Load<Texture2D>("effects/backgrounds/cottonwood0");
            cottonwood2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood2");
            cottonwoodHorizontal2 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodhorizontal2");
            cottonwoodHorizontalWater0 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodhorizontalwater0");
            cottonwoodHorizontalWater1 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodhorizontalwater1");
            cottonwoodHorizontalWater2 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodhorizontalwater2");
            cottonwoodVertical2 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodvertical2");
            cottonwoodWater0 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodwater0");
            cottonwoodWater1 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodwater1");
            cottonwoodWater2 = Content.Load<Texture2D>("effects/backgrounds/cottonwoodwater2");
        }
        public void Update(StateData state)
        {
            
        }
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            
        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            
        }


        public void Dispose()
        {
            Content.Dispose();
        }
    }
}
