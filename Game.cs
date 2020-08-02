using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using System.Collections.Generic;
using System.Linq.Expressions;

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

    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        const float MOVE_STICK_SCALE = 1.0f;
        const float MOVE_STICK_MAX = 1.0f;

        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<Texture2D> textures = new List<Texture2D>();

        public InputData inputData = new InputData();
        private Level openLevel;

        public RewindGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textures.Add(Content.Load<Texture2D>("square"));
            textures.Add(Content.Load<Texture2D>("platform"));


            openLevel = new Level(Services, this);
        }

        protected override void Update(GameTime game_time)
        {
            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed)
                Exit();

            openLevel.Update(game_time);

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

            spriteBatch.Begin();

            openLevel.Draw(game_time, spriteBatch);

            spriteBatch.End();

            base.Draw(game_time);
        }
    }
}
