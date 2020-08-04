using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game.Graphics
{
    class Animation
    {
        public Animation(String path, int frametime, int frameheight, int framelength, bool doloop)  {
            asset_path = path;
            frame_height = frameheight;
            frame_length = framelength;
            change_frame_every = frametime;
            do_loop = doloop;
        }

        int frame_height;
        int frame_length;
        bool do_loop;
        int change_frame_every;
        string asset_path;

    }
}
