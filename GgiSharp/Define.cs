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
namespace AntMicro.GgiSharp
{
	public static class Define
	{
		public static readonly int MODE_8BIT 	= 0x1;
		public static readonly int MODE_16BIT 	= 0x2;
        public static readonly int MODE_24BIT   = 0x3;
		public static readonly int MODE_32BIT 	= 0x4;

		public static readonly int GGI_AUTO = 0;
		public static readonly int GT_8BIT	= 67110920;
		public static readonly int GT_16BIT	= 33558544;
        public static readonly int GT_24BIT = 33560600;
        public static readonly int GT_32BIT	= 33562648;

		public static readonly int TYPE_MOUSEDOWN 			= 0x001;
		public static readonly int TYPE_MOUSEUP 			= 0x002;
		public static readonly int TYPE_MOUSEMOVE 			= 0x003;
		public static readonly int TYPE_MOUSEMOVE_RELATIVE 	= 0x004;
		public static readonly int TYPE_KEYDOWN 			= 0x101;
		public static readonly int TYPE_KEYUP 				= 0x102;
		public static readonly int TYPE_KEYREPEAT			= 0x103;

        public static readonly int SIZEOF_GGI_MODE      = 24;
        public static readonly int SIZEOF_GGI_EVENT     = 16;
        public static readonly int SIZEOF_GGI_CONTEXT   = 16416;
        public static readonly int SIZEOF_TIMEVAL       = 16;

        public static readonly int EmAll = 16382;

        public const byte evKeyPress            = 5;
        public const byte evKeyRelease          = 6;
        public const byte evKeyRepeat           = 7;
        public const byte evPtrRelative         = 8;
        public const byte evPtrAbsolute         = 9;
        public const byte evPtrButtonPress      = 10;
        public const byte evPtrButtonRelease    = 11;

        public const int GGI_ACTYPE_WRITE = 0x2;
	}
}
