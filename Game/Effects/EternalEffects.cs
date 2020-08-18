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
            greenhouse0 = Content.Load<Texture2D>("effects/backgrounds/greenhouse/greenhouse0");
            greenhousehorizontal0 = Content.Load<Texture2D>("effects/backgrounds/greenhouse/greenhousehorizontal0");
            greenhousehorizontal1 = Content.Load<Texture2D>("effects/backgrounds/greenhouse/greenhousehorizontal1");
            greenhousehorizontal2 = Content.Load<Texture2D>("effects/backgrounds/greenhouse/greenhousehorizontal2");
            greenhousehorizontal3 = Content.Load<Texture2D>("effects/backgrounds/greenhouse/greenhousehorizontal3");
            terrariumvertical = Content.Load<Texture2D>("effects/backgrounds/greenhouse/terrariumvertical");

        }
        public void Update(StateData state)
        {
        }
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(GameUtils.LEVEL_SIZE_X / 2, GameUtils.LEVEL_SIZE_Y / 2);
            if (parentGame.activeLevel.screensHorizontal > parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(greenhousehorizontal0, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.5f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal0.Width, greenhousehorizontal0.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal1, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.75f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal1.Width, greenhousehorizontal1.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal2, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal2.Width, greenhousehorizontal2.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal3, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 1f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal3.Width, greenhousehorizontal3.Height), Color.White);

            }
            else if (parentGame.activeLevel.screensVertical > parentGame.activeLevel.screensHorizontal)
            {
                sprite_batch.Draw(terrariumvertical, new Vector2(CameraPosReal.X, state.camera_position.Y - GameUtils.LEVEL_SIZE_Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * .8f), terrariumvertical.Width, terrariumvertical.Height), Color.White);
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
