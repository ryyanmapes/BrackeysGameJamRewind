using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Abstract;

namespace RewindGame.Game.Solids
{
    public class Floof : CollisionObject
    {

        public bool isForwards;
        public Vector2 velocity;
        public Vector2 startingPos;

        public static Floof Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isForwards)
        {
            var tile = new Floof();
            tile.Initialize(level, starting_pos, velocity_, isForwards);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_forwards)
        {
            collisionSize.Y = Level.SEMISOLID_THICKNESS_WINDOW;
            isForwards = is_forwards;
            velocity = velocity_;
            this.startingPos = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            position += new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime());
        }

        public override void LoadContent()
        { 
            if (isForwards)
            {
                texturePath = "cottonwood/jumppoofpink";
            }
            else
            {
                texturePath = "cottonwood/jumppoofgreen";
            }
            base.LoadContent();
        }

        public override void Reset()
        {
            position = startingPos;
            base.Reset();
        }

        public override CollisionReturn getCollisionReturn()
        {
            return new CollisionReturn(isForwards ? CollisionType.forward_floof : CollisionType.backward_floof, this, 3);
        }
    }
}
