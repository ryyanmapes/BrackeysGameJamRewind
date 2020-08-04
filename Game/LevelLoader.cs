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
        right_transition,
        left_transition,
        up_transition,
        down_transition,
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
        public RawLevelInfo values;
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
    class RawLevelInfo
    {
        public string exit_right;
        public string exit_left;
        public string exit_up;
        public string exit_down;
    }
    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void RenderTile()
        {
            //Maybe use later?
        }
        public static void LoadLevel(String level_name, Level level)
        {
            level.name = level_name;

            string level_json;
            using (StreamReader sr = new StreamReader("Content/levels/"+level_name+".json"))
            {
                 level_json = sr.ReadToEnd();
            }
            var raw_level = JsonConvert.DeserializeObject<RawLevel>(level_json);

            level.screensHorizontal = (float)raw_level.layers[0].gridCellsX / (float)RewindGame.LEVEL_GRID_SIZE_X;
            level.screensVertical = (float)raw_level.layers[0].gridCellsY / (float)RewindGame.LEVEL_GRID_SIZE_Y;

            // todo tag processing for these

            level.connectedLevelNames[0] = raw_level.values.exit_right;
            level.connectedLevelNames[1] = raw_level.values.exit_left;
            level.connectedLevelNames[2] = raw_level.values.exit_up;
            level.connectedLevelNames[3] = raw_level.values.exit_down;


            foreach (RawLayer layer in raw_level.layers)
            {
                if (layer.name == "entities")
                {
                    LoadEntities(layer, level);
                }
                else
                {
                    LoadTileLayer(layer, level);
                }
            }
        }

        public static void LoadEntities(RawLayer tile_layer, Level level)
        {
            foreach (RawEntities entity in tile_layer.entities)
            {
                level.PlaceEntity(getEntityTypeFromName(entity.name), entity.x, entity.y);
            }
        }

        public static void LoadTileLayer(RawLayer tile_layer, Level level)
        {
            bool is_collision_layer = false;
            bool is_large_tile = false;
            TileSheet sheet_type = TileSheet.none;
            int sorting_layer = 0;

            
            switch (tile_layer.name)
            {
                case "technical":
                    is_collision_layer = true;
                    sheet_type = TileSheet.none;
                    break;
                case "collisionlayer":
                    sheet_type = TileSheet.collision;
                    is_collision_layer = true;
                    sorting_layer = 1;
                    break;
                case "background":
                    sheet_type = TileSheet.collision;
                    sorting_layer = -1;
                    break;
                case "foreground":
                    sorting_layer = 2;
                    is_large_tile = true;
                    sheet_type = TileSheet.decorative;
                    break;
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
                case 49:
                    return TileType.topright_oneway;
                case 48:
                    return TileType.platform;
                case 47:
                    return TileType.topleft_oneway;
                case 51:
                    return TileType.left_oneway;
                case 50:
                    return TileType.right_oneway;
                case 69:
                    return TileType.right_transition;
                case 70:
                    return TileType.left_transition;
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
