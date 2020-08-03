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
        Spawnpoint
    }

    class RawLevel
    {
        public string ogmoVersion;
        public int width;
        public int height;
        public int offsetX;
        public int offsetY;
        public List<RawLayer> layers;
    }
    class RawLayer
    {
        public string name;
        public string _eid;
        public int offsetX;
        public int offsetY;
        public int gridCellWidth;
        public int gridCellHeight;
        public int gridCellsX;
        public int gridCellsY;
        public string tileset;
        public List<int> data;
        public int exportMode;
        public int arrayMode;
        public List<RawEntities> entities;

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
        public static void RenderTile()
        {
            //Maybe use later?
        }
        public static void LoadLevel(String fileName, Level level)
        {
            string level_json;
            using (StreamReader sr = new StreamReader("Content/levels/"+fileName))
            {
                 level_json = sr.ReadToEnd();
            }
            var raw_level = JsonConvert.DeserializeObject<RawLevel>(level_json);


            foreach (RawEntities entity in raw_level.layers[1].entities)
            {
                switch (entity.name)
                {
                    case "Spawnpoint":
                        level.PlaceEntity(EntityType.Spawnpoint, new Vector2(entity.x, entity.y));
                        break;
                    default:
                        Console.WriteLine("Unable to find entity type of name: {0}", entity.name);
                        break;
                }
            }

            LoadTileLayer(raw_level.layers[0], level);
            LoadTileLayer(raw_level.layers[2], level);
            LoadTileLayer(raw_level.layers[3], level);
            // call level.placeTile, level.placeEntity, level.placeDecorative
            //todo
        }


        public static void LoadTileLayer(RawLayer tile_layer, Level level)
        {

            bool isCollisionLayer = tile_layer.name == "collisionlayer";
            bool isForeground = tile_layer.name == "foreground";

            int n = 0;
            int x_pos = 0;
            int y_pos = 0;
            while (n < tile_layer.data.Count)
            {
                if (tile_layer.data[n] != -1)
                {

                    if (isCollisionLayer)
                    {
                        switch (tile_layer.data[n])
                        {
                            default:
                                level.PlaceTile(TileType.solid, x_pos, y_pos, new TileSprite());
                                break;
                        }
                    }
                    else
                    {
                        level.PlaceDecorative(x_pos, y_pos, new TileSprite());
                    }
                }

                n++;
                x_pos++;
                if (x_pos > tile_layer.gridCellsX)
                {
                    x_pos = 0;
                    y_pos++;
                }
            }
        }

    }
}
