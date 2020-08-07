using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Effects
{
    class EffectParticle : GameObject
    {
        protected float initFadeinTime;
        protected float initLifeTime;
        protected float initFadeoutTime;

        protected float fadeinTime;
        protected float fadeoutTime;
        protected float lifeTime;

        public bool isDead = false;

        public static EffectParticle Make(Level level, Vector2 starting_pos, float fadein_time, float life_time, float fadeout_time, Texture2D tex)
        {
            var tile = new EffectParticle();
            tile.Initialize(level, starting_pos, fadein_time, life_time, fadeout_time, tex);
            return tile;
        }

        public void Initialize(Level level, Vector2 starting_pos, float fadein_time, float life_time, float fadeout_time, Texture2D tex)
        {
            fadeinTime = fadein_time;
            lifeTime = life_time;
            fadeoutTime = fadeout_time;
            initFadeinTime = fadein_time;
            initLifeTime = life_time;
            initFadeoutTime = fadeout_time;
            texture = tex;
            textureColor = new Color(0, 0, 0, 0);
            base.Initialize(level, starting_pos);
        }

        public override void LoadContent() { }

        public override void Update(StateData state)
        {
            if (fadeinTime > 0)
            {
                var fade = (byte)(255f * 1/(fadeinTime / initFadeinTime));
                textureColor = new Color(fade, fade, fade, fade);
                fadeinTime -= state.getDeltaTime();
            }
            else if (lifeTime > 0)
            {
                textureColor = Color.White;
                lifeTime -= state.getDeltaTime();
            }
            if (fadeoutTime > 0)
            {
                //var fade = (byte)(255f * (initFadeoutTime / fadeoutTime));
                //textureColor = new Color(fade, fade, fade, fade);
                fadeoutTime -= state.getDeltaTime();
            }
            else
            {
                isDead = true;
                textureColor = new Color(0, 0, 0, 0);
            }
        }

        public override void Draw(StateData state, SpriteBatch sprite_batch)
        {
            if (hidden) return;
            sprite_batch.Draw(texture, position, textureColor);
        }

    }
}
