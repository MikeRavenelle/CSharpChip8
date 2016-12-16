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
                    case 0x0000:
                        switch (opcode & 0x000F)
                        {
                            case 0x0000: Opcode00E0(opcode); break; //clear screen
                            case 0x000E: Opcode00EE(opcode); break;
                        }
                        break;
                    case 0x1000: Opcode1NNN(opcode); break;
                    case 0x2000: Opcode2NNN(opcode); break;
                    case 0x3000: Opcode3XNN(opcode); break;
                    case 0x4000: Opcode4XNN(opcode); break;
                    case 0x5000: Opcode5XY0(opcode); break;
                    case 0x6000: Opcode6XNN(opcode); break;
                    case 0x7000: Opcode7XNN(opcode); break;
                    case 0x8000:
                        switch (opcode & 0x000F)
                        {
                            case 0x0005: Opcode8XY5(opcode); break; //clear screen
                        }
                        break;
                    case 0xD000: OpcodeDXYN(opcode); break;
                    case 0xF000:
                        switch(opcode & 0x000F)
                        {
                            case 0x0003: OpcodeFX33(opcode); break;
                            case 0x0005:
                                switch(opcode & 0x00F0)
                                {
                                    case 0x0050: OpcodeFX55(opcode); break;
                                    case 0x0060: OpcodeFX65(opcode); break;
                                }
                                break;
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

        public void Opcode3XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            int value = opcode & 0x00FF;

            if(_registers[regx] == value)
            {
                _programCounter += 2;
            }

        }

        public void Opcode4XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            int value = opcode & 0x00FF;

            if (_registers[regx] != value)
            {
                _programCounter += 2;
            }

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

        public void Opcode6XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[regx] = (byte)(opcode & 0x00FF);
        }

        public void Opcode7XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[regx] += (byte)(opcode & 0x00FF);
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

        public void OpcodeDXYN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            int height = opcode & 0x000F;
            int coordx = _registers[regx];
            int coordy = _registers[regy];

            _registers[0xf] = 0;

            for(int yline = 0; yline < height; ++yline)
            {
                byte data = _gameMemory[_addressI + yline];

                int xpixerlinv = 7;
                int xpixel = 0;

                for (xpixel = 0; xpixel < 8; xpixel++, xpixerlinv--)
                {
                    int mask = 1 << xpixerlinv;

                    if((data & mask) == 1)
                    {
                        int x = coordx + xpixel;
                        int y = coordy + yline;

                        if(_screenData[x,y] == 1)
                        {
                            _registers[0xF] = 1;
                        }

                        _screenData[x, y] ^= 1;
                    }
                }
            }

        }

        public void OpcodeFX33(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx >>= 8;

            int value = _registers[regx];

            int hundreds = value / 100;
            int tens = (value / 10) % 10;
            int units = value % 10;

            _gameMemory[_addressI] = (byte)hundreds;
            _gameMemory[_addressI + 1] = (byte)tens;
            _gameMemory[_addressI + 2] = (byte)units;
        }

        public void OpcodeFX55(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx >>= 8;

            for(int i = 0; i <= regx; ++i)
            {
                _gameMemory[_addressI + i] = _registers[i];
            }

            _addressI = (ushort)(_addressI + regx + 1);
        }

        public void OpcodeFX65(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx >>= 8;

            for(int i = 0; i < regx; ++i)
            {
                _registers[i] = _gameMemory[_addressI + i];
            }

            _addressI = (ushort)(_addressI + regx + 1);
        }

    }
}

//TODO Opcodes
/*
8XY0	Assign	Vx=Vy	Sets VX to the value of VY.
8XY1	BitOp	Vx=Vx|Vy	Sets VX to VX or VY. (Bitwise OR operation)
8XY2	BitOp	Vx=Vx&Vy	Sets VX to VX and VY. (Bitwise AND operation)
8XY3	BitOp	Vx=Vx^Vy	Sets VX to VX xor VY.
8XY4	Math	Vx += Vy	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
8XY6	BitOp	Vx >> 1	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift.[2]
8XY7	Math	Vx=Vy-Vx	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
8XYE	BitOp	Vx << 1	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift.[2]
9XY0	Cond	if(Vx!=Vy)	Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
ANNN	MEM	I = NNN	Sets I to the address NNN.
BNNN	Flow	PC=V0+NNN	Jumps to the address NNN plus V0.
CXNN	Rand	Vx=rand()&NN	Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.
EX9E	KeyOp	if(key()==Vx)	Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
EXA1	KeyOp	if(key()!=Vx)	Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
FX07	Timer	Vx = get_delay()	Sets VX to the value of the delay timer.
FX0A	KeyOp	Vx = get_key()	A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
FX15	Timer	delay_timer(Vx)	Sets the delay timer to VX.
FX18	Sound	sound_timer(Vx)	Sets the sound timer to VX.
FX1E	MEM	I +=Vx	Adds VX to I.[3]
FX29	MEM	I=sprite_addr[Vx]	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
*/
