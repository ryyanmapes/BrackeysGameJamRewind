﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Tiles
{

    class WaterTile : RenderedSolidTile
    {
        public bool isFrozen = false;

        public new static WaterTile Make(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            var tile = new WaterTile();
            tile.Initialize(level, starting_pos, tile_sprite_);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, TileSpriteInfo tile_sprite_)
        {
            base.Initialize(level, starting_pos, tile_sprite_);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            renderer.Draw(state, sprite_batch, position + new Vector2(0, state.time_data.getFloaty(position.X, true)), SpriteEffects.None);
        }

        public override void Update(StateData state)
        {
            isFrozen = state.time_data.time_status == TimeState.still;
            base.Update(state);
        }

        public override CollisionReturn getCollisionReturn()
        {
            if (isFrozen) return new CollisionReturn(CollisionType.normal, this, 5);
            return CollisionReturn.None();
        }

    }
}