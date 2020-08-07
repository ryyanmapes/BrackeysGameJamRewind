using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RewindGame.Game.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Special
{
    public class SpecialObject : CollisionObject
    {

        String[] lunarShrineLines = { "", "" };
        String[] obeliskLines = { "", "" };
        String[] currentLines;

        Animation lunarShrineTex = new Animation("limbo/lunarshrine", 1, 1, true); //todo
        Animation obeliskTex = new Animation("limbo/obelisk", 1, 1, true); //todo

        AnimationPlayer anims;

        // # chars a line, 3 lines a superline
        public int charState = -1;
        public int lineState = 0;
        public float delay = -1f;

        public const float write_line_delay = 1f;
        public const float write_superline_delay = 5f;
        public const float end_delay = 10f;

        // either waiting for superline or waiting for line
        public bool waitingForSuperline = false;

        EntityType type;

        public static SpecialObject Make(Level level, Vector2 starting_pos, EntityType type)
        {
            var tile = new SpecialObject();
            tile.Initialize(level, starting_pos, type);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, EntityType type_)
        {
            type = type_;
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent()
        {
            Animation anim;
            switch (type) {
                case EntityType.lunarshrine:
                    currentLines = lunarShrineLines;
                    anim = lunarShrineTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE*4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case EntityType.obelisk:
                    currentLines = obeliskLines;
                    anim = obeliskTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 2, Level.TILE_WORLD_SIZE * 4);
                    break;
                default:
                    anim = null;
                    break;
            }

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, localLevel.Content);
        }

        public override void Update(StateData state)
        {
            if (charState >= currentLines[lineState].Length) 
            {

                if (lineState % 3 == 0) delay = write_superline_delay;
                else delay = write_line_delay;
                charState = -2;
                
            }
            else if (charState > -1)
            {
                // always?
                charState += 1;
                if (charState >= currentLines[lineState].Length)
                {
                    
                    if (lineState % 3 == 0) delay = write_superline_delay;
                    else delay = write_line_delay;
                }
            }

            if (delay != -1)
            {
                delay -= state.getDeltaTime();
                if (delay <= 0)
                {
                    lineState += 1;
                    if (lineState == currentLines.Length)
                    {
                        delay = end_delay;
                        charState = 0;
                    }
                    else if (lineState > currentLines.Length)
                    {
                        Reset();
                    }
                }
            }
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            anims.Draw(state, sprite_batch, position, SpriteEffects.None, state.getTimeN());
            // todo text render
        }

        public override void Reset() 
        {
            charState = -1;
            lineState = 0;
            delay = -1;
        }

        public override void SetInactive()
        {
            hidden = true;
        }

        public override void SetActive()
        {
            hidden = false;
        }

    }
}
