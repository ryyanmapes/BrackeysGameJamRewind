using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    class CollisionTile : CollisionObject, ISolidTile
    {
        public TileSprite tile_sprite { get; set; }

        public CollisionTile(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            tile_sprite = tile_sprite_;
            collisionSize = new Vector2(Level.TILE_WORLD_SIZE, Level.TILE_WORLD_SIZE);
            Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state) { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }

    }
}
