using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System;

namespace Chip8Emulator.Chip8
{
    public class GameScreen : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _pixel;
        Texture2D _pixel2;
        Emulator _emulator;
        Thread _thread;
        KeyboardState _keyState;
        string _path;
        byte[,] _screenData;

        public static event EventHandler<byte[,]> RaiseUpdateEmulator;
        public static event EventHandler RaiseClearEmulator;

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        public GameScreen(string path)
        {
            _path = path;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Emulator.RaiseUpdateEmulator += Emulator_RaiseUpdateEmulator;
            Emulator.RaiseClearEmulator += Emulator_RaiseClearEmulator;
            _emulator = new Emulator();
        }

        private void Emulator_RaiseClearEmulator(object sender, EventArgs e)
        {
            //RaiseClearEmulator.Invoke(this, null);
            ClearEmulator();
        }

        private void Emulator_RaiseUpdateEmulator(object sender, byte[,] m)
        {
            //RaiseUpdateEmulator.Invoke(this, m);
            UpdateEmulator(m);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 256;
            _graphics.PreferredBackBufferWidth = 512;
            _graphics.ApplyChanges();
            base.Initialize();
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            _thread = new Thread(() => _emulator.ReadGame(_path));
            _thread.Start();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 8, 8);
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[64];
            for (int i = 0; i < data.Length; ++i) data[i] = Microsoft.Xna.Framework.Color.Green;
            _pixel.SetData(data);
            _pixel2 = new Texture2D(GraphicsDevice, 8, 8);
            for (int i = 0; i < data.Length; ++i) data[i] = Microsoft.Xna.Framework.Color.Black;
            _pixel2.SetData(data);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(
                Keys.Escape))
            {
                Exit();
            }

            UpdateInput();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_spriteBatch != null && _screenData != null)
            {
                _spriteBatch.Begin();
                for (int i = 0; i < 64; ++i)
                {
                    for (int j = 0; j < 32; ++j)
                    {
                        Vector2 pos = new Vector2(i * 8, j * 8);
                        if (_screenData[i, j] == 1)
                        {
                            _spriteBatch.Draw(_pixel, pos, Microsoft.Xna.Framework.Color.Green);
                        }
                        else
                        {
                            _spriteBatch.Draw(_pixel2, pos, Microsoft.Xna.Framework.Color.Green);
                        }
                    }
                }

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void UpdateEmulator(byte[,] screenData)
        {
            _screenData = screenData;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            _emulator.TurnOff();
        }


        public void ClearEmulator()
        {
            _graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
        }

        public void ResetEmulator()
        {
            _emulator.CPUReset();
        }

        private void UpdateInput()
        {
            _keyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (_keyState.IsKeyDown(Keys.D1))
            {
                _emulator.KeypadInput(0x01, true);
            }
            else
            {
                //emulator.KeypadInput(0x01, false);
            }

            if (_keyState.IsKeyDown(Keys.D2))
            {
                _emulator.KeypadInput(0x02, true);
            }
            else
            {
                //emulator.KeypadInput(0x02, false);
            }

            if (_keyState.IsKeyDown(Keys.D3))
            {
                _emulator.KeypadInput(0x03, true);
            }
            else
            {
                //emulator.KeypadInput(0x03, false);
            }

            if (_keyState.IsKeyDown(Keys.D4))
            {
                _emulator.KeypadInput(0x0C, true);
            }
            else
            {
                //emulator.KeypadInput(0x0C, false);
            }

            if (_keyState.IsKeyDown(Keys.Q))
            {
                _emulator.KeypadInput(0x04, true);
            }
            else
            {
                //emulator.KeypadInput(0x04, false);
            }

            if (_keyState.IsKeyDown(Keys.W))
            {
                _emulator.KeypadInput(0x05, true);
            }
            else
            {
                //emulator.KeypadInput(0x05, false);
            }

            if (_keyState.IsKeyDown(Keys.E))
            {
                _emulator.KeypadInput(0x06, true);
            }
            else
            {
                //emulator.KeypadInput(0x06, false);
            }

            if (_keyState.IsKeyDown(Keys.R))
            {
                _emulator.KeypadInput(0x0D, true);
            }
            else
            {
                //emulator.KeypadInput(0x0D, false);
            }

            if (_keyState.IsKeyDown(Keys.A))
            {
                _emulator.KeypadInput(0x07, true);
            }
            else
            {
                //emulator.KeypadInput(0x07, false);
            }

            if (_keyState.IsKeyDown(Keys.S))
            {
                _emulator.KeypadInput(0x08, true);
            }
            else
            {
                //emulator.KeypadInput(0x08, false);
            }

            if (_keyState.IsKeyDown(Keys.D))
            {
                _emulator.KeypadInput(0x09, true);
            }
            else
            {
                //emulator.KeypadInput(0x09, false);
            }

            if (_keyState.IsKeyDown(Keys.F))
            {
                _emulator.KeypadInput(0x0E, true);
            }
            else
            {
                //emulator.KeypadInput(0x0E, false);
            }

            if (_keyState.IsKeyDown(Keys.Z))
            {
                _emulator.KeypadInput(0x0A, true);
            }
            else
            {
                //emulator.KeypadInput(0x0A, false);
            }

            if (_keyState.IsKeyDown(Keys.X))
            {
                _emulator.KeypadInput(0x00, true);
            }
            else
            {
                //emulator.KeypadInput(0x00, false);
            }

            if (_keyState.IsKeyDown(Keys.C))
            {
                _emulator.KeypadInput(0x0B, true);
            }
            else
            {
                //emulator.KeypadInput(0x0B, false);
            }

            if (_keyState.IsKeyDown(Keys.V))
            {
                _emulator.KeypadInput(0x0F, true);
            }
            else
            {
                //emulator.KeypadInput(0x0F, false);
            }

        }
    }
}
