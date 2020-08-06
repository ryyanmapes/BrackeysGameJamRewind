using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using System;
using System.Collections.Generic;
using System.IO;

namespace RewindGame.Game.Effects
{
    public interface ILevelEffect : IDisposable
    {
        public void Update(StateData state);

        public void DrawBackground(StateData state, SpriteBatch sprite_batch);

        public void DrawForeground(StateData state, SpriteBatch sprite_batch);

    }
}
