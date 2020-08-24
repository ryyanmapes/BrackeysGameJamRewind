using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using RewindGame.Game.Tiles;
using RewindGame.Game.Solids;
using RewindGame.Game.Special;
using RewindGame.Game.Physics.Temporal;

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
        public List<NodeInfo> nodes;
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
        public int rotations=1;
    }

    public class NodeInfo
    {
        public int x;
        public int y;
    }
    
    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void RenderTile()
        {
            //Maybe use later?
        }

        public static RawLevel GetLevelData(String level_name, AreaState area)
        {
            string prefix;
            switch (area)
            {
                case AreaState.limbo:
                    prefix = "Content/levels/limbo/";
                    break;
                case AreaState.cotton:
                    prefix = "Content/levels/cotton/";
                    break;
                case AreaState.eternal:
                    prefix = "Content/levels/eternal/";
                    break;
                default:
                    prefix = "Content/levels/";
                    break;
            }

            string level_json;
            using (StreamReader sr = new StreamReader(prefix + level_name + ".json"))
            {
                level_json = sr.ReadToEnd();
            }
            var raw_level = JsonConvert.DeserializeObject<RawLevel>(level_json);
            return raw_level;
        }

        public static void LoadLevel(RawLevel raw_level, String level_name, Level level)
        {
            level.name = level_name;

            level.screensHorizontal = (float)raw_level.layers[0].gridCellsX / (float)GameUtils.LEVEL_GRID_SIZE_X;
            level.screensVertical = (float)raw_level.layers[0].gridCellsY / (float)GameUtils.LEVEL_GRID_SIZE_Y;

            // todo tag processing for these

            level.connectedLevelNames[0] = raw_level.values.exit_right;
            level.connectedLevelNames[1] = raw_level.values.exit_left;
            level.connectedLevelNames[2] = raw_level.values.exit_up;
            level.connectedLevelNames[3] = raw_level.values.exit_down;

            level.startTriggers = raw_level.values.triggers;

            LoadTileLayer(raw_level.layers[5], level);
            LoadEntities(raw_level.layers[0], level);
            LoadTileLayer(raw_level.layers[1], level);
            LoadTileLayer(raw_level.layers[2], level);
            LoadTileLayer(raw_level.layers[3], level);
            LoadTileLayer(raw_level.layers[4], level);
            LoadTileLayer(raw_level.layers[6], level);
        }

        public static void LoadEntities(RawLayer tile_layer, Level level)
        {
            foreach (RawEntities entity in tile_layer.entities)
            {
                PlaceEntity(entity.name, entity.x, entity.y, entity.values, entity.nodes, level);
            }
        }

        public static void LoadTileLayer(RawLayer tile_layer, Level level)
        {
            bool is_collision_layer = false;
            bool is_technical_layer = false;
            bool is_large_tile = false;
            bool is_foreforefront = false;
            TileSheet sheet_type = TileSheet.none;
            SortingLayer sorting_layer = SortingLayer.normal;

            
            switch (tile_layer.name)
            {
                case "technical":
                    sheet_type = TileSheet.none;
                    is_technical_layer = true;
                    break;
                case "collisionlayer":
                    sheet_type = TileSheet.collision;
                    is_collision_layer = true;
                    break;
                case "background":
                    sheet_type = TileSheet.collision;
                    sorting_layer = SortingLayer.background;
                    break;
                case "visual":
                    sheet_type = TileSheet.collision;
                    break;
                case "foreground":
                    sorting_layer = SortingLayer.foreground;
                    is_large_tile = true;
                    sheet_type = TileSheet.decorative;
                    break;
                case "foreforeground":
                    sorting_layer = SortingLayer.foreforeground;
                    sheet_type = TileSheet.collision;
                    is_foreforefront = true;
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
                        PlaceTile(getCollisionTileTypeFromID(tile), x_pos, y_pos, new TileSpriteInfo(sheet_type, tile), level);
                    }
                    else if (is_technical_layer)
                    {
                        PlaceTile(getTechnicalTileTypeFromID(tile), x_pos, y_pos, new TileSpriteInfo(sheet_type, tile), level);
                    }
                    else
                    {
                        PlaceDecorative(is_large_tile, x_pos, y_pos, new TileSpriteInfo(sheet_type, tile), sorting_layer, level);
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


        public static void PlaceTile(TileType type, int x, int y, TileSpriteInfo sprite, Level level)
        {
            Vector2 position = level.getPositionFromGrid(x, y);

            switch (type)
            {
                case TileType.intangible:
                    // I don't think any of these will ever be rendered
                    //sceneDecorativesForeground.Add(DecorativeTile.Make(level, position, sprite));
                    break;
                case TileType.solid:
                    level.AddSolid(SolidTile.Make(level, position, sprite));
                    break;
                case TileType.platform:
                    level.AddSolid(PlatformTile.Make(level, position, sprite));
                    break;
                case TileType.left_oneway:
                    level.AddSolid(LeftOnewayTile.Make(level, position, sprite));
                    break;
                case TileType.right_oneway:
                    level.AddSolid(RightOnewayTile.Make(level, position, sprite));
                    break;
                case TileType.topleft_oneway:
                    level.AddSolid(PlatformTile.Make(level, position, sprite));
                    level.AddSolid(LeftOnewayTile.Make(level, position, sprite));
                    break;
                case TileType.topright_oneway:
                    level.AddSolid(PlatformTile.Make(level, position, sprite));
                    level.AddSolid(RightOnewayTile.Make(level, position, sprite));
                    break;
                case TileType.right_transition:
                    level.AddSolid(TransitionTriggerTile.Make(level, position, sprite, MoveDirection.right));
                    break;
                case TileType.left_transition:
                    level.AddSolid(TransitionTriggerTile.Make(level, position, sprite, MoveDirection.left));
                    break;
                case TileType.up_transition:
                    level.AddSolid(TransitionTriggerTile.Make(level, position, sprite, MoveDirection.up));
                    break;
                case TileType.down_transition:
                    level.AddSolid(TransitionTriggerTile.Make(level, position, sprite, MoveDirection.down));
                    break;
                case TileType.right_wallspike:
                    level.AddSolid(RightWallspike.Make(level, position, sprite));
                    break;
                case TileType.left_wallspike:
                    level.AddSolid(LeftWallspike.Make(level, position, sprite));
                    break;
                case TileType.up_wallspike:
                    level.AddSolid(TopSpike.Make(level, position, sprite));
                    break;
                case TileType.down_wallspike:
                    level.AddSolid(BottomSpike.Make(level, position, sprite));
                    break;
                case TileType.centerspike:
                    level.AddSolid(Centerspike.Make(level, position, sprite));
                    break;
                case TileType.freezetime:
                    level.AddSolid(StaticZone.Make(level, position, sprite));
                    break;
                case TileType.water:
                    level.AddSolid(WaterTile.Make(level, position, sprite));
                    break;
                default:
                    // todo
                    break;
            }
        }

        public static void PlaceEntity(string name, int x, int y, EntityInfo ent_info, List<NodeInfo> node_infos, Level level)
        {
            Vector2 position = new Vector2(x, y) * 4 + level.levelOrgin;

            switch (name)
            {
                case "playerspawn":
                    position.Y += 55;
                    position.X += 24;
                    level.playerSpawnpoint = position;
                    return;
                case "warp":
                    level.warp = Warp.Make(level, position);
                    return;
                case "limboplatform":
                    level.AddSolid(LimboPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "limboplatformlarge":
                    level.AddSolid(LimboPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), true));
                    return;
                case "limbospikeplatform":
                    level.AddSolid(LimboSpikePlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "limbospikeplatformlarge":
                    level.AddSolid(LimboSpikePlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), true));
                    return;
                case "limbospikyball":
                    level.AddSolid(LimboSpikyBall.Make(level, position, getRadiusFromNode(x, y, node_infos), ent_info.rotations, getAngleFromNode(x, y, node_infos)));
                    return;
                case "limbosolid":
                    level.AddSolid(LimboSquare.Make(level, position, getDisplacementFromNode(x, y, node_infos)));
                    return;

                case "citycrate":
                    level.AddEntity(CityCrateEntity.Make(level, position));
                    return;

                case "cottonwoodplatform":
                    level.AddSolid(CottonwoodPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "cottonwoodplatformlarge":
                    level.AddSolid(CottonwoodPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), true));
                    return;
                case "floof_forward":
                    level.AddSolid(Floof.Make(level, position, getDisplacementFromNode(x, y, node_infos), true));
                    return;
                case "floof_backwards":
                    level.AddSolid(Floof.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "eternalplatform":
                    level.AddSolid(EternalPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "eternalplatformlarge":
                    level.AddSolid(EternalPlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), true));
                    return;
                case "eternalspikyball":
                    level.AddSolid(EternalSpikyBall.Make(level, position, getRadiusFromNode(x,y,node_infos), ent_info.rotations, getAngleFromNode(x,y,node_infos)));
                    return;
                case "eternalspikyplatform":
                    level.AddSolid(EternalSpikePlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "eternalspikyplatformlarge":
                    level.AddSolid(EternalSpikePlatform.Make(level, position, getDisplacementFromNode(x, y, node_infos), false));
                    return;
                case "lunarshrine":
                case "obelisk":
                    level.specialObject = SpecialObject.Make(level, position, name);
                    return;
                // todo decorative entities, other special objects
                default:
                    //todo
                    return;
            }
        }

        public static void PlaceDecorative(bool is_large, int x, int y, TileSpriteInfo sprite, SortingLayer sorting_layer, Level level)
        {
            Vector2 position = is_large ? level.getLargePositionFromGrid(x, y) : level.getPositionFromGrid(x, y);

            level.AddDecorative(DecorativeTile.Make(level, position, sprite), sorting_layer);

        }


        public static Vector2 getDisplacementFromNode(int x, int y, List<NodeInfo> nodes)
        {
            if (nodes == null) return Vector2.Zero;
            var node = nodes[0];
            return new Vector2(node.x-x, node.y-y) * 4;
        }

        public static float getRadiusFromNode(int x, int y, List<NodeInfo> nodes)
        {
            if (nodes == null) return 0;
            var node = nodes[0];
            return (new Vector2(node.x - x, node.y - y).Length() + 7) * 4;
        }

        public static int getAngleFromNode(int x, int y, List<NodeInfo> nodes)
        {
            if (nodes == null) return 0;
            var node = nodes[0];
            return (int)Math.Atan2(node.x - x, node.y - y);
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

    }
}
