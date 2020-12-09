# poc

based on the [rosettacode hello world example](//https://rosettacode.org/wiki/Window_creation/X11#Xlib)



```csharp
using System;
using System.Runtime.InteropServices;
using X11;

namespace x11dotnet
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            string msg = "Hello, World!";
            
            IntPtr d = Xlib.XOpenDisplay(null);

            if (d == IntPtr.Zero) {
                Console.WriteLine("Cannot open display");
                System.Environment.Exit(1);
            }
            
            var s = Xlib.XDefaultScreen(d);
            Window w = Xlib.XCreateSimpleWindow(d, Xlib.XRootWindow(d, s), 
                                    10, 10, 100, 100, 1,
                                    Xlib.XBlackPixel(d, s), Xlib.XWhitePixel(d, s));
            Xlib.XSelectInput(d, w, EventMask.ExposureMask | EventMask.KeyPressMask);
            Xlib.XMapWindow(d, w);
            
            IntPtr e = Marshal.AllocHGlobal(24 * sizeof(long));
            bool done = false;

            while (!done) {
                Xlib.XNextEvent(d, e);
                var xevent = Marshal.PtrToStructure<X11.XAnyEvent>(e);

                switch (xevent.type) {
                    case (int)Event.Expose:
                        Xlib.XFillRectangle(d, w, Xlib.XDefaultGC(d, s), 20, 20, 10, 10);
                        Xlib.XDrawString(d, w, Xlib.XDefaultGC(d, s), 10, 50, msg, msg.Length);
                        break;
                    case (int)Event.KeyPress:
                        done = true;
                        break;
                }
            }
            
            Xlib.XCloseDisplay(d);
            
        }
    }
}

```