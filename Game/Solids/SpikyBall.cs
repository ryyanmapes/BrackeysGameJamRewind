using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;

namespace RewindGame.Game.Solids
{
    class SpikyBall : Solid
    {

        public float radius;
        public float speed;
        public int starting_rotation_degrees;
        private float current_rotation_degrees;
        private float current_rotation_radians;
        public static SpikyBall Make(Level level, Vector2 starting_pos, float radius, float speed, int starting_rotation)
        {
            var tile = new SpikyBall();
            tile.Initialize(level, starting_pos, radius, speed, starting_rotation);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, float radius_, float speed_, int startingrotation)
        {
            radius = radius_;
            speed = speed_;
            current_rotation_degrees = startingrotation;

            texturePath = "limbo/chainball";
            collisionSize = new Vector2(28, 28);
            collisionType = CollisionType.death;
            starting_rotation_degrees = startingrotation;
            base.Initialize(level, starting_pos);
            Reset();

        }

        public override void Update(StateData state)
        {
            if (state.getSignedDeltaTime() > 0)
            {
                current_rotation_degrees += speed;
                current_rotation_radians = (float)((current_rotation_degrees * Math.PI / 180));
                Move(new Vector2((int)(radius * Math.Cos(current_rotation_radians)), (int)(radius * Math.Sin(current_rotation_radians))));
            }
            else if (state.getSignedDeltaTime() < 0)
            {
                current_rotation_degrees -= speed;
                current_rotation_radians = (float)((current_rotation_degrees * Math.PI / 180));
                Move(new Vector2((int)(radius * -Math.Cos(current_rotation_radians)), (int)(radius * -Math.Sin(current_rotation_radians))));
            }
            if (current_rotation_degrees == 0)
            {
                current_rotation_degrees = 360;
            }
        }

        public override void Reset() 
        {
          // current_rotation_degrees = starting_rotation_degrees;
        }

    }
}
