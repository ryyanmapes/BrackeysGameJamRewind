using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ChaiFoxes.FMODAudio;
using ChaiFoxes.FMODAudio.Studio;
using FMOD.Studio;
using FMOD;

namespace RewindGame.Game.Sound
{
    public class SoundManager
    {
        public RewindGame parentGame;
        public ContentManager Content;
        public ChaiFoxes.FMODAudio.Studio.EventInstance loop1;
        public ChaiFoxes.FMODAudio.Studio.EventInstance loop2;
        public float fadeIntoLoop2;
        public bool pianoFadeoutInc = false;
        public float fadeIntoPiano;
        public float menuChange;
        public bool menuInc;


        public SoundManager(RewindGame parent_game, ContentManager content)
        {
            parentGame = parent_game;
            Content = content;
            //does anything else need to be passed in initially?

            //Initialization stuff?
            fadeIntoLoop2 = -1f;
            fadeIntoPiano = -1f;
            menuChange = -1f;
            FMODManager.Init(FMODMode.CoreAndStudio, "Content/music");

            StudioSystem.LoadBank("Master Bank.bank").LoadSampleData();
            StudioSystem.LoadBank("Master Bank.strings.bank").LoadSampleData();

            this.loop1 = StudioSystem.GetEvent("Event:/Music/Limbo/Loop1").CreateInstance();
            this.loop2 = StudioSystem.GetEvent("Event:/Music/Limbo/Loop2").CreateInstance();





        }


        public void Update(StateData state)
        {
            float elapsed = (float)state.getDeltaTime();
            FMODManager.Update();
            // you can use state to get deltatime, whether time is backwards or forwards, etc
            if(fadeIntoLoop2 != -1)
            {
                loop1.SetParameterValue("loop1 to loop2", 1-fadeIntoLoop2);
                loop2.SetParameterValue("loop1 to loop2", fadeIntoLoop2);
                fadeIntoLoop2 -= elapsed;
                if(fadeIntoLoop2 == 0)
                {
                    fadeIntoLoop2 = -1;
                }
            }
            if(fadeIntoPiano != -1)
            {
                fadeIntoPiano += elapsed * (pianoFadeoutInc ? 1 : -1);
                if ((fadeIntoPiano > 1 && pianoFadeoutInc) || (fadeIntoPiano < 0 && !pianoFadeoutInc))
                {
                    fadeIntoPiano = -1;
                }
                loop2.SetParameterValue("full to piano only", fadeIntoPiano);
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

                loop1.SetParameterValue("menu open", menuChange);
                loop2.SetParameterValue("menu open", menuChange);
                if(menuChange > 1 || menuChange < 0)
                {
                    menuChange = -1;
                }
            }

        }

        public void TriggerPlayerJump()
        {
            var jump = CoreSystem.LoadStreamedSound("jump.wav");
            var channel = jump.Play();
        }

        public void TriggerPlayerLand()
        {
            var land = CoreSystem.LoadStreamedSound("land.wav");
            var channel = land.Play();
        }

        public void BeginPlayerWallslide()
        {

        }

        public void EndPlayerWallslide()
        {

        }

        public void TriggerPlayerWalljump()
        {

        }

        public void TriggerPlayerDie()
        {
            var death = CoreSystem.LoadStreamedSound("death.wav");
            var channel = death.Play();
        }

        public void TriggerTimeFreeze()
        {

        }

        public void BeginLimboMusic1()
        {
            loop1.Start();
        }


        public void BeginLimboMusic2()
        {
            loop2.Start();
            fadeIntoLoop2 = 1f;
        }

        // this is the relaxed ending theme
        public void BeginPiano()
        {
            fadeIntoPiano = 1f;
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
        public void BeginLimboMusic3()
        {

        }
    }
}
