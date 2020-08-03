﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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

    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        const float MOVE_STICK_SCALE = 1.0f;
        const float MOVE_STICK_MAX = 1.0f;

        private Vector2 baseScreenSize = new Vector2(1600, 900);
        int backBufferWidth, backBufferHeight;
        private Matrix globalTransformation;

        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<Texture2D> textures = new List<Texture2D>();

        public InputData inputData = new InputData();
        public TimeData timeData = new TimeData();

        public int timeNegBound = -1000000;
        public int timePosBound = 1000000;

        private Level openLevel;

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

            // TODO: use this.Content to load your game content here
            textures.Add(Content.Load<Texture2D>("square"));
            textures.Add(Content.Load<Texture2D>("platform"));

            //ScaleScreen();

            openLevel = new Level(Services, this);
            LevelLoader.LoadLevel("limbospiketest.json", openLevel);
        }

        protected override void Update(GameTime game_time)
        {
            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed)
                Exit();

            if (inputData.is_jump_held)
            {
                if (timeData.time_moment > timeNegBound) timeData.time_status = TimeState.backward;
            }
            else

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

            openLevel.Update(state);

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

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.CadetBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate);

            StateData state = new StateData(inputData, timeData, game_time);

            openLevel.Draw(state, spriteBatch);

            spriteBatch.End();

            base.Draw(game_time);
        }


        // says if the object can save it's state in this moment-
        // more often means more memory consumption
        public static bool isSavableMoment(int moment)
        {
            return moment % 3 == 0;
        }
    }
}
