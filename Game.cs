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

            int real_xpos = (int)xpos / Level.TILE_WORLD_SIZE;
            int real_mod = (time_moment + real_xpos)/10;
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
        public StateData(InputData inputdata, TimeData timedata, GameTime gametime, Vector2 levelcenter, Vector2 cameraoffset, int nb, int pb)
        {
            input_data = inputdata;
            time_data = timedata;
            game_time = gametime;
            level_center = levelcenter;
            camera_position = cameraoffset;
            time_bound_neg = nb;
            time_bound_pos = pb;
        }

        public int getTimeN()
        {
            return time_data.time_status == TimeState.backward ? -1 : time_data.time_status == TimeState.still ? 0 : 1;
        }

        public InputData input_data;
        public TimeData time_data;
        public GameTime game_time;
        public Vector2 level_center;
        public Vector2 camera_position;
        public int time_bound_neg = -1000000;
        public int time_bound_pos = 1000000;

        public float getDeltaTime()
        {
            return (float)game_time.ElapsedGameTime.TotalSeconds;
        }

        public float getTimeDependentDeltaTime()
        {
            return (float) getDeltaTime() * getTimeN();
        }

        public float getTimeLen()
        {
            return time_bound_pos - time_bound_neg;
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

    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        public const float MOVE_STICK_SCALE = 1.0f;
        public const float MOVE_STICK_MAX = 1.0f;

        public const int LEVEL_GRID_SIZE_X = 29;
        public const int LEVEL_GRID_SIZE_Y = 17;

        public const float LEVEL_SIZE_X = Level.TILE_WORLD_SIZE * LEVEL_GRID_SIZE_X;
        public const float LEVEL_SIZE_Y = Level.TILE_WORLD_SIZE * LEVEL_GRID_SIZE_Y;

        public const bool SHOULD_SOLIDTILES_RENDER = true;

        public const float playerDeathTime = 0.5f;
        // will change
        public int timeNegBound = -1000000;
        public int timePosBound = 1000000;
        public int timeDangerNegBound = -1000000;
        public int timeDangerPosBound = 1000000;

        public Vector2 baseScreenSize = new Vector2(1600, 900);

        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<Texture2D> textures = new List<Texture2D>();

        public InputData inputData = new InputData();
        public TimeData timeData = new TimeData();

        public Texture2D decorativeSheetTexture;
        public Texture2D collisionSheetTexture;

        public Level activeLevel;
        public Vector2 activeLevelOffset = Vector2.Zero;
        public Vector2 currentLevelCenter;
        public Vector2 currentCameraPosition;

        public ILevelEffect areaEffect;
        public OverlayEffects overlayEffect;
        public SoundManager soundManager;
        public TimelineBarGUI timelineGUI;

        public String qued_level_load_name = "";
        public bool qued_player_death = false;

        public AreaState area = AreaState.none;
        public RunState runState = RunState.playing;
        public float stateTimer = -1f;
        public float playerHoverTime = 2;
        public float warpTime = 2;
        public float areaSwapTime = 3;

        public int deathsStat = 0;

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
            //textures.Add(Content.Load<Texture2D>("debug/square"));
            //textures.Add(Content.Load<Texture2D>("debug/platform"));


            decorativeSheetTexture = Content.Load<Texture2D>("tilesets/decorative");
            collisionSheetTexture = Content.Load<Texture2D>("tilesets/collision");


            overlayEffect = new OverlayEffects(this, Content);
            soundManager = new SoundManager(this, Content);
            timelineGUI = new TimelineBarGUI(this, Content);

            LoadArea(AreaState.limbo);

            player = new PlayerEntity(this, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 - 300));
            player.position = activeLevel.playerSpawnpoint;
            //DoTrigger("limbo_fourth");
        }

        public void LoadNextArea()
        {
            switch (area)
            {
                case AreaState.limbo:
                    LoadArea(AreaState.cotton);
                    break;
                case AreaState.cotton:
                    LoadArea(AreaState.eternal);
                    break;

            }
        }

        public void LoadArea(AreaState new_area)
        {
            soundManager.stopAllMusic();
            if (areaEffect != null) areaEffect.Dispose();
            switch (new_area)
            {
                case AreaState.limbo:
                    this.area = AreaState.limbo;
                    soundManager.BeginLimboMusic1();
                    areaEffect = new LimboEffects(this, Services);
                    timeData.time_kind = TimeKind.limbo;
                    loadLevelAndConnections("limbo1");

                    timelineGUI.SetBar(timelineGUI.limboBar1);
                    timelineGUI.currentBarSize = 105 * 4;
                    timeNegBound = -300;
                    timePosBound = 300;
                    timeDangerNegBound = -250;
                    timeDangerPosBound = 250;
                    break;
                case AreaState.cotton:
                    this.area= AreaState.cotton;
                    soundManager.BeginCottonwoodMusic1();
                    areaEffect = new CottonwoodEffects(this, Services);
                    timeData.time_kind = TimeKind.cottonwood;
                    loadLevelAndConnections("cotton1");
                    break;
                case AreaState.eternal:
                    this.area = AreaState.eternal;
                    //soundManager.beg
                    areaEffect = new EternalEffects(this, Services);
                    timeData.time_kind = TimeKind.eternal;
                    loadLevelAndConnections("eternal1");
                    break;

            }
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
                    new_connected_levels[i].SetInactive();
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

            if (activeLevel != null) { center_level.RunStartTriggers(); }

            connectedLevels = new_connected_levels;
            activeLevel = center_level;
            center_level.isActiveScene = true;
            center_level.SetActive();

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

                var raw_level = LevelLoader.GetLevelData(name_data.name, this.area);


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
                qued_player_death = true;
            
            if (Keyboard.GetState().IsKeyDown(Keys.K))
                loadLevelAndConnections("limbo1");      
            else if (Keyboard.GetState().IsKeyDown(Keys.L))
                loadLevelAndConnections("limbo19");
            else if (Keyboard.GetState().IsKeyDown(Keys.J))
                loadLevelAndConnections("limbofinal");
            else if (Keyboard.GetState().IsKeyDown(Keys.P))
                loadLevelAndConnections("cottonfinal");
            else if (Keyboard.GetState().IsKeyDown(Keys.U)) LoadArea(AreaState.cotton);
            else if (Keyboard.GetState().IsKeyDown(Keys.N)) LoadArea(AreaState.eternal);
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            {
                if (activeLevel.connectedLevelNames[2] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[2]);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                if (activeLevel.connectedLevelNames[0] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[0]);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            {
                    if (activeLevel.connectedLevelNames[3] != "")
                    {
                        loadLevelAndConnections(activeLevel.connectedLevelNames[3]);
                    }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                        if (activeLevel.connectedLevelNames[1] != "")
                        {
                            loadLevelAndConnections(activeLevel.connectedLevelNames[1]);
                        }
            }

            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition, timeNegBound, timePosBound);


            if (stateTimer != -1)
            {
                stateTimer -= (float)state.getDeltaTime();
                if (stateTimer <= 0)
                {
                    switch (runState)
                    {
                        case RunState.playerdead:
                            player.position = activeLevel.playerSpawnpoint;
                            runState = RunState.playing;

                            timeData.Reset();
                            activeLevel.Reset();

                            stateTimer = -1;
                            qued_player_death = false;
                            break;

                        case RunState.areaswap_1:
                            activeLevel.warp.TriggerActivation();
                            overlayEffect.StartWarpPlayer(player.getCenterPosition(), state);
                            overlayEffect.StartWarpArtifact(activeLevel.specialObject.getCenterPosition(), state);

                            soundManager.TriggerWarp();

                            stateTimer = warpTime;
                            runState = RunState.areaswap_2;
                            break;

                        case RunState.areaswap_2:
                            player.hidden = true;
                            activeLevel.specialObject.hidden = true;
                            activeLevel.warp.hidden = true;

                            overlayEffect.StartAreaFadeout();

                            stateTimer = warpTime;
                            runState = RunState.areaswap_3;
                            break;

                        case RunState.areaswap_3:
                            LoadNextArea();

                            player.position = activeLevel.playerSpawnpoint;

                            timeData.Reset();

                            stateTimer = -1;
                            runState = RunState.playing;
                            player.hidden = false;
                            break;

                    }
                    if (runState == RunState.playerdead)
                    {
                        player.position = activeLevel.playerSpawnpoint;
                        runState = RunState.playing;

                        timeData.Reset();
                        activeLevel.Reset();

                        stateTimer = -1;
                        qued_player_death = false;
                    }
                }
            }



            switch (runState)
            {
                case RunState.playing:
                    FullUpdate(state);
                    break;
                case RunState.areaswap_1:
                    player.position.Y -= 10*state.getDeltaTime();
                    break;
            }

            overlayEffect.Update(state);
            soundManager.Update(state);
            timelineGUI.Update(state);

            base.Update( game_time);
        }

        protected void FullUpdate(StateData state)
        {
            // special text collision check
            if (activeLevel.getIsInSpecial(player.getCollisionBox()))
            {
                if (activeLevel.specialObject.charState == -1) activeLevel.specialObject.charState = 0;
            }
            else if (activeLevel.specialObject != null)
            {
                activeLevel.specialObject.Reset();
            }

            //warp collision check
            if (activeLevel.getIsInWarp(player.getCollisionBox()))
            {
                // begin warp!
                runState = RunState.areaswap_1;
                stateTimer = playerHoverTime;
                soundManager.stopAllMusic();
                return;
            }

            if (activeLevel.getIsInStasis(player.getCollisionBox()))
            {
                if (timeData.time_status != TimeState.still)
                {
                    timeData.time_status = TimeState.still;
                    timeData.time_moment = 0;
                    activeLevel.Reset();
                    areaEffect.Reset();
                    overlayEffect.MakeStasisParticles(state);
                }
            }
            else if (timeData.time_kind == TimeKind.none)
            {
                timeData.time_status = TimeState.still;
                timeData.time_moment = 0;
            }
            else if (player.isRewinding)
            {
                timeData.time_status = TimeState.backward;
            }
            else
            {
                timeData.time_status = TimeState.forward;
            }

            // what to do at the bounds of time
            if (timeData.time_moment <= timeNegBound)
            {
                switch (timeData.time_kind)
                {
                    case TimeKind.limbo:
                        qued_player_death = true;
                        soundManager.TriggerLightining();
                        break;
                    case TimeKind.cottonwood:
                        if (timeData.time_status == TimeState.backward) timeData.time_status = TimeState.still;
                        break;
                    case TimeKind.eternal:
                        timeData.time_moment = timePosBound - 1;
                        break;
                }
            }
            else if (timeData.time_moment >= timePosBound)
            {
                switch (timeData.time_kind)
                {
                    case TimeKind.limbo:
                        qued_player_death = true;
                        soundManager.TriggerLightining();
                        break;
                    case TimeKind.cottonwood:
                        if (timeData.time_status == TimeState.forward) timeData.time_status = TimeState.still;
                        break;
                    case TimeKind.eternal:
                        timeData.time_moment = timeNegBound + 1;
                        break;
                }
            }

            // what happens if the player goes out of bounds
            if (player.position.Y > activeLevelOffset.Y + LEVEL_SIZE_Y * activeLevel.screensVertical + Level.TILE_WORLD_SIZE * 2
                || player.position.Y < activeLevelOffset.Y - Level.TILE_WORLD_SIZE)
            {
                qued_player_death = true;
            }

            // time state incrementing
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

            areaEffect.Update(state);

            activeLevel.Update(state);

            player.Update(state);

            if (qued_level_load_name != "")
            {
                loadLevelAndConnections(qued_level_load_name);
                qued_level_load_name = "";
                timeData.Reset();
            }
            else if (qued_player_death && runState != RunState.playerdead)
            {
                KillPlayer();
                qued_player_death = false;
            }
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

            if (gamepad_state.Buttons.Y == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.R))
                input_data.is_restart_pressed = true;

            // todo interact, restart bindings

            return input_data;
        }

        public void KillPlayer()
        {
            deathsStat += 1;
            runState = RunState.playerdead;
            stateTimer = playerDeathTime;
            timeData.time_status = TimeState.still;

            overlayEffect.TriggerDeath();

            soundManager.TriggerPlayerDie();
        }

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            var matrix = Matrix.Identity;
            matrix.Translation = new Vector3(-1 * (currentCameraPosition.X - LEVEL_SIZE_X / 2), -1 * (currentCameraPosition.Y - LEVEL_SIZE_Y / 2), 0);

            //spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix:matrix);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, transformMatrix: matrix);

            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition, timeNegBound, timePosBound);

            areaEffect.DrawBackground(state, spriteBatch);

            activeLevel.DrawBackground(state, spriteBatch);

            DrawAllConnectedLevelBackgrounds(state, spriteBatch);

            activeLevel.DrawForeground(state, spriteBatch);

            DrawAllConnectedLevelForegrounds(state, spriteBatch);

            player.Draw(state, spriteBatch);

            areaEffect.DrawForeground(state, spriteBatch);

            overlayEffect.Draw(state, spriteBatch);

            timelineGUI.Draw(state, spriteBatch);

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


        public void DoTrigger(string trigger)
        {
            if (trigger == "limbo_pickup")soundManager.BeginLimboMusic2();
            else if (trigger == "cotton_pickup") soundManager.BeginCottonwoodMusic2();
            else if (trigger == "limbo_pickup") soundManager.BeginLimboMusic2();
            else if (trigger == "pianostart")soundManager.BeginPiano();
            else if (trigger == "pianoend")soundManager.EndPiano();
           
            if (trigger == "still")
            {
                timeData.time_kind = TimeKind.none;
                timelineGUI.SetBar(null);
            }
            else if (trigger == "limbo_begin" || trigger == "limbo_full")
            {
                timeData.time_kind = TimeKind.limbo;
                timelineGUI.SetBar(timelineGUI.limboBar1);
                setBarSizeLarge();
            }
            else if (trigger == "limbo_half")
            {
                timeData.time_kind = TimeKind.limbo;
                timelineGUI.SetBar(timelineGUI.limboBarHalf);
                setBarSizeMedium();
            }
            else if (trigger == "limbo_fourth")
            {
                timeData.time_kind = TimeKind.limbo;
                timelineGUI.SetBar(timelineGUI.limboBarFourth);
                setBarSizeSmall();
            }
            else if (trigger == "cotton_begin" || trigger == "cotton_full")
            {
                timeData.time_kind = TimeKind.cottonwood;
                timelineGUI.SetBar(timelineGUI.cottonBar1);
                setBarSizeLarge();
            }
            else if (trigger == "cotton_half")
            {
                timeData.time_kind = TimeKind.cottonwood;
                timelineGUI.SetBar(timelineGUI.cottonBarHalf);
                setBarSizeMedium();
            }
            else if (trigger == "cotton_fourth")
            {
                timeData.time_kind = TimeKind.cottonwood;
                timelineGUI.SetBar(timelineGUI.cottonBarFourth);
                setBarSizeSmall();
            }
            else if (trigger == "eternal_begin" || trigger == "eternal_full")
            {
                timeData.time_kind = TimeKind.eternal;
                timelineGUI.SetBar(timelineGUI.eternalBar1);
                setBarSizeLarge();
            }
            else if (trigger == "eternal_half")
            {
                timeData.time_kind = TimeKind.eternal;
                timelineGUI.SetBar(timelineGUI.eternalBarHalf);
                setBarSizeMedium();
            }
            else if (trigger == "eternal_fourth")
            {
                timeData.time_kind = TimeKind.eternal;
                timelineGUI.SetBar(timelineGUI.eternalBarFourth);
                setBarSizeSmall();
            }
        }

        public void setBarSizeLarge()
        {
            timelineGUI.currentBarSize = 105 * 4;
            timeNegBound = -300;
            timePosBound = 300;
            timeDangerNegBound = -250;
            timeDangerPosBound = 250;
        }

        public void setBarSizeMedium()
        {
            timelineGUI.currentBarSize = 102 * 2;
            timeNegBound = -150;
            timePosBound = 150;
            timeDangerNegBound = -110;
            timeDangerPosBound = 110;
        }

        public void setBarSizeSmall()
        {
            timelineGUI.currentBarSize = 100;
            timeNegBound = -75;
            timePosBound = 75;
            timeDangerNegBound = -25;
            timeDangerPosBound = 25;
        }

        // says if the object can save it's state in this moment-
        // more often means more memory consumption
        public static bool isSavableMoment(int moment)
        {
            return moment % 3 == 0;
        }

    
        
    
    }
}
