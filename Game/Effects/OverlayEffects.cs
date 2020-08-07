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

        private Texture2D deathSquare;
        private float fadeValue = 0f;
        private bool showCube = false;
        private Color cubeColor = Color.Black;
        private float stall = 1.75f;
        private bool fadeOut = false;
        private float starFade = 0f;
        private float starAlpha = 0f;
        private bool showStars = false;
        private Random rnd = new Random();
        private Vector2 starPos0, starPos1, starPos2, starPos3, starPos4, starPos5, starPos6, starPos7, starPos8, starPos9;
        private Texture2D starType0, starType1, starType2, starType3, starType4, starType5, starType6, starType7, starType8, starType9;
        private float fadeRate0, fadeRate1, fadeRate2, fadeRate3, fadeRate4, fadeRate5, fadeRate6, fadeRate7, fadeRate8, fadeRate9;
        private Vector2 starPos10, starPos11, starPos12, starPos13, starPos14, starPos15, starPos16, starPos17, starPos18, starPos19;
        private Texture2D starType10, starType11, starType12, starType13, starType14, starType15, starType16, starType17, starType18, starType19;
        private float fadeRate10, fadeRate11, fadeRate12, fadeRate13, fadeRate14, fadeRate15, fadeRate16, fadeRate17, fadeRate18, fadeRate19;

        public OverlayEffects(RewindGame parent_game, ContentManager content) 
        { 
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
            deathSquare = Content.Load<Texture2D>("debug/square");
            //star0 = Content.Load<Texture2D>("effects/star0");
            //star1 = Content.Load<Texture2D>("effects/star1");
            //star2 = Content.Load<Texture2D>("effects/star2");
            //star3 = Content.Load<Texture2D>("effects/star3");
            //star4 = Content.Load<Texture2D>("effects/star4");
            //star5 = Content.Load<Texture2D>("effects/star5");
        }



        public void TriggerDeath()
        {
            //when death runstate is runstate.platerdead get with parentgame.runstate
            //parentgame.statetimer tells how long will stay in state
            //rewindgame.playerdeathtime is max death time (ticks down to 0(when player respawns))
            //
            //render a big black squear on entire screen (content/debug) if turned to white can be any color with draw function
            //fade into black starting at about halfway through platyerdeath time (when playerdeath time hits 50% fade to black) fade out to black when it is no longer runstate.playerdead
        }

        public void Update(StateData state)
        {
            if (parentGame.runState == RunState.playerdead)
            {
                if (parentGame.stateTimer <= 0.25)
                {
                    fadeValue += .1f;
                    showCube = true;
                }
            }
            else if (showCube == true)
            {
                if (fadeValue < stall && fadeOut == false)
                {
                    fadeValue += .1f;
                }
                else if (fadeValue >= stall && fadeOut == false)
                {
                    fadeValue -= .05f;
                    fadeOut = true;
                }
                else if (fadeOut == true)
                {
                    fadeValue -= .05f;
                }
                if(fadeValue <= 0.01f)
                {
                    showCube = false;
                    fadeValue = 0.00f;
                    fadeOut = false;
                }
            }
                if (showStars == false && state.getTimeDependentDeltaTime() == 0 && parentGame.runState != RunState.playerdead && !showCube)
                {
                    showStars = true;
                    starFade = 1f;
                    starAlpha = 1f;
                    MakeStasisParticles(state);
                }
                else if (showStars == true && state.getTimeDependentDeltaTime() == 0 && starAlpha >= 0.0 && parentGame.runState != RunState.playerdead && !showCube)
                {
                     starAlpha -= .01f;
                }
                else if (showStars == true && state.getTimeDependentDeltaTime() == 0 && starAlpha <= 0 && parentGame.runState != RunState.playerdead && !showCube)
                 {
                starFade -= 1f;
                   }
                else if (showStars == true && state.getTimeDependentDeltaTime() != 0 && parentGame.runState != RunState.playerdead && !showCube)
                {
                    showStars = false;
                 }
        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            if (showCube)
            {
                sprite_batch.Draw(deathSquare, CameraPosReal, new Rectangle((int)CameraPosReal.X, (int)CameraPosReal.Y, (int)RewindGame.LEVEL_SIZE_X, (int)RewindGame.LEVEL_SIZE_Y), new Color(cubeColor, fadeValue), 0f, Vector2.Zero, new Vector2(RewindGame.LEVEL_SIZE_X, RewindGame.LEVEL_SIZE_Y), SpriteEffects.None, 0f);
            }
            if (showStars == true)
            {
                sprite_batch.Draw(starType0, starPos0, Color.White * (starFade * fadeRate0));
                sprite_batch.Draw(starType1, starPos1, Color.White * (starFade * fadeRate1));
                sprite_batch.Draw(starType2, starPos2, Color.White * (starFade * fadeRate2));
                sprite_batch.Draw(starType3, starPos3, Color.White * (starFade * fadeRate3));
                sprite_batch.Draw(starType4, starPos4, Color.White * (starFade * fadeRate4));
                sprite_batch.Draw(starType5, starPos5, Color.White * (starFade * fadeRate5));
                sprite_batch.Draw(starType6, starPos6, Color.White * (starFade * fadeRate6));
                sprite_batch.Draw(starType7, starPos7, Color.White * (starFade * fadeRate7));
                sprite_batch.Draw(starType8, starPos8, Color.White * (starFade * fadeRate8));
                sprite_batch.Draw(starType9, starPos9, Color.White * (starFade * fadeRate9));
                sprite_batch.Draw(starType10, starPos10, Color.White * (starFade * fadeRate10));
                sprite_batch.Draw(starType11, starPos11, Color.White * (starFade * fadeRate11));
                sprite_batch.Draw(starType12, starPos12, Color.White * (starFade * fadeRate12));
                sprite_batch.Draw(starType13, starPos13, Color.White * (starFade * fadeRate13));
                sprite_batch.Draw(starType14, starPos14, Color.White * (starFade * fadeRate14));
                sprite_batch.Draw(starType15, starPos15, Color.White * (starFade * fadeRate15));
                sprite_batch.Draw(starType16, starPos16, Color.White * (starFade * fadeRate16));
                sprite_batch.Draw(starType17, starPos17, Color.White * (starFade * fadeRate17));
                sprite_batch.Draw(starType18, starPos18, Color.White * (starFade * fadeRate18));
                sprite_batch.Draw(starType19, starPos19, Color.White * (starFade * fadeRate19));
            }
        }
        public void MakeStasisParticles(StateData state)
        {
            starPos0 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos1 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos2 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos3 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos4 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos5 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos6 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos7 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos8 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos9 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starType0 = Content.Load<Texture2D>("effects/star" + rnd.Next(0,6));
            starType1 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType2 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType3 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType4 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType5 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType6 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType7 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType8 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType9 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            fadeRate0 = rnd.Next(1, 10);
            fadeRate1 = rnd.Next(1, 10);
            fadeRate2 = rnd.Next(1, 10);
            fadeRate3 = rnd.Next(1, 10);
            fadeRate4 = rnd.Next(1, 10);
            fadeRate5 = rnd.Next(1, 10);
            fadeRate6 = rnd.Next(1, 10);
            fadeRate7 = rnd.Next(1, 10);
            fadeRate8 = rnd.Next(1, 10);
            fadeRate9 = rnd.Next(1, 10);
            starPos10 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos11 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos12 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos13 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos14 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos15 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos16 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos17 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos18 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starPos19 = new Vector2(rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X)), rnd.Next((int)-Math.Abs(state.camera_position.X), (int)Math.Abs(state.camera_position.X))) + state.camera_position;
            starType10 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType11 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType12 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType13 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType14 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType15 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType16 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType17 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType18 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            starType19 = Content.Load<Texture2D>("effects/star" + rnd.Next(0, 6));
            fadeRate10 = rnd.Next(1, 10);
            fadeRate11 = rnd.Next(1, 10);
            fadeRate12 = rnd.Next(1, 10);
            fadeRate13 = rnd.Next(1, 10);
            fadeRate14 = rnd.Next(1, 10);
            fadeRate15 = rnd.Next(1, 10);
            fadeRate16 = rnd.Next(1, 10);
            fadeRate17 = rnd.Next(1, 10);
            fadeRate18 = rnd.Next(1, 10);
            fadeRate19 = rnd.Next(1, 10);


        }
        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
