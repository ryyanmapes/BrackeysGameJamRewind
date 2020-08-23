using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{
    class LinkedSolid : SimpleSolid
    {

        public static LinkedSolid Make(Level level, Vector2 starting_pos, Vector2 size, Entity linked_entity)
        {
            var tile = new LinkedSolid();
            tile.Initialize(level, starting_pos, size, linked_entity);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 size, Entity linked_entity)
        {
            renderer = new BasicSprite("debug/square");
            renderWithCollisionBox = true;
            linkedEntity = linked_entity;
            base.Initialize(level, starting_pos, Vector2.Zero, size);
        }

    }
}
