using ChaiFoxes.FMODAudio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RewindGame.Game;
using RewindGame.Game.ExternalUtills;
using RewindGame.Game.Effects;
using RewindGame.Game.Solids;
using RewindGame.Game.Sound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data.Common;

namespace RewindGame
{


    public class RewindGame : Microsoft.Xna.Framework.Game
    {
        // rendering stuff

        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<Texture2D> textures = new List<Texture2D>();

        public Texture2D decorativeSheetTexture;
        public Texture2D collisionSheetTexture;


        // children

        public ILevelEffect areaEffect;
        public GlobalEffects overlayEffect;
        public SoundManager soundManager;
        public TimelineBarGUI timelineGUI;

        public PlayerEntity player;

        // connected levels are loaded, but not updated actively
        // 1: right
        // 2: left
        // 3: up
        // 4: down
        private Level[] connectedLevels = { null, null, null, null };


        // state

        public Interval timeBound;
        public Interval timeDangerBound;

        public Level activeLevel;
        public Vector2 activeLevelOffset = Vector2.Zero;
        public Vector2 currentLevelCenter;
        public Vector2 currentCameraPosition;

        public String quedLevelLoadName = "";
        public bool isPlayerDeathQued = false;

        public AreaState area = AreaState.none;
        public ContentManager areaContent;


        public RunState runState = RunState.playing;
        public float stateTimer = -1f;
        public float playerHoverTime = 2;
        public float warpTime = 2;
        public float areaSwapTime = 3;

        public int deathsStat = 0;


        // datas


        public InputData inputData = new InputData();
        public TimeData timeData = new TimeData();


        // Initialization

        public RewindGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            IsFixedTimeStep = true;
            // run at a fixed timestep for 60 fps
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);

            graphics.PreferredBackBufferWidth = (int)GameUtils.BASE_SCREEN_SIZE.X;
            graphics.PreferredBackBufferHeight = (int)GameUtils.BASE_SCREEN_SIZE.Y;

            graphics.ApplyChanges();


            ProgressStore.Init(); // Creates progress store file if none exists

        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }
        private bool HasReadFromStored = false;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // this is useless!
            //textures.Add(Content.Load<Texture2D>("debug/square"));
            //textures.Add(Content.Load<Texture2D>("debug/platform"));


            decorativeSheetTexture = Content.Load<Texture2D>("tilesets/decorative");
            collisionSheetTexture = Content.Load<Texture2D>("tilesets/collision");


            overlayEffect = new GlobalEffects(this);
            soundManager = new SoundManager(this);
            timelineGUI = new TimelineBarGUI(this);

            if (!HasReadFromStored && !GameUtils.OVERRIDE_LEVEL_LOAD)
            {
                if (new FileInfo(ProgressStore.path).Exists && ProgressStore.DoesFileContainLevel())
                {
                    string current = ProgressStore.readSavedLevel();
                    if (current.Contains("limbo"))
                        LoadArea(AreaState.limbo);
                    else if (current.Contains("cotton"))
                        LoadArea(AreaState.cotton);
                    else if (current.Contains("eternal"))
                        LoadArea(AreaState.eternal);
                    else
                    {
                        //current = "limbo1";
                        LoadArea(AreaState.limbo);
                    }
                        
                    loadLevelAndConnections(current);
                }
                else
                    LoadArea(AreaState.limbo);
                HasReadFromStored = true;
            }
            else
                LoadArea(AreaState.limbo);
                HasReadFromStored = true;

            player = new PlayerEntity(this, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 - 300));
            player.position = activeLevel.playerSpawnpoint;
            //DoTrigger("limbo_fourth");
        }


        // Loading Stuff

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

            // we dispose all assets loaded specifically for the current area and start anew
            if (areaEffect != null) areaEffect.Dispose();
            if (areaContent != null) areaContent.Dispose();

            areaContent = new ContentManager(Services, "Content");

            switch (new_area)
            {
                case AreaState.limbo:
                    this.area = AreaState.limbo;

                    soundManager.BeginLimboMusic1();
                    areaEffect = new LimboEffects(this);
                    timeData.time_kind = TimeKind.limbo;
                    loadLevelAndConnections("start");

                    timelineGUI.SetBar(timelineGUI.limboBar1);
                    timelineGUI.currentBarSize = GameUtils.TIMELINE_BAR_SIZE_FULL;

                    setBarSizeLarge(AreaState.limbo);

                    break;
                case AreaState.cotton:
                    this.area = AreaState.cotton;
                    soundManager.BeginCottonwoodMusic1();
                    areaEffect = new CottonwoodEffects(this);
                    timeData.time_kind = TimeKind.cottonwood;
                    loadLevelAndConnections("start");

                    setBarSizeLarge(AreaState.cotton);

                    break;
                case AreaState.eternal:
                    this.area = AreaState.eternal;
                    soundManager.BeginEternalMusic1();
                    areaEffect = new EternalEffects(this);
                    timeData.time_kind = TimeKind.eternal;
                    loadLevelAndConnections("start");

                    setBarSizeLarge(AreaState.eternal);

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

            // todo do we need to do some special level unloading method?
            /*
            foreach (Level lvl in connectedLevels)
            {
                if (lvl != null)
                {
                    if (lvl.name != center_level.name)
                    {
                        lvl.Dispose();
                    }
                }
            }*/

            if (activeLevel != null) { center_level.RunStartTriggers(); }

            connectedLevels = new_connected_levels;
            activeLevel = center_level;
            center_level.isActiveScene = true;
            center_level.SetActive();
            if (!GameUtils.OVERRIDE_LEVEL_SAVE)
            {
                var data = new ProgressStore.ProgressData 
                { 
                    Level = activeLevel.name,
                };
                ProgressStore.StoreLevel(data);
            }

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

                    float new_horiz = (float)raw_level.layers[0].gridCellsX / (float)GameUtils.LEVEL_GRID_SIZE_X;
                    float new_vert = (float)raw_level.layers[0].gridCellsY / (float)GameUtils.LEVEL_GRID_SIZE_Y;

                    if (is_horizontal)
                    {

                        if (is_forwards) 
                        {
                            orgin.X += GameUtils.LEVEL_SIZE_X * current_level.screensHorizontal;
                        }
                        else
                        {
                            orgin.X -= GameUtils.LEVEL_SIZE_X * new_horiz;
                        }
                        
                        if (name_data.isUpward())
                        {
                            orgin.Y -= (GameUtils.LEVEL_SIZE_Y * (new_vert - 1));
                        }
                        else if (name_data.isDownward()) orgin.Y += (GameUtils.LEVEL_SIZE_Y * (current_level.screensVertical - 1));
                    }
                    else
                    {
                        if (is_forwards)
                        {
                            orgin.Y += GameUtils.LEVEL_SIZE_Y * current_level.screensVertical;
                        }
                        else
                        {
                            orgin.Y -= GameUtils.LEVEL_SIZE_Y * new_vert;
                        }

                        if (name_data.isUpward())
                        {
                            orgin.X -= (GameUtils.LEVEL_SIZE_X * (new_horiz - 1));
                        }
                        else if (name_data.isDownward()) orgin.X += (GameUtils.LEVEL_SIZE_X * (current_level.screensHorizontal - 1));
                    }

                }

                level = Level.Make(orgin, this);
                LevelLoader.LoadLevel(raw_level, name_data.name, level);
            }

            return level;
        }


        // Update Stuff

        private static InputData ReadInputs(InputData previous_input_data)
        {
            var input_data = new InputData();

            var keyboard_state = Keyboard.GetState();
            var gamepad_state = GamePad.GetState(PlayerIndex.One);

            if (!GlobalEffects._showCube)
            {
                var is_any_left_button_down = gamepad_state.IsButtonDown(Buttons.DPadLeft) || keyboard_state.IsKeyDown(Keys.A) || keyboard_state.IsKeyDown(Keys.Left);

                var is_any_right_button_down = gamepad_state.IsButtonDown(Buttons.DPadRight) || keyboard_state.IsKeyDown(Keys.D) || keyboard_state.IsKeyDown(Keys.Right);

                if (is_any_left_button_down && is_any_right_button_down)
                    input_data.horizontal_axis_value = 0;
                else if (is_any_left_button_down)
                    input_data.horizontal_axis_value = -GameUtils.MOVE_STICK_MAX;
                else if (is_any_right_button_down)
                    input_data.horizontal_axis_value = GameUtils.MOVE_STICK_MAX;
                else
                    input_data.horizontal_axis_value = gamepad_state.ThumbSticks.Left.X * GameUtils.MOVE_STICK_SCALE;



                var is_any_jump_button_down = gamepad_state.Buttons.A == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.Z) || keyboard_state.IsKeyDown(Keys.Space);

                if (!(previous_input_data.is_jump_pressed || previous_input_data.is_jump_held) && is_any_jump_button_down)
                    input_data.is_jump_pressed = true;
                else if (is_any_jump_button_down)
                    input_data.is_jump_held = true;

            }
            var is_level_left_down = keyboard_state.IsKeyDown(Keys.NumPad4);
            var is_level_right_down = keyboard_state.IsKeyDown(Keys.NumPad6);
            var is_level_up_down = keyboard_state.IsKeyDown(Keys.NumPad8);
            var is_level_down_down = keyboard_state.IsKeyDown(Keys.NumPad2);

            if (!(previous_input_data.is_level_down || previous_input_data.is_devkey_down[3]) && is_level_down_down)
                    input_data.is_level_down = true;
                else if (is_level_down_down)
                    input_data.is_devkey_down[3] = true;

            if (!(previous_input_data.is_level_up || previous_input_data.is_devkey_down[2]) && is_level_up_down)
                    input_data.is_level_up = true;
                else if (is_level_up_down)
                    input_data.is_devkey_down[2] = true;

            if (!(previous_input_data.is_level_left || previous_input_data.is_devkey_down[0]) && is_level_left_down)
                    input_data.is_level_left = true;
                else if (is_level_left_down)
                    input_data.is_devkey_down[0] = true;

            if (!(previous_input_data.is_level_right || previous_input_data.is_devkey_down[1]) && is_level_right_down)
                    input_data.is_level_right = true;
                else if (is_level_right_down)
                    input_data.is_devkey_down[1] = true;



            var is_limbo1_down_here = keyboard_state.IsKeyDown(Keys.Insert);
            var is_limboHalf_down_here = keyboard_state.IsKeyDown(Keys.Delete);
            var is_cotton1_down_here = keyboard_state.IsKeyDown(Keys.Home);
            var is_cottonHalf_down_here = keyboard_state.IsKeyDown(Keys.End);
            var is_eternal1_down_here = keyboard_state.IsKeyDown(Keys.PageUp);
            var is_eternalHalf_down_here = keyboard_state.IsKeyDown(Keys.PageDown);

            if (!(previous_input_data.is_limbo1_down || previous_input_data.is_devkey_down[4]) && is_limbo1_down_here)
                input_data.is_limbo1_down = true;
            else if (is_limbo1_down_here)
                input_data.is_devkey_down[4] = true;

            if (!(previous_input_data.is_limboHalf_down || previous_input_data.is_devkey_down[7]) && is_limboHalf_down_here)
                input_data.is_limboHalf_down = true;
            else if (is_limboHalf_down_here)
                input_data.is_devkey_down[7] = true;

            if (!(previous_input_data.is_cotton1_down || previous_input_data.is_devkey_down[5]) && is_cotton1_down_here)
                input_data.is_cotton1_down = true;
            else if (is_cotton1_down_here)
                input_data.is_devkey_down[5] = true;

            if (!(previous_input_data.is_cottonHalf_down || previous_input_data.is_devkey_down[8]) && is_cottonHalf_down_here)
                input_data.is_cottonHalf_down = true;
            else if (is_cottonHalf_down_here)
                input_data.is_devkey_down[8] = true;

            if (!(previous_input_data.is_eternal1_down || previous_input_data.is_devkey_down[6]) && is_eternal1_down_here)
                input_data.is_eternal1_down = true;
            else if (is_eternal1_down_here)
                input_data.is_devkey_down[6] = true;

            if (!(previous_input_data.is_eternalHalf_down || previous_input_data.is_devkey_down[9]) && is_eternalHalf_down_here)
                input_data.is_eternalHalf_down = true;
            else if (is_eternalHalf_down_here)
                input_data.is_devkey_down[9] = true;



            var is_level_select_here = keyboard_state.IsKeyDown(Keys.NumPad5);

            if (!(previous_input_data.is_level_select || previous_input_data.is_devkey_down[10]) && is_level_select_here)
                input_data.is_level_select = true;
            else if (is_level_select_here)
                input_data.is_devkey_down[10] = true;


            var is_level_reload_down = keyboard_state.IsKeyDown(Keys.Back);

            if (!(previous_input_data.is_level_reload || previous_input_data.is_devkey_down[11]) && is_level_reload_down)
                input_data.is_level_reload = true;
            else if (is_level_reload_down)
                input_data.is_devkey_down[11] = true;



            // force exit
            if (gamepad_state.Buttons.Back == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.Escape))
                input_data.is_exit_pressed = true;

            if (gamepad_state.Buttons.Y == ButtonState.Pressed || keyboard_state.IsKeyDown(Keys.R))
                input_data.is_restart_pressed = true;

            // todo interact, restart bindings

            return input_data;
        }

        protected override void Update(GameTime game_time)
        {
            // setting up state
            currentLevelCenter = getLevelCenter();
            currentCameraPosition = getCameraPosition();

            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed) Exit();
            else if (inputData.is_restart_pressed) isPlayerDeathQued = true;
            else if (GameUtils.USE_DEBUG_KEYS) CheckDebugKeys(inputData);


            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition, timeBound);

            // state timer ticking and warp stuff
            // should this have a method of it's own?
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
                            isPlayerDeathQued = false;
                            break;

                        case RunState.areaswap_1:
                            activeLevel.warp.TriggerActivation();
                            overlayEffect.StartWarpPlayer(player.getCenterPosition(), state);
                            //overlayEffect.StartWarpArtifact(activeLevel.specialObject.getCenterPosition(), state);

                            soundManager.TriggerWarp();

                            stateTimer = warpTime;
                            runState = RunState.areaswap_2;
                            break;

                        case RunState.areaswap_2:
                            player.isHidden = true;
                            //activeLevel.specialObject.hidden = true;
                            activeLevel.warp.isHidden = true;

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
                            player.isHidden = false;
                            break;

                    }
                    if (runState == RunState.playerdead)
                    {
                        player.position = activeLevel.playerSpawnpoint;
                        runState = RunState.playing;

                        timeData.Reset();
                        activeLevel.Reset();

                        stateTimer = -1;
                        isPlayerDeathQued = false;
                    }
                }
            }


            switch (runState)
            {
                case RunState.playing:
                    TickUpdate(state);
                    break;
                case RunState.areaswap_1:
                    // player ascend speed once warp is reached
                    player.position.Y -= 16*state.getDeltaTime();
                    break;
            }

            overlayEffect.Update(state);
            soundManager.Update(state);
            timelineGUI.Update(state);

            base.Update( game_time);
        }

        // Updates that only occur during normal gameplay- so not paused, not during a cutscene
        protected void TickUpdate(StateData state)
        {
            // special text collision check (is this wonky?)
            if (activeLevel.getPlayerIsInSpecial(player.getCollisionBox()))
            {
                if (activeLevel.specialObject.charState == -1) activeLevel.specialObject.charState = 0;
            }
            else if (activeLevel.specialObject != null)
            {
                activeLevel.specialObject.Reset();
            }

            //warp collision check
            if (activeLevel.getPlayerIsInWarp(player.getCollisionBox()))
            {
                // begin warp!
                runState = RunState.areaswap_1;
                stateTimer = playerHoverTime;
                soundManager.stopAllMusic();
                return;
            }

            if (activeLevel.getPlayerIsInStasis(player.getCollisionBox()))
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
            else if (player.justChangedRewinding)
            {
                timeData.time_status = TimeState.still;
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
            if (timeData.time_moment <= timeBound.min)
            {
                switch (timeData.time_kind)
                {
                    case TimeKind.limbo:
                        isPlayerDeathQued = true;
                        soundManager.TriggerLightining();
                        break;
                    case TimeKind.cottonwood:
                        if (timeData.time_status == TimeState.backward) timeData.time_status = TimeState.still;
                        break;
                    case TimeKind.eternal:
                        timeData.time_moment = timeBound.max - 1;
                        break;
                }
            }
            else if (timeData.time_moment >= timeBound.max)
            {
                switch (timeData.time_kind)
                {
                    case TimeKind.limbo:
                        isPlayerDeathQued = true;
                        soundManager.TriggerLightining();
                        break;
                    case TimeKind.cottonwood:
                        if (timeData.time_status == TimeState.forward) timeData.time_status = TimeState.still;
                        break;
                    case TimeKind.eternal:
                        timeData.time_moment = timeBound.min + 1;
                        break;
                }
            }

            // what happens if the player goes out of bounds
            if (player.position.Y > activeLevelOffset.Y + GameUtils.LEVEL_SIZE_Y * activeLevel.screensVertical + GameUtils.TILE_WORLD_SIZE * 2
                || player.position.Y < activeLevelOffset.Y - GameUtils.TILE_WORLD_SIZE)
            {
                isPlayerDeathQued = true;
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

            if (quedLevelLoadName != "")
            {
                loadLevelAndConnections(quedLevelLoadName);
                quedLevelLoadName = "";
                timeData.Reset();
            }
            else if (isPlayerDeathQued && runState != RunState.playerdead)
            {
                KillPlayer();
                isPlayerDeathQued = false;
            }
        }
        bool hello = false;
        private void CheckDebugKeys(InputData inputs)
        {
            // This needs to be redone- 
            // If a new level is loaded, always kill the player with isPlayerDeathQued = true;
            // All debug keys need to first be defined as bools in InputData, read in ReadInputs, then have functionality defined here
            // (you can define keys that should only be triggered once per press much easier in ReadInputs since we pass in the previous input data, see jump_pressed
            if (!hello)
            {
                Console.WriteLine("Developer Tools are running! (do not close this window)"); //ok so basically right click the csproj click properties and under application you can change that back to a windows application to do things like build for release, im just doing it like this cause it is less jank
                hello = true;
            }

            if (inputs.is_limbo1_down)
            {
                LoadArea(AreaState.limbo);
                isPlayerDeathQued = true;
            }
            else if (inputs.is_cotton1_down)
            {
                LoadArea(AreaState.cotton);
                isPlayerDeathQued = true;
            }

            else if (inputs.is_eternal1_down)
            {
                LoadArea(AreaState.eternal);
                isPlayerDeathQued = true;
            }
            else if (inputs.is_limboHalf_down)
            {
                LoadArea(AreaState.limbo);
                loadLevelAndConnections("limbo12");
                isPlayerDeathQued = true;
            }
            else if (inputs.is_cottonHalf_down)
            {
                LoadArea(AreaState.cotton);
                loadLevelAndConnections("cotton3");
                isPlayerDeathQued = true;
            }
            else if (inputs.is_eternalHalf_down)
            {
                //todo: get to the point where there is a 50% point for eternal
                DevkeyUtills.ShowEternalUnimplimentedBox();

            }

            else if (inputs.is_level_up)
            {
                if (activeLevel.connectedLevelNames[2] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[2]);
                    isPlayerDeathQued = true;
                }
                else
                {
                    DevkeyUtills.NoConnectedLevel("Up");
                }
            }
            else if (inputs.is_level_right)
            {
                if (activeLevel.connectedLevelNames[0] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[0]);
                    isPlayerDeathQued = true;
                }
                else
                {
                    DevkeyUtills.NoConnectedLevel("Right");
                }
            }
            else if (inputs.is_level_down)
            {
                if (activeLevel.connectedLevelNames[3] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[3]);
                    isPlayerDeathQued = true;
                }
                else
                {
                    DevkeyUtills.NoConnectedLevel("Down");
                }
            }
            else if (inputs.is_level_left)
            {
                if (activeLevel.connectedLevelNames[1] != "")
                {
                    loadLevelAndConnections(activeLevel.connectedLevelNames[1]);
                    isPlayerDeathQued = true;
                }
                else
                {
                    DevkeyUtills.NoConnectedLevel("Left");
                }
            }
            else if(inputs.is_level_select)
            {
                string userout = DevkeyUtills.GetLevel();
                if (userout.Contains("limbo"))
                {
                    LoadArea(AreaState.limbo);
                    loadLevelAndConnections(userout);
                    isPlayerDeathQued = true;
                }
                else if (userout.Contains("cotton"))
                {
                    LoadArea(AreaState.cotton);
                    loadLevelAndConnections(userout);
                    isPlayerDeathQued = true;
                }
                else if (userout.Contains("eternal"))
                {
                    LoadArea(AreaState.eternal);
                    loadLevelAndConnections(userout);
                    isPlayerDeathQued = true;
                }
            }
            else if(inputs.is_level_reload)
            {
                //Console.WriteLine(ProgressStore.readSavedLevel());
                loadLevelAndConnections(activeLevel.name);
                isPlayerDeathQued = true;
            }
        }
        
        public void KillPlayer()
        {
            deathsStat += 1;
            runState = RunState.playerdead;
            stateTimer = GameUtils.PLAYER_DEATH_TIME;
            timeData.time_status = TimeState.still;

            overlayEffect.TriggerDeath();

            soundManager.TriggerPlayerDie();
        }

        
        // Graphics

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // camera position
            var matrix = Matrix.Identity;
            matrix.Translation = new Vector3(-1 * (currentCameraPosition.X - GameUtils.LEVEL_SIZE_X / 2), -1 * (currentCameraPosition.Y - GameUtils.LEVEL_SIZE_Y / 2), 0);

            //spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix:matrix);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, transformMatrix: matrix);

            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition, timeBound);

            areaEffect.DrawBackground(state, spriteBatch);

            activeLevel.DrawBackground(state, spriteBatch);

            DrawAllConnectedLevelBackgrounds(state, spriteBatch);

            activeLevel.DrawForeground(state, spriteBatch);

            DrawAllConnectedLevelForegrounds(state, spriteBatch);

            player.Draw(state, spriteBatch);

            areaEffect.DrawForeground(state, spriteBatch);

            overlayEffect.Draw(state, spriteBatch);

            timelineGUI.Draw(state, spriteBatch);

            if (activeLevel.specialObject != null) activeLevel.specialObject.DrawText(state, spriteBatch);

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


        // Helper Methods

        public Vector2 getLevelCenter()
        {
            return new Vector2(12 + activeLevelOffset.X + GameUtils.LEVEL_SIZE_X /2, activeLevelOffset.Y + GameUtils.LEVEL_GRID_SIZE_Y /2);
        }

        // gets the position of the center of the camera
        public Vector2 getCameraPosition()
        {
            float edge_left = 12 + activeLevelOffset.X + GameUtils.LEVEL_SIZE_X / 2;
            float edge_right = 12 + activeLevelOffset.X + (GameUtils.LEVEL_SIZE_X * (activeLevel.screensHorizontal - 0.5f));
            float edge_down = 12 + activeLevelOffset.Y + GameUtils.LEVEL_SIZE_Y / 2;
            float edge_up = 12 + activeLevelOffset.Y + (GameUtils.LEVEL_SIZE_Y * (activeLevel.screensVertical - 0.5f));
            return new Vector2(Math.Clamp(player.position.X, edge_left, edge_right), Math.Clamp(player.position.Y + GameUtils.CAMERA_FOCUS_Y_OFFSET, edge_down, edge_up));
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
                // this should probably be in it's own function
                timeData.time_kind = TimeKind.none;
                timelineGUI.SetBar(null);
            }
            else if (trigger == "limbo_begin" || trigger == "limbo_full")
            {
                setBarSizeLarge(AreaState.limbo);
            }
            else if (trigger == "limbo_half")
            {
                setBarSizeMedium(AreaState.limbo);
            }
            else if (trigger == "limbo_fourth")
            {
                setBarSizeSmall(AreaState.limbo);
            }
            else if (trigger == "cotton_begin" || trigger == "cotton_full")
            {
                setBarSizeLarge(AreaState.cotton);
            }
            else if (trigger == "cotton_half")
            {
                setBarSizeMedium(AreaState.cotton);
            }
            else if (trigger == "cotton_fourth")
            {
                setBarSizeSmall(AreaState.cotton);
            }
            else if (trigger == "eternal_begin" || trigger == "eternal_full")
            {
                setBarSizeLarge(AreaState.eternal);
            }
            else if (trigger == "eternal_half")
            {
                setBarSizeMedium(AreaState.eternal);
            }
            else if (trigger == "eternal_fourth")
            {
                setBarSizeSmall(AreaState.eternal);
            }
        }

        public void setBarSizeLarge(AreaState this_area)
        {
            timelineGUI.currentBarSize = GameUtils.TIMELINE_BAR_SIZE_FULL;
            timeBound = GameUtils.FULL_TIME_BOUND;
            timeDangerBound = GameUtils.FULL_DANGER_BOUND;
            
            switch(this_area)
            {
                case AreaState.limbo:
                    timeData.time_kind = TimeKind.limbo;
                    timelineGUI.SetBar(timelineGUI.limboBar1);
                    return;
                case AreaState.cotton:
                    timeData.time_kind = TimeKind.cottonwood;
                    timelineGUI.SetBar(timelineGUI.cottonBar1);
                    return;
                case AreaState.eternal:
                    timeData.time_kind = TimeKind.eternal;
                    timelineGUI.SetBar(timelineGUI.eternalBar1);
                    return;
            }
        }

        public void setBarSizeMedium(AreaState this_area)
        {
            timelineGUI.currentBarSize = GameUtils.TIMELINE_BAR_SIZE_HALF;
            timeBound = GameUtils.HALF_TIME_BOUND;
            timeDangerBound = GameUtils.HALF_DANGER_BOUND;

            switch (this_area)
            {
                case AreaState.limbo:
                    timeData.time_kind = TimeKind.limbo;
                    timelineGUI.SetBar(timelineGUI.limboBarHalf);
                    return;
                case AreaState.cotton:
                    timeData.time_kind = TimeKind.cottonwood;
                    timelineGUI.SetBar(timelineGUI.cottonBarHalf);
                    return;
                case AreaState.eternal:
                    timeData.time_kind = TimeKind.eternal;
                    timelineGUI.SetBar(timelineGUI.eternalBarHalf);
                    return;
            }
        }

        public void setBarSizeSmall(AreaState this_area)
        {
            timelineGUI.currentBarSize = GameUtils.TIMELINE_BAR_SIZE_FOURTH;
            timeBound = GameUtils.FOURTH_TIME_BOUND;
            timeDangerBound = GameUtils.FOURTH_DANGER_BOUND;

            switch (this_area)
            {
                case AreaState.limbo:
                    timeData.time_kind = TimeKind.limbo;
                    timelineGUI.SetBar(timelineGUI.limboBarFourth);
                    return;
                case AreaState.cotton:
                    timeData.time_kind = TimeKind.cottonwood;
                    timelineGUI.SetBar(timelineGUI.cottonBarFourth);
                    return;
                case AreaState.eternal:
                    timeData.time_kind = TimeKind.eternal;
                    timelineGUI.SetBar(timelineGUI.eternalBarFourth);
                    return;
            }
        }

        // says if the object can save it's state in this moment-
        // more often means more memory consumption
        public static bool isSavableMoment(int moment)
        {
            return true;
        }

    
        
    
    }
}
