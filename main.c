/* 
 
  main -- Hello world with Xlib.
 
clang main.c -lX11 -o main
 
 */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <X11/Xlib.h>
#include <X11/Xutil.h>
 
 
int main(argc,argv)
     int argc;
     char **argv;
{
  char hello[] = "Hello World!";
  char hi[] = "hi!";
 
  /* setup display/screen */
  Display *mydisplay = XOpenDisplay("");
  
  int myscreen = DefaultScreen(mydisplay);
  unsigned long myforeground = BlackPixel(mydisplay, myscreen);
  unsigned long mybackground = WhitePixel(mydisplay, myscreen);
  XSizeHints myhint = {
    .x = 200, 
    .y = 300,
    .width = 350,
    .height = 250,
    .flags = PPosition|PSize
  };

 
  /* create window */
Window mywindow = XCreateSimpleWindow(mydisplay, DefaultRootWindow(mydisplay),
                                 myhint.x, myhint.y,
                                 myhint.width, myhint.height,
                                 5, myforeground, mybackground);
 
  /* window manager properties (yes, use of StdProp is obsolete) */
  XSetStandardProperties(mydisplay, mywindow, hello, hello,
                         None, argv, argc, &myhint);
 
  /* graphics context */
  GC mygc = XCreateGC(mydisplay, mywindow, 0, 0);
  XSetBackground(mydisplay, mygc, mybackground);
  XSetForeground(mydisplay, mygc, myforeground);
 
  /* allow receiving mouse events */
  XSelectInput(mydisplay,mywindow,
               ButtonPressMask|KeyPressMask|ExposureMask);
 
  /* show up window */
  XMapWindow(mydisplay, mywindow);
 
  XEvent myevent;
  KeySym mykey;
  char text[10];
  int done = 0;
  int i;
  while(done==0){
 
    /* fetch event */
    XNextEvent(mydisplay, &myevent);
    printf("XNextEvent = %x\n", myevent.type);
 
    switch(myevent.type){
      
    case Expose:
      /* Window was showed. */
      if(myevent.xexpose.count==0)
        printf("\tExpose\n");
        XDrawImageString(myevent.xexpose.display,
                         myevent.xexpose.window,
                         mygc, 
                         50, 50, 
                         hello, strlen(hello));
      break;
    case MappingNotify:
      /* Modifier key was up/down. */
      printf("\tMappingNotify\n");
      XRefreshKeyboardMapping((XMappingEvent*)&myevent);
      break;
    case ButtonPress:
      /* Mouse button was pressed. */
      printf("\tButtonPress\n");
      XDrawImageString(myevent.xbutton.display,
                       myevent.xbutton.window,
                       mygc, 
                       myevent.xbutton.x, myevent.xbutton.y,
                       hi, strlen(hi));
      break;
    case KeyPress:
      /* Key input. */
      printf("\tKeyPress\n");
      i = XLookupString((XKeyEvent*)&myevent, text, 10, &mykey, 0);
      if(i==1 && text[0]=='q') done = 1;
      break;
    }
  }
  
  /* finalization */
  XFreeGC(mydisplay,mygc);
  XDestroyWindow(mydisplay, mywindow);
  XCloseDisplay(mydisplay);
 
  exit(0);
}