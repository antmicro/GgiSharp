/*
  Copyright (c) 2013 Ant Micro <www.antmicro.com>

  Authors:
   * Mateusz Holenko (mholenko@antmicro.com)

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Runtime.InteropServices;

namespace AntMicro.GgiSharp
{
    public class GgiEvent : IDisposable
    {
        private readonly IntPtr pointer;
        public IntPtr   Pointer         { get { return pointer; } }

        public byte     AnyType         { get { return Marshal.ReadByte( pointer, 0x01); } }
        public int      KeyButton       { get { return Marshal.ReadInt32(pointer, 0x2c); } }
        public int      PmoveX          { get { return Marshal.ReadInt32(pointer, 0x20); } }
        public int      PmoveY          { get { return Marshal.ReadInt32(pointer, 0x24); } }
        public int      PbuttonButton   { get { return Marshal.ReadInt32(pointer, 0x20); } }
   
        public GgiEvent()
        {
            pointer = Marshal.AllocCoTaskMem(248);
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(pointer);
        }
    }
}

