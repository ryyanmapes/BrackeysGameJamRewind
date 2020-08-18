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
        // must all have a multiple of four lines
        String[] lunarShrineLines = { "It's some sort of shrine- the basin is almost empty", "There's a poem engraved on a plaque:", "","",
                                        "In night it shimmers indomitably", "A symphony of Shadows over time","They dance for none but demonstrate to all", "Silent, Eternal, inevitable"};
        String[] obeliskLines = { "A tattered note is pinned under the obelisk:", "\"To whoever finds this: take it with you and go far, far away. There is no future here.", "A thing we never conceived is happening now as I write this,", "one that ignores everything we believed and held in a sacred manner.",
                                        "Our actions become undone, as if a higher power is turning back the second hand.", "Those among us have also noticed something quite unsettling recently: the heavenly body is not the same as it once was.", "It waxes when it should wane, new when meant to be full.", "It is a bad omen to our people.",
                                        "Periods of waiting yield no change,and appeasement rituals offer nothing but less food the next day.", "The only option now is to leave our home and seek some sort of normalcy.", "I fear that if the Heavens remain this way, though, there will no longer be a normal to seek out.", "Left to you is only one of many obelisks built in reverence to the great moon above.",
                                        "To my knowledge, it is the last.", "May the moon regain its rhythm once again.\"", "", ""};
        String[] seartreeLines = { "There\'s a message seared into the side of this tree:", "and so with the ends we seek,", "then endings we shall receive", "" };
        String[] barrelLines = { "Theres what seems to be an old order form among the broken barrels","It outlines the details of a product called \"stasis seeds \":","With the power of temporal manipulation, say goodbye to famine and hunger!","Our patent pending technology will assure your harvest is ALWAYS perfect!",
                                        "Unlike your average fragile seedling, these are engineered to have the ultimate defense against any conflict- temporal locking!", "Too much water? That little germ will seize right up until the moisture is just right!", "Sun a bit too harsh for your delicate sapling? It'll petrify until that perfect evening temperature hits.", " Everything is just right, now and forever with Stasis Seeds on your farm.",
                                        "And with our engineers ensuring no time shenanigans beyond your precious plants, what do you have to lose? ","","",""};
        String[] posterLines = { "You're quite familiar with this- it's a diagram of the nearby spacetime dimensions", "Most of it is covered with black streaks, with the exception of an area surrounding a red dot in the center", "It must be outdated.", "" };

        String[] sadLines = { "This is the end of the game", "Thank you for playing!", "", "" };

        String[] currentLines;

        Animation lunarShrineTex = new Animation("limbo/lunarshrine", 5, 4, true); 
        Animation obeliskTex = new Animation("limbo/obelisk", 1, 1, true);
        Animation seartreeTex = new Animation("cottonwood/cottonwoodsecret", 1, 1, true); 
        Animation barrelTex = new Animation("cottonwood/barrel", 1, 1, true);
        Animation barrelTreeTex = new Animation("cottonwood/wagon", 1, 1, true);
        Animation posterTex = new Animation("eternal/poster", 1, 1, true);
        Animation deskTex = new Animation("limbo/tableplant", 1, 1, true);
        Animation deskTexPost = new Animation("limbo/table", 1, 1, true);

        SpriteFont font;

        AnimationPlayer anims;

        // # chars a line, 4 lines a superline
        public int charState = -1;
        public int lineState = 0;
        public float delay = -1f;

        public const float write_line_delay = 1.5f;
        public const float write_superline_delay = 3f;
        public const float end_delay = 30f;

        public const float line_spacing = 40f;

        // either waiting for superline or waiting for line
        public bool waitingForSuperline = false;

        string type;

        public static SpecialObject Make(Level level, Vector2 starting_pos, string type)
        {
            var tile = new SpecialObject();
            tile.Initialize(level, starting_pos, type);
            return tile;
        }

        public virtual void Initialize(Level level, Vector2 starting_pos, string type_)
        {
            type = type_;
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent()
        {
            Animation anim;
            switch (type) {
                case "lunarshrine":
                    currentLines = lunarShrineLines;
                    anim = lunarShrineTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE*4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case "obelisk":
                    currentLines = obeliskLines;
                    anim = obeliskTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 2, Level.TILE_WORLD_SIZE * 4);
                    break;
                    /*
                case EntityType.treesear:
                    currentLines = seartreeLines;
                    anim = seartreeTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case EntityType.barrel:
                    currentLines = barrelLines;
                    anim = barrelTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case EntityType.poster:
                    currentLines = sadLines;
                    anim = posterTex;
                    collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case EntityType.desk:
                    anim = deskTex;
                    //collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 4, Level.TILE_WORLD_SIZE * 4);
                    break;
                case EntityType.barreltree:
                    anim = barrelTreeTex;
                    //collisionSize = new Vector2(Level.TILE_WORLD_SIZE * 4, Level.TILE_WORLD_SIZE * 4);
                    break;*/
                default:
                    anim = null;
                    break;
            }

            anims = new AnimationPlayer(anim, 1, Vector2.Zero, localLevel.parentGame.Content);

            font = localLevel.parentGame.Content.Load<SpriteFont>("fonts/Roboto");


        }

        public override void Update(StateData state)
        {
            if (charState >= currentLines[lineState].Length) 
            {

                if (lineState+1 % 4 == 0) 
                    delay = write_superline_delay;
                else 
                    delay = write_line_delay;
                charState = -2;
                
            }
            else if (charState > -1)
            {
                // always?
                charState += 1;
            }

            if (delay != -1)
            {
                delay -= state.getDeltaTime();
                if (delay <= 0)
                {
                    lineState += 1;
                    delay = -1;
                    charState = 0;
                    if (lineState == currentLines.Length)
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

            if (charState == -1) return;

            /*if (charState == -2)
            {
                DrawText(sprite_batch, currentLines[lineState], currentLines[lineState].Length, 0);
                DrawText(sprite_batch, currentLines[lineState+1], currentLines[lineState+1].Length, 1);
                DrawText(sprite_batch, currentLines[lineState+2], currentLines[lineState+2].Length, 2);
                DrawText(sprite_batch, currentLines[lineState + 3], currentLines[lineState + 3].Length, 2);
            }*/

            int line_state_rem = lineState % 4;
            int real_line_state = lineState - line_state_rem;

            if (line_state_rem == 0) DrawText(sprite_batch, currentLines[lineState], charState, 0);
            else if (line_state_rem == 1)
            {
                DrawText(sprite_batch, currentLines[real_line_state], currentLines[real_line_state].Length, 0);
                DrawText(sprite_batch, currentLines[real_line_state + 1], charState, 1);
            }
            else if (line_state_rem == 2)
            {
                DrawText(sprite_batch, currentLines[real_line_state], currentLines[real_line_state].Length, 0);
                DrawText(sprite_batch, currentLines[real_line_state + 1], currentLines[real_line_state + 1].Length, 1);
                DrawText(sprite_batch, currentLines[real_line_state + 2], charState, 2);
            }
            else if (line_state_rem == 3)
            {
                DrawText(sprite_batch, currentLines[real_line_state], currentLines[real_line_state].Length, 0);
                DrawText(sprite_batch, currentLines[real_line_state + 1], currentLines[real_line_state + 1].Length, 1);
                DrawText(sprite_batch, currentLines[real_line_state + 2], currentLines[real_line_state + 2].Length, 2);
                DrawText(sprite_batch, currentLines[real_line_state + 3], charState, 3);
            }

            // todo text render
        }

        public void DrawText(SpriteBatch sprite_batch, string full_string, int chars_to_render, int line)
        {
            
            float text_length = font.MeasureString(full_string).X;
            float middle_pos = position.X + collisionSize.X / 2;

            Vector2 renderPos = new Vector2(middle_pos - text_length / 2, position.Y - (5 - line)*line_spacing);

            string real_string = chars_to_render == -2? full_string : full_string.Substring(0, chars_to_render);

            sprite_batch.DrawString(font, real_string, renderPos, Color.White);
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
