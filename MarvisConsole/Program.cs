using System;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using System.Threading;
using System.Diagnostics;
using System.Timers;

namespace MarvisConsole {
    class Program {
        static float timestep = 0;

        static PanelEMG panelEMG = new PanelEMG();
        static PanelRaw panelRaw = new PanelRaw();
        static PanelMotion panelMotion = new PanelMotion();

        static System.Timers.Timer timerfps;
        static volatile int fpscounter = 0;
        
        static void SetupTimers() {
            timerfps = new System.Timers.Timer(1000);
            timerfps.Elapsed += Timerfps_Elapsed;
            timerfps.AutoReset = true;
            timerfps.Enabled = true;
        }

        private static void Timerfps_Elapsed(object sender, ElapsedEventArgs e) {
            //Console.WriteLine(fpscounter);
            fpscounter = 0;
        }

        static void init_graphics() {
            //Gl.glEnable(Gl.GL_LIGHTING);
            //Gl.glEnable(Gl.GL_LIGHT0);
            //float[] light_pos = new float[3] { 1, 0.5F, 1 };
            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_pos);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            //Gl.glEnable(Gl.GL_STENCIL_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            RendererWrapper.SetBlendMode(RendererWrapper.BlendModes.Normal);
            Gl.glClearColor(0.1f, 0.1f, 0.1f, 0.0f);
            Gl.glClearDepth(1.0);

            Wgl.wglSwapIntervalEXT(-1);
            
            Globals.thserial.Start();
            Globals.thapp.Start();
            Debug.Print(Wgl.wglGetSwapIntervalEXT().ToString());
        }

        static void on_display() {
            fpscounter++;
            timestep += 0.1f;
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT );
            Gl.glLoadIdentity();
            //Glu.gluLookAt(0, 0, 5, 0, 0, 1, 0, 1, 0);
            /*
            
            Glut.glutSolidTeapot(1);
            */
            Gl.glScaled(1.0/ Globals.defaultwindowwidth, 1.0/ Globals.defaultwindowheight, 1);
            RendererWrapper.currentdepth = 0.0f;
            Globals.currentwindowwidth = Glut.glutGet(Glut.GLUT_WINDOW_WIDTH);
            Globals.currentwindowheight = Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT);

            Globals.panelreg.Update();
            Globals.clickablereg.UpdateGraphics();
            Globals.appreg.UpdatePanels();

            //RendererWrapper.DrawRectangle(new RectangleBox<double>(100 + 50.0 * Math.Sin(timestep), 200 + 50.0 * Math.Sin(timestep), 100, 200), new RGBAColor(1.0, 0.0, 0.0, 1.0), 5, outer: true, glow: true);
            //RendererWrapper.DrawRectangle(new RectangleBox<double>(100+15, 200+15, 100+15, 200+15), new RGBAColor(0.0, 1.0, 0.0, 1.0),5);

            Glut.glutSwapBuffers();
            //Gl.glFinish();
        }

        static void idle() {
            Glut.glutPostRedisplay();
        }

        static void on_reshape(int w, int h) {
            RendererWrapper.Set2D(w, h);
        }

        static void on_mousemove(int x,int y) {
            Globals.mousex = (int)((double)x / Globals.currentwindowwidth * Globals.defaultwindowwidth);
            Globals.mousey = (int)((1.0 - (double)y / Globals.currentwindowheight) * Globals.defaultwindowheight);
            Globals.clickablereg.UpdateActions(Globals.mousex, Globals.mousey, ClickableArea.MouseAction.None);
            Globals.appreg.UpdateClickables(Globals.mousex, Globals.mousey, ClickableArea.MouseAction.None);
            //Console.WriteLine("X: {0:N} Y:{1:N}", Globals.mousex, Globals.mousey);
        }

        static void on_mouseclick(int button,int state,int x,int y) {
            if (button == Glut.GLUT_LEFT_BUTTON) {
                Globals.clickablereg.UpdateActions(Globals.mousex, Globals.mousey,
                    state == Glut.GLUT_DOWN ? ClickableArea.MouseAction.LeftDown : ClickableArea.MouseAction.LeftUp);
                Globals.appreg.UpdateClickables(Globals.mousex, Globals.mousey,
                    state == Glut.GLUT_DOWN ? ClickableArea.MouseAction.LeftDown : ClickableArea.MouseAction.LeftUp);
            } else if(button==Glut.GLUT_RIGHT_BUTTON) {
                Globals.clickablereg.UpdateActions(Globals.mousex, Globals.mousey,
                    state == Glut.GLUT_DOWN ? ClickableArea.MouseAction.RightDown : ClickableArea.MouseAction.RightUp);
                Globals.appreg.UpdateClickables(Globals.mousex, Globals.mousey,
                    state == Glut.GLUT_DOWN ? ClickableArea.MouseAction.RightDown : ClickableArea.MouseAction.RightUp);

            }
        }

        static void on_mousewheel(int wheel, int direction, int x, int y) {
            Globals.clickablereg.UpdateActions(Globals.mousex, Globals.mousey,
                wheel == -1 ? ClickableArea.MouseAction.WheelDown : ClickableArea.MouseAction.WheelUp);
            Globals.appreg.UpdateClickables(Globals.mousex, Globals.mousey,
                wheel == -1 ? ClickableArea.MouseAction.WheelDown : ClickableArea.MouseAction.WheelUp);
        }

        static void Main() {
            Glut.glutInit();
            Glut.glutSetOption(Glut.GLUT_MULTISAMPLE, 8);
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_RGBA | Glut.GLUT_DEPTH | Glut.GLUT_MULTISAMPLE);
            Glut.glutInitWindowSize(Globals.defaultwindowwidth, Globals.defaultwindowheight);
            Glut.glutCreateWindow("Marvis Console");
            SetupTimers();
            init_graphics();
            Glut.glutDisplayFunc(on_display);
            Glut.glutPassiveMotionFunc(on_mousemove);
            Glut.glutMotionFunc(on_mousemove);
            Glut.glutMouseFunc(on_mouseclick);
            Glut.glutMouseWheelFunc(on_mousewheel);
            Glut.glutIdleFunc(idle);
            Glut.glutReshapeFunc(on_reshape);
            Glut.glutMainLoop();
        }
    }
}