using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Abstract;
using RewindGame.Game.Debug;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public enum TileSheet
    {
        decorative,
        collision,
        none
    }
    public class TileSprite
    {
        public TileSprite(TileSheet sheet_, int tex_id_, int sorting_layer_)
        {
            sheet = sheet_;
            tex_id = tex_id_;
            sorting_layer = sorting_layer_;
        }

        public TileSheet sheet;
        public int tex_id;
        public int sorting_layer;
    }

    public class Level : IDisposable
    {
        public const int TILE_WORLD_SIZE = 56;
        public const int LARGE_TILE_WORLD_SIZE = 112;

        public const int TILE_SHEET_SIZE = 56;
        public const int LARGE_TILE_SHEET_SIZE = 112;

        public const int DECORATIVE_SHEET_TILES_X = 10;
        public const int DECORATIVE_SHEET_TILES_Y = 13;

        public const int COLLISION_SHEET_TILES_X = 20;
        public const int COLLISION_SHEET_TILES_Y = 20;

        public const float SEMISOLID_THICKNESS = 1f;

        public List<TimeEntity> sceneEntities = new List<TimeEntity>();
        public List<Solid> sceneSolids = new List<Solid>();
        public List<ISolidTile> sceneSolidTiles = new List<ISolidTile>();
        public List<ITile> sceneDecorativesBackground = new List<ITile>();
        public List<ITile> sceneDecorativesForeground = new List<ITile>();
        public Vector2 playerSpawnpoint = Vector2.Zero;

        public Vector2 levelOrgin;
        public ContentManager Content;
        public RewindGame parentGame;

        public bool isActiveScene = false;

        public String name = "";

        public String[] connectedLevelNames = { "", "", "", "" };


        public Level(IServiceProvider serviceProvider, Vector2 orgin, RewindGame parent)
        {
            Content = new ContentManager(serviceProvider, "Content");

            levelOrgin = orgin;

            parentGame = parent;


            //sceneSolids.Add(new DebugPlatform(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2)));
            //sceneEntities.Add(new DebugTimePhysicsEntity(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2 - 200, parentGame.graphics.PreferredBackBufferHeight / 2)));
            parentGame.player = new PlayerEntity(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2 - 300));

        }


        // draw order: background, background tiles, scene entities, scene solids, player, collision tiles, foreground tiles
        // level DOES NOT draw player
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {

            foreach (ITile tile in sceneDecorativesBackground)
            {
                tile.Draw(state, sprite_batch);
            }

            foreach (Entity entity in sceneEntities)
            {
                entity.Draw(state, sprite_batch);
            }

            foreach (Solid solid in sceneSolids)
            {
                solid.Draw(state, sprite_batch);
            }

            foreach (ITile tile in sceneSolidTiles)
            {
                tile.Draw(state, sprite_batch);
            }

        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch) { 


            foreach (ITile tile in sceneDecorativesForeground)
            {
                tile.Draw(state, sprite_batch);
            }

        }


        public void DrawTile(TileSprite tile_sprite, Vector2 position, SpriteBatch sprite_batch)
        {
            if (tile_sprite.sheet == TileSheet.none) return;

            Texture2D sheet_texture = (tile_sprite.sheet == TileSheet.decorative ? parentGame.decorativeSheetTexture : parentGame.collisionSheetTexture);

            int sheet_size = (tile_sprite.sheet == TileSheet.decorative ? LARGE_TILE_SHEET_SIZE : TILE_SHEET_SIZE);
            int sheet_tile_len_x = (tile_sprite.sheet == TileSheet.decorative ? DECORATIVE_SHEET_TILES_X : COLLISION_SHEET_TILES_X);
            int sheet_tile_len_y = (tile_sprite.sheet == TileSheet.decorative ? DECORATIVE_SHEET_TILES_Y : COLLISION_SHEET_TILES_Y);

            int sheet_x = (tile_sprite.tex_id % sheet_tile_len_x) * sheet_size;
            int sheet_y = (int)Math.Floor((float)tile_sprite.tex_id / (float)sheet_tile_len_x) * sheet_size;

            Rectangle source_rect = new Rectangle(sheet_x, sheet_y, sheet_size, sheet_size);

            int world_size = (tile_sprite.sheet == TileSheet.decorative ? LARGE_TILE_WORLD_SIZE : TILE_WORLD_SIZE);
            Rectangle end_rect = new Rectangle((int)position.X, (int)position.Y, world_size, world_size);

            sprite_batch.Draw(sheet_texture, end_rect, source_rect, Color.White);
        }

        public void Update(StateData state)
        {
            foreach (TimeEntity entity in sceneEntities)
            {
                entity.TemporalUpdate(state);
            }

            // We don't have any moving platforms yet so we don't have temporal updates here yet
            foreach (Solid solid in sceneSolids)
            {
                solid.Update(state);
            }

            foreach (ITile tile in sceneSolidTiles)
            {
                tile.Update(state);
            }

            foreach (ITile tile in sceneDecorativesForeground)
            {
                tile.Update(state);
            }

            foreach (ITile tile in sceneDecorativesBackground)
            {
                tile.Update(state);
            }
        }

        public CollisionReturn getSolidCollisionAt(FRectangle rect, MoveDirection direction)
        {
            foreach (Solid solid in sceneSolids)
            {
                if (solid.isThisOverlapping(rect, direction))
                {
                    return new CollisionReturn(solid.getCollisionType(), solid);
                }
            }

            foreach (ISolidTile tile in sceneSolidTiles)
            {
                if (tile.isThisOverlapping(rect, direction))
                {
                    return new CollisionReturn(tile.getCollisionType(), (CollisionObject)tile);
                }
            }

            return CollisionReturn.None();
        }



        public void PlaceTile(TileType type, int x, int y, TileSprite sprite)
        {
            Vector2 position = getPositionFromGrid(x, y);

            switch (type)
            {
                case TileType.intangible:
                    sceneDecorativesForeground.Add(DecorativeTile.Make(this, position, sprite));
                    break;
                case TileType.solid:
                    sceneSolidTiles.Add(SolidTile.Make(this, position, sprite));
                    break;
                case TileType.platform:
                    sceneSolidTiles.Add(PlatformTile.Make(this, position, sprite));
                    break;
                case TileType.left_oneway:
                    sceneSolidTiles.Add(LeftOnewayTile.Make(this, position, sprite));
                    break;
                case TileType.right_oneway:
                    sceneSolidTiles.Add(RightOnewayTile.Make(this, position, sprite));
                    break;
                case TileType.topleft_oneway:
                    sceneSolidTiles.Add(LeftCornerOnewayTile.Make(this, position, sprite));
                    break;
                case TileType.topright_oneway:
                    sceneSolidTiles.Add(RightCornerOnewayTile.Make(this, position, sprite));
                    break;
                default:
                    // todo
                    break;
            }
        }

        public void PlaceEntity(EntityType type, int x, int y)
        {
            Vector2 position = getPositionFromGrid(x, y);

            switch (type)
            {
                case EntityType.Spawnpoint:
                    playerSpawnpoint = position;
                    return;
                default:
                    //todo
                    return;
            }
        }

        public void PlaceDecorative(bool is_large, int x, int y, TileSprite sprite)
        {
            Vector2 position = is_large ? getLargePositionFromGrid(x, y) : getPositionFromGrid(x, y);

            if (sprite.sorting_layer > 0)
            {
                sceneDecorativesForeground.Add(DecorativeTile.Make(this, position, sprite));
            }
            else
            {
                sceneDecorativesBackground.Add(DecorativeTile.Make(this, position, sprite));
            }

        }


        public Vector2 getPositionFromGrid(int x, int y)
        {
            return new Vector2(levelOrgin.X + x * TILE_WORLD_SIZE, levelOrgin.Y + y * TILE_WORLD_SIZE);
        }

        public Vector2 getLargePositionFromGrid(int x, int y)
        {
            return new Vector2(levelOrgin.X + x * TILE_WORLD_SIZE, levelOrgin.Y + y * TILE_WORLD_SIZE);
        }


        public IEnumerable<Entity> getAllEntities()
        {
            yield return parentGame.player;
            foreach (Entity entity in sceneEntities)
            {
                yield return entity;
            }
        }


        public void Dispose()
        {
            Content.Unload();
        }

    }
}
