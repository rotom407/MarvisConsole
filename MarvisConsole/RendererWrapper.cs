using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace MarvisConsole {
    public class Particle {
        public bool aseyecandy = true;
        public enum ParticleShapes { Diamond,Cross};
        public ParticleShapes shape;
        public double x, y, xsp, ysp, xacc, yacc, friction;
        public int maxlife, life;
        public int lifefade;
        public double xscales, xscalef, yscales, yscalef;
        public double xscale, yscale;
        public RGBAColor cols;
        public RGBAColor col = new RGBAColor(1, 1, 1, 1);
        public RendererWrapper.BlendModes bm = RendererWrapper.BlendModes.Normal;
        public bool dead = false;
        public Particle(ParticleShapes shape_,RGBAColor col_, int maxlife_, double xscales_, double yscales_, double x_, double y_,
            double xsp_ = 0, double ysp_ = 0, double xacc_ = 0, double yacc_ = 0, double friction_ = 0, int lifefade_ = 0,
            double xscalef_=1,double yscalef_=1,RendererWrapper.BlendModes bm_=RendererWrapper.BlendModes.Normal,
            bool aseyecandy_=false) {
            aseyecandy = aseyecandy_;
            shape = shape_;
            cols = col_;
            maxlife = maxlife_;
            xscales = xscales_;yscales = yscales_;
            x = x_;y = y_;
            xsp = xsp_;ysp = ysp_;
            xacc = xacc_;yacc = yacc_;
            friction = friction_;
            lifefade = lifefade_;
            xscalef = xscalef_;yscalef = yscalef_;
            bm = bm_;
            col.R = cols.R;col.G = cols.G;col.B = cols.B;col.A = cols.A;
            life = maxlife;
            xscale = xscales;
            yscale = yscales;
        }
        public void Simulate() {
            life -= 1;
            double lifeperc = (double)life / maxlife;
            xscale = xscales * (lifeperc + (1 - lifeperc) * xscalef);
            yscale = yscales * (lifeperc + (1 - lifeperc) * yscalef);
            if (life < lifefade) {
                col.A = cols.A * life / lifefade;
            }
            if (life < 0) dead = true;
            xsp += xacc;ysp += yacc;
            xsp *= friction;ysp *= friction;
            x += xsp;y += ysp;
        }
    }
    public static class RendererWrapper {
        public enum BlendModes {Normal,Add};
        public static double currentdepth;
        public static void SetBlendMode(BlendModes bm) {
            switch (bm) {
                case BlendModes.Add:
                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
                    break;
                case BlendModes.Normal:
                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                    break;
            }
        }
        static int Mod(int x, int m) {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        public static void glColor4d(RGBAColor col) {
            Gl.glColor4d(col.R, col.G, col.B, col.A);
        }
        private static void UpdateDepth(double input) {
            if (input < 0.0) {
                currentdepth += 0.001;
            } else {
                currentdepth = input;
            }
        }
        public static void DrawRectangle(RectangleBox rect,RGBAColor col,double borderwidth, double depth = -1.0,bool outer=false,bool glow=false) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            glColor4d(col);
            double rectw = rect.right - rect.left, recth = rect.top - rect.bottom;
            double borderwidthr=outer?-borderwidth:borderwidth;
            if (borderwidth <= 0) {
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex2d(0, 0);
                Gl.glVertex2d(rectw, 0);
                Gl.glVertex2d(rectw, recth);
                Gl.glVertex2d(0, recth);
                Gl.glEnd();
            } else {
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex2d(0, 0);
                Gl.glVertex2d(rectw, 0);
                if (glow) glColor4d(col.Fade(0.0));
                Gl.glVertex2d(rectw-borderwidthr, borderwidthr);
                Gl.glVertex2d(borderwidthr, borderwidthr);
                Gl.glVertex2d(borderwidthr, recth- borderwidthr);
                if (glow) glColor4d(col);
                Gl.glVertex2d(0, recth);
                Gl.glEnd();
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex2d(rectw, recth);
                Gl.glVertex2d(rectw, 0);
                if (glow) glColor4d(col.Fade(0.0));
                Gl.glVertex2d(rectw - borderwidthr, borderwidthr);
                Gl.glVertex2d(rectw - borderwidthr, recth - borderwidthr);
                Gl.glVertex2d(borderwidthr, recth - borderwidthr);
                if (glow) glColor4d(col);
                Gl.glVertex2d(0, recth);
                Gl.glEnd();
            }
            Gl.glPopMatrix();
        }
        public static void DrawCaption(double height,double x,double y,String caption,RGBAColor col,double depth=-1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();

            double width = 30 + height + 8 * caption.Length;

            Gl.glTranslated(x, y, currentdepth);
            glColor4d(col);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(width, 0);
            Gl.glVertex2d(width-height, height);
            Gl.glVertex2d(0, height);
            Gl.glEnd();
            Gl.glPopMatrix();

            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(x+10.0, y+height*0.22, currentdepth);
            Gl.glScaled(0.10, 0.10, 1.0);
            glColor4d(new RGBAColor(1, 1, 1, 1));
            Gl.glLineWidth(1);
            foreach (char c in caption) {
                Gl.glTranslated(10, 0.0, 0.0);
                Glut.glutStrokeCharacter(Glut.GLUT_STROKE_ROMAN, c);
            }
            Gl.glPopMatrix();
        }
        public static void DrawBaseline(RectangleBox rect, double y, RGBAColor col,double depth = -1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            glColor4d(col);
            Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(0, y);
            Gl.glVertex2d(rect.right-rect.left, y);
            Gl.glEnd();
            Gl.glLineWidth(1);
            Gl.glPopMatrix();
        }
        public static void DrawChannelLabel(RectangleBox rect,double x,double y,RGBAColor col,string text,bool selected,double depth=-1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            Gl.glLineWidth(1);
            Gl.glTranslated(x, y, 0);

            glColor4d(new RGBAColor(0, 0, 0, 1));
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(-7, 7);
            Gl.glVertex2d(-30, 7);
            Gl.glVertex2d(-30, -7);
            Gl.glVertex2d(-7, -7);
            Gl.glVertex2d(0, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            Gl.glTranslated(x, y, 0);

            glColor4d(col);
            if (selected)
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            else
                Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(-7, 7);
            Gl.glVertex2d(-30, 7);
            Gl.glVertex2d(-30, -7);
            Gl.glVertex2d(-7, -7);
            Gl.glVertex2d(0, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            Gl.glTranslated(x-26, y-3, 0);
            Gl.glScaled(0.07, 0.07, 1.0);
            if(selected)
                glColor4d(new RGBAColor(0, 0, 0, 1));
            else
                glColor4d(col);
            Gl.glLineWidth(1);
            foreach (char c in text) {
                Gl.glTranslated(3, 0.0, 0.0);
                Glut.glutStrokeCharacter(Glut.GLUT_STROKE_ROMAN, c);
            }
            Gl.glPopMatrix();
        }
        public static void DrawString(double x,double y,string str,RGBAColor col,double depth=-1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(x, y, currentdepth);
            Gl.glScaled(0.10, 0.10, 1.0);
            Gl.glLineWidth(1);
            Gl.glColor3d(col.R, col.G, col.B);
            foreach (char c in str) {
                Gl.glTranslated(10, 0.0, 0.0);
                Glut.glutStrokeCharacter(Glut.GLUT_STROKE_ROMAN, c);
            }
            Gl.glPopMatrix();
        }

        public static void DrawEMGChannel(RectangleBox rect, double x, double y, RGBAColor col,ref CyclicBuffer<PanelEMGData>buf,int ch, bool drawlabel=false, double depth = -1.0) {
            List<PanelLabel> pl = new List<PanelLabel>();
            List<double> plx = new List<double>();
            double origdepth = currentdepth;
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left+x, rect.bottom, currentdepth);
            glColor4d(col);
            //Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            for (int i = 0; i < buf.maxlen; i++) {
                byte amp = buf[i].amp[ch];
                double xpos = (double)i / (buf.maxlen - 3) * rect.Width;  //width+INTERMID
                if (drawlabel) {
                    foreach (var lab in buf[i].labels) {
                        pl.Add(lab);
                        plx.Add(xpos);
                        break;
                    }
                } else {
                    glColor4d(col);
                }
                Gl.glVertex2d(xpos, y + (amp>0?0:0) + 0.1 * amp);
                Gl.glVertex2d(xpos, y - (amp>0?0:0) - 0.1 * amp);
            }
            Gl.glEnd();
            Gl.glLineWidth(1);
            Gl.glPopMatrix();

            if (drawlabel) {
                for(int i = 0; i < pl.Count; i++) {
                    UpdateDepth(depth);
                    Gl.glPushMatrix();
                    Gl.glTranslated(rect.left + x + plx[i], rect.bottom, origdepth+0.001/2);
                    Gl.glLineWidth(pl[i].bold);
                    glColor4d(pl[i].col);
                    Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex2d(0, 0);
                    Gl.glVertex2d(0, rect.Height);
                    Gl.glEnd();

                    Gl.glLineWidth(1);
                    Gl.glBegin(Gl.GL_LINE_STRIP);
                    Gl.glVertex2d(0, 0 + 15);
                    Gl.glVertex2d(7, 7 + 15);
                    Gl.glVertex2d(30, 7 + 15);
                    Gl.glVertex2d(30, -7 + 15);
                    Gl.glVertex2d(7, -7 + 15);
                    Gl.glVertex2d(0, 0 + 15);
                    Gl.glEnd();

                    Gl.glTranslated(10, 12, 0.0);
                    Gl.glScaled(0.07, 0.07, 1.0);
                    foreach (char c in pl[i].label) {
                        Gl.glTranslated(10, 0.0, 0.0);
                        Glut.glutStrokeCharacter(Glut.GLUT_STROKE_ROMAN, c);
                    }
                    Gl.glPopMatrix();

                }
            }
        }

        public static void DrawRawChannel(RectangleBox rect, double x, double y, RGBAColor col, ref CyclicBuffer<sbyte> buf, double depth = -1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left + x, rect.bottom, currentdepth);
            glColor4d(col);
            //Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < buf.maxlen; i++) {
                double perc = (double)Mod(i - buf.currentpos, buf.maxlen) / buf.maxlen;/*
                if (perc<0.1)
                    glColor4d(col.Fade(perc/0.1));
                else
                    glColor4d(col);*/
                sbyte amp = buf[i];
                double xpos = (double)i / (buf.maxlen - 50) * rect.Width;  //width+INTERMID
                Gl.glVertex2d(xpos, y + amp);
            }
            Gl.glEnd();
            Gl.glPopMatrix();
            /*
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left + x, rect.bottom, currentdepth);
            Gl.glLineWidth(3);
            SetBlendMode(BlendModes.Add);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < buf.maxlen; i++) {
                double perc = (double)Mod(i - buf.currentpos, buf.maxlen) / buf.maxlen;
                if (perc < 0.8)
                    glColor4d(new RGBAColor(1, 1, 1, 0));
                else
                    glColor4d(new RGBAColor(1, 1, 1, 1).Fade(5 * (perc - 0.8)));
                sbyte amp = buf.buf[i];
                double xpos = (double)i / (buf.maxlen - 50) * rect.Width;  //width+INTERMID
                Gl.glVertex2d(xpos, y + amp);
            }
            Gl.glEnd();
            SetBlendMode(BlendModes.Normal);
            Gl.glPopMatrix();*/
            Gl.glLineWidth(1);
        }

        public static void DrawGyroChannel(RectangleBox rect, double x, double y, ref CyclicBuffer<PanelMotionData> buf, double depth = -1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left + x, rect.bottom, currentdepth);
            Gl.glLineWidth(1);
            for (int ch = 0; ch < 3; ch++) {
                glColor4d(Globals.motionchannelcols[ch]);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (int i = 0; i < buf.maxlen; i++) {
                    short amp = buf[i].gyro[ch];
                    double xpos = (double)i / (buf.maxlen) * rect.Width;  //width+INTERMID
                    Gl.glVertex2d(xpos, y + 0.1*amp);
                }
                Gl.glEnd();
            }
            
            Gl.glPopMatrix();
        }

        public static void SetStencil(RectangleBox rect,bool on) {
            if (on) {
                Gl.glEnable(Gl.GL_STENCIL_TEST);
                //Gl.glEnable(Gl.GL_DEPTH_TEST);
                Gl.glStencilFunc(Gl.GL_ALWAYS, 1, 0xFF);
                Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE);
                Gl.glStencilMask(0xFF);
                Gl.glDepthMask(Gl.GL_FALSE);
                Gl.glClear(Gl.GL_STENCIL_BUFFER_BIT);

                UpdateDepth(-1);
                Gl.glColor4d(1.0, 1.0, 1, 0);
                Gl.glPushMatrix();
                Gl.glTranslated(rect.left, rect.bottom, currentdepth);
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex2d(0, 0);
                Gl.glVertex2d(0, rect.Height);
                Gl.glVertex2d(rect.Width, rect.Height);
                Gl.glVertex2d(rect.Width, 0);
                Gl.glEnd();
                Gl.glPopMatrix();

                Gl.glStencilFunc(Gl.GL_EQUAL, 1, 0xFF);
                Gl.glStencilMask(0x00);
                Gl.glDepthMask(Gl.GL_TRUE);
            } else {
                Gl.glDisable(Gl.GL_STENCIL_TEST);
            }
        }

        public static void Set3D(RectangleBox vp) {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glLoadIdentity();
            Gl.glViewport((int)(vp.left*Globals.currentwindowwidth/Globals.defaultwindowwidth),
                (int)(vp.bottom * Globals.currentwindowheight / Globals.defaultwindowheight),
                (int)(vp.Width * Globals.currentwindowwidth / Globals.defaultwindowwidth),
                (int)(vp.Height * Globals.currentwindowheight / Globals.defaultwindowheight));
            Glu.gluPerspective(40, vp.Width/vp.Height,1.5, 64);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            //Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 5, 0, 0, 1, 0, 1, 0);
            Gl.glTranslated(0, 0, 2);
        }

        public static void Set2D(int w,int h) {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glViewport(0, 0, w, h);
            Glu.gluOrtho2D(0, 1, 0, 1);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glScaled(1.0 / Globals.defaultwindowwidth, 1.0 / Globals.defaultwindowheight, 1);
        }

        public static void Set2D() {
            Set2D(Globals.currentwindowwidth, Globals.currentwindowheight);
        }

        public static void DrawPlate3D(double xpos,double ypos,double xang,double yang,double zang,RGBAColor col) {
            glColor4d(col.Mix(col, new RGBAColor(0, 0, 0, 1), 0.2));
            Gl.glPushMatrix();
            Gl.glTranslated(xpos, ypos, 0.7);
            Gl.glRotated(xang, 1, 0, 0);
            Gl.glRotated(yang, 0, 1, 0);
            Gl.glRotated(zang, 0, 0, 1);
            Gl.glScaled(0.7, 0.1, 0.7);
            Glut.glutSolidCube(1);
            Gl.glLineWidth(2);
            glColor4d(col);
            Glut.glutWireCube(1);
            Gl.glPopMatrix();
        }

        public static void DrawCoordinate(RectangleBox rect,RGBAColor col,double depth = -1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            glColor4d(col);
            Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(0, rect.Height / 2);
            Gl.glVertex2d(rect.right - rect.left, rect.Height / 2);
            Gl.glVertex2d(rect.Width / 2, rect.Height);
            Gl.glVertex2d(rect.Width / 2, 0);
            Gl.glEnd();
            Gl.glLineWidth(1);
            Gl.glPopMatrix();
        }

        public static void DrawMarkers(RectangleBox rect,List<Point2D> pts,RGBAColor col,int style,double size,double depth = -1.0) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            glColor4d(col);

            switch (style) {
                case 0://pts
                    foreach(var pt in pts) {
                        Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                        Gl.glVertex2d(pt.x - size / 2, pt.y);
                        Gl.glVertex2d(pt.x, pt.y + size / 2);
                        Gl.glVertex2d(pt.x + size / 2, pt.y);
                        Gl.glVertex2d(pt.x, pt.y - size / 2);
                        Gl.glEnd();
                    }
                    break;
                case 1://cross
                    Gl.glBegin(Gl.GL_LINES);
                    Gl.glLineWidth(1.0f);
                    foreach (var pt in pts) {
                        Gl.glVertex2d(pt.x - size / 2, pt.y - size / 2);
                        Gl.glVertex2d(pt.x + size / 2, pt.y + size / 2);
                        Gl.glVertex2d(pt.x - size / 2, pt.y + size / 2);
                        Gl.glVertex2d(pt.x + size / 2, pt.y - size / 2);
                    }
                    Gl.glEnd();
                    break;
                case 2://crosshair
                    Gl.glBegin(Gl.GL_LINES);
                    Gl.glLineWidth(1.0f);
                    foreach (var pt in pts) {
                        Gl.glVertex2d(pt.x - size / 2, pt.y);
                        Gl.glVertex2d(pt.x + size / 2, pt.y);
                        Gl.glVertex2d(pt.x, pt.y + size / 2);
                        Gl.glVertex2d(pt.x, pt.y - size / 2);
                    }
                    Gl.glEnd();
                    break;
                default:
                    break;
            }
            Gl.glPopMatrix();
        }

        public static void DrawArrow(RectangleBox rect,Point2D p1,Point2D p2,RGBAColor col,double thickness=1.0, double headsize=15.0, double depth = -1.0) {
            double ang = Math.Atan2(p2.y - p1.y, p2.x - p1.x);
            double siz = Math.Min(headsize, Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y)));

            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            glColor4d(col);
            Gl.glLineWidth((float)thickness);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(p1.x, p1.y);
            Gl.glVertex2d(p2.x - 0.5*siz * Math.Cos(ang), p2.y - 0.5*siz * Math.Sin(ang));
            Gl.glEnd();

            UpdateDepth(depth);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex2d(p2.x, p2.y);
            Gl.glVertex2d(p2.x - siz * Math.Cos(ang + 0.3), p2.y - siz * Math.Sin(ang + 0.3));
            Gl.glVertex2d(p2.x - siz * Math.Cos(ang - 0.3), p2.y - siz * Math.Sin(ang - 0.3));
            Gl.glEnd();

            Gl.glPopMatrix();
        }

        public static void DrawEffectExcitation(RectangleBox rect, RGBAColor col, double intensity, double depth = -1.0) {
            if (Globals.enableeffects) {
                UpdateDepth(depth);
                SetBlendMode(BlendModes.Add);
                Gl.glPushMatrix();
                Gl.glTranslated(rect.left, rect.bottom, currentdepth);
                double left = rect.Width * (1.0 - intensity);
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                glColor4d(col.Fade(0));
                Gl.glVertex2d(left, 0);
                Gl.glVertex2d(left, rect.Height);
                glColor4d(col.Fade(0.5));
                Gl.glVertex2d(0.2 * left + 0.8 * rect.Width, rect.Height);
                Gl.glVertex2d(0.2 * left + 0.8 * rect.Width, 0);
                Gl.glEnd();
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                glColor4d(col.Fade(0.5));
                Gl.glVertex2d(0.2 * left + 0.8 * rect.Width, 0);
                Gl.glVertex2d(0.2 * left + 0.8 * rect.Width, rect.Height);
                glColor4d(col);
                Gl.glVertex2d(rect.Width, rect.Height);
                Gl.glVertex2d(rect.Width, 0);
                Gl.glEnd();
                Gl.glPopMatrix();
                SetBlendMode(BlendModes.Normal);
            }
        }

        public static void DrawParticles(RectangleBox rect, List<Particle> particles, double depth = -1.0,bool simulate=true) {
            if (simulate) {
                foreach(var p in particles) {
                    p.Simulate();
                }
                particles.RemoveAll(x => x.dead);
            }
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            foreach (var p in particles) {
                glColor4d(p.col);
                SetBlendMode(p.bm);
                switch (p.shape) {
                    case Particle.ParticleShapes.Cross:
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glLineWidth(1.0f);
                        Gl.glVertex2d(p.x - p.xscale / 2, p.y - p.yscale / 2);
                        Gl.glVertex2d(p.x + p.xscale / 2, p.y + p.yscale / 2);
                        Gl.glVertex2d(p.x - p.xscale / 2, p.y + p.yscale / 2);
                        Gl.glVertex2d(p.x + p.xscale / 2, p.y - p.yscale / 2);
                        Gl.glEnd();
                        break;
                    case Particle.ParticleShapes.Diamond:
                        Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                        Gl.glVertex2d(p.x - p.xscale / 2, p.y);
                        Gl.glVertex2d(p.x, p.y + p.yscale / 2);
                        Gl.glVertex2d(p.x + p.xscale / 2, p.y);
                        Gl.glVertex2d(p.x, p.y - p.yscale / 2);
                        Gl.glEnd();
                        break;
                }
            }
            SetBlendMode(BlendModes.Normal);
            Gl.glEnd();
            Gl.glPopMatrix();
        }

        public static void DrawPieMark(RectangleBox rect,double xpos,double ypos,double size,double percent,RGBAColor col,string text,double depth = -1.0,bool bold=false) {
            UpdateDepth(depth);
            Gl.glPushMatrix();
            Gl.glTranslated(rect.left, rect.bottom, currentdepth);
            Gl.glPushMatrix();
            Gl.glTranslated(xpos, ypos, 0);
            glColor4d(col);
            if (bold)
                Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for(int i = 0; i <= 32; i++) {
                Gl.glVertex2d(-size * Math.Sin((double)i / 32 * 2 * Math.PI), size * Math.Cos((double)i / 32 * 2 * Math.PI));
            }
            Gl.glEnd();
            Gl.glLineWidth(1);
            glColor4d(col.Fade(0.5));
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex2d(0, 0);
            for (int i = 0; i <= 32 * percent; i++) {
                Gl.glVertex2d(-0.8 * size * Math.Sin((double)i / 32 * 2 * Math.PI), 0.8 * size * Math.Cos((double)i / 32 * 2 * Math.PI));
            }
            Gl.glEnd();
            Gl.glPopMatrix();
            DrawString(xpos-5, ypos-5, text, new RGBAColor(1, 1, 1, 1));
            Gl.glPopMatrix();
        }
    }
}
