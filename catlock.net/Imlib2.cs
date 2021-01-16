using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Imlib2
{

    public partial class Imlib
    {
        [DllImport("libImlib2.so")]
        public static extern void imlib_set_cache_size(int bytes);

        [DllImport("libImlib2.so")]
        public static extern void imlib_set_font_cache_size(int bytes);

        [DllImport("libImlib2.so")]
        public static extern void imlib_add_path_to_font_path(string filename);

        [DllImport("libImlib2.so")]
		public static extern void imlib_set_color_usage(int max);

        [DllImport("libImlib2.so")]
        public static extern void imlib_context_set_dither(int dither);

        [DllImport("libImlib2.so")]
        public static extern void imlib_context_set_display(IntPtr display);

        [DllImport("libImlib2.so")]
        public static extern void imlib_context_set_visual(IntPtr visual);
        
        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_colormap(X11.Colormap colormap);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_drawable(X11.Window drawable);

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_updates_init();

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_update_append_rect(IntPtr updates, int x, int y, int w, int h);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_image(IntPtr image);

        [DllImport("libImlib2.so")]
		public static extern void imlib_free_image();

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_load_font(string font_name);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_font(IntPtr font);

        [DllImport("libImlib2.so")]
		public static extern void imlib_get_text_size(string text, out int width, out int height);

        [DllImport("libImlib2.so")]
		public static extern void imlib_free_font();

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_updates_merge_for_rendering(IntPtr updates, int w, int h);

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_updates_get_next(IntPtr updates);

        [DllImport("libImlib2.so")]
		public static extern void imlib_updates_get_coordinates(IntPtr updates, out int x, out int y, out int w, out int h);

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_create_image(int w, int h);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_blend(byte blend);

        [DllImport("libImlib2.so")]
		public static extern void imlib_blend_image_onto_image(IntPtr source, byte merge_alpha, int x, int y, int w, int h, int dest_x, int dest_y, int dest_w, int dest_h);

        [DllImport("libImlib2.so")]
		public static extern IntPtr imlib_create_color_range();

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_color_range(IntPtr color_range);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_color(int r, int g, int b, int a);

        [DllImport("libImlib2.so")]
		public static extern void imlib_add_color_to_color_range(int distance_away);

        [DllImport("libImlib2.so")]
		public static extern void imlib_image_fill_color_range_rectangle(int x, int y, int w, int h, double angle);

        [DllImport("libImlib2.so")]
		public static extern void imlib_free_color_range();

        [DllImport("libImlib2.so")]
		public static extern void imlib_text_draw(int x, int y, string text);

        [DllImport("libImlib2.so")]
		public static extern void imlib_updates_free(IntPtr updates);

        // [DllImport("libImlib2.so")]
		// public static extern ();

        // [DllImport("libImlib2.so")]
		// public static extern ();




        [DllImport("libImlib2.so")]
        public static extern IntPtr imlib_load_image(string filename);

        [DllImport("libImlib2.so")]
		public static extern void imlib_context_set_image();

        [DllImport("libImlib2.so")]
        public static extern int imlib_image_get_width();

        [DllImport("libImlib2.so")]
        public static extern int imlib_image_get_height();

        [DllImport("libImlib2.so")]
        public static extern void imlib_context_set_drawable(IntPtr pixmap);

        [DllImport("libImlib2.so")]
        public static extern void imlib_render_image_on_drawable(int x, int y);

    }
}