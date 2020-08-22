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

        protected new AnimationPlayer renderLongDown = new AnimationPlayer("eternal/terrariumplatformdownlong", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderLongUp = new AnimationPlayer("eternal/terrariumplatformuplong", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderDown = new AnimationPlayer("eternal/terrariumplatformup", 1, 1, true, 1, Vector2.Zero);
        protected new AnimationPlayer renderUp = new AnimationPlayer("eternal/terrariumplatformdown", 1, 1, true, 1, Vector2.Zero);

        public new static EternalPlatform Make(Level level, Vector2 starting_pos, Vector2 velocity_, bool isLong)
        {
            var tile = new EternalPlatform();
            tile.Initialize(level, starting_pos, velocity_, isLong);
            return tile;
        }

        public override void Update(StateData state)
        {
            int sign = state.time_data.time_moment < 0 ? -1 : 1;

            // This didn't make sense as we had it, so it's removed for now
            // todo add a proper indicator of when the platform swaps directions
            //if (sign * velocity.Y < 0) anims.changeAnimation("up");
            //else anims.changeAnimation("down");
            

            Move(new Vector2(velocity.X * state.getTimeDependentDeltaTime() * sign, velocity.Y * state.getTimeDependentDeltaTime() * sign));
        }

    }
}
