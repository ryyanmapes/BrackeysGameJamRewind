using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Debug;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    class Level
    {
        public RewindGame parentGame;

        private List<Entity> sceneEntities = new List<Entity>();
        private List<Solid> sceneSolids = new List<Solid>();

        public ContentManager Content;


        public Level(IServiceProvider serviceProvider, RewindGame parent)
        {
            Content = new ContentManager(serviceProvider, "Content");
            
            parentGame = parent;


            sceneSolids.Add(new DebugPlatform(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2)));
            sceneEntities.Add(new PlayerEntity(this, new Vector2(parentGame.graphics.PreferredBackBufferWidth / 2, parentGame.graphics.PreferredBackBufferHeight / 2 - 300)));

        }


        public void Draw(GameTime game_time, SpriteBatch sprite_batch)
        {
            foreach (Entity entity in sceneEntities)
            {
                entity.Draw(game_time, sprite_batch);
            }

            foreach (Solid solid in sceneSolids)
            {
                solid.Draw(game_time, sprite_batch);
            }
        }

        public void Update(GameTime game_time)
        {
            foreach (Entity entity in sceneEntities)
            {
                entity.Update(game_time);
            }

            foreach (Solid solid in sceneSolids)
            {
                solid.Update(game_time);
            }
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

        public List<Entity> getAllEntities()
        {
            return sceneEntities;
        }

        public List<Solid> getAllSolids()
        {
            return sceneSolids;
        }

    }
}
