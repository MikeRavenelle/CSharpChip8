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
					// add more controls here
				}
			};

			// create a few commands that can be used for the menu and toolbar
			var open = new Command { MenuText = "Open", ToolBarText = "Open" };
			open.Executed += (sender, e) =>
			{
                var dialog = new OpenFileDialog();

                // Set the title of the dialog
                dialog.Title = "Select a File";

                // Set the filters for the dialog
                dialog.Filters.Add(new FileFilter("All Files", ".*"));

                // Show the dialog and wait for the user to select a file
                var result = dialog.ShowDialog(Application.Instance.MainForm);

				// If the user clicked OK, get the selected file(s)
				if (result == DialogResult.Ok)
				{
					using var game = new Chip8Emulator.Chip8.GameScreen(dialog.FileName);
					game.Run();
				}
			};

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command { MenuText = "About..." };
			aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new SubMenuItem { Text = "&File", Items = { open } },
					// new SubMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new SubMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar { Items = { open } };
		}
	}
}

