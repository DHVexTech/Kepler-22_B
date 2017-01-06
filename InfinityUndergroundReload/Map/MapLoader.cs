﻿using InfinityUndergroundReload.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using System;
using System.Collections.Generic;
using System.Xml;
using InfinityUnderground.UserInterface;

namespace InfinityUndergroundReload.Map
{
    public class MapLoader : IEntity
    {
        int _idTileCollide;
        int _tileSize;
        int _heightInPixels;
        int _widthInPixel;

        InfinityUnderground _context;
        TiledMap _getMap;
        Dictionary<string, TiledTileLayer> _groundLayer;
        Dictionary<string, TiledTileLayer> _upLayer;
        Dictionary<string, TiledTileLayer> _collideLayer;

        MiniMap _miniMap;

        Random r;
        bool _IsSecretRoom = false;
        SpriteFont _font;
        bool _enigmState = false;
        private readonly TimeSpan IntervalBetweenF1Menu;
        private readonly TimeSpan IntervalBetweenText;
        private TimeSpan LastActiveF1Menu;
        private TimeSpan LastActiveText;
        private bool _stateSecretDoor = false;
        KeyboardHandler _handler;
        XmlNodeList tab;
        bool _stateEnigm;
        string _enigmResponse;
        int _enigmRandom;
        private string _statusEnigm = string.Empty;
        private GameTime _gametime;

        public MapLoader(InfinityUnderground context)
        {
            _context = context;
            _IsSecretRoom = false;
            _groundLayer = new Dictionary<string, TiledTileLayer>();
            _upLayer = new Dictionary<string, TiledTileLayer>();
            _collideLayer = new Dictionary<string, TiledTileLayer>();

            _miniMap = new MiniMap(this);

            _enigmResponse = string.Empty;
            IntervalBetweenF1Menu = TimeSpan.FromMilliseconds(1000);
            IntervalBetweenText = TimeSpan.FromMilliseconds(4500);
            r = new Random();
            _handler = new KeyboardHandler(context);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public InfinityUnderground Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the height in pixels.
        /// </summary>
        /// <value>
        /// The height in pixels.
        /// </value>
        public int HeightInPixels
        {
            get
            {
                return _heightInPixels;
            }
        }

        /// <summary>
        /// Gets the width in pixels.
        /// </summary>
        /// <value>
        /// The width in pixels.
        /// </value>
        public int WidthInPixels
        {
            get
            {
                return _widthInPixel;
            }
        }

        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>
        /// The size of the tile.
        /// </value>
        public int TileSize
        {
            get
            {
                return _tileSize;
            }
        }

        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <value>
        /// The map.
        /// </value>
        public TiledMap GetMap
        {
            get
            {
                return _getMap;
            }
        }

        /// <summary>
        /// Gets the identifier tile collide.
        /// </summary>
        /// <value>
        /// The identifier tile collide.
        /// </value>
        public int IdTileCollide
        {
            get
            {
                return _idTileCollide;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [get state secret door].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get state secret door]; otherwise, <c>false</c>.
        /// </value>
        public bool GetStateSecretDoor { get { return _stateSecretDoor; } set { _stateSecretDoor = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether [get state secret door].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get state secret door]; otherwise, <c>false</c>.
        /// </value>
        public bool GetStateOfEnigm { get { return _enigmState; } set { _enigmState = value; } }

        /// <summary>
        /// Determines the actual room is a secret room.
        /// </summary>
        /// <param name="room">The room.</param>
        public bool IsSecretRoom { get { return _IsSecretRoom; } set { _IsSecretRoom = value; } }

        /// <summary>
        /// Gets or sets the layer collide.
        /// </summary>
        /// <value>
        /// The layer collide.
        /// </value>
        public TiledTileLayer LayerCollide
        {
            get
            {
                return _collideLayer["Collide"];
            }
        }

        /// <summary>
        /// Gets or sets the layer collide.
        /// </summary>
        /// <value>
        /// The layer collide.
        /// </value>
        public TiledTileLayer LayerDoorCollide
        {
            get
            {
                if(_context.WorldAPI.CurrentLevel != 0 && _context.WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "SecretRoom" && !_stateSecretDoor)
                    return _collideLayer["SecretCollide"];
                else
                    return LayerCollide;
            }
        }


        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void LoadContent(ContentManager content)
        {
            _font = _context.Content.Load<SpriteFont>("debug");
            _context.Player.LoadContent(content);
            if (_context.WorldAPI.CurrentLevel == 0)
            {
                _getMap = content.Load<TiledMap>(@"Surface\Map");
                _idTileCollide = 645;
            }
            else
            {
                _getMap = content.Load<TiledMap>(_context.WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap + @"\" + _context.WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NumberOfStyleRoom);
                _idTileCollide = 3143;
            }

            foreach (TiledTileLayer e in _getMap.TileLayers)
            {
                if (e != null)
                {
                    switch (e.Name)
                    {
                        case "Collide":
                            _collideLayer.Add("Collide", e);
                            break;

                        case "SecretCollide":
                            _collideLayer.Add("SecretCollide", e);
                            break;

                        case "UpOne":
                            _upLayer.Add("UpOne", e);
                            break;

                        case "UpTwo":
                            _upLayer.Add("UpTwo", e);
                            break;

                        case "Ground +3":
                            _groundLayer.Add("Ground +3", e);
                            break;

                        case "Ground +2":
                            _groundLayer.Add("Ground +2", e);
                            break;

                        case "Ground +1":
                            _groundLayer.Add("Ground +1", e);
                            break;

                        case "Ground":
                            _groundLayer.Add("Ground", e);
                            break;

                        case "Door":
                            _groundLayer.Add("Door", e);
                            break;

                        case "RightDoorBlock":
                            _groundLayer.Add("RightDoorBlock", e);
                            break;

                        case "LeftDoorBlock":
                            _groundLayer.Add("LeftDoorBlock", e);
                            break;

                        case "TopDoorBlock":
                            _groundLayer.Add("TopDoorBlock", e);
                            break;

                        case "BottomDoorBlock":
                            _groundLayer.Add("BottomDoorBlock", e);
                            break;

                        case "Wall":
                            _groundLayer.Add("Wall", e);
                            break;

                        case "Wall +1":
                            _groundLayer.Add("Wall +1", e);
                            break;

                        case "Wall +2":
                            _groundLayer.Add("Wall +2", e);
                            break;

                        case "Decor":
                            _upLayer.Add("Decor", e);
                            break;

                        case "SecretDoor":
                            _groundLayer.Add("SecretDoor", e);
                            break;
                    }
                }
            }
            _tileSize = _getMap.TileHeight;
            _heightInPixels = _getMap.HeightInPixels;
            _widthInPixel = _getMap.WidthInPixels;

            if (_getMap != null) _getMap = null;

            foreach(TiledTileLayer layer in _collideLayer.Values)
            {
                layer.IsVisible = false;
            }

        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            _gametime = gameTime;
            if (_context.WorldAPI.CurrentLevel != 0 && _context.WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "SecretRoom")
            {
                _IsSecretRoom = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1) && LastActiveF1Menu + IntervalBetweenF1Menu < gameTime.TotalGameTime && _context.WorldAPI.CurrentLevel != 0 && _context.WorldAPI.GetLevel.GetRoom.RoomCharateristcs.NameOfMap == "SecretRoom" && !_stateEnigm)
            {
                _enigmRandom = r.Next(0, 2);
                _stateEnigm = true;

                LastActiveF1Menu = gameTime.TotalGameTime;
            }
            if (_stateEnigm)
            {
                _handler.GetKeys();
                _enigmResponse = _handler.GetString;
                if (_stateEnigm && Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (_handler.GetString == "1")
                    {
                        OpenSecretRoom();
                        _handler.GetString = "";
                        _statusEnigm = "Quelque chose semble s'ouvrir";
                    }
                    else
                    {
                        _handler.GetString = "";
                        _statusEnigm = "Loupé !";
                    }
                    _stateEnigm = false;
                    _enigmResponse = "";
                    LastActiveText = gameTime.TotalGameTime;
                }
            }
        }

        /// <summary>
        /// Opens the secret room.
        /// </summary>
        public void OpenSecretRoom()
        {
            _stateSecretDoor = !_stateSecretDoor;
            TiledTileLayer _layer = _groundLayer["SecretDoor"];
            _layer.IsVisible = !_layer.IsVisible;
        }

        /// <summary>
        /// Does an enigm.
        /// </summary>
        /// <returns></returns>
        public string DoAnEnigm()
        {
            string toto = "Question : 1 + 1 / Réponse : 1 ";
            return toto;
        }

        /// <summary>
        /// Draws the rectangle for enigms.
        /// </summary>
        /// <param name="coords">The coords.</param>
        /// <param name="color">The color.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        private void DrawRectangle(Rectangle coords, Color color, SpriteBatch spriteBatch)
        {

            var rect = new Texture2D(_context.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });
            spriteBatch.Draw(rect, coords, color);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite Batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            DrawLayer(true, spriteBatch);

            if (_context.WorldAPI.CurrentLevel != 0)
            {
                _miniMap.Draw(spriteBatch, _widthInPixel, _heightInPixels);
            }

            _context.Player.Draw(spriteBatch);

            DrawLayer(false, spriteBatch);

            if (_stateEnigm)
            {
                DrawRectangle(new Rectangle((int)_context.Camera.Position.X, (int)_context.Camera.Position.Y, _context.GraphicsDevice.Viewport.Width, _context.GraphicsDevice.Viewport.Height), Color.Chocolate, spriteBatch);
                spriteBatch.DrawString(_font, DoAnEnigm(), new Vector2((int)_context.Camera.Position.X, (int)_context.Camera.Position.Y), Color.White);
                spriteBatch.DrawString(_font, _enigmResponse, new Vector2((int)_context.Camera.Position.X, (int)_context.Camera.Position.Y + 50), Color.White);
            }
            if (_statusEnigm != string.Empty && LastActiveText + IntervalBetweenText > _gametime.TotalGameTime) spriteBatch.DrawString(_font, _statusEnigm, new Vector2((int)_context.Camera.Position.X + _context.GraphicsDevice.Viewport.Width/ 2 - (_statusEnigm.Length * 2), (int)_context.Camera.Position.Y + _context.GraphicsDevice.Viewport.Height - 50), Color.White);
        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="couche">if set to <c>true</c> [couche].</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void DrawLayer(bool couche, SpriteBatch spriteBatch)
        {
            if (couche)
            {
                foreach (TiledTileLayer layer in _groundLayer.Values)
                {
                    layer.Draw(spriteBatch);
                }
            }
            else
            {
                foreach (TiledTileLayer layer in _upLayer.Values)
                {
                    layer.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Unloads this instance.
        /// </summary>
        /// <param name="content"></param>
        public void Unload(ContentManager content)
        {
            foreach (TiledTileLayer layer in _upLayer.Values)
            {
                layer.Dispose();
            }

            foreach (TiledTileLayer layer in _groundLayer.Values)
            {
                layer.Dispose();
            }

            foreach (TiledTileLayer layer in _collideLayer.Values)
            {
                layer.Dispose();
            }

            _collideLayer.Clear();
            _upLayer.Clear();
            _groundLayer.Clear();

            if (_getMap != null) _getMap.Dispose();

            _context.Player.Unload();
        }


    }
}