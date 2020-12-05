using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using X11;

namespace x11dotnet
{
    class Program
    {
        private SimpleLogger Log;
        private IntPtr display;
        private Window root;
        private Window window;
        public XErrorHandlerDelegate OnError;

        static void Main(string[] args)
        {
            var p = new Program(SimpleLogger.LogLevel.Error);
            Console.WriteLine("Hello Worldz!");
        }

        private ulong GetPixelByName(string name)
        {
            var screen = Xlib.XDefaultScreen(display);
            XColor color = new XColor();
            if (0 == Xlib.XParseColor(display, Xlib.XDefaultColormap(display, screen), name, ref color))
            {
                Log.Error($"Invalid Color {name}");
            }

            if (0 == Xlib.XAllocColor(display, Xlib.XDefaultColormap(display, screen), ref color))
            {
                Log.Error($"Failed to allocate color {name}");
            }

            return color.pixel;
        }

        public int ErrorHandler(IntPtr display, ref XErrorEvent ev)
        {
            if (ev.error_code == 10) // BadAccess, i.e. another window manager has already claimed those privileges.
            {
                Log.Error("X11 denied access to window manager resources - another window manager is already running");
                Environment.Exit(1);
            }

            // Other runtime errors and warnings.
            var description = Marshal.AllocHGlobal(1024);
            Xlib.XGetErrorText(display, ev.error_code, description, 1024);
            var desc = Marshal.PtrToStringAnsi(description);
            Log.Warn($"X11 Error: {desc}");
            Marshal.FreeHGlobal(description);
            return 0;
        }

        public Program(SimpleLogger.LogLevel level)
        {
            Log = new SimpleLogger(level);
            Log.Info("Started class");

            var pDisplayText = Xlib.XDisplayName(null);
            var DisplayText = Marshal.PtrToStringAnsi(pDisplayText);
            if (DisplayText == String.Empty)
            {
                Log.Error("No display configured for X11; check the value of the DISPLAY variable is set correctly");
                Environment.Exit(1);
            }

            Log.Info($"Connecting to X11 Display {DisplayText}");
            display = Xlib.XOpenDisplay(null);

            if (display == IntPtr.Zero)
            {
                Log.Error("Unable to open the default X display");
                Environment.Exit(1);
            }

            root = Xlib.XDefaultRootWindow(display);
            OnError = ErrorHandler;
            Xlib.XSetErrorHandler(OnError);

            window = Xlib.XCreateSimpleWindow(display, root,
                200, 300, 350, 350, 5, GetPixelByName("black"), GetPixelByName("white"));

            /* graphics context */
            XGCValues x = new XGCValues();
            var gc = Xlib.XCreateGC(display, window, 0, ref x);
            Xlib.XSetBackground(display, gc, GetPixelByName("white"));
            Xlib.XSetForeground(display, gc, GetPixelByName("black"));

//               ButtonPressMask|KeyPressMask|ExposureMask);

            // This will trigger a bad access error if another window manager is already running
            Xlib.XSelectInput(display, root,
                EventMask.ExposureMask | EventMask.ButtonPressMask | EventMask.KeyPressMask);

                
            Xlib.XMapWindow(display, window);
            IntPtr ev = Marshal.AllocHGlobal(24 * sizeof(long));

            Xlib.XDrawImageString(display,
                            window,
                            gc, 
                            10, 10, 
                            "Hello World", 11);


            var done = false;
            while (!done)
            {
                while(Xlib.XPending(display) > 0) {
                    Console.WriteLine("check for event");
                    Xlib.XNextEvent(display, ev);
                }

                var xevent = Marshal.PtrToStructure<X11.XAnyEvent>(ev);
                if (xevent.type != 0)
                    Console.WriteLine($"Event Loop {xevent.type}");

                switch (xevent.type) {
                    case (int)Event.Expose:
                        Console.WriteLine("Event.Expose");
                        var expose_event = Marshal.PtrToStructure<X11.XExposeEvent>(ev);
                        /* Window was showed. */
                        if(expose_event.count==0)
                            Xlib.XDrawString(expose_event.display,
                                            expose_event.window,
                                            gc, 
                                            50, 50, 
                                            "Hello World", 11);

                        break;

                    case (int)Event.MappingNotify:
                        Console.WriteLine("Event.MappingNotify");
                        /* Modifier key was up/down. */
                        // Xlib.XRefreshKeyboardMapping((XMappingEvent*)&ev);
                        break;
                    
                    case (int)Event.ButtonPress:
                        Console.WriteLine("Event.ButtonPress");
                        var button_event = Marshal.PtrToStructure<X11.XButtonEvent>(ev);
                        /* Mouse button was pressed. */
                        Xlib.XDrawString(button_event.display,
                                        button_event.window,
                                        gc, 
                                        button_event.x, button_event.y,
                                        "Hello World", 11);
                        break;
                    
                    case (int)Event.KeyPress:
                        Console.WriteLine("Event.KeyPress");
                        var key_event = Marshal.PtrToStructure<X11.XKeyEvent>(ev);
                        /* Key input. */
                        Console.WriteLine("keycode = %x", key_event.keycode);
                        if (key_event.keycode == 0x09) {
                            done = true;
                        }

                        // var i = Xlib.XLookupString(key_event, text, 10, &mykey, 0);
                        // if(i==1 && text[0]=='q') done = 1;
                        break;
                }
            }

            Xlib.XFreeGC(display,gc);
            Xlib.XDestroyWindow(display, window);
            Xlib.XCloseDisplay(display);

        }
    }
}
