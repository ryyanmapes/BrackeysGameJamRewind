using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Debug
{

    public class DecorativeTile : GameObject
    {
        public TileSprite tile_sprite;

        public DecorativeTile() { }

        public static DecorativeTile Make(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            var tile = new DecorativeTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, TileSprite tile_sprite_)
        {
            tile_sprite = tile_sprite_;
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state) { }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            localLevel.DrawTile(tile_sprite, position, sprite_batch);
        }

    }
}
