using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8Emulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread screenThread = new Thread(StartScreen);
            screenThread.Start();
        }

        public void StartScreen()
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
