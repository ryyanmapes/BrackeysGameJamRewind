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
        private Texture2D floofies;

        private int waterflow = 0;
        private int floofifate = 0;

        public CottonwoodEffects(RewindGame parent_game, IServiceProvider serviceProvider)
        {
            Content = new ContentManager(serviceProvider, "Content");
            parentGame = parent_game;
            //
            //Texture Init
            //
            cottonwood0 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwood0");
            cottonwood2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwood2");
            cottonwoodHorizontal2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodhorizontal2");
            cottonwoodHorizontalWater0 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodhorizontalwater0");
            cottonwoodHorizontalWater1 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodhorizontalwater1");
            cottonwoodHorizontalWater2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodhorizontalwater2");
            cottonwoodVertical2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodvertical2");
            cottonwoodWater0 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodwater0");
            cottonwoodWater1 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodwater1");
            cottonwoodWater2 = Content.Load<Texture2D>("effects/backgrounds/cottonwood/cottonwoodwater2");
            floofies = Content.Load<Texture2D>("effects/floofies");
        }
        public void Update(StateData state)
        {
            waterflow -= (int)(100 * state.getTimeDependentDeltaTime());
            if(waterflow >= cottonwoodHorizontalWater0.Width)
            {
                waterflow = 0;
            }
            floofifate -= (int)(100 * state.getTimeDependentDeltaTime());
            if(floofifate >= floofies.Height)
            {
                floofifate = 0;
            }
        }
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            Vector2 CameraPosRealWater = CameraPosReal - new Vector2(0, RewindGame.LEVEL_SIZE_Y / 8);
            if (parentGame.activeLevel.screensHorizontal > parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(cottonwood0, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.0f), cottonwood0.Width, cottonwood0.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater0, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater0.Width, cottonwoodHorizontalWater0.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater1, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 200, (int)(CameraPosRealWater.Y * 0.0), cottonwoodHorizontalWater1.Width, cottonwoodHorizontalWater1.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater2, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 400, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater2.Width, cottonwoodHorizontalWater2.Height), Color.White);
                //sprite_batch.Draw(cottonwood2, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2), new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), cottonwood2.Width, cottonwood2.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontal2, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), cottonwoodHorizontal2.Width, cottonwoodHorizontal2.Height), Color.White);

            }
            else if (parentGame.activeLevel.screensVertical > parentGame.activeLevel.screensHorizontal)
            {
                sprite_batch.Draw(cottonwood0, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.0f), cottonwood0.Width, cottonwood0.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater0, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater0.Width, cottonwoodHorizontalWater0.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater1, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 200, (int)(CameraPosRealWater.Y * 0.0), cottonwoodHorizontalWater1.Width, cottonwoodHorizontalWater1.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater2, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 400, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater2.Width, cottonwoodHorizontalWater2.Height), Color.White);
                //sprite_batch.Draw(cottonwood2, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2), new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), cottonwood2.Width, cottonwood2.Height), Color.White);
                sprite_batch.Draw(cottonwoodVertical2, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.5f), cottonwoodVertical2.Width, cottonwoodVertical2.Height), Color.White);
            }
            else if (parentGame.activeLevel.screensHorizontal == parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(cottonwood0, CameraPosReal, Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater0, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater0.Width, cottonwoodHorizontalWater0.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater1, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 200, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater1.Width, cottonwoodHorizontalWater1.Height), Color.White);
                sprite_batch.Draw(cottonwoodHorizontalWater2, CameraPosRealWater, new Rectangle((int)(CameraPosRealWater.X * 0.0f) + waterflow + 400, (int)(CameraPosRealWater.Y * 0.0f), cottonwoodHorizontalWater2.Width, cottonwoodHorizontalWater2.Height), Color.White);
                sprite_batch.Draw(cottonwood2, CameraPosReal, Color.White);
            }
        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            sprite_batch.Draw(floofies, CameraPosReal, new Rectangle((int)(CameraPosReal.X) - floofifate / 3, (int)(CameraPosReal.Y) + floofifate, floofies.Width, floofies.Height), Color.White);
            sprite_batch.Draw(floofies, CameraPosReal, new Rectangle(((int)(CameraPosReal.X) - floofifate / 3) + 200, ((int)(CameraPosReal.Y) + floofifate) + 200, floofies.Width, floofies.Height), Color.White);
            sprite_batch.Draw(floofies, CameraPosReal, new Rectangle(((int)(CameraPosReal.X) - floofifate / 3) + 400, ((int)(CameraPosReal.Y) + floofifate) + 400, floofies.Width, floofies.Height), Color.White);
        }

        public void Reset()
        {
            waterflow = 0;
        }


        public void Dispose()
        {
            Content.Dispose();
        }
    }
}
