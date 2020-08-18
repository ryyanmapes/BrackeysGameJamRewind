using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using RewindGame.Game.Tiles;
using RewindGame.Game.Solids;
using RewindGame.Game.Special;
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

    public enum SortingLayer
    {
        background,
        normal,
        foreground,
        foreforeground
    }

    public class TileSprite
    {
        public TileSprite(TileSheet sheet_, int tex_id_)
        {
            sheet = sheet_;
            tex_id = tex_id_;
        }

        public TileSheet sheet;
        public int tex_id;
    }

    public class Level : UnpopulatedLevel
    {
        // I don't even think we have any entities besides the player, which isn't even kept here...
        public List<TimeEntity> sceneEntities = new List<TimeEntity>();

        public List<ICollisionObject> sceneSolids = new List<ICollisionObject>();

        public List<IGameObject> sceneDecorativesBackground = new List<IGameObject>();
        public List<IGameObject> sceneDecoratives = new List<IGameObject>();
        public List<IGameObject> sceneDecorativesForeground = new List<IGameObject>();
        public List<IGameObject> sceneDecorativesForeforeground = new List<IGameObject>();

        public SpecialObject specialObject;
        public Warp warp;


        public static Level Make(IServiceProvider serviceProvider, Vector2 orgin, RewindGame parent)
        {
            Level level = new Level();

            level.Content = parent.Content;

            level.levelOrgin = orgin;

            level.parentGame = parent;

            // add debug stuff
            return level;
        }


        // draw order: background, background tiles, scene entities, scene solids, player, collision tiles, foreground tiles
        // level DOES NOT draw player
        public void DrawBackground(StateData state, SpriteBatch sprite_batch)
        {

            foreach (IGameObject i in getAllBackgroundDecoratives())
            {
                i.Draw(state, sprite_batch);
            }

            foreach (IGameObject i in getAllSolids())
            {
                i.Draw(state, sprite_batch);
            }

            foreach (Entity i in getAllEntities())
            {
                i.Draw(state, sprite_batch);
            }

        }

        public void DrawForeground(StateData state, SpriteBatch sprite_batch) {

            foreach (IGameObject i in getAllForegroundDecoratives())
            {
                i.Draw(state, sprite_batch);
            }
        }
        
        public void Update(StateData state)
        {
            // someone who knows how to do mapping functons in c# redo this
            foreach (IGameObject i in getAllEverything())
            {
                i.Update(state);
            }
        }

        public void Reset()
        {
            foreach (IGameObject i in getAllEverything())
            {
                i.Reset();
            }
        }

        public void SetInactive()
        {
            foreach (IGameObject i in getAllEverything())
            {
                i.SetInactive();
            }
        }

        public void SetActive()
        {
            foreach (IGameObject i in getAllEverything())
            {
                i.SetActive();
            }
        }




        public CollisionReturn getSolidCollisionAt(FRectangle rect, MoveDirection direction) { return getSolidCollisionAt(rect, direction, null); }

        public CollisionReturn getSolidCollisionAt(FRectangle rect, MoveDirection direction, Solid pusher)
        {
            var return_collision = CollisionReturn.None();
            foreach (CollisionObject solid in sceneSolids)
            {
                if (solid == pusher) continue;
                CollisionReturn collision = solid.getCollision(rect, direction);
                if (collision.priority > return_collision.priority) return_collision = collision;
            }

            return return_collision;
        }

        public List<Entity> getAllRidingEntities(Solid solid)
        {
            List<Entity> returns = new List<Entity>();
            foreach (Entity entity in getAllEntitiesAndPlayer())
            {
                if (entity.isRiding(solid)) returns.Add(entity);
            }
            return returns;
        }

        public bool getPlayerIsInStasis(FRectangle rect)
        {
            foreach (CollisionObject solid in sceneSolids)
            {
                if (solid.getCollision(rect, MoveDirection.none).type == CollisionType.timestop) return true;
            }

            return false;
        }

        public bool getPlayerIsInSpecial(FRectangle rect)
        {
            if (specialObject != null)
            {
                return specialObject.isThisOverlapping(rect);
            }

            return false;
        }

        public bool getPlayerIsInWarp(FRectangle rect)
        {
            if (warp != null)
            {
                return warp.isThisOverlapping(rect);
            }

            return false;
        }



        public void DrawTile(TileSprite tile_sprite, Vector2 position, SpriteBatch sprite_batch)
        {
            if (tile_sprite == null || tile_sprite.sheet == TileSheet.none) return;

            Texture2D sheet_texture = (tile_sprite.sheet == TileSheet.decorative ? parentGame.decorativeSheetTexture : parentGame.collisionSheetTexture);

            int sheet_size = (tile_sprite.sheet == TileSheet.decorative ? GameUtils.LARGE_TILE_SHEET_SIZE : GameUtils.TILE_SHEET_SIZE);
            int sheet_tile_len_x = (tile_sprite.sheet == TileSheet.decorative ? GameUtils.DECORATIVE_SHEET_TILES_X : GameUtils.COLLISION_SHEET_TILES_X);
            int sheet_tile_len_y = (tile_sprite.sheet == TileSheet.decorative ? GameUtils.DECORATIVE_SHEET_TILES_Y : GameUtils.COLLISION_SHEET_TILES_Y);

            int sheet_x = (tile_sprite.tex_id % sheet_tile_len_x) * sheet_size;
            int sheet_y = (int)Math.Floor((float)tile_sprite.tex_id / (float)sheet_tile_len_x) * sheet_size;

            Rectangle source_rect = new Rectangle(sheet_x, sheet_y, sheet_size, sheet_size);

            int world_size = (tile_sprite.sheet == TileSheet.decorative ? GameUtils.LARGE_TILE_WORLD_SIZE : GameUtils.TILE_WORLD_SIZE);
            Rectangle end_rect = new Rectangle((int)position.X, (int)position.Y, world_size, world_size);

            sprite_batch.Draw(sheet_texture, end_rect, source_rect, Color.White);
        }



        public IEnumerable<Entity> getAllEntities()
        {
            foreach (Entity i in sceneEntities)
            {
                yield return i;
            }
        }

        // This is only used when we are checking if there are any riders on some solid
        public IEnumerable<Entity> getAllEntitiesAndPlayer()
        {
            if (parentGame.player != null) yield return parentGame.player;
            foreach (Entity i in getAllEntities())
            {
                yield return i;
            }
        }

        public IEnumerable<ICollisionObject> getAllSolids()
        {
            foreach (ICollisionObject i in sceneSolids)
            {
                yield return i;
            }
        }

        public IEnumerable<IGameObject> getAllBackgroundDecoratives()
        {
            foreach (IGameObject i in sceneDecorativesBackground)
            {
                yield return i;
            }

            foreach (IGameObject i in sceneDecoratives)
            {
                yield return i;
            }
        }

        public IEnumerable<IGameObject> getAllForegroundDecoratives()
        {
            foreach (IGameObject i in sceneDecorativesForeground)
            {
                yield return i;
            }

            foreach (IGameObject i in sceneDecorativesForeforeground)
            {
                yield return i;
            }

            // todo this is wonky
            if (specialObject != null) yield return specialObject;
            if (warp != null) yield return warp;
        }
        
        // DOES NOT INCLUDE PLAYER (should we have a variant that does?)
        public IEnumerable<IGameObject> getAllEverything()
        {
            foreach (Entity i in getAllEntities())
            {
                yield return i;
            }

            foreach (ICollisionObject i in getAllSolids())
            {
                yield return i;
            }

            foreach (IGameObject i in getAllBackgroundDecoratives())
            {
                yield return i;
            }

            foreach (IGameObject i in getAllForegroundDecoratives())
            {
                yield return i;
            }
        }
    
    
        public void AddEntity(TimeEntity ent)
        {
            sceneEntities.Add(ent);
        }

        public void AddSolid(CollisionObject s)
        {
            sceneSolids.Add(s);
        }

        public void AddDecorative(GameObject s, SortingLayer layer)
        {
            switch (layer)
            {
                case SortingLayer.background:
                    sceneDecorativesBackground.Add(s);
                    break;
                case SortingLayer.normal:
                    sceneDecoratives.Add(s);
                    break;
                case SortingLayer.foreground:
                    sceneDecorativesForeground.Add(s);
                    break;
                case SortingLayer.foreforeground:
                    sceneDecorativesForeforeground.Add(s);
                    break;
            }
        }


    }
}
