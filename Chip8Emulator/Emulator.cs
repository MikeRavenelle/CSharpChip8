using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emulator
{
    class Emulator
    {
        byte[] _gameMemory = new byte[0xFFF];
        byte[] _registers = new byte[16];
        ushort _addressI;
        ushort _programCounter;
        List<ushort> _stack = new List<ushort>();
        byte[,] _screenData = new byte[64,32];
        bool _runLoop;
        Form1 _mainForm;

        public Emulator(Form1 mainForm)
        {
            _programCounter = 0x200;
            _addressI = 0;
            _mainForm = mainForm;
        }

        public void CPUReset()
        {
            _programCounter = 0x200;
            _addressI = 0;
        }

        public void ReadGame(string gamePath)
        {
            FileStream fileStream = new FileStream(gamePath,FileMode.Open ,FileAccess.Read);
            fileStream.Read(_gameMemory, 0x200, 0xfff);

        }

        public ushort GetNextOpCode()
        {
            ushort res = 0;
            res = _gameMemory[_programCounter];
            res <<= 8;
            res |= _gameMemory[_programCounter + 1];
            _programCounter += 2;

            return res;
        }

        public void MainLoop()
        {
            while (_runLoop)
            {
                ushort opcode = GetNextOpCode();

                switch (opcode & 0xF000)
                {
                    case 0x1000: Opcode1NNN(opcode); break; //jump
                    case 0x0000:
                        switch (opcode & 0x000F)
                        {
                            case 0x0000: Opcode00E0(opcode); break; //clear screen
                            case 0x000E: Opcode00EE(opcode); break;
                        }
                        break;
                    default: break; //not yet handled
                }
            }
        }

        public void Opcode00E0(ushort opcode)
        {
            _mainForm.ClearScreen();
        }

        public void Opcode00EE(ushort opcode)
        {
            _programCounter = _stack.First();
            _stack.RemoveAt(0);
        }

        public void Opcode1NNN(ushort opcode)
        {
            _programCounter = (ushort)(opcode & 0x0FFF);
        }

        public void Opcode2NNN(ushort opcode)
        {
            _stack.Add(_programCounter);
            _programCounter = (ushort)(opcode & 0x0FFF);
        }

        public void Opcode5XY0(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            if(_registers[regx] == _registers[regy])
            {
                _programCounter += 2;
            }
        }

        public void Opcode8XY5(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            int xval = _registers[regx];
            int yval = _registers[regy];

            if (yval > xval)
                _registers[0xF] = 0;

            _registers[regx] = (byte)(xval - yval);
        }



    }
}
