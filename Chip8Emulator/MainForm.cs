using System;
using Eto.Forms;
using Eto.Drawing;
using Chip8Emulator;
using Microsoft.Xna.Framework;
using Chip8Emulator.Chip8;

namespace Chip8Emulator_eto
{
	public partial class MainForm : Form
	{
        GameScreen _game;
        public MainForm()
		{
			Title = "Chip8Emulator";
			MinimumSize = new Size(400, 200);

			Content = new StackLayout
			{
				Padding = 10,
				Items =
				{
					"Hello NebraskaCode() 2023",
				}
			};

			var open = new Command { MenuText = "Open"};
			open.Executed += (sender, e) =>
			{
                var dialog = new OpenFileDialog();
                dialog.Title = "Select a File";
                dialog.Filters.Add(new FileFilter("All Files", ".*"));

                var result = dialog.ShowDialog(Application.Instance.MainForm);

				if (result == DialogResult.Ok)
				{
					using (_game = new GameScreen(dialog.FileName))
					{
						GameScreen.RaiseUpdateEmulator += Emulator_RaiseUpdateEmulator;
						GameScreen.RaiseClearEmulator += Emulator_RaiseClearEmulator;
						_game.Run();
					}
				}
			};

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					new SubMenuItem { Text = "&File", Items = { open } },
				},
				QuitItem = quitCommand,
			};
		}

        private void Emulator_RaiseClearEmulator(object sender, EventArgs e)
        {
            Eto.Forms.Application.Instance.Invoke(() =>
            {
                _game.ClearEmulator();
            });
        }

        private void Emulator_RaiseUpdateEmulator(object sender, byte[,] e)
        {
            Eto.Forms.Application.Instance.Invoke(() =>
            {
                _game.UpdateEmulator(e);
            });
        }
    }
}

