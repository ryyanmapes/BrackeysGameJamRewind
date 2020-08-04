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
        left_oneway,
        right_oneway,
        topleft_oneway,
        topright_oneway,
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


            foreach (RawEntities entity in raw_level.layers[0].entities)
            {
                level.PlaceEntity(getEntityTypeFromName(entity.name), entity.x, entity.y);
            }

            
            LoadTileLayer(raw_level.layers[4], level);
            LoadTileLayer(raw_level.layers[3], level);
            LoadTileLayer(raw_level.layers[1], level);

            LoadTileLayer(raw_level.layers[2], level);
            // call level.placeTile, level.placeEntity, level.placeDecorative
            //todo
        }


        public static void LoadTileLayer(RawLayer tile_layer, Level level)
        {
            bool is_collision_layer = false;
            bool is_large_tile = false;
            TileSheet sheet_type = TileSheet.none;
            int sorting_layer = 0;

            /*
            switch (tile_layer.name)
            {
                case "collisionlayer":
                    is_collision_layer = true;
                    sorting_layer = 1;
                    break;
                case "background":
                    sorting_layer = -1;
                    break;
                case "foreground":
                    sorting_layer = 2;
                    is_large_tile = true;
                    sheet_type = TileSheet.decorative;
                    break;
            }*/
            if (tile_layer.name == "collisionlayer")
            {
                sheet_type = TileSheet.collision;
                is_collision_layer = true;
            }

            int n = 0;
            int x_pos = 0;
            int y_pos = 0;
            while (n < tile_layer.data.Count)
            {
                int tile = tile_layer.data[n];

                if (tile != -1)
                {

                    if (is_collision_layer)
                    {
                        level.PlaceTile(getTileTypeFromID(tile), x_pos, y_pos, new TileSprite(sheet_type, tile, sorting_layer));
                    }
                    else
                    {
                        level.PlaceDecorative(is_large_tile, x_pos, y_pos, new TileSprite(sheet_type, tile, sorting_layer));
                    }
                }

                n++;
                x_pos++;
                if (x_pos >= tile_layer.gridCellsX)
                {
                    x_pos = 0;
                    y_pos++;
                }
            }
        }

        public static TileType getTileTypeFromID(int tile)
        {
            switch (tile)
            {
                case 47:
                    return TileType.topright_oneway;
                case 48:
                    return TileType.platform;
                case 49:
                    return TileType.topleft_oneway;
                case 50:
                    return TileType.left_oneway;
                case 51:
                    return TileType.right_oneway;
                default:
                    return TileType.solid;
            }
        }

        public static EntityType getEntityTypeFromName(string name)
        {
            switch (name)
            {
                case "Spawnpoint":
                    return EntityType.Spawnpoint;
                default:
                    Console.WriteLine("Unable to find entity type of name: {0}", name);
                    return EntityType.Spawnpoint;
            }
        }

    }
}
