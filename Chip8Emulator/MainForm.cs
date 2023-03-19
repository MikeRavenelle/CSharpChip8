using System;
using Eto.Forms;
using Eto.Drawing;
using Chip8Emulator;

namespace Chip8Emulator_eto
{
	public partial class MainForm : Form
	{
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
					using var game = new Chip8Emulator.Chip8.GameScreen(dialog.FileName);
					game.Run();
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
	}
}

