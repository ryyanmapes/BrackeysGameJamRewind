using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class SolidTile : CollisionObject, ISolidTile
    {
        public TileSprite tile_sprite { get; set; }

        public SolidTile() { }

        public static SolidTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new SolidTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            tile_sprite = tile_sprite_;
            collisionSize = new Vector2(Level.TILE_WORLD_SIZE, Level.TILE_WORLD_SIZE);
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state) { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden || !RewindGame.SHOULD_SOLIDTILES_RENDER) return;
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }

    }
}
