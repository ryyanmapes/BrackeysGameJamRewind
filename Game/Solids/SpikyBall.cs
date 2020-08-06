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

            texturePath = "limbo/chainball";
            collisionSize = new Vector2(28, 28);
            collisionType = PrimaryCollisionType.death;
            starting_rotation_degrees = startingrotation;
            base.Initialize(level, starting_pos);
            Reset();
        }

        public override void Update(StateData state)
        {
            // change rotation by speed*elapsed and position
            // position, state.getDeltaTime()
        }

        public virtual void Reset() 
        { 
            // set to initial rotation
        }

    }
}
