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
        private Random rnd = new Random();
        private List<Texture2D> starTextures = new List<Texture2D>();
        private List<EffectParticle> starParticles = new List<EffectParticle>();

        private const int numStarParticles = 100;
        private const float starFadeIn = 0.1f;
        private const float starLifetime = 0.5f;
        private const float starFadeOut = 0.2f;

        private bool forceFadeout;

        private bool showPlayerWarp = false;
        private bool showArtifactWarp = false;
        private bool stallPlayer = false;
        private bool stallArtifact = false;
        private float growTimePlayer = 0f;
        private float growTimeArtifact = 0f;
        private float currentBeamWidthPlayer = 0f;
        private float currentBeamWidthArtifact = 0f;

        private const float stallTimeGlobal = 0f;
        private float stallTimePlayer = stallTimeGlobal;
        private float stallTimeArtifact = stallTimeGlobal;
        private float maxGrowTime = RewindGame.LEVEL_SIZE_X;

        private Vector2 playerWarpPos;
        private Vector2 artifactWarpPos;

        public OverlayEffects(RewindGame parent_game, ContentManager content)
        {
            Content = content;
            parentGame = parent_game;
            //setup textures here
            // see GameObject

            //texturename = Content.Load<Texture2D>( texturePath );
            deathSquare = Content.Load<Texture2D>("debug/square");
            starTextures.Add(Content.Load<Texture2D>("effects/star0"));
            starTextures.Add(Content.Load<Texture2D>("effects/star1"));
            starTextures.Add(Content.Load<Texture2D>("effects/star2"));
            starTextures.Add(Content.Load<Texture2D>("effects/star3"));
            starTextures.Add(Content.Load<Texture2D>("effects/star4"));
            starTextures.Add(Content.Load<Texture2D>("effects/star5"));
        }



        public void TriggerDeath()
        {
            //when death runstate is runstate.platerdead get with parentgame.runstate
            //parentgame.statetimer tells how long will stay in state
            //rewindgame.playerdeathtime is max death time (ticks down to 0(when player respawns))
            //
            //render a big black squear on entire screen (content/debug) if turned to white can be any color with draw function
            //fade into black starting at about halfway through platyerdeath time (when playerdeath time hits 50% fade to black fade out to black when it is no longer runstate.playerdead
        }
        public void StartWarpPlayer(Vector2 position, StateData state)
        {
            playerWarpPos = new Vector2(position.X + 42, position.Y);
            showPlayerWarp = true;
            parentGame.soundManager.TriggerWarp();
        }
        public void StartWarpArtifact(Vector2 position, StateData sate)
        {
            artifactWarpPos = new Vector2(position.X + 42, position.Y);
            showArtifactWarp = true;
        }
        public void StartAreaFadeout()
        {
            forceFadeout = true;
            growTimeArtifact = 0f;
            growTimePlayer = 0f;
            currentBeamWidthArtifact = 0f;
            currentBeamWidthPlayer = 0f;
            fadeValue = .25f;
        }

        public void Update(StateData state)
        {
            //StartWarpPlayer(parentGame.player.position, state);
            if (showPlayerWarp == true && currentBeamWidthPlayer < maxGrowTime)
            {
                growTimePlayer += 1f;
                currentBeamWidthPlayer = growTimePlayer * growTimePlayer;
            }
            else if(showPlayerWarp == true && currentBeamWidthPlayer >= maxGrowTime && stallPlayer == false)
            {
                stallPlayer = true;
            }
            else if(stallPlayer == true && stallTimePlayer > 0f)
            {
                stallTimePlayer -= 0.01f;
            }
            else if(stallPlayer == true && stallTimePlayer <= 0f)
            {
                showPlayerWarp = false;
                stallPlayer = false;
                stallTimePlayer = stallTimeGlobal;
            }
            if (showArtifactWarp == true && currentBeamWidthArtifact < maxGrowTime)
            {
                growTimeArtifact += 1f;
                currentBeamWidthArtifact = growTimeArtifact * growTimeArtifact;
            }
            else if (showArtifactWarp == true && currentBeamWidthArtifact >= maxGrowTime && stallArtifact == false)
            {
                stallArtifact = true;
            }
            else if (stallArtifact == true && stallTimeArtifact > 0f)
            {
                stallTimeArtifact -= 0.01f;
            }
            else if (stallArtifact == true && stallTimeArtifact <= 0f)
            {
                showArtifactWarp = false;
                //System.Diagnostics.Debugger.Break();
                stallArtifact = false;

                stallTimeArtifact = stallTimeGlobal;
            }
            if (parentGame.runState == RunState.playerdead || forceFadeout)
            {
                if (parentGame.stateTimer <= 0.25)
                {
                    fadeValue += .1f;
                    stall = 1.75f;
                     showCube = true;
                    forceFadeout = false;
                }
                if (forceFadeout)
                {
                    stall = 8f;
                    showCube = true;
                    forceFadeout = false;
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
                if (fadeValue <= 0.01f)
                {
                    showCube = false;
                    fadeValue = 0.00f;
                    fadeOut = false;
                }
            }



            for (int i = starParticles.Count - 1; i >= 0; i--)
            {
                var particle = starParticles[i];
                particle.Update(state);
                if (particle.isDead) starParticles.RemoveAt(i);
            }

        }

        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            Vector2 CameraPosReal = state.camera_position - new Vector2(RewindGame.LEVEL_SIZE_X / 2, RewindGame.LEVEL_SIZE_Y / 2);
            Vector2 playerWarpBeamPos = new Vector2(playerWarpPos.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y/2);
            Vector2 playweWarpBeamRealPos = new Vector2(playerWarpBeamPos.X - currentBeamWidthPlayer, playerWarpBeamPos.Y);
            Vector2 artifactWarpBeamPos = new Vector2(artifactWarpPos.X, state.camera_position.Y - RewindGame.LEVEL_SIZE_Y / 2);
            Vector2 artifactWarpBeamRealPos = new Vector2(artifactWarpBeamPos.X - currentBeamWidthArtifact, artifactWarpBeamPos.Y);

            foreach (EffectParticle particle in starParticles)
            {
                particle.Draw(state, sprite_batch);
            }
            if (showPlayerWarp == true)
            {
                sprite_batch.Draw(deathSquare, playweWarpBeamRealPos, new Rectangle((int)playweWarpBeamRealPos.X, (int)playweWarpBeamRealPos.Y, (int)currentBeamWidthPlayer * 2, (int)RewindGame.LEVEL_SIZE_Y), Color.White * (1));
            }
            if (showArtifactWarp == true)
            {
                sprite_batch.Draw(deathSquare, artifactWarpBeamRealPos, new Rectangle((int)artifactWarpBeamRealPos.X, (int)artifactWarpBeamRealPos.Y, (int)currentBeamWidthArtifact * 2, (int)RewindGame.LEVEL_SIZE_Y), Color.White * (1));
            }
            if (showCube)
            {
                sprite_batch.Draw(deathSquare, CameraPosReal, new Rectangle((int)CameraPosReal.X, (int)CameraPosReal.Y, (int)RewindGame.LEVEL_SIZE_X, (int)RewindGame.LEVEL_SIZE_Y), new Color(cubeColor, fadeValue), 0f, Vector2.Zero, new Vector2(RewindGame.LEVEL_SIZE_X, RewindGame.LEVEL_SIZE_Y), SpriteEffects.None, 0f);
            }
            
        }
        public void MakeStasisParticles(StateData state)
        {
            if (starParticles.Count > 100) return;
            for (int i = 0; i < numStarParticles; i++)
            {
                float rand_x =  (float)((rnd.NextDouble()-0.5) * RewindGame.LEVEL_SIZE_X) + state.camera_position.X;
                float rand_y = (float)((rnd.NextDouble() - 0.5) * RewindGame.LEVEL_SIZE_Y) + state.camera_position.Y;
                float rand_fadein = (float)((rnd.NextDouble() - 0.5) / 50) + starFadeIn;
                float rand_lifetime = (float)((rnd.NextDouble() - 0.5) / 5) + starLifetime;
                float rand_fadeout = (float)((rnd.NextDouble() - 0.5) / 30) + starFadeOut;
                Texture2D particle_tex = starTextures[rnd.Next(0, 6)];

                starParticles.Add(EffectParticle.Make(parentGame.activeLevel, new Vector2(rand_x, rand_y), rand_fadein, rand_lifetime, rand_fadeout, particle_tex));

            }


        }
        public void Dispose()
        {
            Content.Dispose();
        }

    }
}
