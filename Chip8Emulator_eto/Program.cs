using System;
using Eto.Forms;
using Eto.Drawing;

namespace Chip8Emulator_eto
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			new Application(Eto.Platform.Detect).Run(new MainForm());
		}
	}
}
