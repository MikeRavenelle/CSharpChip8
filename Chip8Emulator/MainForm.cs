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
    public partial class MainForm : Form
    {
        GameScreen game;
        public MainForm()
        {
            InitializeComponent();
            resetMenu.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
            this.BeginInvoke(new Action(game.ClearEmulator));    
        }

        private void Emulator_RaiseUpdateEmulator(object sender, byte[,] e)
        {
            this.BeginInvoke(new Action(() => game.UpdateEmulator(e)));
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StartScreen(openFileDialog1.FileName);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.ResetEmulator();
        }
    }
}
