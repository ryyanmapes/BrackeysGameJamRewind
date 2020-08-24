using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;
using RewindGame.Game.Graphics;
using System.Drawing;

namespace RewindGame.Game.Solids
{
    class LimboSpikyBall : CollisionObject
    {
        // todo what's with the unused variables here
        public float radius;
        public float rotations;
        public int starting_rotation_degrees;
        private float current_rotation_degrees;
        private float current_rotation_radians;
        private Vector2 startingPosition;
        private Vector2 currentPosition;
        private static int rott;

        public static LimboSpikyBall Make(Level level, Vector2 starting_pos, float radius, float rotations, int starting_rotation)
        {
            var tile = new LimboSpikyBall();
            tile.Initialize(level, starting_pos, radius, rotations, starting_rotation);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, float radius_, float rotations_, int startingrotation)
        {
            radius = radius_;
            rotations = rotations_;
            current_rotation_degrees = startingrotation;
            startingPosition = starting_pos - new Vector2(GameUtils.TILE_WORLD_SIZE/2, GameUtils.TILE_WORLD_SIZE/2);
            currentPosition = startingPosition;

            renderer = new BasicSprite("limbo/chainball");

            collisionSize = new Vector2(84, 84);
            collisionType = CollisionType.death;
            starting_rotation_degrees = startingrotation;
            base.Initialize(level, startingPosition);
            Reset();

        }

        public override void Update(StateData state)
        {
            float speed = (float)360 / (float)(state.time_bound.max*112);
            //Console.WriteLine(state);
             speed *= rotations;

            //current_rotation_degrees = rott;

            if (state.getTimeDependentDeltaTime() > 0)
            {
                current_rotation_degrees += speed;
                current_rotation_radians = (float)(((current_rotation_degrees + 90) * Math.PI / 180));
                // currentPosition += new Vector2((int)(Math.Cos(current_rotation_degrees)), (int)(Math.Sin(current_rotation_degrees)));
                position = new Vector2((int)(startingPosition.X + radius * Math.Cos(current_rotation_degrees - 90)), (int)(startingPosition.Y + radius * Math.Sin(current_rotation_degrees - 90)));
                //Move(new Vector2((int)(radius * Math.Cos(current_rotation_radians)), (int)(radius * Math.Sin(current_rotation_radians))));
            }
            else if (state.getTimeDependentDeltaTime() < 0)
            {
                current_rotation_degrees -= speed;
                current_rotation_radians = (float)(((current_rotation_degrees + 90) * Math.PI / 180));
                //currentPosition += new Vector2((int)(radius * Math.Cos(current_rotation_degrees)), (int)(radius * Math.Sin(current_rotation_degrees)));
                position = new Vector2((int)(startingPosition.X + radius * Math.Cos(current_rotation_degrees - 90)), (int)(startingPosition.Y + radius * Math.Sin(current_rotation_degrees - 90)));
                //Move(new Vector2((int)(radius * -Math.Cos(current_rotation_radians)), (int)(radius * -Math.Sin(current_rotation_radians))));
            }
            // todo what is the point of this?
            // to keep the number from overflowing, just as a saftey point not really needed unless the speed is really high or for some godforsaken reason someone spends way to long in a level
            if (current_rotation_degrees == 0)
            {
                current_rotation_degrees = 360;
            }
        }

        public override void Reset() 
        {
            currentPosition = startingPosition;
            current_rotation_degrees = 0;
            position = new Vector2((int)(startingPosition.X + radius * Math.Cos(current_rotation_degrees)), (int)(startingPosition.Y + radius * Math.Sin(current_rotation_degrees)));
            base.Reset();
        }
    }
}
