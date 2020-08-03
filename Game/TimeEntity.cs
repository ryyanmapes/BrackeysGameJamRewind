using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewindGame.Game
{

    public class PhysData
    {
        public PhysData(Vector2 pos, Vector2 vel)
        {
            position = pos;
            velocity = vel;
        }

        public Vector2 position;
        public Vector2 velocity;
    }

    abstract class TimeEntity : Entity, ITimeTrackable 
    {

        protected Dictionary<int, PhysData> pastPhysData = new Dictionary<int, PhysData>();

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
            pastPhysData.Add(time_moment, new PhysData(position, velocity));
        }

        public void LoadState(int time_moment)
        {
            PhysData phys_data;
            pastPhysData.Remove(time_moment, out phys_data);

            if (phys_data == null)
            {
                Console.WriteLine("Unable to find time moment for " + time_moment);
                return;
            }

            position = phys_data.position;
            velocity = phys_data.velocity;
        }

    }
}
