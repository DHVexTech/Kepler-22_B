﻿using InfinityUndergroundReload.API;
using InfinityUndergroundReload.Map;
using InfinityUndergroundReload.CharactersUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Threading;
using InfinityUndergroundReload.API.Characters;
using Microsoft.Xna.Framework.Content;
using InfinityUndergroundReload.Interface;
using Microsoft.Xna.Framework.Media;

namespace InfinityUndergroundReload
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class InfinityUnderground : Game
    {
        const int WindowWidth = 1920;
        const int WindowHeight = 1080;
        float _zoom;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera2D _camera;
        BoxingViewportAdapter _viewportAdapter;

        private readonly TimeSpan IntervalBetweenF11Menu;
        private TimeSpan LastActiveF11Menu;
        World _worldAPI;
        SPlayer _player;
        MapLoader _map;
        Door _door;
        DataSave _dataSave;
        //List<IEntity> _entities;
        List<SpriteSheet> _listOfMonster;
        FightsUI _fights;
        TimeSpan _timeForTakeNextDoor;
        TimeSpan _timeMaxForTakeNextDoor;
        bool _playerCantMove;
        Song _music;
        int _lastLevel;
        string _lastMusic;
        ContentManager _songContent;

        FightsState _fightState;

        SDylan _dylan;
        SAlex _alex;

        /// <summary>
        /// Gets the game time.
        /// </summary>
        /// <value>
        /// The game time.
        /// </value>
        public GameTime GetGameTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether [player cant move].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [player cant move]; otherwise, <c>false</c>.
        /// </value>
        public bool PlayerCantMove
        {
            get
            {
                return _playerCantMove;
            }

            set
            {
                _playerCantMove = value;
            }
        }

        /// <summary>
        /// Gets the content of the song.
        /// </summary>
        /// <value>
        /// The content of the song.
        /// </value>
        public ContentManager SongContent
        {
            get
            {
                return _songContent;
            }
        }

        /// <summary>
        /// Gets the height of the get windows.
        /// </summary>
        /// <value>
        /// The height of the get windows.
        /// </value>
        public int GetWindowsHeight
        {
            get
            {
                return WindowHeight;
            }
        }

        /// <summary>
        /// Gets the width of the get window.
        /// </summary>
        /// <value>
        /// The width of the get window.
        /// </value>
        public int GetWindowWidth
        {
            get
            {
                return WindowWidth;
            }
        }

        /// <summary>
        /// Gets the zoom.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        public float Zoom
        {
            get
            {
                return _zoom;
            }
        }

        /// <summary>
        /// Get the list of Monster.
        /// </summary>
        public List<SpriteSheet> ListOfMonsterUI
        {
            get
            {
                return _listOfMonster;
            }
        }

        /// <summary>
        /// Gets the world API.
        /// </summary>
        /// <value>
        /// The world API.
        /// </value>
        public World WorldAPI
        {
            get
            {
                return _worldAPI;
            }
        }

        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public Camera2D Camera
        {
            get
            {
                return _camera;
            }
        }

        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <value>
        /// The map.
        /// </value>
        public MapLoader Map
        {
            get
            {
                return _map;
            }
        }

        /// <summary>
        /// Gets the dylan.
        /// </summary>
        /// <value>
        /// The dylan.
        /// </value>
        public SDylan Dylan
        {
            get
            {
                return _dylan;
            }
        }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public SPlayer Player
        {
            get
            {
                return _player;
            }
        }

        /// <summary>
        /// Gets the fights.
        /// </summary>
        /// <value>
        /// The fights.
        /// </value>
        public FightsUI Fights
        {
            get
            {
                return _fights;
            }
        }

        /// <summary>
        /// Gets or sets the load or unload fights.
        /// </summary>
        /// <value>
        /// The load or unload fights.
        /// </value>
        public FightsState LoadOrUnloadFights
        {
            get
            {
                return _fightState;
            }

            set
            {
                _fightState = value;
            }
        }

        /// <summary>
        /// Gets the alex.
        /// </summary>
        /// <value>
        /// The alex.
        /// </value>
        public SAlex Alex
        {
            get
            {
                return _alex;
            }
        }

        public InfinityUnderground()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _songContent = new ContentManager(Content.ServiceProvider, Content.RootDirectory);

            graphics.IsFullScreen = true;

            _zoom = 0.1f;
            _dataSave = new DataSave(this);

            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.PreferredBackBufferWidth = WindowWidth;

            IntervalBetweenF11Menu = TimeSpan.FromMilliseconds(1000);
            
            _worldAPI = new World();

            _player = new SPlayer(this, 21, 13);
            _map = new MapLoader(this);
            if (_dataSave.IsExistSave)
            {
                _dataSave.LoadValuesFromTheFile();
                _dataSave.SetValuesInThePlayer();
            }

            _listOfMonster = new List<SpriteSheet>();
            _fights = new FightsUI(this);
            
            _worldAPI.CreateDoor();
 

            _fightState = FightsState.Close;
            _timeMaxForTakeNextDoor = TimeSpan.FromMilliseconds(1000);

            _lastLevel = int.MaxValue;

            _dylan = new SDylan(this, WorldAPI.Dylan);
            _alex = new SAlex(this, WorldAPI.Alex);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, WindowWidth, WindowHeight);
            _camera = new Camera2D(_viewportAdapter);
            _camera.LookAt(new Vector2(_player.PlayerAPI.PositionX + 30, _player.PlayerAPI.PositionY + 40));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            try
            {
                if (LoadOrUnloadFights != FightsState.Close && LoadOrUnloadFights != FightsState.Exit)
                {
                    _music = _songContent.Load<Song>(@"Song\FightsMusic");
                }
                else if (WorldAPI.CurrentLevel == 0)
                {
                    _music = _songContent.Load<Song>(@"Song\Surface");
                }
                else if (_music == null || LoadOrUnloadFights == FightsState.Exit)
                {
                    _music = _songContent.Load<Song>(@"Song\Underground");
                }

                MediaPlayer.Volume = 0.2f;

                if ((_music.Position.TotalMilliseconds <= 1000 && WorldAPI.CurrentLevel != 0) || WorldAPI.CurrentLevel == 0 || _lastMusic != _music.Name/*LoadOrUnloadFights == FightsState.Exit*/)
                {
                    MediaPlayer.Play(_music);
                    _lastMusic = _music.Name;
                }
                MediaPlayer.IsRepeating = true;
            }
            catch
            {}

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            _map.LoadContent(Content);

            if (WorldAPI.CurrentLevel == 0)
            {
                Dylan.LoadContent(Content);
                Alex.LoadContent(Content);
            }

            if (ListOfMonsterUI.Count != 0)
            {
                foreach (SpriteSheet monster in ListOfMonsterUI)
                {
                    monster.LoadContent(Content);
                }
            }

            if (LoadOrUnloadFights == FightsState.Enter || LoadOrUnloadFights == FightsState.InFights) _fights.LoadContent(Content);


            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

            if (_lastLevel != WorldAPI.CurrentLevel && _music != null)
            {
                //MediaPlayer.Stop();
                //_music.Dispose();
                _lastLevel = WorldAPI.CurrentLevel;
                _music = null;
            }

            // TODO: Unload any non ContentManager content here
            _map.Unload(Content);
            spriteBatch.Dispose();

            foreach (SpriteSheet monster in _listOfMonster)
            {
                monster.Unload(Content);
            }

            _fights.Unload(Content);

            _listOfMonster.Clear();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && LastActiveF11Menu + IntervalBetweenF11Menu < gameTime.TotalGameTime)
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                } else { 
                    graphics.PreferredBackBufferWidth = 960;
                    graphics.PreferredBackBufferHeight = 540;
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                }
                graphics.ApplyChanges();
                LastActiveF11Menu = gameTime.TotalGameTime;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _dataSave.LoadValuesOfThePlayerInTheClass();
                _dataSave.WriteValuesInTheFile();
                Exit();
            }
            // TODO: Add your update logic here
            GetGameTime = gameTime;

            _player.Update(gameTime);
            _map.Update(gameTime);

            ActionChangeEnvironment(gameTime);
            _fights.Update(gameTime);

            foreach (SpriteSheet monster in ListOfMonsterUI)
            {
                monster.Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(39, 33, 41));
            
            // TODO: Add your drawing code here

            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            _map.Draw(spriteBatch);

            if (Map.GetStateTransition)
            {
                Map.MonitorTransitionOn(spriteBatch);
                Map.MonitorTransitionOff(spriteBatch);
            }

            _fights.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }



        /// <summary>
        /// Actions the with door UI.
        /// </summary>
        public void ActionChangeEnvironment(GameTime gameTime)
        {
            
            _door = _worldAPI.PlayerTakeDoor();

            if ((_door != null || LoadOrUnloadFights != FightsState.Close) && LoadOrUnloadFights != FightsState.InFights && _timeForTakeNextDoor + _timeMaxForTakeNextDoor < gameTime.TotalGameTime)
            {
                _timeForTakeNextDoor = gameTime.TotalGameTime;
                _playerCantMove = true;

                switch(LoadOrUnloadFights)
                {
                    case FightsState.Close:
                        _worldAPI.ActionWithDoor(_door);
                        _camera.LookAt(_player.PlayerAPI.Position);
                        break;

                    case FightsState.Enter:
                        _camera.LookAt(new Vector2(960, 500));
                        LoadOrUnloadFights = FightsState.InFights;
                        break;

                    case FightsState.Exit:
                        WorldAPI.ExitFights();
                        _camera.LookAt(_player.PlayerAPI.Position);
                        break;
                }



                UnloadContent();
                Thread.Sleep(500);
                _listOfMonster.Clear();


                switch(LoadOrUnloadFights)
                {
                    case FightsState.InFights:
                        switch (_fights.MonsterFights.Monster.TypeOfMonster)
                        {
                            case "Dragon":
                                _listOfMonster.Add(new SDragon(4, 4, this, (CDragon)_fights.MonsterFights.Monster));
                                break;

                            case "Curiosity4":
                                _listOfMonster.Add(new SCuriosity4(4, 3, this, (CCuriosity4)_fights.MonsterFights.Monster));
                                break;

                            case "Angel":
                                _listOfMonster.Add(new SAngel(4, 3, this, (CAngel)_fights.MonsterFights.Monster));
                                break;
                        }
                        break;

                    default:
                        if (WorldAPI.CurrentLevel != 0 && (WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap != "RoomIn" && WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap != "RoomOut"))
                        {
                            CreateMonster();
                        }
                        break;
                }

                LoadContent();
                Thread.Sleep(500);



                if (LoadOrUnloadFights == FightsState.Close)
                {
                    foreach (SpriteSheet monster in ListOfMonsterUI)
                    {
                        if (_worldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "BossRoom")
                        {
                            monster.Monster.Position = new Vector2(800, 500);
                        }
                        else
                        {

                              monster.SetPosition();
                        }
                    }

                    if (WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "SecretRoom")
                    {
                        Map.GetStateSecretDoor = false;
                    }
                }
                else if (LoadOrUnloadFights == FightsState.Exit)
                {
                    LoadOrUnloadFights = FightsState.Close;
                }
            }

        }

        /// <summary>
        /// Creates the monster.
        /// </summary>
        void CreateMonster()
        {
            if (WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "TrapRoom" && WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NumberOfStyleRoom == "2" && WorldAPI.ListOfMonster.ToArray().Length > 0) WorldAPI.ListOfMonster = new List<CNPC>();
            else
                foreach (CNPC monster in WorldAPI.ListOfMonster)
                {
                    switch (monster.TypeOfMonster)
                    {
                        case "Dragon":
                            _listOfMonster.Add(new SDragon(4, 4, this, (CDragon)monster));
                            break;

                        case "Curiosity4":
                            _listOfMonster.Add(new SCuriosity4(4, 3, this, (CCuriosity4)monster));
                            break;

                        case "Angel":
                            _listOfMonster.Add(new SAngel(4, 3, this, (CAngel)monster));
                            break;
                    }
                }
        }

    }

}
