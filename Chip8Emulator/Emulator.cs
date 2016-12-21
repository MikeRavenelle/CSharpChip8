using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

namespace Chip8Emulator
{
    class Emulator
    {
        //Hardware
        byte[] _gameMemory = new byte[0xFFF];
        byte[] _registers = new byte[16];
        ushort _addressI;
        ushort _programCounter;
        List<ushort> _stack = new List<ushort>();
        byte[,] _screenData = new byte[64, 32];
        int _delayTimer;
        int _cpuTick;

        //C# Variables
        Game1 _screenGame;
        Random _random = new Random();

        //Font Array
        byte[] _font = { 0xF0, 0x90, 0x90, 0x90, 0xF0, 0x20, 0x60, 0x20, 0x20, 0x70, 0xF0, 0x10, 0xF0, 0x80, 0xF0, 0xF0, 0x10, 0xF0, 0x10,
                        0xF0, 0x90, 0x090, 0xF0, 0x10, 0x10, 0xF0, 0x80, 0xF0, 0x10, 0xF0, 0xF0, 0x80, 0xF0, 0x90, 0xF0, 0xF0, 0x10, 0x20,
                        0x40, 0x40, 0xF0, 0x90, 0xF0, 0x90, 0xF0, 0xF0, 0x90, 0xF0, 0x10, 0xF0, 0xF0, 0x90, 0xF0, 0x90, 0x90, 0xE0, 0x90,
                        0xE0, 0x90,0xE0, 0xF0, 0x80, 0x80, 0x80, 0xF0, 0xE0, 0x90, 0x90, 0x90, 0xE0, 0xF0, 0x80, 0xF0, 0x80, 0xF0, 0xF0,
                        0x80, 0xF0, 0x80, 0x80 };

        public Emulator(Game1 game)
        {
            _programCounter = 0x200;
            _addressI = 0;
            _screenGame = game;
        }

        public void CPUReset()
        {
            _programCounter = 0x200;
            _addressI = 0;
        }

        public void ReadGame(string gamePath)
        {
            FileStream fileStream = new FileStream(gamePath, FileMode.Open, FileAccess.Read);
            fileStream.Read(_gameMemory, 0x200, 0xdff);
            MainLoop();
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
            _cpuTick = 0;
            while (true)
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
                            case 0x0000: Opcode8XY0(opcode); break;
                            case 0x0001: Opcode8XY1(opcode); break;
                            case 0x0002: Opcode8XY2(opcode); break;
                            case 0x0003: Opcode8XY3(opcode); break;
                            case 0x0004: Opcode8XY4(opcode); break;
                            case 0x0005: Opcode8XY5(opcode); break;
                            case 0x0006: Opcode8XY6(opcode); break;
                            case 0x0007: Opcode8XY7(opcode); break;
                            case 0x000E: Opcode8XYE(opcode); break;
                        }
                        break;
                    case 0x9000: Opcode9XY0(opcode); break;
                    case 0xA000: OpcodeANNN(opcode); break;
                    case 0xB000: OpcodeBNNN(opcode); break;
                    case 0xC000: OpcodeCXNN(opcode); break;
                    case 0xD000: OpcodeDXYN(opcode); break;
                    case 0xE000:
                        switch (opcode & 0x000F)
                        {
                            case 0x000E: OpcodeEX9E(opcode); break;
                            case 0x0001: OpcodeEXA1(opcode); break;
                        }
                        break;
                    case 0xF000:
                        switch (opcode & 0x000F)
                        {
                            case 0x0003: OpcodeFX33(opcode); break;
                            case 0x0005:
                                switch (opcode & 0x00F0)
                                {
                                    case 0x0010: OpcodeFX15(opcode); break;
                                    case 0x0050: OpcodeFX55(opcode); break;
                                    case 0x0060: OpcodeFX65(opcode); break;
                                }
                                break;
                            case 0x0007: OpcodeFX07(opcode); break;
                            case 0x000A: OpcodeFX0A(opcode); break;
                            case 0x0008: OpcodeFX18(opcode); break;
                            case 0x000E: OpcodeFX1E(opcode); break;
                            case 0x0009: OpcodeFX29(opcode); break;
                        }
                        break;
                    default: break; //not yet handled
                }

                if (_cpuTick > 8)
                {
                    --_delayTimer;
                    _cpuTick = 0;
                }

                ++_cpuTick;
                Thread.Sleep(17);
            }
        }

        //00E0	Display disp_clear()    Clears the screen.
        public void Opcode00E0(ushort opcode)
        {
            _screenGame.ClearEmulator();
            _screenGame.UpdateEmulator(_screenData);
            Array.Clear(_screenData, 0, _screenData.Length);

        }

        //00EE Flow	return; Returns from a subroutine.
        public void Opcode00EE(ushort opcode)
        {
            _programCounter = _stack.First();
            _stack.RemoveAt(0);
        }

        //1NNN	Flow	goto NNN;	Jumps to address NNN.
        public void Opcode1NNN(ushort opcode)
        {
            _programCounter = (ushort)(opcode & 0x0FFF);
        }

        //2NNN	Flow	*(0xNNN)()	Calls subroutine at NNN.
        public void Opcode2NNN(ushort opcode)
        {
            _stack.Add(_programCounter);
            _programCounter = (ushort)(opcode & 0x0FFF);
        }

        //3XNN	Cond	if(Vx==NN)	Skips the next instruction if VX equals NN. (Usually the next instruction is a jump to skip a code block)
        public void Opcode3XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            int value = opcode & 0x00FF;

            if (_registers[regx] == value)
            {
                _programCounter += 2;
            }

        }

        //4XNN	Cond	if(Vx!=NN)	Skips the next instruction if VX doesn't equal NN. (Usually the next instruction is a jump to skip a code block)
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

        //5XY0	Cond	if(Vx==Vy)	Skips the next instruction if VX equals VY. (Usually the next instruction is a jump to skip a code block)
        public void Opcode5XY0(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            if (_registers[regx] == _registers[regy])
            {
                _programCounter += 2;
            }
        }

        //6XNN	Const	Vx = NN	Sets VX to NN.
        public void Opcode6XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[regx] = (byte)(opcode & 0x00FF);
        }

        //7XNN	Const	Vx += NN	Adds NN to VX.
        public void Opcode7XNN(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[regx] += (byte)(opcode & 0x00FF);
        }

        //8XY0	Assign	Vx=Vy	Sets VX to the value of VY.
        public void Opcode8XY0(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            _registers[regx] = _registers[regy];
        }

        //8XY1 BitOp   Vx=Vx|Vy Sets VX to VX or VY. (Bitwise OR operation)
        public void Opcode8XY1(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            _registers[regx] = (byte)(_registers[regx] | _registers[regy]);
        }

        //8XY2 BitOp   Vx=Vx&Vy Sets VX to VX and VY. (Bitwise AND operation)
        public void Opcode8XY2(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            _registers[regx] = (byte)(_registers[regx] & _registers[regy]);
        }

        //8XY3 BitOp   Vx=Vx^Vy Sets VX to VX xor VY.
        public void Opcode8XY3(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            _registers[regx] = (byte)(_registers[regx] ^ _registers[regy]);
        }

        //8XY4 Math    Vx += Vy Adds VY to VX.VF is set to 1 when there's a carry, and to 0 when there isn't.
        public void Opcode8XY4(ushort opcode)
        {
            _registers[0xF] = 0;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            int xval = _registers[regx];
            int yval = _registers[regy];

            if ((xval + yval) > 255)
                _registers[0xF] = 1;

            _registers[regx] += (byte)yval;
        }

        //8XY5 Math    Vx -= Vy VY is subtracted from VX.VF is set to 0 when there's a borrow, and 1 when there isn't
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


        //8XY6 BitOp   Vx >> 1	Shifts VX right by one.VF is set to the value of the least significant bit of VX before the shift.[2]
        public void Opcode8XY6(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[0xF] = (byte)(_registers[regx] & 1);

            _registers[regx] = (byte)(_registers[regx] >> 1);
        }

        //8XY7 Math    Vx=Vy-Vx Sets VX to VY minus VX.VF is set to 0 when there's a borrow, and 1 when there isn't.
        public void Opcode8XY7(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            int xval = _registers[regx];
            int yval = _registers[regy];

            if (xval > yval)
                _registers[0xF] = 0;

            _registers[regx] = (byte)(yval - xval);
        }

        //8XYE BitOp   Vx << 1	Shifts VX left by one.VF is set to the value of the most significant bit of VX before the shift.[2]
        public void Opcode8XYE(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _registers[0xF] = (byte)((_registers[regx] & 0xFF) >> 7);

            _registers[regx] = (byte)(_registers[regx] << 1);
        }

        //9XY0	Cond	if(Vx!=Vy)	Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
        public void Opcode9XY0(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            int regy = opcode & 0x00F0;
            regy = regy >> 4;

            if (_registers[regx] != _registers[regy])
                _programCounter += 2;
        }

        //ANNN MEM I = NNN Sets I to the address NNN.
        public void OpcodeANNN(ushort opcode)
        {
            ushort address = (ushort)(opcode & 0x0FFF);
            _addressI = address;
        }

        //BNNN	Flow	PC=V0+NNN	Jumps to the address NNN plus V0.
        public void OpcodeBNNN(ushort opcode)
        {
            _programCounter = (ushort)((opcode & 0x0FFF) + _registers[0x00]);
        }

        //CXNN Rand    Vx=rand()&NN Sets VX to the result of a bitwise and operation on a random number(Typically: 0 to 255) and NN.
        public void OpcodeCXNN(ushort opcode)
        {
            _registers[0xF] = 1;
            int regx = opcode & 0x0F00;
            regx = regx >> 8;
            byte randomNumber = (byte)(_random.Next(0, 255));

            _registers[regx] = (byte)(randomNumber & (opcode & 0x00FF));
        }

        // DXYN	Disp	draw(Vx,Vy,N)	Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. 
        // Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. 
        // As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
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

            for (int yline = 0; yline < height; ++yline)
            {
                byte data = _gameMemory[_addressI + yline];

                int xpixerlinv = 7;
                int xpixel = 0;

                for (xpixel = 0; xpixel < 8; xpixel++, xpixerlinv--)
                {
                    int mask = 1 << xpixerlinv;

                    if ((data & mask) > 0)
                    {
                        int x = (coordx + xpixel);
                        int y = (coordy + yline);

                        if (x > 63)
                            x = 63;

                        if (y > 31)
                            y = 31;

                        if (_screenData[x, y] == 1)
                        {
                            _registers[0xF] = 1;
                        }

                        _screenData[x, y] ^= 1;
                    }
                }
            }

            _screenGame.UpdateEmulator(_screenData);

        }

        //EX9E	KeyOp	if(key()==Vx)	Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
        public void OpcodeEX9E(ushort opcode)
        {
            //TODO keyboard
        }

        //EXA1	KeyOp	if(key()!=Vx)	Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
        public void OpcodeEXA1(ushort opcode)
        {
            //TODO keyboard
        }

        //FX07	Timer	Vx = get_delay()	Sets VX to the value of the delay timer.
        public void OpcodeFX07(ushort opcode)
        {
            int timerValue = opcode & 0x0F00;
            timerValue = timerValue >> 8;

            _registers[timerValue] = (byte)_delayTimer;
        }

        //FX0A	KeyOp	Vx = get_key()	A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
        public void OpcodeFX0A(ushort opcode)
        {
            //TODO keyboard
        }

        //FX15	Timer	delay_timer(Vx)	Sets the delay timer to VX.
        public void OpcodeFX15(ushort opcode)
        {
            int timerValue = opcode & 0x0F00;
            timerValue = timerValue >> 8;

            _delayTimer = _registers[timerValue];
        }

        //FX18	Sound	sound_timer(Vx)	Sets the sound timer to VX.
        public void OpcodeFX18(ushort opcode)
        {
            int timerValue = opcode & 0x0F00;
            timerValue = timerValue >> 8;
            timerValue++;
            BeepSound((int)(timerValue * (1000f / 60)));
        }

        //FX1E	MEM	I +=Vx	Adds VX to I.[3]
        public void OpcodeFX1E(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _addressI += _registers[regx];
        }

        //FX29	MEM	I=sprite_addr[Vx]	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
        public void OpcodeFX29(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx = regx >> 8;

            _addressI = _font[_registers[regx]];
        }

        //FX33	BCD	set_BCD(Vx); (I+0)=BCD(3); (I+1)=BCD(2); (I+2)=BCD(1);
        //Stores the binary-coded decimal representation of VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, 
        //and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, 
        //the tens digit at location I+1, and the ones digit at location I+2.)
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

        //FX55	MEM	reg_dump(Vx,&I)	Stores V0 to VX (including VX) in memory starting at address I.[4]
        public void OpcodeFX55(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx >>= 8;

            for (int i = 0; i <= regx; ++i)
            {
                _gameMemory[_addressI + i] = _registers[i];
            }

            _addressI = (ushort)(_addressI + regx + 1);
        }

        //FX65	MEM	reg_load(Vx,&I)	Fills V0 to VX (including VX) with values from memory starting at address I.[4]
        public void OpcodeFX65(ushort opcode)
        {
            int regx = opcode & 0x0F00;
            regx >>= 8;

            for (int i = 0; i < regx; ++i)
            {
                _registers[i] = _gameMemory[_addressI + i];
            }

            _addressI = (ushort)(_addressI + regx + 1);
        }

        public void BeepSound(int time)
        {
            if(time > 0)
                Console.Beep(500, time);
        }
    }
}