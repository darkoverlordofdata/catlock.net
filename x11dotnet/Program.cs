using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using X11;
using Imlib2;

namespace x11dotnet
{
    class Program
    {
        private SimpleLogger Log;
        private IntPtr disp;
        private Window win;
        private IntPtr vis;
        Colormap cm;
        int depth;

        static void Main(string[] args)
        {
            var p = new Program(SimpleLogger.LogLevel.Error);
            Console.WriteLine("Hello Worldz!");
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
            /* areas to update */
            IntPtr updates, current_update;
            /* our virtual framebuffer image we draw into */
            IntPtr buffer;
            /* a font */
            IntPtr font;
            /* our color range */
            IntPtr range;
            /* our mouse x, y coordinates */
            int mouse_x = 0, mouse_y = 0;

            /* connect to X */
            disp  = Xlib.XOpenDisplay(null);
            /* get default visual , colormap etc. you could ask imlib2 for what it */
            /* thinks is the best, but this example is intended to be simple */
            vis   = Xlib.XDefaultVisual(disp, Xlib.XDefaultScreen(disp));
            depth = Xlib.XDefaultDepth(disp, Xlib.XDefaultScreen(disp));
            cm    = Xlib.XDefaultColormap(disp, Xlib.XDefaultScreen(disp));
            /* create a window 640x480 */
            win = Xlib.XCreateSimpleWindow(disp, Xlib.XDefaultRootWindow(disp), 
                                        0, 0, 640, 480, 0, 0, 0);
            /* tell X what events we are interested in */
            Xlib.XSelectInput(disp, win, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | 
                            EventMask.PointerMotionMask | EventMask.ExposureMask);
            /* show the window */
            Xlib.XMapWindow(disp, win);
            /* set our cache to 2 Mb so it doesn't have to go hit the disk as long as */
            /* the images we use use less than 2Mb of RAM (that is uncompressed) */
            Imlib.imlib_set_cache_size(2048 * 1024);
            /* set the font cache to 512Kb - again to avoid re-loading */
            Imlib.imlib_set_font_cache_size(512 * 1024);
            /* add the ./ttfonts dir to our font path - you'll want a notepad.ttf */
            /* in that dir for the text to display */
            Imlib.imlib_add_path_to_font_path("./ttfonts");
            /* set the maximum number of colors to allocate for 8bpp and less to 128 */
            Imlib.imlib_set_color_usage(128);
            /* dither for depths < 24bpp */
            Imlib.imlib_context_set_dither(1);
            /* set the display , visual, colormap and drawable we are using */
            Imlib.imlib_context_set_display(disp);
            Imlib.imlib_context_set_visual(vis);
            Imlib.imlib_context_set_colormap(cm);
            Imlib.imlib_context_set_drawable(win);
            /* infinite event loop */

            IntPtr ev = Marshal.AllocHGlobal(24 * sizeof(long));

            for (;;)
            {
                /* image variable */
                IntPtr image;
                /* width and height values */
                int w, h, text_w, text_h;
                
                /* init our updates to empty */
                updates = Imlib.imlib_updates_init();
                /* while there are events form X - handle them */
                do
                {
                    Xlib.XNextEvent(disp, ev);
                    var xevent = Marshal.PtrToStructure<X11.XAnyEvent>(ev);
                    switch (xevent.type)
                    {
                    case (int)Event.Expose:
                        /* window rectangle was exposed - add it to the list of */
                        /* rectangles we need to re-render */
                        var expose_event = Marshal.PtrToStructure<X11.XExposeEvent>(ev);

                        updates = Imlib.imlib_update_append_rect(updates,
                                                            expose_event.x, expose_event.y,
                                                            expose_event.width, expose_event.height);
                        break;

                    case (int)Event.ButtonPress:
                        /* if we click anywhere in the window, exit */
                        Environment.Exit(0);
                        break;

                    case (int)Event.MotionNotify:
                        /* if the mouse moves - note it */
                        /* add a rectangle update for the new mouse position */
                        var motion_event = Marshal.PtrToStructure<X11.XMotionEvent>(ev);

                        image = Imlib.imlib_load_image("./images/mush.png");
                        Imlib.imlib_context_set_image(image);
                        w = Imlib.imlib_image_get_width();
                        h = Imlib.imlib_image_get_height();
                        Imlib.imlib_context_set_image(image);
                        Imlib.imlib_free_image();
                        /* the old position - so we wipe over where it used to be */
                        updates = Imlib.imlib_update_append_rect(updates,
                                                            mouse_x - (w / 2), mouse_y - (h / 2),
                                                            w, h);
                        font = Imlib.imlib_load_font("notepad/30");
                        if (font != IntPtr.Zero)    
                        {
                            Imlib.imlib_context_set_font(font);
                            string text = String.Format($"Mouse is at {mouse_x}, {mouse_y}");
                            Imlib.imlib_get_text_size(text, out text_w, out text_h); 
                            Imlib.imlib_free_font();
                            updates = Imlib.imlib_update_append_rect(updates,
                                                                320 - (text_w / 2), 240 - (text_h / 2),
                                                                text_w, text_h);
                        }
                        
                        mouse_x = motion_event.x;
                        mouse_y = motion_event.y;
                        /* the new one */
                        updates = Imlib.imlib_update_append_rect(updates,
                                                            mouse_x - (w / 2), mouse_y - (h / 2),
                                                            w, h);
                        font = Imlib.imlib_load_font("notepad/30");
                        if (font != IntPtr.Zero)    
                        {
                            Imlib.imlib_context_set_font(font);
                            string text = String.Format($"Mouse is at {mouse_x}, {mouse_y}");
                            Imlib.imlib_get_text_size(text, out text_w, out text_h); 
                            Imlib.imlib_free_font();
                            updates = Imlib.imlib_update_append_rect(updates,
                                                                320 - (text_w / 2), 240 - (text_h / 2),
                                                                text_w, text_h);
                            }
                        break;

                    default:
                        /* any other events - do nothing */
                        break;
                    }
                }
                while (Xlib.XPending(disp)!=0);
                
                /* no more events for now ? ok - idle time so lets draw stuff */
                
                /* take all the little rectangles to redraw and merge them into */
                /* something sane for rendering */
                updates = Imlib.imlib_updates_merge_for_rendering(updates, 640, 480);
                for (current_update = updates; 
                    current_update != IntPtr.Zero; 
                    current_update = Imlib.imlib_updates_get_next(current_update))
                {
                    int up_x, up_y, up_w, up_h;

                    /* find out where the first update is */
                    Imlib.imlib_updates_get_coordinates(current_update, 
                                                out up_x, out up_y, out up_w, out up_h);
                    
                    /* create our buffer image for rendering this update */
                    buffer = Imlib.imlib_create_image(up_w, up_h);
                    
                    /* we can blend stuff now */
                    Imlib.imlib_context_set_blend(1);
                    
                    /* fill the window background */
                    /* load the background image - you'll need to have some images */
                    /* in ./images lying around for this to actually work */
                    image = Imlib.imlib_load_image("./images/bg.png");
                    /* we're working with this image now */
                    Imlib.imlib_context_set_image(image);
                    /* get its size */
                    w = Imlib.imlib_image_get_width();
                    h = Imlib.imlib_image_get_height();
                    /* now we want to work with the buffer */
                    Imlib.imlib_context_set_image(buffer);
                    /* if the iimage loaded */
                    if (image != IntPtr.Zero) 
                    {
                        /* blend image onto the buffer and scale it to 640x480 */
                        Imlib.imlib_blend_image_onto_image(image, 0, 
                                                    0, 0, w, h, 
                                                    - up_x, - up_y, 640, 480);
                        /* working with the loaded image */
                        Imlib.imlib_context_set_image(image);
                        /* free it */
                        Imlib.imlib_free_image();
                    }
                    
                    /* draw an icon centered around the mouse position */
                    image = Imlib.imlib_load_image("./images/mush.png");
                    Imlib.imlib_context_set_image(image);
                    w = Imlib.imlib_image_get_width();
                    h = Imlib.imlib_image_get_height();
                    Imlib.imlib_context_set_image(buffer);
                    if (image != IntPtr.Zero) 
                    {
                        Imlib.imlib_blend_image_onto_image(image, 0, 
                                                    0, 0, w, h, 
                                                    mouse_x - (w / 2) - up_x, mouse_y - (h / 2) - up_y, w, h);
                        Imlib.imlib_context_set_image(image);
                        Imlib.imlib_free_image();
                    }
                    
                    /* draw a gradient on top of things at the top left of the window */
                    /* create a range */
                    range = Imlib.imlib_create_color_range();
                    Imlib.imlib_context_set_color_range(range);
                    /* add white opaque as the first color */
                    Imlib.imlib_context_set_color(255, 255, 255, 255);
                    Imlib.imlib_add_color_to_color_range(0);
                    /* add an orange color, semi-transparent 10 units from the first */
                    Imlib.imlib_context_set_color(255, 200, 10, 100);
                    Imlib.imlib_add_color_to_color_range(10);
                    /* add black, fully transparent at the end 20 units away */
                    Imlib.imlib_context_set_color(0, 0, 0, 0);
                    Imlib.imlib_add_color_to_color_range(20);
                    /* draw the range */
                    Imlib.imlib_context_set_image(buffer);
                    Imlib.imlib_image_fill_color_range_rectangle(- up_x, - up_y, 128, 128, -45.0);
                    /* free it */
                    Imlib.imlib_free_color_range();
                    
                    /* draw text - centered with the current mouse x, y */
                    font = Imlib.imlib_load_font("notepad/30");
                    if (font != IntPtr.Zero)
                    {
                        /* set the current font */
                        Imlib.imlib_context_set_font(font);
                        /* set the image */
                        Imlib.imlib_context_set_image(buffer);
                        /* set the color (black) */
                        Imlib.imlib_context_set_color(0, 0, 0, 255);
                        /* print text to display in the buffer */
                        string text = String.Format($"Mouse is at {mouse_x}, {mouse_y}");
                        /* query the size it will be */
                        Imlib.imlib_get_text_size(text, out text_w, out text_h); 
                        /* draw it */
                        Imlib.imlib_text_draw(320 - (text_w / 2) - up_x, 240 - (text_h / 2) - up_y, text); 
                        /* free the font */
                        Imlib.imlib_free_font();
                    }
                    
                    /* don't blend the image onto the drawable - slower */
                    Imlib.imlib_context_set_blend(0);
                    /* set the buffer image as our current image */
                    Imlib.imlib_context_set_image(buffer);
                    /* render the image at 0, 0 */
                    Imlib.imlib_render_image_on_drawable(up_x, up_y);
                    /* don't need that temporary buffer image anymore */
                    Imlib.imlib_free_image();
                }
                /* if we had updates - free them */
                if (updates != IntPtr.Zero)
                    Imlib.imlib_updates_free(updates);
                /* loop again waiting for events */
            }

        }

        
    }
}
