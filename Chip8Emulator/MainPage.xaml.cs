using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chip8Emulator.Chip8;

namespace Chip8EmulatorGui
{
    public partial class MainPage : ContentPage
    {
        GameScreen game;

        public MainPage()
        {
            InitializeComponent();
        }

        public void StartScreen(string path)
        {
            using (game = new GameScreen(path))
            {
                GameScreen.RaiseUpdateEmulator += Emulator_RaiseUpdateEmulator;
                GameScreen.RaiseClearEmulator += Emulator_RaiseClearEmulator;
                game.Path = path;
                game.Run();
            }
        }

        private void Emulator_RaiseClearEmulator(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                game.ClearEmulator();
            });
        }

        private void Emulator_RaiseUpdateEmulator(object sender, byte[,] e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                game.UpdateEmulator(e);
            });
        }

        public async Task OpenAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result != null)
                {
                    StartScreen(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                Console.WriteLine(ex.Message);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.ResetEmulator();
        }

        private void MenuFlyoutItem_Clicked(object sender, EventArgs e)
        {
            var openTask = OpenAsync();
        }
    }
}