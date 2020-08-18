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
        public OverlayEffects overlayEffect;
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
        public RunState runState = RunState.playing;
        public float stateTimer = -1f;
        public float playerHoverTime = 2;
        public float warpTime = 2;
        public float areaSwapTime = 3;

        public int deathsStat = 0;


        // datas


        public InputData inputData = new InputData();
        public TimeData timeData = new TimeData();

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

                    setBarSizeLarge(AreaState.limbo);

                    break;
                case AreaState.cotton:
                    this.area = AreaState.cotton;
                    soundManager.BeginCottonwoodMusic1();
                    areaEffect = new CottonwoodEffects(this, Services);
                    timeData.time_kind = TimeKind.cottonwood;
                    loadLevelAndConnections("cotton1");

                    setBarSizeLarge(AreaState.cotton);

                    break;
                case AreaState.eternal:
                    this.area = AreaState.eternal;
                    soundManager.BeginEternalMusic1();
                    areaEffect = new EternalEffects(this, Services);
                    timeData.time_kind = TimeKind.eternal;
                    loadLevelAndConnections("eternal1");

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

                level = Level.Make(Services, orgin, this);
                LevelLoader.LoadLevel(raw_level, name_data.name, level);
            }

            return level;
        }


        //Fixes a bug in devtools I should have thought of a while ago
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
        //
        protected override void Update(GameTime game_time)
        {
            currentLevelCenter = getLevelCenter();
            currentCameraPosition = getCameraPosition();

            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            inputData = ReadInputs(inputData);

            if (inputData.is_exit_pressed)
                Exit();

            if (inputData.is_restart_pressed)
                isPlayerDeathQued = true;
            if (true)
            {
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
                else if (_currentKey.IsKeyDown(Keys.NumPad8) &&
                _previousKey.IsKeyUp(Keys.NumPad8))
                {
                    if (activeLevel.connectedLevelNames[2] != "")
                    {
                        loadLevelAndConnections(activeLevel.connectedLevelNames[2]);
                    }
                }
                else if (_currentKey.IsKeyDown(Keys.NumPad6) &&
                _previousKey.IsKeyUp(Keys.NumPad6))
                {
                    if (activeLevel.connectedLevelNames[0] != "")
                    {
                        loadLevelAndConnections(activeLevel.connectedLevelNames[0]);
                    }
                }
                else if (_currentKey.IsKeyDown(Keys.NumPad2) &&
                _previousKey.IsKeyUp(Keys.NumPad2))
                {
                    if (activeLevel.connectedLevelNames[3] != "")
                    {
                        loadLevelAndConnections(activeLevel.connectedLevelNames[3]);
                    }
                }
                else if (_currentKey.IsKeyDown(Keys.NumPad4) &&
                _previousKey.IsKeyUp(Keys.NumPad4))
                {
                    if (activeLevel.connectedLevelNames[1] != "")
                    {
                        loadLevelAndConnections(activeLevel.connectedLevelNames[1]);
                    }
                }
            }
            StateData state = new StateData(inputData, timeData, game_time, currentLevelCenter, currentCameraPosition, timeBound);


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
                            player.hidden = true;
                            //activeLevel.specialObject.hidden = true;
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
                        isPlayerDeathQued = false;
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
                input_data.horizontal_axis_value = -GameUtils.MOVE_STICK_MAX;
            else if (is_any_right_button_down)
                input_data.horizontal_axis_value = GameUtils.MOVE_STICK_MAX;
            else
                input_data.horizontal_axis_value = gamepad_state.ThumbSticks.Left.X * GameUtils.MOVE_STICK_SCALE;



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
            stateTimer = GameUtils.PLAYER_DEATH_TIME;
            timeData.time_status = TimeState.still;

            overlayEffect.TriggerDeath();

            soundManager.TriggerPlayerDie();
        }

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.DarkGray);

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
            return new Vector2(12 + activeLevelOffset.X + GameUtils.LEVEL_SIZE_X /2, activeLevelOffset.Y + GameUtils.LEVEL_GRID_SIZE_Y /2);
        }

        // gets the position of the center of the camera
        public Vector2 getCameraPosition()
        {
            float edge_left = 12 + activeLevelOffset.X + GameUtils.LEVEL_SIZE_X / 2;
            float edge_right = 12 + activeLevelOffset.X + (GameUtils.LEVEL_SIZE_X * (activeLevel.screensHorizontal - 0.5f));
            float edge_down = 12 + activeLevelOffset.Y + GameUtils.LEVEL_SIZE_Y / 2;
            float edge_up = 12 + activeLevelOffset.Y + (GameUtils.LEVEL_SIZE_Y * (activeLevel.screensVertical - 0.5f));
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
            timelineGUI.currentBarSize = 105 * 4;
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
            timelineGUI.currentBarSize = 102 * 2;
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
            timelineGUI.currentBarSize = 100;
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
            return moment % 3 == 0;
        }

    
        
    
    }
}
