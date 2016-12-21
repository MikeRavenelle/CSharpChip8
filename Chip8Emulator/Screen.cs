using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Chip8Emulator
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        Texture2D pixel2;
        Emulator emulator;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 256;
            graphics.PreferredBackBufferWidth = 512;
            Content.RootDirectory = "Content";
            emulator = new Emulator(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDevice.Clear(Color.White);
            Thread thread = new Thread(() => emulator.ReadGame("D:\\Chip8Roms\\BRIX"));
            thread.Start();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 8, 8);
            Color[] data = new Color[64];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            pixel.SetData(data);
            pixel2 = new Texture2D(GraphicsDevice, 8, 8);
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
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
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void UpdateEmulator(byte[,] screenData)
        {
            spriteBatch.Begin();
            for (int i = 0; i < 63; ++i)
            {
                for(int j = 0; j < 31; ++j)
                {
                    Vector2 pos = new Vector2(i * 8, j * 8);
                    if (screenData[i,j] == 1)
                    {
                        spriteBatch.Draw(pixel, pos, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(pixel2, pos, Color.White);
                    }
                }
            }

            spriteBatch.End();
        }

        public void ClearEmulator()
        {
            graphics.GraphicsDevice.Clear(Color.White);
        }
    }
}
