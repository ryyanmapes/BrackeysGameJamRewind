using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

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
    class RawLevel
    {
        public string ogmoVersion;
        public int width;
        public int height;
        public int offsetX;
        public int offsetY;
        public List<RawLayers> layers;
    }
    class RawLayers
    {
        public string name;
        public string _eid;
        public int offsetX;
        public int offsetY;
        public int gridCellWidth;
        public int gridCellHeight;
        public int gridCellsX;
        public int gridCellsY;
        public string tileset = null;
        public List<int> data = null;
        public int exportMode;
        public int arrayMode;
        public List<RawEntities> entities = null;

    }
    class RawEntities
    {
        public string name;
        public int id;
        public string _eid;
        public int x;
        public int y;
        public int originX;
        public int originY;
        public bool flippedX;
    }
    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void LoadLevel(String fileName, Level unloadedLevel)
        {
            string levelStuff;
            using (StreamReader sr = new StreamReader("Content/levels/"+fileName))
            {
                 levelStuff = sr.ReadToEnd();
            }
            var newLevel = JsonConvert.DeserializeObject<RawLevel>(levelStuff);
            // call level.placeTile, level.placeEntity, level.placeDecorative
            //todo
        }

    }
}
