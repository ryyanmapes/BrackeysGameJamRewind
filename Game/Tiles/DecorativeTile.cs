﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    public class DecorativeTile : GameObject
    {
        public DecorativeTile() { }

        public static DecorativeTile Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new DecorativeTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            renderer = new TileSprite(tile_sprite_, level);
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state) { }

    }
}
