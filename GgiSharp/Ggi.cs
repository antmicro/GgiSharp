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

using System.Collections.Generic;
using System.Linq;

namespace AntMicro.GgiSharp
{
    public class Ggi : IDisposable
    {
        public static int ContextCounter { get; set; }
        public static bool GgiInitialized { get; set; }

        public IntPtr Vis { get; private set; }
        public IntPtr MemVis { get; set; }

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public int ActiveMode { get; set; }

        private DirectBuffer _directMemBuffer;
        private DirectBuffer DirectMemBuffer
        {
            get 
            {
                if (_directMemBuffer == null)
                {
                    var buf = GgiDBGetBuffer(MemVis, 0);
                    _directMemBuffer = new DirectBuffer(buf);
                }
                return _directMemBuffer;
            }
        }

        public Ggi(int x, int y, int mode)
        {
            SizeX = x;
            SizeY = y;
            ActiveMode = mode;

            ContextCounter++;
            if(!GgiInitialized)
            {
                GgiInit();
                GgiInitialized = true;
            }

            Vis = GgiOpen("display-x", IntPtr.Zero);
            if(Vis == IntPtr.Zero)
            {
                GgiPanic("Couldn't open visual!\n");
            }

            MemVis = GgiOpen("display-memory", IntPtr.Zero);
            if(MemVis == IntPtr.Zero)
            {
                GgiPanic("Couldn't open memory visual!\n");
            }

            IntPtr sugMode = Marshal.AllocCoTaskMem(Define.SIZEOF_GGI_MODE);
            if(mode == Define.MODE_8BIT)
            {
                GgiCheckGraphMode(MemVis, x, y, Define.GGI_AUTO, Define.GGI_AUTO, Define.GT_8BIT, sugMode);
            }
            else if(mode == Define.MODE_16BIT)
            {
                GgiCheckGraphMode(MemVis, x, y, Define.GGI_AUTO, Define.GGI_AUTO, Define.GT_16BIT, sugMode);
            }
            else if(mode == Define.MODE_32BIT)
            {
                GgiCheckGraphMode(MemVis, x, y, Define.GGI_AUTO, Define.GGI_AUTO, Define.GT_32BIT, sugMode);
            }

            GgiSetMode(MemVis, sugMode);
            GgiCheckGraphMode(Vis, x, y, Define.GGI_AUTO, Define.GGI_AUTO, Define.GT_32BIT, sugMode);
            if(GgiSetMode(Vis, sugMode) != 0)
            {
                GgiPanic("error setting mode!\n");
            }

            Marshal.FreeCoTaskMem(sugMode);
        }

        ~Ggi()
        {
            Dispose();
        }

        public void Plot(IntPtr fdata, int x, int y, int w, int h)
        {
            LockPtr(); 
            MemCpy(DirectMemBuffer.Write, fdata, w * h * ActiveMode);
            UnlockPtr();
            GgiCrossBlit(MemVis, 0, 0, w, h, Vis, x, y);
            GgiFlush(Vis);
        }

        private void LockPtr()
        {   
            if(AcquireResource(DirectMemBuffer.Resource, Define.GGI_ACTYPE_WRITE) != 0)
            {
                GgiPanic("error!!!");
            }
        }

        private void UnlockPtr()
        {   
            ReleaseResource(DirectMemBuffer.Resource);
        }

        private static int ReleaseResource(IntPtr res)
        {
            return res == IntPtr.Zero ? 0 : GgiResourceFastRelease(res);
        }

        private static int AcquireResource(IntPtr res, int actype)
        {
            return res == IntPtr.Zero ? 0 : GgiResourceFastAcquire(res, actype);
        }

        public List<Event> GetAllEvents()
        {
            eventList.Clear();
            Marshal.Copy(emptyByteTable, 0, emptyTimeVal, Define.SIZEOF_TIMEVAL);
            GgiEventPoll(Vis, Define.EmAll, emptyTimeVal);
            var nevents = GgiEventsQueued(Vis, Define.EmAll);

            var e = new GgiEvent();
            while(nevents > 0)
            {
                GgiEventRead(Vis, e.Pointer, Define.EmAll);

                var type = e.AnyType;
                switch(type)
                {
                case Define.evPtrButtonPress:
                    eventList.Add(new Event(Define.TYPE_MOUSEDOWN, 0, 0, e.PbuttonButton));
                    break;

                case Define.evPtrButtonRelease:
                    eventList.Add(new Event(Define.TYPE_MOUSEUP, 0, 0, e.PbuttonButton));
                    break;

                case Define.evPtrAbsolute:
                    eventList.Add(new Event(Define.TYPE_MOUSEMOVE, e.PmoveX, e.PmoveY, 0));
                    break;

                case Define.evPtrRelative:
                    eventList.Add(new Event(Define.TYPE_MOUSEMOVE_RELATIVE, e.PmoveX, e.PmoveY, 0));
                    break;

                case Define.evKeyPress:
                    eventList.Add(new Event(Define.TYPE_KEYDOWN, 0, 0, e.KeyButton));
                    break;

                case Define.evKeyRepeat:
                    eventList.Add(new Event(Define.TYPE_KEYREPEAT, 0, 0, e.KeyButton));
                    break;

                case Define.evKeyRelease:
                    eventList.Add(new Event(Define.TYPE_KEYUP, 0, 0, e.KeyButton));
                    break;
                }

                nevents--;
            }
            e.Dispose();
            return eventList;
        }

        private readonly List<Event> eventList = new List<Event>();
        private readonly IntPtr emptyTimeVal = Marshal.AllocCoTaskMem(Define.SIZEOF_TIMEVAL);
        private readonly byte[] emptyByteTable = Enumerable.Repeat((byte)0, Define.SIZEOF_TIMEVAL).ToArray();

        private readonly object disposedLock = new object();
        private bool disposed = false;
        public void Dispose()
        {
            lock (disposedLock)
            {
                if (!disposed) 
                {
                    GgiClose(MemVis);
                    GgiClose(Vis);
                    ContextCounter--;
                    if(GgiInitialized && ContextCounter == 0)
                    {
                        GgiExit();
                        GgiInitialized = false;
                    }

                    GC.SuppressFinalize(this);
                    disposed = true;
                }
            }
        }

        #region Raw functions

        [DllImport("libggi", EntryPoint="ggiInit")]
        private static extern int GgiInit();

        [DllImport("libggi", EntryPoint="ggiExit")]
        private static extern int GgiExit();

        [DllImport("libggi", EntryPoint="ggiOpen")]
        private static extern IntPtr GgiOpen(string name, IntPtr ptr);

        [DllImport("libggi", EntryPoint="ggiClose")]
        private static extern int GgiClose(IntPtr vis);

        [DllImport("libggi", EntryPoint="ggiPanic")]
        private static extern void GgiPanic(string msg);

        [DllImport("libggi", EntryPoint="ggiSetMode")]
        private static extern int GgiSetMode(IntPtr visual, IntPtr mode);

        [DllImport("libggi", EntryPoint="ggiCheckGraphMode")]
        private static extern int GgiCheckGraphMode(IntPtr visual, int x, int y, int xv, int yv, int type, IntPtr suggestedMode);


        [DllImport("libggi", EntryPoint="ggiFlush")]
        private static extern int GgiFlush(IntPtr vis);

        [DllImport("libggi", EntryPoint="ggiEventPoll")]
        private static extern void GgiEventPoll(IntPtr vis, int mask, IntPtr timeVal);

        [DllImport("libggi", EntryPoint="ggiEventsQueued")]
        private static extern int GgiEventsQueued(IntPtr vis, int mask);

        [DllImport("libggi", EntryPoint="ggiEventRead")]
        private static extern int GgiEventRead(IntPtr vis, IntPtr ptr, int mask);

        [DllImport("libggi", EntryPoint="ggiDBGetBuffer")]
        private static extern IntPtr GgiDBGetBuffer(IntPtr ctx, int bufnum);

        [DllImport("libggi", EntryPoint="ggiResourceFastAcquire")]
        private static extern int GgiResourceFastAcquire(IntPtr res, int actype);

        [DllImport("libggi", EntryPoint="ggiResourceFastRelease")]
        private static extern int GgiResourceFastRelease(IntPtr res);

        [DllImport("libggi", EntryPoint="ggiCrossBlit")]
        private static extern int GgiCrossBlit(IntPtr src, int sx, int sy, int w, int h, IntPtr dst, int dx, int dy);

        [DllImport("libc", EntryPoint="memcpy")]
        private static extern int MemCpy(IntPtr dst, IntPtr src, int n);

        #endregion
    }
}

