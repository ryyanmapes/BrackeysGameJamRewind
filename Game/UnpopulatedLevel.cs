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
    public class UnpopulatedLevel
    {

        public const int TILE_WORLD_SIZE = 56;
        public const int LARGE_TILE_WORLD_SIZE = 112;

        public const int TILE_SHEET_SIZE = 56;
        public const int LARGE_TILE_SHEET_SIZE = 112;

        public const int DECORATIVE_SHEET_TILES_X = 10;
        public const int DECORATIVE_SHEET_TILES_Y = 13;

        public const int COLLISION_SHEET_TILES_X = 20;
        public const int COLLISION_SHEET_TILES_Y = 20;

        public const float SEMISOLID_THICKNESS = 4f;
        public const float SEMISOLID_THICKNESS_WINDOW = 20f;
        public const float WALLSPIKE_THICKNESS = 16f;


        public Vector2 playerSpawnpoint = Vector2.Zero;

        public Vector2 levelOrgin;
        public ContentManager Content;
        public RewindGame parentGame;

        public bool isActiveScene = false;

        public String name = "";
        public String[] connectedLevelNames = { "", "", "", "" };
        public float screensHorizontal;
        public float screensVertical;
        public string startTriggers = "";

        public UnpopulatedLevel() { }

        public UnpopulatedLevel(IServiceProvider serviceProvider, Vector2 orgin, RewindGame parent)
        {
            Content = new ContentManager(serviceProvider, "Content");

            levelOrgin = orgin;

            parentGame = parent;
        }



        public void RunStartTriggers()
        {
            foreach (string s in startTriggers.Split(","))
            {
                parentGame.DoTrigger(s);
            }
        }

        public void TransitionTo(MoveDirection direction)
        {
            String new_level_name;
            switch (direction)
            {
                case MoveDirection.right:
                    new_level_name = connectedLevelNames[0];
                    break;
                case MoveDirection.left:
                    new_level_name = connectedLevelNames[1];
                    break;
                case MoveDirection.up:
                    new_level_name = connectedLevelNames[2];
                    break;
                case MoveDirection.down:
                    new_level_name = connectedLevelNames[3];
                    break;
                default:
                    return;
            }

            parentGame.qued_level_load_name = new_level_name;
        }

        public Vector2 getPositionFromGrid(int x, int y)
        {
            return new Vector2(levelOrgin.X + x * TILE_WORLD_SIZE, levelOrgin.Y + y * TILE_WORLD_SIZE);
        }

        public Vector2 getLargePositionFromGrid(int x, int y)
        {
            return new Vector2(levelOrgin.X + x * TILE_WORLD_SIZE, levelOrgin.Y + y * TILE_WORLD_SIZE);
        }

    }
}
