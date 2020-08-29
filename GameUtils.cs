using ChaiFoxes.FMODAudio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using RewindGame.Game.Effects;
using RewindGame.Game.Solids;
using RewindGame.Game.Sound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
namespace RewindGame
{

    public class GameUtils
    {
        // DEBUG SETTINGS

        public const bool SHOULD_SOLIDTILES_RENDER = true;
        public const bool USE_DEBUG_KEYS = true;
        public const bool OVERRIDE_LEVEL_LOAD = true;
        public const bool OVERRIDE_LEVEL_SAVE = false;

        // GAME

        public static Vector2 BASE_SCREEN_SIZE = new Vector2(1600, 900);

        public const float MOVE_STICK_SCALE = 1.0f;
        public const float MOVE_STICK_MAX = 1.0f;

        public const int LEVEL_GRID_SIZE_X = 29;
        public const int LEVEL_GRID_SIZE_Y = 17;

        public const float LEVEL_SIZE_X = TILE_WORLD_SIZE * LEVEL_GRID_SIZE_X;
        public const float LEVEL_SIZE_Y = TILE_WORLD_SIZE * LEVEL_GRID_SIZE_Y;

        public const float PLAYER_DEATH_TIME = 0.5f;


        public const int CAMERA_FOCUS_Y_OFFSET = -112;


        public static Interval FULL_TIME_BOUND = new Interval(-300, 300);
        public static Interval FULL_DANGER_BOUND = new Interval(-250, 250);

        public static Interval HALF_TIME_BOUND = new Interval(-150, 150);
        public static Interval HALF_DANGER_BOUND = new Interval(-100, 100);

        public static Interval FOURTH_TIME_BOUND = new Interval(-75, 75);
        public static Interval FOURTH_DANGER_BOUND = new Interval(-40, 40);


        // Level


        public const int TILE_WORLD_SIZE = 56;
        public const int LARGE_TILE_WORLD_SIZE = 112;

        public const int TILE_SHEET_SIZE = 56;
        public const int LARGE_TILE_SHEET_SIZE = 112;

        public const int DECORATIVE_SHEET_TILES_X = 10;
        public const int DECORATIVE_SHEET_TILES_Y = 13;

        public const int COLLISION_SHEET_TILES_X = 20;
        public const int COLLISION_SHEET_TILES_Y = 20;

        public const float SEMISOLID_THICKNESS = 4f;
        public const float SEMISOLID_THICKNESS_WINDOW = 20f;
        public const float WALLSPIKE_THICKNESS = 28f;

        // GUI


        public const float TIMELINE_BAR_SIZE_FOURTH = 100;
        public const float TIMELINE_BAR_SIZE_HALF = 104 * 2;
        public const float TIMELINE_BAR_SIZE_FULL = 107 * 4;
    }

    public class InputData
    {
        public InputData()
        {
            is_jump_pressed = false;
            is_jump_held = false;

            is_interact_pressed = false;
            is_exit_pressed = false;
            is_restart_pressed = false;

            is_level_down = false;
            is_level_left = false;
            is_level_right = false;
            is_level_up = false;
            is_limbo1_down = false;
            is_cotton1_down = false;
            is_eternal1_down = false;
            is_limboHalf_down = false;
            is_cottonHalf_down = false;
            is_eternalHalf_down = false;
            is_level_select = false;
            is_level_reload = false;
            for (int i = 0; i < is_devkey_down.Length; i++)
            {
                is_devkey_down[i] = false;
            }

            horizontal_axis_value = 0;
        }

        public bool is_jump_pressed;
        public bool is_jump_held;

        public bool is_interact_pressed;
        public bool is_exit_pressed;
        public bool is_restart_pressed;

        public bool is_level_left;      //0  LEFT
        public bool is_level_right;     //1  RIGHT
        public bool is_level_up;        //2  UP
        public bool is_level_down;      //3  DOWN
        public bool is_limbo1_down;     //4  LIMBO 1
        public bool is_cotton1_down;    //5  COTTON 1
        public bool is_eternal1_down;   //6  ETERNAL 1
        public bool is_limboHalf_down;  //7  LIMBO 50%
        public bool is_cottonHalf_down; //8  COTTON 50%
        public bool is_eternalHalf_down;//9  ETERNAL 50%
        public bool is_level_select;    //10 LEVEL SELECT
        public bool is_level_reload;    //11 LEVEL RELOAD

        public bool[] is_devkey_down = new bool[12];


        public float horizontal_axis_value;
    }

    public enum TimeState
    {
        forward,
        backward,
        still
    }

    public enum TimeKind
    {
        none,
        limbo,
        cottonwood,
        eternal
    }

    public class TimeData
    {
        public TimeData()
        {
            time_moment = 0;
            time_status = TimeState.forward;
        }

        public int time_moment;
        public TimeState time_status;
        public TimeKind time_kind;

        public void Reset()
        {
            time_moment = 0;
            time_status = TimeState.forward;
        }

        public int getFloaty(float xpos, bool use_still)
        {
            if (use_still && time_status == TimeState.still) return 0;

            int real_xpos = (int)xpos / GameUtils.TILE_WORLD_SIZE;
            int real_mod = (time_moment + real_xpos) / 10;
            int mult = 2;
            switch (real_mod % 10)
            {
                case 5:
                case 0: return 0;
                case 4:
                case 1: return mult;
                case 2:
                case 3: return 2 * mult;
                case 9:
                case 6: return -mult;
                case 8:
                case 7: return -2 * mult;
            }
            return 0;
        }
    }

    public class StateData
    {
        public StateData(InputData inputdata, TimeData timedata, GameTime gametime, Vector2 levelcenter, Vector2 cameraoffset, Interval timebounds)
        {
            input_data = inputdata;
            time_data = timedata;
            game_time = gametime;
            level_center = levelcenter;
            camera_position = cameraoffset;
            time_bound = timebounds;
        }

        public int getTimeSign()
        {
            return time_data.time_status == TimeState.backward ? -1 : time_data.time_status == TimeState.still ? 0 : 1;
        }

        public InputData input_data;
        public TimeData time_data;
        public GameTime game_time;
        public Vector2 level_center;
        public Vector2 camera_position;
        public Interval time_bound;

        public float getDeltaTime()
        {
            return (float)game_time.ElapsedGameTime.TotalSeconds;
        }

        public float getTimeDependentDeltaTime()
        {
            return (float)getDeltaTime() * getTimeSign();
        }

        public float getTimeLen()
        {
            return time_bound.length();
        }
    }

    public enum RunState
    {
        playing,
        playerdead,
        areaswap_1,
        areaswap_2,
        areaswap_3,
    }

    public enum AreaState
    {
        none,
        limbo,
        cotton,
        eternal
    }

    public class LevelNameData
    {
        public LevelNameData(string fullname)
        {
            string[] arr = fullname.Split("/");

            bool is_before_name = true;
            foreach (string s in arr)
            {
                if (s.Length == 0) continue;
                else if (s.Length > 1)
                {
                    name = s.Trim();
                    is_before_name = false;
                }
                else
                {
                    if (is_before_name)
                    {
                        is_entrance_extreme = s == "R" | s == "B" ? true : false;
                    }
                    else
                    {
                        is_exit_extreme = s == "R" | s == "B" ? true : false;
                    }
                }
            }

        }

        public bool isUpward() { return !is_entrance_extreme && is_exit_extreme; }

        public bool isDownward() { return is_entrance_extreme && !is_exit_extreme; }

        public String name;
        public bool is_entrance_extreme = false;
        public bool is_exit_extreme = false;
    }

    public class Interval
    {
        public Interval(int min, int max)
        {
            if (min > max)
            {
                //error
            }
            this.min = min;
            this.max = max;
        }

        public bool isWithinInclusive(int n)
        {
            return n >= min && n <= max;
        }

        public bool isWithinExclusive(int n)
        {
            return n > min && n < max;
        }

        public int length()
        {
            return max - min;
        }

        public int max;
        public int min;
        
    }

}
