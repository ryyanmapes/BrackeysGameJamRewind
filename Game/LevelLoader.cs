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
        freezetime,
        right_wallspike,
        left_wallspike,
        up_wallspike,
        down_wallspike,
        centerspike,
        water,
        unknown
    }

    public enum EntityType
    {
        Spawnpoint,
        Warp,
        LimboPlatform,
        LargeLimboPlatform,
        LimboSpikePlatform,
        LargeLimboSpikePlatform,
        LimboSpikyBall,
        CottonwoodPlatform,
        FloofForwards,
        FloofBackwards,
        obelisk,
        lunarshrine,
        treesear,
        unknown
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
    public class RawLayer
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
    public class RawEntities
    {
        public string name;
        public int id;
        public string _eid;
        public int x;
        public int y;
        public int originX;
        public int originY;
        public bool flippedX;
        public EntityInfo values;
    }
    public class RawLevelInfo
    {
        public string exit_right;
        public string exit_left;
        public string exit_up;
        public string exit_down;
        public string triggers;
    }
    
    public class EntityInfo
    {
        public float velocity_x = 0;
        public float velocity_y = 0;
        public float radius = 0;
        public float speed = 0;
        public int starting_rotation_degrees;
    }
    
    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void RenderTile()
        {
            //Maybe use later?
        }

        public static RawLevel GetLevelData(String level_name)
        {
            string level_json;
            using (StreamReader sr = new StreamReader("Content/levels/limbo/" + level_name + ".json"))
            {
                level_json = sr.ReadToEnd();
            }
            var raw_level = JsonConvert.DeserializeObject<RawLevel>(level_json);
            return raw_level;
        }

        public static void LoadLevel(RawLevel raw_level, String level_name, Level level)
        {
            level.name = level_name;

            level.screensHorizontal = (float)raw_level.layers[0].gridCellsX / (float)RewindGame.LEVEL_GRID_SIZE_X;
            level.screensVertical = (float)raw_level.layers[0].gridCellsY / (float)RewindGame.LEVEL_GRID_SIZE_Y;

            // todo tag processing for these

            level.connectedLevelNames[0] = raw_level.values.exit_right;
            level.connectedLevelNames[1] = raw_level.values.exit_left;
            level.connectedLevelNames[2] = raw_level.values.exit_up;
            level.connectedLevelNames[3] = raw_level.values.exit_down;

            level.startTriggers = raw_level.values.triggers;


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
                level.PlaceEntity(getEntityTypeFromName(entity.name), entity.x, entity.y, entity.values);
            }
        }

        public static void LoadTileLayer(RawLayer tile_layer, Level level)
        {
            bool is_collision_layer = false;
            bool is_technical_layer = false;
            bool is_large_tile = false;
            TileSheet sheet_type = TileSheet.none;
            int sorting_layer = 0;

            
            switch (tile_layer.name)
            {
                case "technical":
                    sheet_type = TileSheet.none;
                    is_technical_layer = true;
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
                        level.PlaceTile(getCollisionTileTypeFromID(tile), x_pos, y_pos, new TileSprite(sheet_type, tile, sorting_layer));
                    }
                    else if (is_technical_layer)
                    {
                        level.PlaceTile(getTechnicalTileTypeFromID(tile), x_pos, y_pos, new TileSprite(sheet_type, tile, sorting_layer));
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

        public static TileType getCollisionTileTypeFromID(int tile)
        {
            if (tile >= 60 && tile <= 68) 
                return TileType.freezetime;
            if (tile >= 105 && tile <= 119)
                return TileType.water;
            switch (tile)
            {
                case 40:
                case 41:
                    return TileType.centerspike;
                case 42:
                    return TileType.down_wallspike;
                case 43:
                    return TileType.right_wallspike;
                case 44:
                    return TileType.left_wallspike;
                case 45:
                    return TileType.up_wallspike;
                case 50:
                    return TileType.topleft_oneway;
                case 51:
                case 52:
                case 53:
                    return TileType.platform;
                case 54:
                    return TileType.topright_oneway;
                case 70:
                    return TileType.left_oneway;
                case 74:
                    return TileType.right_oneway;
                case 69:
                    return TileType.solid;
                default:
                    return TileType.intangible;
            }
        }

        public static TileType getTechnicalTileTypeFromID(int tile)
        {
            switch (tile)
            {
                case 46:
                    return TileType.right_transition;
                case 47:
                    return TileType.left_transition;
                case 48:
                    return TileType.up_transition;
                case 49:
                    return TileType.down_transition;
                default:
                    return TileType.intangible;
            }
        }


        public static EntityType getEntityTypeFromName(string name)
        {
            switch (name)
            {
                case "playerspawn":
                    return EntityType.Spawnpoint;
                case "warp":
                    return EntityType.Warp;
                case "limboplatform":
                    return EntityType.LimboPlatform;
                case "limboplatformlarge":
                    return EntityType.LargeLimboPlatform;
                case "limbospikeplatform":
                    return EntityType.LimboSpikePlatform;
                case "limbospikeplatformlarge":
                    return EntityType.LargeLimboSpikePlatform;
                case "limbospikyball":
                    return EntityType.LimboSpikyBall;
                case "cottonwoodplatform":
                    return EntityType.CottonwoodPlatform;
                case "floof_forward":
                    return EntityType.FloofForwards;
                case "floof_backwards":
                    return EntityType.FloofBackwards;
                case "lunarshrine":
                    return EntityType.lunarshrine;
                case "obelisk":
                    return EntityType.obelisk;
                case "treesear":
                    return EntityType.treesear;
                default:
                    Console.WriteLine("Unable to find entity type of name: {0}", name);
                    return EntityType.unknown;
            }
        }

    }
}
