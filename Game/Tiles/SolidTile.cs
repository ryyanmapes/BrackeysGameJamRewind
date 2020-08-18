using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    public class SolidTile : CollisionObject
    {
        public TileSprite tile_sprite;

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
            collisionSize = new Vector2(GameUtils.TILE_WORLD_SIZE, GameUtils.TILE_WORLD_SIZE);
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state) { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden || !GameUtils.SHOULD_SOLIDTILES_RENDER) return;
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }

    }
}
