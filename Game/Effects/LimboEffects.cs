using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        private Texture2D raindrop0;
        private Texture2D raindrop1;
        private Texture2D vingette;
        private int rainfall;
        private bool speedUp;
        private bool whiteOut;
        private float speedFactor = 1f;
        private float whiteOutFade = 0f;
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
            raindrop0 = Content.Load<Texture2D>("effects/raindrop0");
            raindrop1 = Content.Load<Texture2D>("effects/raindrop1");
        }

        public void Update(StateData state)
        {
            // use this only for time-changing stuff, like falling the rain
            rainfall -= (int)(1500 * state.getTimeDependentDeltaTime() * speedFactor);
                //
            if (state.time_data.time_moment > parentGame.timeDangerPosBound * 1.11 || state.time_data.time_moment < parentGame.timeDangerNegBound * 1.11) // Black area
            {
                speedUp = true;
                whiteOut = true;
            }
            else if (state.time_data.time_moment > parentGame.timeDangerPosBound / 1.25 || state.time_data.time_moment < parentGame.timeDangerNegBound / 1.25) // Purple area
            {
                speedUp = true;
                whiteOut = false;
            }
            else
            {
                whiteOut = false;
                speedUp = false;
                speedFactor = 1f;

            }
            //   fade += (Math.Abs(state.time_data.time_moment) / parentGame.timeDangerPosBound) * 2;
            if (rainfall >= raindrop0.Height)
            {
                rainfall = 0;
            }
            if(speedUp == true)
            {
                if (speedFactor <= 2)
                {
                    speedFactor += .0225f;
                }
            }
            if (whiteOut == true)
            {
                if (whiteOutFade <= .90f)
                {
                    whiteOutFade += .1f;
                }
            }
            else if (whiteOut == false)
            {
                if (whiteOutFade >= .1f )
                {
                    whiteOutFade -= .1f;
                }
            }

        }

        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X/2, RewindGame.LEVEL_SIZE_Y/2);
            // remember to use state.levelcenter and state.camera_offset
            // state.levelcenter- the center of the current level, where you should be drawing the background
            // state.camera_position- the offset of the camera from the levelcenter (for levels larger than one screen)
            // background
            //check if level higher than long
            //vice versa
            //
            
            //sprite_batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            //sprite_batch.Draw(limboStatic, CameraPosReal, Color.White);
            if (parentGame.activeLevel.screensHorizontal > parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(limboHorizontal2, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.0f), limboHorizontal2.Width, limboHorizontal2.Height), Color.White);
                sprite_batch.Draw(limboHorizontal1, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2), new Rectangle((int)(CameraPosReal.X * 0.8f), (int)(CameraPosReal.Y * 0.0f), limboHorizontal1.Width, limboHorizontal1.Height), Color.White);
                sprite_batch.Draw(limboHorizontal0, new Vector2(CameraPosReal.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2), new Rectangle((int)(CameraPosReal.X * 0.5f), (int)(CameraPosReal.Y * 0.0f), limboHorizontal0.Width, limboHorizontal0.Height), Color.White);

            }
            else if (parentGame.activeLevel.screensVertical > parentGame.activeLevel.screensHorizontal)
            {
                sprite_batch.Draw(limboVertical2, new Vector2(state.camera_position.X - RewindGame.LEVEL_SIZE_X / 2, CameraPosReal.Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.0f), limboVertical2.Width, limboVertical2.Height), Color.White);
                sprite_batch.Draw(limboVertical1, new Vector2(state.camera_position.X - RewindGame.LEVEL_SIZE_X / 2, CameraPosReal.Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.8f), limboVertical1.Width, limboVertical1.Height), Color.White);
                sprite_batch.Draw(limboVertical0, new Vector2(state.camera_position.X - RewindGame.LEVEL_SIZE_X / 2, CameraPosReal.Y), new Rectangle((int)(CameraPosReal.X * 0.0f), (int)(CameraPosReal.Y * 0.5f), limboVertical0.Width, limboVertical0.Height), Color.White);
            }
            else if (parentGame.activeLevel.screensHorizontal == parentGame.activeLevel.screensVertical)
            {
                sprite_batch.Draw(limboStatic, CameraPosReal, Color.White);
            }
            sprite_batch.Draw(raindrop1, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 0.5f) - rainfall / 3, (int)(CameraPosReal.Y * 0.5f) + rainfall, raindrop1.Width, raindrop1.Height), Color.White);
            if (whiteOut == true)
            {
                //sprite_batch.Draw(raindrop1, (CameraPosReal - new Vector2(CameraPosReal.X, 0)), new Rectangle((int)((CameraPosReal.X) * 0.8f) - (rainfall * 2) / 3, (int)((CameraPosReal.Y) * 0.8f) + rainfall * 2, raindrop1.Width, raindrop1.Height), new Color(Color.White, fade));
                sprite_batch.Draw(raindrop1, CameraPosReal, new Rectangle((int)((CameraPosReal.X + CameraPosReal.X / 2) * .75f) - (rainfall * 2) / 3, (int)(CameraPosReal.Y * .75f) + rainfall * 2, raindrop1.Width, raindrop1.Height), Color.White * whiteOutFade);
                sprite_batch.Draw(raindrop1, CameraPosReal, new Rectangle((int)((CameraPosReal.X + CameraPosReal.X / 2) * .60f) - (rainfall * 3) / 3, (int)(CameraPosReal.Y * .60f) + rainfall * 3, raindrop1.Width, raindrop1.Height), Color.White * whiteOutFade);
            }


        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            sprite_batch.Draw(raindrop0, CameraPosReal, new Rectangle((int)(CameraPosReal.X * 1.0f) - rainfall/3, (int)(CameraPosReal.Y * 1.0f) + rainfall, raindrop0.Width, raindrop0.Height), Color.White);
            if (whiteOut == true)
            {
                sprite_batch.Draw(raindrop0, CameraPosReal, new Rectangle((int)((CameraPosReal.X + CameraPosReal.X/2) * .8f) - (rainfall * 2) / 3, (int)(CameraPosReal.Y * .8f) + rainfall * 2, raindrop0.Width, raindrop0.Height), Color.White * whiteOutFade);
                sprite_batch.Draw(raindrop0, CameraPosReal, new Rectangle((int)((CameraPosReal.X + CameraPosReal.X / 2) * .9f) - (rainfall * 3) / 3, (int)(CameraPosReal.Y * .9f) + rainfall * 3, raindrop0.Width, raindrop0.Height), Color.White * whiteOutFade);
                //sprite_batch.Draw(raindrop0, (CameraPosReal + new Vector2(CameraPosReal.X, 0)), new Rectangle((int)((CameraPosReal.X) * 0.75f) - rainfall * 2 / 3, (int)((CameraPosReal.Y) * 0.75f) + rainfall, raindrop0.Width, raindrop0.Height), new Color(Color.White, fade));
            }
        }

        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
