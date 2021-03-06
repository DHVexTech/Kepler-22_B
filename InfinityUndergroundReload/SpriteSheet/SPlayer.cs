﻿using InfinityUndergroundReload.API.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Maps.Tiled;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityUndergroundReload.SpriteSheet
{
    public class SPlayer : SpriteSheet
    {
        KeyboardState _state;
        CPlayer _player;
        int _lastMoveSpeed;
        int _widthHealthBar;
        bool _isAttacking;
        bool _sprint;
        List<ActionSpriteSheet> _playerAction;
        ActionSpriteSheet _actualAction;
        ActionSpriteSheet _lastAction;
        IEnumerable<ActionSpriteSheet> _action;
        Vector2 _lastPosition;
        LifePoint _healthBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPlayer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="spriteSheetRows">The sprite sheet rows.</param>
        /// <param name="spriteSheetColumns">The sprite sheet columns.</param>
        public SPlayer(InfinityUnderground context, int spriteSheetRows, int spriteSheetColumns)
        {
            Context = context;

            _player = Context.WorldAPI.Player;

            SpriteSheetColumns = spriteSheetColumns;
            SpriteSheetRows = spriteSheetRows;

            TotalFrames = SpriteSheetRows * SpriteSheetColumns;

            _lastMoveSpeed = _player.CharacterType.MoveSpeed;

            _playerAction = new List<ActionSpriteSheet>();

            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.WalkTop, 9));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.WalkLeft, 9));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.WalkBottom, 9));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.WalkRight, 9));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.AttackTop, 6));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.AttackLeft, 6));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.AttackBottom, 6));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.AttackRight, 6));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.TakeDamageTop, 7));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.TakeDamageLeft, 7));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.TakeDamageBottom, 7));
            _playerAction.Add(new ActionSpriteSheet((int)RowActionOnSpriteSheetPlayer.TakeDamageRight, 7));

            var _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.WalkBottom select action;
            foreach (ActionSpriteSheet action in _action)
            {
                _actualAction = action;
            }
            _lastAction = _actualAction;

            _widthHealthBar = 100;
            _healthBar = new LifePoint(_widthHealthBar, 25);
        }

        /// <summary>
        /// Gets the index of the player.
        /// </summary>
        /// <value>
        /// The index of the player.
        /// </value>
        public CPlayer PlayerAPI
        {
            get
            {
                return _player;
            }
        }


        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void LoadContent(ContentManager content)
        {
            Spritesheet = content.Load<Texture2D>("Player/Player");

            Width = Spritesheet.Width / SpriteSheetColumns;
            Height = Spritesheet.Height / SpriteSheetRows;
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            _actualAction = PlayerAction(_state);

            TimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds; 
            if (TimeSinceLastFrame > MillisecondsPerFrame) 
            { 
                TimeSinceLastFrame -= MillisecondsPerFrame; 
 
                CurrentFrame++; 
 
                TimeSinceLastFrame = 0; 
                if (CurrentFrame == TotalFrames) 
                { 
                    CurrentFrame = 0; 
                } 
            } 
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
           _state = Keyboard.GetState(); 

            if ((_player.Position == _lastPosition) && !_isAttacking)
            {
                Column = 0;
            } 
            else
            {
                Column = CurrentFrame % _lastAction.Column;
            }

            if (_isAttacking && Column == (_actualAction.Column-1))
            {
                _isAttacking = false;
                MillisecondsPerFrame = 80;
            }
            _lastPosition = _player.Position;
            

            Rectangle _sourceRectangle = new Rectangle(Width * Column, Height * _actualAction.RowAction, Width, Height); 
            Rectangle _destinationRectangle = new Rectangle(_player.PositionX, _player.PositionY, Width, Height); 
 
            spriteBatch.Draw(Spritesheet, _destinationRectangle, _sourceRectangle, Color.White);

            _healthBar.Draw(spriteBatch, (int)(Context.Camera.Position.X + 20), (int)(Context.Camera.Position.Y + 20), _player.CharacterType.LifePoint, Context.GraphicsDevice, (_widthHealthBar * _player.CharacterType.LifePoint / 500), 10);
        }

        /// <summary>
        /// Select the action forl the player.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>return the action</returns>
        ActionSpriteSheet PlayerAction(KeyboardState state)
        {
            _action = null;

            if (_state.IsKeyDown(Keys.PageUp))
            {
                Context.Camera.ZoomIn(Context.Zoom);
            }
            if (_state.IsKeyDown(Keys.PageDown))
            {
                Context.Camera.ZoomOut(Context.Zoom);
            }

            if (state.IsKeyDown(Keys.LeftShift) && !_sprint)
            {
                _player.CharacterType.MoveSpeed *= 4;
                MillisecondsPerFrame /= 4;
                _sprint = true;
            }
            else if (state.IsKeyUp(Keys.LeftShift) && _sprint)
            {
                _player.CharacterType.MoveSpeed /= 4;
                MillisecondsPerFrame *= 4;
                _sprint = false;
            }


            if (state.IsKeyDown(Keys.Z) || state.IsKeyDown(Keys.Up))
            {
                if (_player.PositionY >= 0 && (Context.Map.LayerCollide.GetTile((int)Math.Floor(_player.PositionX / (decimal)Context.Map.TileSize) + 1, (int)Math.Round((decimal)_player.PositionY / (decimal)Context.Map.TileSize)).Id != Context.Map.IdTileCollide) && (Context.Map.LayerDoorCollide.GetTile((int)Math.Floor(_player.PositionX / (decimal)Context.Map.TileSize) + 1, (int)Math.Round((decimal)_player.PositionY / (decimal)Context.Map.TileSize)).Id != Context.Map.IdTileCollide))
                {
                    _player.ChangePosition(CDirection.Top);
                    if (Context.WorldAPI.CurrentLevel == 0 && Context.Camera.Position.Y > 10 && Context.Camera.Position.Y < Context.Map.HeightInPixels && _player.PositionY < Context.Map.HeightInPixels - (Context.GetWindowsHeight / 2 - (Context.Player.Height / 2))) Context.Camera.Move(new Vector2(0, -_player.CharacterType.MoveSpeed));
                    else if (Context.WorldAPI.CurrentLevel != 0) { Context.Camera.Move(new Vector2(0, -_player.CharacterType.MoveSpeed)); }
                }
                _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.WalkTop select action;
            }
            else if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
            {
                if ((_player.PositionY <= (Context.Map.HeightInPixels - 50)) && (Context.Map.LayerCollide.GetTile((int)Math.Floor(_player.PositionX / (decimal)Context.Map.TileSize) + 1, ((int)Math.Floor(_player.PositionY / (decimal)Context.Map.TileSize)) + 2).Id != Context.Map.IdTileCollide) && (Context.Map.LayerDoorCollide.GetTile((int)Math.Floor(_player.PositionX / (decimal)Context.Map.TileSize) + 1, ((int)Math.Floor(_player.PositionY / (decimal)Context.Map.TileSize)) + 2).Id != Context.Map.IdTileCollide))
                {
                    _player.ChangePosition(CDirection.Bottom);
                    if (Context.WorldAPI.CurrentLevel == 0 && Context.Camera.Position.Y < Context.Map.HeightInPixels - 550 && _player.PositionY > (Context.GetWindowsHeight / 2 - (Context.Player.Height / 2))) Context.Camera.Move(new Vector2(0, +_player.CharacterType.MoveSpeed));
                    else if (Context.WorldAPI.CurrentLevel != 0) { Context.Camera.Move(new Vector2(0, +_player.CharacterType.MoveSpeed)); }
                }
                _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.WalkBottom select action;
            }
            else if (state.IsKeyDown(Keys.Q) || state.IsKeyDown(Keys.Left))
            {
                if (_player.PositionX >= 0 && (Context.Map.LayerCollide.GetTile((int)Math.Round((decimal)_player.PositionX / Context.Map.TileSize), (int)Math.Round((decimal)_player.PositionY / Context.Map.TileSize) + 1).Id != Context.Map.IdTileCollide) && (Context.Map.LayerDoorCollide.GetTile((int)Math.Round((decimal)_player.PositionX / Context.Map.TileSize), (int)Math.Round((decimal)_player.PositionY / Context.Map.TileSize) + 1).Id != Context.Map.IdTileCollide))
                {
                    _player.ChangePosition(CDirection.Left);
                    if (Context.WorldAPI.CurrentLevel == 0 && Context.Camera.Position.X > 10 && Context.Camera.Position.X < Context.Map.WidthInPixels && _player.PositionX < Context.Map.WidthInPixels - (Context.GetWindowWidth / 2 - (Context.Player.Width / 2))) Context.Camera.Move(new Vector2(-_player.CharacterType.MoveSpeed, 0));
                    else if (Context.WorldAPI.CurrentLevel != 0) { Context.Camera.Move(new Vector2(-_player.CharacterType.MoveSpeed, 0)); }
                }
                _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.WalkLeft select action;
            }
            else if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
            {
                if ((_player.PositionX <= (Context.Map.WidthInPixels)) && (Context.Map.LayerCollide.GetTile(((int)Math.Round((decimal)_player.PositionX / Context.Map.TileSize)) + 1, ((int)Math.Round((decimal)_player.PositionY / Context.Map.TileSize)) + 1).Id != Context.Map.IdTileCollide) && (Context.Map.LayerDoorCollide.GetTile(((int)Math.Round((decimal)_player.PositionX / Context.Map.TileSize)) + 1, ((int)Math.Round((decimal)_player.PositionY / Context.Map.TileSize)) + 1).Id != Context.Map.IdTileCollide))
                {
                    _player.ChangePosition(CDirection.Right);
                    if (Context.WorldAPI.CurrentLevel == 0 && Context.Camera.Position.X < Context.Map.WidthInPixels - 960 && _player.PositionX > (Context.GetWindowWidth / 2 - (Context.Player.Width / 2))) Context.Camera.Move(new Vector2(+_player.CharacterType.MoveSpeed, 0));
                    else if (Context.WorldAPI.CurrentLevel != 0) { Context.Camera.Move(new Vector2(+_player.CharacterType.MoveSpeed, 0)); }
                }
                _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.WalkRight select action;
            }
            else if (_state.IsKeyDown(Keys.Space))
            {
                _isAttacking = true;
                MillisecondsPerFrame = 40;
                switch ((int)_actualAction.RowAction % 4)
                {
                    case 0:
                        _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.AttackTop select action;
                        break;

                    case 1:
                        _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.AttackLeft select action;
                        break;

                    case 2:
                        _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.AttackBottom select action;
                        break;

                    case 3:
                        _action = from action in _playerAction where action.RowAction == (int)RowActionOnSpriteSheetPlayer.AttackRight select action;
                        break;
                }
            }



            if (_action != null)
            {
                foreach (ActionSpriteSheet action in _action)
                {
                    _lastAction = action;
                    return action;
                }
            }

            return _lastAction;
        }

        /// <summary>
        /// Unloads this instance.
        /// </summary>
        public void Unload()
        {
            Spritesheet.Dispose();
        }

    }
}
