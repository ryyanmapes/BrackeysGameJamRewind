using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;

namespace RewindGame.Game.Solids
{
    class LimboSpikyBall : CollisionObject
    {

        public float radius;
        public float speed;
        public int starting_rotation_degrees;
        private float current_rotation_degrees;
        private float current_rotation_radians;
        private Vector2 startingPosition;
        private Vector2 currentPosition;
        public static LimboSpikyBall Make(Level level, Vector2 starting_pos, float radius, float speed, int starting_rotation)
        {
            var tile = new LimboSpikyBall();
            tile.Initialize(level, starting_pos, radius, speed, starting_rotation);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, float radius_, float speed_, int startingrotation)
        {
            radius = radius_;
            speed = speed_;
            current_rotation_degrees = startingrotation;
            startingPosition = starting_pos - new Vector2(Level.TILE_WORLD_SIZE/2, Level.TILE_WORLD_SIZE/2);
            currentPosition = startingPosition;

            texturePath = "limbo/chainball";
            collisionSize = new Vector2(84, 84);
            collisionType = CollisionType.death;
            starting_rotation_degrees = startingrotation;
            base.Initialize(level, startingPosition);
            Reset();

        }

        public override void Update(StateData state)
        {
            if (state.getTimeDependentDeltaTime() > 0)
            {
                current_rotation_degrees += speed * .1f;
                current_rotation_radians = (float)(((current_rotation_degrees + 90) * Math.PI / 180));
                // currentPosition += new Vector2((int)(Math.Cos(current_rotation_degrees)), (int)(Math.Sin(current_rotation_degrees)));
                position = new Vector2((int)(startingPosition.X + radius * Math.Cos(current_rotation_degrees - 90)), (int)(startingPosition.Y + radius * Math.Sin(current_rotation_degrees - 90)));
                //Move(new Vector2((int)(radius * Math.Cos(current_rotation_radians)), (int)(radius * Math.Sin(current_rotation_radians))));
            }
            else if (state.getTimeDependentDeltaTime() < 0)
            {
                current_rotation_degrees -= speed * .1f;
                current_rotation_radians = (float)(((current_rotation_degrees + 90) * Math.PI / 180));
                //currentPosition += new Vector2((int)(radius * Math.Cos(current_rotation_degrees)), (int)(radius * Math.Sin(current_rotation_degrees)));
                position = new Vector2((int)(startingPosition.X + radius * Math.Cos(current_rotation_degrees - 90)), (int)(startingPosition.Y + radius * Math.Sin(current_rotation_degrees - 90)));
                //Move(new Vector2((int)(radius * -Math.Cos(current_rotation_radians)), (int)(radius * -Math.Sin(current_rotation_radians))));
            }
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
