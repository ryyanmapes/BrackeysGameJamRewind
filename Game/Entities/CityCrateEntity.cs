using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using RewindGame.Game.Graphics;
using RewindGame.Game.Solids;

namespace RewindGame.Game.Physics.Temporal
{
    class CityCrateEntity : TimePhysicsEntity
    {
        protected const float pushVelocity = 110f;

        public static CityCrateEntity Make(Level level, Vector2 starting_pos)
        {
            var tile = new CityCrateEntity();
            tile.Initialize(level, starting_pos);
            return tile;
        }


        public override void Update(StateData state)
        {
            var player = localLevel.parentGame.player;
            // criteria for pushing
            if (player.hungObject == this.linkedSolid && player.velocity.X != 0 && player.grounded == GroundedReturn.grounded && is_grounded)
            {
                velocity.X = pushVelocity * (player.hangDirection == HangDirection.Left ? -1 : 1);
            }

            base.Update(state);
        }

        public override void Initialize(Level level, Vector2 starting_pos)
        {
            renderer = new BasicSprite("debug/square", Color.Aqua);
            renderWithCollisionBox = true;
            collisionSize = new Vector2(GameUtils.TILE_WORLD_SIZE * 2);

            linkedSolid = LinkedSolid.Make(level, starting_pos, collisionSize, this);
            level.AddSolid(linkedSolid);

            base.Initialize(level, starting_pos);
        }

    }
}
