using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{
    class LimboSpikyBall : CollisionObject
    {
        public float radius;
        public float rotations;
        // BOTH OF THESE ONLY USE RADIANS!
        public float starting_rotation;
        private float current_rotation;
        private Vector2 pivotPos;

        //StartingRotation is given is radians
        public static LimboSpikyBall Make(Level level, Vector2 starting_pos, float radius, float rotations, float starting_rotation)
        {
            var tile = new LimboSpikyBall();
            tile.Initialize(level, starting_pos, radius, rotations, starting_rotation);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, float radius_, float rotations_, float startingrotation)
        {
            radius = radius_;
            rotations = rotations_;
            current_rotation = startingrotation;
            starting_rotation = startingrotation;
            pivotPos = starting_pos - new Vector2(GameUtils.TILE_WORLD_SIZE/2, GameUtils.TILE_WORLD_SIZE/2);

            renderer = new BasicSprite("limbo/chainball");

            collisionSize = new Vector2(84, 84);
            collisionType = CollisionType.death;
            base.Initialize(level, pivotPos);
            Reset();

        }

        public override void Update(StateData state)
        {
            float speed = (float)Math.PI * 2 / state.time_bound.max;
            current_rotation += speed * rotations * state.getTimeSign();

            position = new Vector2((float)(pivotPos.X + radius * Math.Cos(current_rotation)), 
                (float)(pivotPos.Y + radius * Math.Sin(current_rotation)));

        }

        public override void Reset() 
        {
            position = pivotPos;
            current_rotation = starting_rotation;
            position = new Vector2((float)(pivotPos.X + radius * Math.Cos(current_rotation)),
                (float)(pivotPos.Y + radius * Math.Sin(current_rotation)));
            base.Reset();
        }

    }
}
