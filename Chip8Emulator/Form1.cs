using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8Emulator
{
    public partial class Form1 : Form
    {
        Game1 game;
        public Form1()
        {
            InitializeComponent();
            resetMenu.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void StartScreen(string path)
        {
            using (game = new Game1())
            {
                game.Path = path;
                game.Run();
            }
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread screenThread = new Thread(() => StartScreen(openFileDialog1.FileName));
                screenThread.Start();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.ResetEmulator();
        }
    }
}
