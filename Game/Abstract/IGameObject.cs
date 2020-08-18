using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public interface IGameObject
    {

        public abstract void Initialize(Level level, Vector2 starting_pos);

        public abstract void LoadContent();

        public abstract void Update(StateData state);

        public abstract void Draw(StateData state, SpriteBatch sprite_batch);

        public abstract void Reset();

        public abstract void SetInactive();

        public abstract void SetActive();

    }
}
