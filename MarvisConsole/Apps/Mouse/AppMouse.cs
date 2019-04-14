using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarvisConsole {

    public class MouseFilter {
        public double trend;
        public double smooth;
        public MouseFilter() {
            trend = smooth = 0.0f;
        }
        public double Feed(double input) {
            trend = 0.99 * trend + 0.01 * input;
            smooth = 0.5 * smooth + 0.5 * input;
            return smooth - trend;
        }
    }
    public class MouseTrigger {
        public double max;
        public int laststate;
        public bool schmit = false;
        public MouseTrigger() {
            laststate = 0;
            max = 0;
        }
        public int Feed(byte input) { //1:0-1 -1:1-0 0:stay
            if (input > 0) {
                if (laststate < 5) laststate++;
                if (laststate == 3 && schmit == false) {
                    schmit = true;
                    return 1;
                }
            } else {
                if (laststate > 0) laststate--;
                if (laststate == 1 && schmit == true) {
                    schmit = false;
                    return -1;
                }
            }
            return 0;
        }
    }
    public class AppMouse : AppBase {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public List<PanelGroupApp> panels = new List<PanelGroupApp>();
        public List<ClickableArea> clickables = new List<ClickableArea>();
        public override List<PanelGroupApp> Panels { get => panels; set => throw new NotImplementedException(); }
        public override List<ClickableArea> Clickables { get => clickables; set => throw new NotImplementedException(); }

        public MouseFilter facx = new MouseFilter(), facz = new MouseFilter();
        public MouseTrigger ltr = new MouseTrigger(), rtr = new MouseTrigger();
        public bool enablemotion = false;
        private double xremain = 0.0, yremain = 0.0;
        public double xsp = 0.0, ysp = 0.0;

        private void MoveCursor(int dx, int dy) {
            //Cursor = new System.Windows.Forms.Cursor(System.Windows.Forms.Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X + dx, Cursor.Position.Y + dy);
        }

        public void DoMouseClick(uint action) {
            //Call the imported function with the cursor's current position
            int X = (int)Cursor.Position.X;
            int Y = (int)Cursor.Position.Y;
            mouse_event(action, X, Y, 0, 0);
        }

        void applymotion() {
            enablemotion = !enablemotion;
        }
        
        public AppMouse() {
            panels.Add(new PanelMouseSpeed());
            panels.Add(new PanelMouseLocation());

            ClickableButton btnapply = new ClickableButton(new RectangleBox(
                (Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio*1.1,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 - 30,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470
                ));
            btnapply.inapp = true;
            btnapply.boundingbox.left = btnapply.boundingbox.right - 1;
            btnapply.col = Globals.guicolors[1];
            btnapply.border = false;
            btnapply.caption = "Apply";
            btnapply.MouseDown = applymotion;
            clickables.Add(btnapply);
        }

        public override void Run(DataRecord rec) {
            if (rec != null) {
                DataRecordRaw drr = new DataRecordRaw(rec);
                List<byte> bytes = new List<byte> { 0x01 };
                double tmpxsp, tmpysp, norm;
                //Console.WriteLine(drr.emgamplitude[0]);
                tmpxsp = -0.2 * facx.Feed(drr.accelmeter[0, 1]);
                tmpysp = -0.2 * facz.Feed(drr.accelmeter[0, 2]);
                //mouse_event(0, 100, 0, 0, 0);
                norm = Math.Sqrt(tmpxsp * tmpxsp + tmpysp * tmpysp);
                double deadzone = 3;
                if (norm >= deadzone) {
                    tmpxsp *= Math.Pow((norm - deadzone) / 1.5, 1.3) / norm;
                    tmpysp *= Math.Pow((norm - deadzone) / 1.5, 1.3) / norm;
                } else {
                    tmpxsp = tmpysp = 0.0;
                }
                xsp = tmpxsp;
                ysp = tmpysp;
                int xb, yb;
                xb = 128 + (int)(10 * tmpxsp);
                if (xb > 255) xb = 255;if (xb < 0) xb = 0;
                yb = 128 + (int)(10 * tmpysp);
                if (yb > 255) yb = 255;if (yb < 0) yb = 0;
                bytes.Add((byte)xb);
                bytes.Add((byte)yb);

                int ltrr = ltr.Feed(drr.emgamplitude[0]);
                switch (ltrr) {
                    case 1:
                        bytes.Add(0x01);
                        AppUtils.AddLabelToEMGPanel(new PanelLabel("LD", new RGBAColor(1, 0.5, 0, 1), 0, bold_: 2));
                        if (enablemotion) DoMouseClick(MOUSEEVENTF_LEFTDOWN);
                        break;
                    case -1:
                        bytes.Add(0x02);
                        AppUtils.AddLabelToEMGPanel(new PanelLabel("LU", new RGBAColor(1, 0.5, 0, 0.5), 0));
                        if (enablemotion) DoMouseClick(MOUSEEVENTF_LEFTUP);
                        break;
                    default:
                        bytes.Add(0x00);
                        break;
                }
                int rtrr = rtr.Feed(drr.emgamplitude[1]);
                switch (rtrr) {
                    case 1:
                        bytes.Add(0x01);
                        AppUtils.AddLabelToEMGPanel(new PanelLabel("RD", new RGBAColor(0, 0.3, 1, 1), 0, bold_: 2));
                        if (enablemotion) DoMouseClick(MOUSEEVENTF_RIGHTDOWN);
                        break;
                    case -1:
                        bytes.Add(0x02);
                        AppUtils.AddLabelToEMGPanel(new PanelLabel("RU", new RGBAColor(0, 0.3, 1, 0.5), 0));
                        if (enablemotion) DoMouseClick(MOUSEEVENTF_RIGHTUP);
                        break;
                    default:
                        bytes.Add(0x00);
                        break;
                }
                DataRecord dr = new DataRecord(0x00, bytes);
                Globals.appdatbuf.Push(dr);
            }
            if (enablemotion) {
                xremain += 1 * xsp;
                yremain += 1 * ysp;
                MoveCursor((int)xremain, -(int)yremain);
                xremain -= (int)xremain;
                yremain -= (int)yremain;
            }
        }
    }
}
