using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{
    abstract class TimeEntity : Entity, ITimeTrackable 
    {

        protected Dictionary<int, Vector2> pastPositions = new Dictionary<int, Vector2>();

        public void TemporalUpdate(StateData state)
        {
            int time_moment = state.time_data.time_moment;

            switch (state.time_data.time_status)
            {
                case TimeState.forward:
                    if (RewindGame.isSavableMoment(time_moment))
                    {
                        SaveState(time_moment);
                    }
                    Update(state);
                    break;
                case TimeState.backward:
                    if (RewindGame.isSavableMoment(time_moment))
                    {
                        LoadState(time_moment);
                    }
                    // In the update we need to check if time is backwards
                    Update(state);
                    break;
                case TimeState.still:
                    return;

            }
        }

        public void SaveState(int time_moment)
        {
            pastPositions.Add(time_moment, position);
        }

        public void LoadState(int time_moment)
        {
            pastPositions.Remove(time_moment, out position);
        }

    }
}
