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
    class EternalEscapeEffects : ILevelEffect
    {
        public ContentManager Content;
        public RewindGame parentGame;
        private Texture2D greenhouseurgempt;
        private Texture2D greenhousehorizontal0urgempt;
        private Texture2D greenhousehorizontal1urgempt;
        private Texture2D greenhousehorizontal2urgempt;
        private Texture2D greenhousehorizontal3urgempt;
        private Texture2D terrariumverticalurgempt;
        public EternalEscapeEffects(RewindGame parent_game, IServiceProvider serviceProvider)
        {
            Content = new ContentManager(serviceProvider, "Content");
            parentGame = parent_game;
            //
            //TextureInit
            //
            greenhouseurgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/greenhouseurgempt");
            greenhousehorizontal0urgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/greenhousehorizontal0urgempt");
            greenhousehorizontal1urgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/greenhousehorizontal1urgempt");
            greenhousehorizontal2urgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/greenhousehorizontal2urgempt");
            greenhousehorizontal3urgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/greenhousehorizontal3urgempt");
            terrariumverticalurgempt = Content.Load<Texture2D>("effects/backgrounds/redhouse/terrariumverticalurgempt");

        }
        public void Update(StateData state)
        {

        }

        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(GameUtils.LEVEL_SIZE_X / 2, GameUtils.LEVEL_SIZE_Y / 2);
            if (parentGame.activeLevel.screensHorizontal > parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(greenhousehorizontal0urgempt, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.5f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal0urgempt.Width, greenhousehorizontal0urgempt.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal1urgempt, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.75f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal1urgempt.Width, greenhousehorizontal1urgempt.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal2urgempt, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal2urgempt.Width, greenhousehorizontal2urgempt.Height), Color.White);
                sprite_batch.Draw(greenhousehorizontal3urgempt, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 1f), (int)(CameraPosReal.Y * 0.0f), greenhousehorizontal3urgempt.Width, greenhousehorizontal3urgempt.Height), Color.White);

            }
            else if (parentGame.activeLevel.screensVertical > parentGame.activeLevel.screensHorizontal)
            {
                sprite_batch.Draw(terrariumverticalurgempt, new Vector2(CameraPosReal.X, state.camera_position.Y - GameUtils.LEVEL_SIZE_Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * .8f), terrariumverticalurgempt.Width, terrariumverticalurgempt.Height), Color.White);
            }
            else if (parentGame.activeLevel.screensHorizontal == parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(greenhouseurgempt, CameraPosReal, Color.White);
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
        }
    }
}
