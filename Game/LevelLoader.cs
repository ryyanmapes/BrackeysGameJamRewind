using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public enum TileType
    {
        intangible,
        solid,
        death,
        platform,
        freezetime
    }

    public enum EntityType
    {
        playerspawn
    }

    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void LoadLevel(String level_name, Level level)
        {

            // call level.placeTile, level.placeEntity, level.placeDecorative
            //todo
        }

    }
}
