using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Tiles
{

    public class RenderedSolidTile : CollisionObject
    {
        public RenderedSolidTile() { }

        public static RenderedSolidTile Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new RenderedSolidTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            renderer = new TileSprite(tile_sprite_, level);
            collisionSize = new Vector2(GameUtils.TILE_WORLD_SIZE, GameUtils.TILE_WORLD_SIZE);
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state) { }

    }
}
