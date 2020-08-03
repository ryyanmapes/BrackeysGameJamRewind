using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    public class TileSprite
    {
        //todo
    }

    class Level 
    {
        public RewindGame parentGame;

        private PlayerEntity player;
        private List<TimeEntity> sceneEntities = new List<TimeEntity>();
        private List<Solid> sceneSolids = new List<Solid>();
        private List<Solid> sceneTiles = new List<Solid>();
        private List<Solid> sceneDecoratives = new List<Solid>();

        public ContentManager Content;


        public Level(IServiceProvider serviceProvider, RewindGame parent)
        {
            Content = new ContentManager(serviceProvider, "Content");
            
            parentGame = parent;


            sceneSolids.Add(new DebugPlatform(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2)));
            sceneEntities.Add(new DebugTimePhysicsEntity(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2 - 200, parentGame.graphics.PreferredBackBufferHeight / 2)));
            player = new PlayerEntity(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2 - 300));

        }


        public void Draw(StateData state, SpriteBatch sprite_batch)
        {
            foreach (Entity entity in sceneEntities)
            {
                entity.Draw(state, sprite_batch);
            }

            foreach (Solid solid in sceneSolids)
            {
                solid.Draw(state, sprite_batch);
            }
            player.Draw(state, sprite_batch);
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

            player.Update(state);
        }



        public void PlaceTile(TileType type, int x, int y, TileSprite sprite)
        {
            // we'll do this later
        }

        public void PlaceEntity(EntityType type, Vector2 position)
        {
            // we'll do this later
        }

        public void PlaceDecorative(int x, int y, TileSprite sprite)
        {
            // we'll do this later
        }


        public CollisionReturn getSolidCollisionAt(Rectangle rect, MoveDirection direction)
        {
            foreach (Solid solid in getAllSolids())
            {
                if (solid.isThisOverlapping(rect))
                {
                    return new CollisionReturn(solid.getCollisionType(), solid);
                }
            }
            return CollisionReturn.None();
        }

        public List<TimeEntity> getAllEntities()
        {
            return sceneEntities;
        }

        public List<Solid> getAllSolids()
        {
            return sceneSolids;
        }

    }
}
