using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ChaiFoxes.FMODAudio;
using ChaiFoxes.FMODAudio.Studio;
using Microsoft.Xna.Framework.Audio;
using FMOD.Studio;
using FMOD;

namespace RewindGame.Game.Sound
{
    public class SoundManager
    {
        public RewindGame parentGame;
        public ContentManager Content;
        public ChaiFoxes.FMODAudio.Studio.EventInstance limboLoop1;
        public ChaiFoxes.FMODAudio.Studio.EventInstance limboLoop2;
        public ChaiFoxes.FMODAudio.Studio.EventInstance cottonLoop1;
        public ChaiFoxes.FMODAudio.Studio.EventInstance cottonLoop2;
        public ChaiFoxes.FMODAudio.Sound playerJumpSound;
        public ChaiFoxes.FMODAudio.Sound playerLandSound;
        public ChaiFoxes.FMODAudio.Sound playerDieSound;
        public ChaiFoxes.FMODAudio.Sound peltingRain;
        public ChaiFoxes.FMODAudio.Sound sliding;
        public float fadeIntoLimboLoop2;
        public float fadeIntoCottonLoop2;
        public bool pianoFadeoutInc = false;
        public float fadeIntoPiano;
        public float menuChange;
        public bool menuInc;
        public bool canSlide = false;


        public SoundManager(RewindGame parent_game, ContentManager content)
        {
            parentGame = parent_game;
            Content = content;
            //does anything else need to be passed in initially?

            //Initialization stuff

            //playerJumpSound = CoreSystem.LoadSound("Content/sfx/jump.wav");
            //playerLandSound = CoreSystem.LoadSound("Content/sfx/wood.wav"); // needs variants?
            //playerDieSound = CoreSystem.LoadSound("Content/sfx/death.wav"); for simple sounds we should just be using the builtin Content.Load

            fadeIntoLimboLoop2 = -1f;
            fadeIntoCottonLoop2 = -1f;
            fadeIntoPiano = -1f;
            menuChange = -1f;
            FMODManager.Init(FMODMode.CoreAndStudio, "Content/music");

            StudioSystem.LoadBank("Master Bank.bank").LoadSampleData();
            StudioSystem.LoadBank("Master Bank.strings.bank").LoadSampleData();

            this.limboLoop1 = StudioSystem.GetEvent("Event:/Music/Limbo/Loop1").CreateInstance();
            this.limboLoop2 = StudioSystem.GetEvent("Event:/Music/Limbo/Loop2").CreateInstance();
            this.cottonLoop1 = StudioSystem.GetEvent("Event:/Music/Cotton Forest/Loop1").CreateInstance();
            this.cottonLoop2 = StudioSystem.GetEvent("Event:/Music/Cotton Forest/Loop2").CreateInstance();
            this.peltingRain = CoreSystem.LoadStreamedSound("peltingrain.wav");
            peltingRain.Volume = 1;
            peltingRain.Play();
            this.sliding = CoreSystem.LoadStreamedSound("slide.wav");
            sliding.Volume = 0.1f;
            sliding.Looping = true;
        }


        public void Update(StateData state)
        {
            float elapsed = (float)state.getDeltaTime();
            FMODManager.Update();
            // you can use state to get deltatime, whether time is backwards or forwards, etc
            if(fadeIntoLimboLoop2 != -1)
            {
                limboLoop1.SetParameterValue("loop1 to loop2", fadeIntoLimboLoop2);
                limboLoop2.SetParameterValue("loop1 to loop2", 1-fadeIntoLimboLoop2);
                fadeIntoLimboLoop2 -= elapsed;
                if(fadeIntoLimboLoop2 <= 0)
                {
                    fadeIntoLimboLoop2 = -1;
                    limboLoop1.Stop();
                }
            }
            if (fadeIntoCottonLoop2 != -1)
            {
                limboLoop1.SetParameterValue("loop1 to loop2", fadeIntoCottonLoop2);
                limboLoop2.SetParameterValue("loop1 to loop2", 1 - fadeIntoCottonLoop2);
                fadeIntoCottonLoop2 -= elapsed;
                if (fadeIntoCottonLoop2 <= 0)
                {
                    fadeIntoCottonLoop2 = -1;
                    cottonLoop1.Stop();
                }
            }
            if (fadeIntoPiano != -1)
            {
                fadeIntoPiano += elapsed * (pianoFadeoutInc ? 1 : -1);
                if ((fadeIntoPiano > 1 && pianoFadeoutInc) || (fadeIntoPiano < 0 && !pianoFadeoutInc))
                {
                    fadeIntoPiano = -1;
                }
                limboLoop2.SetParameterValue("full to piano only", fadeIntoPiano);
            }
            if(menuChange != -1)
            {
                if(menuInc)
                {
                    menuChange += elapsed;
                } else
                {
                    menuChange -= elapsed;
                }

                limboLoop1.SetParameterValue("menu open", menuChange);
                limboLoop2.SetParameterValue("menu open", menuChange);
                if(menuChange > 1 || menuChange < 0)
                {
                    menuChange = -1;
                }
            }

        }

        public void TriggerPlayerJump(){//playerJumpSound.Play();
            SoundEffect playerJumpSound = Content.Load<SoundEffect>("sfx/jump1");
            playerJumpSound.Play();
        }

        public void TriggerPlayerLand() { //playerLandSound.Play();
            SoundEffect playerLandSound = Content.Load<SoundEffect>("sfx/land1");
            playerLandSound.Play();
        }

        public void BeginPlayerWallslide()
        {
            if(!canSlide)
            {
                sliding.Volume = 0.1f;
                sliding.Play();
                canSlide = true;
            }

        }

        public void EndPlayerWallslide()
        {
            sliding.Volume = 0;
            sliding.Play();
            canSlide = false;
        }

        // 0 = no rain, 1 = bad storm
        public void ModifyOverrain(float intensity) {
            peltingRain.Volume = intensity;
        }
        public void EndOverrain() {
            peltingRain.Volume = 0;
            peltingRain.Play();
        }

        public void TriggerLightining() {
            SoundEffect lightningEffect = Content.Load<SoundEffect>("sfx/thunderstrike");
            lightningEffect.Play();
        }

        public void TriggerPlayerWalljump()
        {
            SoundEffect playerWallJumpSound = Content.Load<SoundEffect>("sfx/jump1");
            playerWallJumpSound.Play();
        }

        public void TriggerPlayerDie()
        {
            SoundEffect playerDieSound = Content.Load<SoundEffect>("sfx/death");
            playerDieSound.Play();
        }

        public void TriggerTimeFreeze()
        {

        }

        public void BeginLimboMusic1()
        {
            limboLoop1.Start();
        }


        public void BeginLimboMusic2()
        {
            limboLoop2.Start();
            fadeIntoLimboLoop2 = 1f;
        }

        // this is the relaxed ending theme
        public void BeginPiano()
        {
            fadeIntoPiano = 1f;
            pianoFadeoutInc = false;
        }

        public void EndPiano()
        {
            fadeIntoPiano = 0f;
            pianoFadeoutInc = true;
        }
        public void OpenMenu()
        {
            menuChange = 0f;
            menuInc = true;
        }
        public void CloseMenu()
        {
            menuChange = 1f;
            menuInc = false;
        }
        public void stopAllMusic()
        {
            limboLoop1.Stop();
            limboLoop2.Stop();
            cottonLoop1.Stop();
            cottonLoop2.Stop();
            EndPiano();
        }
        public void BeginCottonwoodMusic1()
        {
            cottonLoop1.Start();
        }
        public void BeginCottonwoodMusic2()
        {
            cottonLoop2.Start();
            fadeIntoCottonLoop2 = 1f;
        }
        public void TriggerTimeBackwards()
        {
            SoundEffect playerTimeBackSound = Content.Load<SoundEffect>("sfx/jump");
            playerTimeBackSound.Play();
        }
        public void TriggerTimeBackwardsEnd()
        {
            SoundEffect playerEndTimeBackSound = Content.Load<SoundEffect>("sfx/end time reverse");
            playerEndTimeBackSound.Play();
        }
        public void TriggerWarp()
        {
            SoundEffect warpSFX = Content.Load<SoundEffect>("sfx/area exit");
            warpSFX.Play();
        }
        //  freeze sfx.  piano stuff. area 3 music.
    }
}
