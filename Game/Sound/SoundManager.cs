using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace RewindGame.Game.Sound
{
    public class SoundManager
    {
        public RewindGame parentGame;
        public ContentManager Content;

        public SoundManager(RewindGame parent_game, ContentManager content)
        {
            parentGame = parentGame;
            Content = content;
            //does anything else need to be passed in initially?
        }


        public void Update(StateData state)
        {
            // you can use state to get deltatime, whether time is backwards or forwards, etc
        }

        public void TriggerPlayerJump()
        {

        }

        public void TriggerPlayerLand()
        {

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

        }

        public void TriggerTimeFreeze()
        {

        }

        public void BeginLimboMusic1()
        {

        }


        public void BeginLimboMusic2()
        {

        }

        // this is the relaxed ending theme
        public void BeginLimboMusic3()
        {

        }
    }
}
