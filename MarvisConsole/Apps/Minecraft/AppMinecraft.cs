using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace MarvisConsole {
    public class AppMinecraft : AppBase {

        [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);
        [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public InputSimulator isim = new InputSimulator();

        public static string ActiveApplTitle() {
            //This method is used to get active application's title using GetWindowText() method present in user32.dll
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd.Equals(IntPtr.Zero)) return "";
            string lpText = new string((char)0, 100);
            int intLength = GetWindowText(hwnd, lpText, lpText.Length);
            if ((intLength <= 0) || (intLength > lpText.Length)) return "unknown";
            return lpText.Trim();
        }

        public List<PanelGroupApp> panels = new List<PanelGroupApp>();
        public List<ClickableArea> clickables = new List<ClickableArea>();
        public override List<PanelGroupApp> Panels { get => panels; set => throw new NotImplementedException(); }
        public override List<ClickableArea> Clickables { get => clickables; set => throw new NotImplementedException(); }
        const int appuid = 0x09;
        const string mctitle = "Minecraft 1.7.10";

        public bool enablemotion;
        public bool onwindow;
        public int onwindowchecknum = 0;
        void applymotion(ClickableArea o,bool right) {
            enablemotion = !enablemotion;
        }

        public AppMinecraft() {
            panels.Add(new PanelMinecraftView());

            ClickableButton btnapply = new ClickableButton(new RectangleBox(
                (Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio * 1.1,
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

        private void MoveCursor(int dx, int dy) {
            //Cursor = new System.Windows.Forms.Cursor(System.Windows.Forms.Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X + dx, Cursor.Position.Y + dy);
        }

        public void DoMouseClick(uint action) {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(action, X, Y, 0, 0);
        }

        //app specific
        public bool mouseenable = true;
        public double mousexsp = 0.0, mouseysp = 0.0;
        public double mousexremain = 0, mouseyremain = 0;
        public double movexspd = 0.0, moveyspd = 0.0;
        public int movexstate = 0, moveystate = 0;
        public const double mousexspfactor = -1.0, mouseyspfactor = 0.0;
        public const double mousedeadzone = 30, mousedeadzonefactor = 3.0;
        public const double moveyspthresh = 5, movexspthresh = 12;
        public int moveforwardcd = 0;

        public override void Run(DataRecord rec) {
            if (rec != null) {  //valid data
                DataRecordRaw drr = new DataRecordRaw(rec); //translation

                //calculate motions
                mousexsp = (double)drr.gyro[0, 0] / 16384 * 8000 * mousexspfactor;
                mouseysp = (double)drr.gyro[0, 2] / 16384 * 8000 * mouseyspfactor;

                moveyspd = -(double)drr.accelmeter[0, 2] / 16384 * 4000;
                movexspd = (double)drr.accelmeter[0, 1] / 16384 * 4000;

                if (moveyspd > moveyspthresh && moveystate <= 0) {
                    moveystate = 1;
                    moveforwardcd = 7;
                    isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                }
                if (moveforwardcd > 0) {
                    moveforwardcd--;
                }

                if ((moveyspd < 0.9 * moveyspthresh && moveystate == 1 && moveforwardcd==0) || (moveyspd > -0.9 * moveyspthresh && moveystate == -1)) {
                    moveystate = 0;
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                }

                if (moveyspd < -moveyspthresh && moveystate >= 0) {
                    moveystate = -1;
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                    isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_S);
                }

                if (movexspd > movexspthresh && movexstate <= 0) {
                    movexstate = 1;
                    isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_A);
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                }

                if ((movexspd < 0.9 * movexspthresh && movexstate == 1 && moveforwardcd == 0) || (movexspd > -0.9 * movexspthresh && movexstate == -1)) {
                    movexstate = 0;
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                }

                if (movexspd < -movexspthresh && movexstate >= 0) {
                    movexstate = -1;
                    isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                    isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_D);
                }

                if (Math.Abs(mousexsp) < mousedeadzone) {
                    bool neg = mousexsp < 0;
                    mousexsp = Math.Pow(Math.Abs(mousexsp) / mousedeadzone, mousedeadzonefactor) * mousedeadzone;
                    if (neg) mousexsp = -mousexsp;
                }

                //send to panels
                List<byte> txbytes = new List<byte> { appuid };
                txbytes.Add(mouseenable ? (byte)0x01 : (byte)0x00);
                txbytes.Add(AppUtils.ValueMapToByte(mousexsp, -30, 30));  //exampledata
                txbytes.Add(AppUtils.ValueMapToByte(mouseysp, -30, 30));
                DataRecord txdr = new DataRecord(0x00, txbytes);
                Globals.appdatbuf.Push(txdr);
            }

            //check on window
            onwindowchecknum++;
            if (onwindowchecknum > 10) {
                onwindowchecknum = 0;
                string title = ActiveApplTitle();
                //Console.WriteLine(title);
                onwindow = string.Compare(title, mctitle) == 0;
            }

            //apply motions
            if (enablemotion && onwindow) {
                if (mouseenable) {
                    mousexremain += 1 * mousexsp;
                    mouseyremain += 1 * mouseysp;
                    MoveCursor((int)mousexremain, -(int)mouseyremain);
                    mousexremain -= (int)mousexremain;
                    mouseyremain -= (int)mouseyremain;
                }
            }
        }
    }
}
