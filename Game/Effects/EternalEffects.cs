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
    class EternalEffects : ILevelEffect
    {
        public ContentManager Content;
        public RewindGame parentGame;
        //
        //Textures:
        //
        private Texture2D greenhouse0;
        private Texture2D greenhousehorizontal0;
        private Texture2D greenhousehorizontal1;
        private Texture2D greenhousehorizontal2;
        private Texture2D greenhousehorizontal3;
        private Texture2D terrariumvertical;

        public EternalEffects(RewindGame parent_game, IServiceProvider serviceProvider)
        {
            Content = new ContentManager(serviceProvider, "Content");
            parentGame = parent_game;
            //
            //Texture Init
            //
            greenhouse0 = Content.Load<Texture2D>("effects/greenhouse/greenhouse0");
            greenhousehorizontal0 = Content.Load<Texture2D>("effects/greenhouse/greenhousehorizontal0");
            greenhousehorizontal1 = Content.Load<Texture2D>("effects/greenhouse/greenhousehorizontal1");
            greenhousehorizontal2 = Content.Load<Texture2D>("effects/greenhouse/greenhousehorizontal2");
            greenhousehorizontal3 = Content.Load<Texture2D>("effects/greenhouse/greenhousehorizontal3");
            terrariumvertical = Content.Load<Texture2D>("effects/greenhouse/terrariumvertical");

        }
        public void Update(StateData state)
        {
        }
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            if (parentGame.activeLevel.screensHorizontal > parentGame.activeLevel.screensVertical)
            {

            }
            else if (parentGame.activeLevel.screensVertical > parentGame.activeLevel.screensHorizontal)
            {
            }
            else if (parentGame.activeLevel.screensHorizontal == parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(greenhouse0, CameraPosReal, Color.White);
            }
        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            
        }

        public void Reset()
        {
        }


        public void Dispose()
        {
            Content.Dispose();
        }
    }
}
