using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using System;
using System.Collections.Generic;
using System.IO;

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
        public StateData(InputData inputdata, TimeData timedata, GameTime gametime)
        {
            input_data = inputdata;
            time_data = timedata;
            game_time = gametime;
        }

        public InputData input_data;
        public TimeData time_data;
        public GameTime game_time;

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

    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        const float MOVE_STICK_SCALE = 1.0f;
        const float MOVE_STICK_MAX = 1.0f;

        const float LEVEL_SIZE_X = Level.TILE_WORLD_SIZE * 29;
        const float LEVEL_SIZE_Y = Level.TILE_WORLD_SIZE * 17;

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

        private Level activeLevel;
        private Vector2 activeLevelOffset = Vector2.Zero;

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

        public void ScaleScreen()
        {
            backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            float horScaling = backBufferWidth / baseScreenSize.X;
            float verScaling = backBufferHeight / baseScreenSize.Y;

            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);

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
            activeLevel = new Level(Services, Vector2.Zero ,this);
            activeLevel.isActiveScene = true;
            LevelLoader.LoadLevel("techdemolevel.json", activeLevel);
        }




        protected override void Update(GameTime game_time)
        {
            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed)
                Exit();

            if (runState == RunState.playing || runState == RunState.playerdead) { 

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

                StateData state = new StateData(inputData, timeData, game_time);

                activeLevel.Update(state);

                player.Update(state);
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
        }

        public Level getConnectedOrLoadLevel(String name, Vector2 offset)
        {
            Level level = null;

            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    if (lvl.name == name)
                    {
                        level = lvl;
                    }
                }
            }

            if (level == null)
            {
                level = new Level(Services, offset, this);
                LevelLoader.LoadLevel(name + ".json", level);
            }

            return level;
        }

        public void loadLevelAndConnections(String name)
        {
            Level center_level = getConnectedOrLoadLevel(name, Vector2.Zero);
            activeLevelOffset = center_level.levelOrgin;


            Level[] new_connected_levels = { null, null, null, null };
            int i = 0;
            foreach (String level_name in center_level.connectedLevelNames)
            {
                if (level_name != "")
                {
                    Vector2 offset = offsetLevelOrgin(activeLevelOffset, i);
                    new_connected_levels[i] = getConnectedOrLoadLevel(level_name, offset);
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



        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            var matrix = Matrix.Identity;
            matrix.Translation = new Vector3(-28, 0,0);

            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix:matrix);

            StateData state = new StateData(inputData, timeData, game_time);

            activeLevel.DrawBackground(state, spriteBatch);

            DrawAllConnectedBackgrounds(state, spriteBatch);

            player.Draw(state, spriteBatch);

            activeLevel.DrawForeground(state, spriteBatch);

            DrawAllConnectedForegrounds(state, spriteBatch);

            spriteBatch.End();

            base.Draw(game_time);
        }

        public void DrawAllConnectedBackgrounds(StateData state, SpriteBatch sprite_batch)
        {
            foreach(Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    lvl.DrawBackground(state, sprite_batch);
                }
            }
        }

        public void DrawAllConnectedForegrounds(StateData state, SpriteBatch sprite_batch)
        {
            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    lvl.DrawForeground(state, sprite_batch);
                }
            }
        }

        // says if the object can save it's state in this moment-
        // more often means more memory consumption
        public static bool isSavableMoment(int moment)
        {
            return moment % 3 == 0;
        }

        // 1: right
        // 2: left
        // 3: up
        // 4: down
        public static Vector2 offsetLevelOrgin(Vector2 orgin, int index)
        {
            switch(index)
            {
                case 0:
                    orgin.X += LEVEL_SIZE_X;
                    break;
                case 1:
                    orgin.X -= LEVEL_SIZE_X;
                    break;
                case 2:
                    orgin.Y -= LEVEL_SIZE_Y;
                    break;
                case 3:
                    orgin.Y += LEVEL_SIZE_Y;
                    break;
            }
            return orgin;
        }
    }
}
