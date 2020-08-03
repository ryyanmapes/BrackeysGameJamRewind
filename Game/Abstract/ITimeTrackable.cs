using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    interface ITimeTrackable
    {
        public abstract void TemporalUpdate(StateData state);

        public abstract void SaveState(int time_moment);

        public abstract void LoadState(int time_moment);


    }
}
