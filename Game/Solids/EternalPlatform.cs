using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;

namespace RewindGame.Game.Solids
{


    class EternalPlatform : LimboPlatform
    {

        public Animation longDown = new Animation("eternal/terrariumplatformdownlong", 1, 1, true);
        public Animation longUp = new Animation("eternal/terrariumplatformuplong", 1, 1, true);
        public Animation shortUp = new Animation("eternal/terrariumplatformup", 1, 1, true);
        public Animation shortDown = new Animation("eternal/terrariumplatformdown", 1, 1, true);



        public AnimationChooser anims = new AnimationChooser(1, Vector2.Zero);

        public new static EternalPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new EternalPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public override void Initialize(Level level, Vector2 starting_pos, Vector2 velocity_, bool is_long)
        {

            if (is_long) {
                anims.addAnimaton(longDown, "down", level.parentGame.Content);
                anims.addAnimaton(longUp, "up", level.parentGame.Content);
            } else
            {
                anims.addAnimaton(shortDown, "down", level.parentGame.Content);
                anims.addAnimaton(shortUp, "up", level.parentGame.Content);
            }
            anims.changeAnimation("up");

            doLeftCollision = false;
            doRightCollision = false;
            doDownCollision = false;

            base.Initialize(level, starting_pos, velocity_, is_long? 4 : 2);
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            anims.Draw(state, sprite_batch, position);
        }

        public override void Update(StateData state)
        {
            int sign = state.time_data.time_moment < 0 ? -1 : 1;

            if (sign * velocity.Y < 0) anims.changeAnimation("up");
            else anims.changeAnimation("down");

            Move(new Vector2(velocity.X * state.getTimeDependentDeltaTime() * sign, velocity.Y * state.getTimeDependentDeltaTime() * sign));
        }

    }
}
