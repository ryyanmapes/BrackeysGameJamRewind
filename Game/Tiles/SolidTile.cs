using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Tiles
{

    public class SolidTile : CollisionObject
    {
        public SolidTile() { }

        public static SolidTile Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new SolidTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            if (GameUtils.SHOULD_SOLIDTILES_RENDER) renderer = new TileSprite(tile_sprite_, level);
            else renderer = new NullRender();

            collisionSize = new Vector2(GameUtils.TILE_WORLD_SIZE, GameUtils.TILE_WORLD_SIZE);
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state) { }

    }
}
