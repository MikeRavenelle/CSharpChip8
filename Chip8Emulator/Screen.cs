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
            graphics.PreferredBackBufferHeight = 32;
            graphics.PreferredBackBufferWidth = 64;
            Content.RootDirectory = "Content";
            emulator = new Emulator(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDevice.Clear(Color.White);
            Thread thread = new Thread(() => emulator.ReadGame("D:\\Chip8Roms\\UFO"));
            thread.Start();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.Black });
            pixel2 = new Texture2D(GraphicsDevice, 1, 1);
            pixel2.SetData<Color>(new Color[1] { Color.White });
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
            for (int i = 0; i < 63; ++i)
            {
                for(int j = 0; j < 31; ++j)
                {
                    if(screenData[i,j] == 1)
                    {
                       Vector2 pos = new Vector2(i, j);
                        spriteBatch.Begin();
                        spriteBatch.Draw(pixel, pos, Color.White);
                        spriteBatch.End();
                    }
                    else
                    {
                        Vector2 pos = new Vector2(i, j);
                        spriteBatch.Begin();
                        spriteBatch.Draw(pixel2, pos, Color.White);
                        spriteBatch.End();
                    }
                }
            }
        }

        public void ClearEmulator()
        {
            graphics.GraphicsDevice.Clear(Color.White);
        }
    }
}
