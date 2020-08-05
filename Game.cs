using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using RewindGame.Game.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RewindGame
{

    public class InputData
    {
        public InputData()
        {
            is_jump_pressed = false;
            is_jump_held = false;

            is_interact_pressed = false;
            is_exit_pressed = false;
            is_restart_pressed = false;

            horizontal_axis_value = 0;
        }

        public bool is_jump_pressed;
        public bool is_jump_held;

        public bool is_interact_pressed;
        public bool is_exit_pressed;
        public bool is_restart_pressed;


        public float horizontal_axis_value;
    }

    public enum TimeState
    {
        forward,
        backward,
        still
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
    }

    public class StateData
    {
        public StateData(InputData inputdata, TimeData timedata, GameTime gametime, Vector2 levelcenter, Vector2 cameraoffset)
        {
            input_data = inputdata;
            time_data = timedata;
            game_time = gametime;
            level_center = levelcenter;
            camera_position = cameraoffset;
        }

        public InputData input_data;
        public TimeData time_data;
        public GameTime game_time;
        public Vector2 level_center;
        public Vector2 camera_position;

        public double getDeltaTime()
        {
            return game_time.ElapsedGameTime.TotalSeconds;
        }

        public double getSignedDeltaTime()
        {
            return getDeltaTime() * (time_data.time_status == TimeState.backward ? -1 : 1);
        }
    }

    public enum RunState
    {
        playing,
        playerdead,
        levelswap,
        paused
    }

    public enum AreaState
    {
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

    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        public const float MOVE_STICK_SCALE = 1.0f;
        public const float MOVE_STICK_MAX = 1.0f;

        public const int LEVEL_GRID_SIZE_X = 29;
        public const int LEVEL_GRID_SIZE_Y = 17;

        const float LEVEL_SIZE_X = Level.TILE_WORLD_SIZE * LEVEL_GRID_SIZE_X;
        const float LEVEL_SIZE_Y = Level.TILE_WORLD_SIZE * LEVEL_GRID_SIZE_Y;

        private Vector2 baseScreenSize = new Vector2(1600, 900);
        int backBufferWidth, backBufferHeight;
        private Matrix globalTransformation;

        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<Texture2D> textures = new List<Texture2D>();

        public InputData inputData = new InputData();
        public TimeData timeData = new TimeData();

        public Texture2D decorativeSheetTexture;
        public Texture2D collisionSheetTexture;

        public int timeNegBound = -1000000;
        public int timePosBound = 1000000;

        public RunState runState = RunState.playing;

        public Level activeLevel;
        public Vector2 activeLevelOffset = Vector2.Zero;
        public IEffectManager currentEffects;
        public Vector2 currentLevelCenter;
        public Vector2 currentCameraPosition;

        public String qued_level_load_name = "";

        // connected levels are loaded, but not updated actively
        // 1: right
        // 2: left
        // 3: up
        // 4: down
        private Level[] connectedLevels = { null, null, null, null };

        public PlayerEntity player;

        public RewindGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            IsFixedTimeStep = true;
            // run at a fixed timestep for 60 fps
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);

            graphics.PreferredBackBufferWidth = (int)baseScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)baseScreenSize.Y;

            graphics.ApplyChanges();

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // this is useless!
            textures.Add(Content.Load<Texture2D>("debug/square"));
            textures.Add(Content.Load<Texture2D>("debug/platform"));


            decorativeSheetTexture = Content.Load<Texture2D>("tilesets/decorative");
            collisionSheetTexture = Content.Load<Texture2D>("tilesets/collision");



            // the offset here is for when we have many levels
            loadLevelAndConnections("techdemolevel");

            player = new PlayerEntity(this, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 - 300));
            player.position = activeLevel.playerSpawnpoint;

            currentEffects = new LimboEffects(this, Content);
        }



        public void loadLevelAndConnections(String name)
        {
            Level center_level = getConnectedOrLoadLevel(name, activeLevel, -1);
            activeLevelOffset = center_level.levelOrgin;


            Level[] new_connected_levels = { null, null, null, null };
            int i = 0;
            foreach (String level_name in center_level.connectedLevelNames)
            {
                if (level_name != "")
                {
                    new_connected_levels[i] = getConnectedOrLoadLevel(level_name, center_level, i);
                }
                i += 1;
            }


            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    if (lvl.name != center_level.name)
                    {
                        lvl.Dispose();
                    }
                }
            }

            connectedLevels = new_connected_levels;
            activeLevel = center_level;
            center_level.isActiveScene = true;

        }

        public Level getConnectedOrLoadLevel(String name, Level current_level, int index)
        {
            Level level = null;

            var name_data = new LevelNameData(name);

            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    if (lvl.name == name_data.name)
                    {
                        level = lvl;
                        break;
                    }
                }
            }

            if (activeLevel != null && activeLevel.name == name_data.name)
            {
                level = activeLevel;
            }



            // 1: right
            // 2: left
            // 3: up
            // 4: down
            if (level == null)
            {

                var raw_level = LevelLoader.GetLevelData(name_data.name);


                Vector2 orgin = new Vector2(activeLevelOffset.X, activeLevelOffset.Y);

                if (current_level != null && index != -1)
                {

                    bool is_horizontal = index <= 1;
                    bool is_forwards = index == 0 || index == 3;

                    float new_horiz = (float)raw_level.layers[0].gridCellsX / (float)RewindGame.LEVEL_GRID_SIZE_X;
                    float new_vert = (float)raw_level.layers[0].gridCellsY / (float)RewindGame.LEVEL_GRID_SIZE_Y;

                    if (is_horizontal)
                    {

                        if (is_forwards) 
                        {
                            orgin.X += LEVEL_SIZE_X * current_level.screensHorizontal;
                        }
                        else
                        {
                            orgin.X -= LEVEL_SIZE_X * new_horiz;
                        }
                        
                        if (name_data.isUpward())
                        {
                            orgin.Y -= (LEVEL_SIZE_Y * (new_vert - 1));
                        }
                        else if (name_data.isDownward()) orgin.Y += (LEVEL_SIZE_Y * (current_level.screensVertical - 1));
                    }
                    else
                    {
                        if (is_forwards)
                        {
                            orgin.Y += LEVEL_SIZE_Y * current_level.screensVertical;
                        }
                        else
                        {
                            orgin.Y -= LEVEL_SIZE_Y * new_vert;
                        }

                        if (name_data.isUpward())
                        {
                            orgin.X -= (LEVEL_SIZE_X * (new_horiz - 1));
                        }
                        else if (name_data.isDownward()) orgin.X += (LEVEL_SIZE_X * (current_level.screensHorizontal - 1));
                    }

                }

                level = new Level(Services, orgin, this);
                LevelLoader.LoadLevel(raw_level, name_data.name, level);
            }

            return level;
        }




        protected override void Update(GameTime game_time)
        {
            currentLevelCenter = getLevelCenter();
            currentCameraPosition = getCameraPosition();

            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed)
                Exit();

            if (inputData.is_restart_pressed)
                player.position = activeLevel.playerSpawnpoint;

            if (runState == RunState.playing) { 

                // still a bit indev-y
                if (player.isRewinding)
                {
                    if (timeData.time_moment > timeNegBound) timeData.time_status = TimeState.backward;
                }
                else
                {
                    if (timeData.time_moment < timePosBound) timeData.time_status = TimeState.forward;
                }

                switch (timeData.time_status)
                {
                    case TimeState.forward:
                        timeData.time_moment += 1;
                        break;
                    case TimeState.backward:
                        timeData.time_moment -= 1;
                        break;
                    default:
                        break;

                }

                if (timeData.time_moment <= timeNegBound || timeData.time_moment >= timePosBound)
                {
                    timeData.time_status = TimeState.still;
                }

                StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition);

                currentEffects.Update(state);

                activeLevel.Update(state);

                player.Update(state);

                if (qued_level_load_name != "")
                {
                    loadLevelAndConnections(qued_level_load_name);
                    qued_level_load_name = "";
                }
            }

            base.Update( game_time);
        }

        private static InputData ReadInputs(InputData previous_input_data)
        {
            var input_data = new InputData();

            var keyboard_state = Keyboard.GetState();
            var gamepad_state = GamePad.GetState(PlayerIndex.One);

            var is_any_left_button_down = gamepad_state.IsButtonDown(Buttons.DPadLeft) || keyboard_state.IsKeyDown(Keys.A) || keyboard_state.IsKeyDown(Keys.Left);

            var is_any_right_button_down = gamepad_state.IsButtonDown(Buttons.DPadRight) || keyboard_state.IsKeyDown(Keys.D) || keyboard_state.IsKeyDown(Keys.Right);

            if (is_any_left_button_down && is_any_right_button_down)
                input_data.horizontal_axis_value = 0;
            else if (is_any_left_button_down)
                input_data.horizontal_axis_value = -MOVE_STICK_MAX;
            else if (is_any_right_button_down)
                input_data.horizontal_axis_value = MOVE_STICK_MAX;
            else
                input_data.horizontal_axis_value = gamepad_state.ThumbSticks.Left.X * MOVE_STICK_SCALE;



            var is_any_jump_button_down = gamepad_state.Buttons.A == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.Z) || keyboard_state.IsKeyDown(Keys.Space);

            if (!(previous_input_data.is_jump_pressed || previous_input_data.is_jump_held) && is_any_jump_button_down )
                input_data.is_jump_pressed = true;
            else if (is_any_jump_button_down)
                input_data.is_jump_held = true;



            // force exit
            if (gamepad_state.Buttons.Back == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.Escape))
                input_data.is_exit_pressed = true;

            // todo interact, restart bindings

            return input_data;
        }

        public void killPlayer()
        {
            player.hidden = true;
            //TODO
        }

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            var matrix = Matrix.Identity;
            matrix.Translation = new Vector3(-1 * (currentCameraPosition.X - LEVEL_SIZE_X / 2), -1 * (currentCameraPosition.Y - LEVEL_SIZE_Y / 2), 0);

            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix:matrix);

            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition);

            currentEffects.DrawBackground(state, spriteBatch);

            activeLevel.DrawBackground(state, spriteBatch);

            DrawAllConnectedLevelBackgrounds(state, spriteBatch);

            player.Draw(state, spriteBatch);

            activeLevel.DrawForeground(state, spriteBatch);

            DrawAllConnectedLevelForegrounds(state, spriteBatch);

            currentEffects.DrawForeground(state, spriteBatch);

            spriteBatch.End();

            base.Draw(game_time);
        }

        public void DrawAllConnectedLevelBackgrounds(StateData state, SpriteBatch sprite_batch)
        {
            foreach(Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    lvl.DrawBackground(state, sprite_batch);
                }
            }
        }

        public void DrawAllConnectedLevelForegrounds(StateData state, SpriteBatch sprite_batch)
        {
            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    lvl.DrawForeground(state, sprite_batch);
                }
            }
        }


        public Vector2 getLevelCenter()
        {
            return new Vector2(12 + activeLevelOffset.X + LEVEL_SIZE_X/2, activeLevelOffset.Y + LEVEL_GRID_SIZE_Y/2);
        }

        // gets the position of the center of the camera
        public Vector2 getCameraPosition()
        {
            float edge_left = 12 + activeLevelOffset.X + LEVEL_SIZE_X / 2;
            float edge_right = 12 + activeLevelOffset.X + (LEVEL_SIZE_X * (activeLevel.screensHorizontal - 0.5f));
            float edge_down = 12 + activeLevelOffset.Y + LEVEL_SIZE_Y / 2;
            float edge_up = 12 + activeLevelOffset.Y + (LEVEL_SIZE_Y * (activeLevel.screensVertical - 0.5f));
            return new Vector2(Math.Clamp(player.position.X, edge_left, edge_right), Math.Clamp(player.position.Y, edge_down, edge_up));
        }



        // says if the object can save it's state in this moment-
        // more often means more memory consumption
        public static bool isSavableMoment(int moment)
        {
            return moment % 3 == 0;
        }

    
        
    
    }
}
