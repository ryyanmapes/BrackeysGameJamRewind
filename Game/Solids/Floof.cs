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
        public int timeMomentConsumedOn = -1;
        public bool isActive = false;

        public static Floof Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isForwards)
        {
            var tile = new Floof();
            tile.Initialize(level, starting_pos, velocity_, isForwards);
            return tile;
        }


        public virtual void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_forwards)
        {
            collisionSize.X = Level.LARGE_TILE_WORLD_SIZE;
            collisionSize.Y = Level.SEMISOLID_THICKNESS_WINDOW;
            isForwards = is_forwards;
            velocity = velocity_;
            this.startingPos = starting_pos;
            base.Initialize(level, starting_pos);
        }

        public override void Update(StateData state)
        {
            position += new Vector2(velocity.X * state.getTimeDependentDeltaTime(), velocity.Y * state.getTimeDependentDeltaTime());

            if (timeMomentConsumedOn != -1 && state.time_data.time_moment < timeMomentConsumedOn)
            {
                isActive = true;
            }
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

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (isActive)
                sprite_batch.Draw(this.texture, position + new Vector2(0,state.time_data.getFloaty(position.X)), Color.White);
        }

        public override void Reset()
        {
            position = startingPos;
            isActive = true;
            base.Reset();
        }

        public void Consume(StateData state)
        {
            timeMomentConsumedOn = state.time_data.time_moment - 1;
            isActive = false;
        }

        public override CollisionReturn getCollisionReturn()
        {
            if (isActive)
            {
                return new CollisionReturn(isForwards ? CollisionType.forward_floof : CollisionType.backward_floof, this, 3);
            }
            return CollisionReturn.None();
        }
    }
}
