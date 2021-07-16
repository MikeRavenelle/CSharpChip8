using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System;

namespace Chip8Emulator
{
    public class GameScreen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        Texture2D pixel2;
        Emulator emulator;
        Thread thread;
        KeyboardState keyState;
        string path;

        public static event EventHandler<byte[,]> RaiseUpdateEmulator;
        public static event EventHandler RaiseClearEmulator;

        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public GameScreen(string path)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Emulator.RaiseUpdateEmulator += Emulator_RaiseUpdateEmulator;
            Emulator.RaiseClearEmulator += Emulator_RaiseClearEmulator;
            emulator = new Emulator();
        }

        private void Emulator_RaiseClearEmulator(object sender, EventArgs e)
        {
            RaiseClearEmulator.Invoke(this, null);
        }

        private void Emulator_RaiseUpdateEmulator(object sender, byte[,] m)
        {
            RaiseUpdateEmulator.Invoke(this, m);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 256;
            graphics.PreferredBackBufferWidth = 512;
            graphics.ApplyChanges();
            base.Initialize();
            GraphicsDevice.Clear(Color.White);
            thread = new Thread(() => emulator.ReadGame(path));
            thread.Start();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 8, 8);
            Color[] data = new Color[64];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            pixel.SetData(data);
            pixel2 = new Texture2D(GraphicsDevice, 8, 8);
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            pixel2.SetData(data);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
            {
                Exit();
            }

            UpdateInput();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void UpdateEmulator(byte[,] screenData)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin();
                for (int i = 0; i < 64; ++i)
                {
                    for (int j = 0; j < 32; ++j)
                    {
                        Vector2 pos = new Vector2(i * 8, j * 8);
                        if (screenData[i, j] == 1)
                        {
                            spriteBatch.Draw(pixel, pos, Color.Green);
                        }
                        else
                        {
                            spriteBatch.Draw(pixel2, pos, Color.Green);
                        }
                    }
                }

                spriteBatch.End();
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            emulator.TurnOff();
        }


        public void ClearEmulator()
        {
            //graphics.GraphicsDevice.Clear(Color.White);
        }

        public void ResetEmulator()
        {
            emulator.CPUReset();
        }

        private void UpdateInput()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.D1))
            {
                emulator.KeypadInput(0x01, true);
            }
            else
            {
                //emulator.KeypadInput(0x01, false);
            }

            if (keyState.IsKeyDown(Keys.D2))
            {
                emulator.KeypadInput(0x02, true);
            }
            else
            {
                //emulator.KeypadInput(0x02, false);
            }

            if (keyState.IsKeyDown(Keys.D3))
            {
                emulator.KeypadInput(0x03, true);
            }
            else
            {
                //emulator.KeypadInput(0x03, false);
            }

            if (keyState.IsKeyDown(Keys.D4))
            {
                emulator.KeypadInput(0x0C, true);
            }
            else
            {
                //emulator.KeypadInput(0x0C, false);
            }

            if (keyState.IsKeyDown(Keys.Q))
            {
                emulator.KeypadInput(0x04, true);
            }
            else
            {
                //emulator.KeypadInput(0x04, false);
            }

            if (keyState.IsKeyDown(Keys.W))
            {
                emulator.KeypadInput(0x05, true);
            }
            else
            {
                //emulator.KeypadInput(0x05, false);
            }

            if (keyState.IsKeyDown(Keys.E))
            {
                emulator.KeypadInput(0x06, true);
            }
            else
            {
                //emulator.KeypadInput(0x06, false);
            }

            if (keyState.IsKeyDown(Keys.R))
            {
                emulator.KeypadInput(0x0D, true);
            }
            else
            {
               //emulator.KeypadInput(0x0D, false);
            }

            if (keyState.IsKeyDown(Keys.A))
            {
                emulator.KeypadInput(0x07, true);
            }
            else
            {
                //emulator.KeypadInput(0x07, false);
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                emulator.KeypadInput(0x08, true);
            }
            else
            {
                //emulator.KeypadInput(0x08, false);
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                emulator.KeypadInput(0x09, true);
            }
            else
            {
                //emulator.KeypadInput(0x09, false);
            }

            if (keyState.IsKeyDown(Keys.F))
            {
                emulator.KeypadInput(0x0E, true);
            }
            else
            {
                //emulator.KeypadInput(0x0E, false);
            }

            if (keyState.IsKeyDown(Keys.Z))
            {
                emulator.KeypadInput(0x0A, true);
            }
            else
            {
                //emulator.KeypadInput(0x0A, false);
            }

            if (keyState.IsKeyDown(Keys.X))
            {
                emulator.KeypadInput(0x00, true);
            }
            else
            {
                //emulator.KeypadInput(0x00, false);
            }

            if (keyState.IsKeyDown(Keys.C))
            {
                emulator.KeypadInput(0x0B, true);
            }
            else
            {
                //emulator.KeypadInput(0x0B, false);
            }

            if (keyState.IsKeyDown(Keys.V))
            {
                emulator.KeypadInput(0x0F, true);
            }
            else
            {
                //emulator.KeypadInput(0x0F, false);
            }

        }
    }
}
