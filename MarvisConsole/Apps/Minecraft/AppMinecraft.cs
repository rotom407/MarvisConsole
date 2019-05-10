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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
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
        const string mctitle = "Minecraft";

        public bool enablemotion;
        public bool onwindow;
        public int onwindowchecknum = 0;
        void applymotion(ClickableArea o,bool right) {
            enablemotion = !enablemotion;
        }

        public AppMinecraft() {
            panels.Add(new PanelMinecraftView());
            panels.Add(new PanelMinecraftAction());

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
        public const double mousexspfactor = 1.0, mouseyspfactor = 0.0;
        public const double mousedeadzone = 30, mousedeadzonefactor = 3.0;
        public const double moveyspthresh = 6, movexspthresh = 8;
        public double moveyspoffset = 0, movexspoffset = 0;
        public const double moveoffsetadjspd = 0.01;
        public int moveforwardcd = 0;

        public int actionstate = 0;//0 idle 1 L 2 R 3 A 4 D
        public int actioncooldown = 0, actioncooldownmax = 10;
        public int Dchcount = 0;

        public override void Run(DataRecord rec) {
            if (rec != null) {  //valid data
                DataRecordRaw drr = new DataRecordRaw(rec); //translation

                //calculate motions
                mousexsp = 0.5 * mousexsp + 0.5 * (double)drr.gyro[0, 0] / 16384 * 10000 * mousexspfactor;
                mouseysp = 0.5 * mouseysp + 0.5 * (double)drr.gyro[0, 2] / 16384 * 10000 * mouseyspfactor;

                moveyspd = -(double)drr.accelmeter[0, 2] / 16384 * 4000;
                movexspd = -(double)drr.accelmeter[0, 1] / 16384 * 4000;

                if (movexstate == 0) {
                    movexspoffset = (1 - moveoffsetadjspd) * movexspoffset + moveoffsetadjspd * movexspd;
                }
                if (moveystate == 0) {
                    moveyspoffset = (1 - moveoffsetadjspd) * moveyspoffset + moveoffsetadjspd * moveyspd;
                }

                if ((moveyspd - moveyspoffset) > moveyspthresh && moveystate <= 0) {
                    moveystate = 1;
                    moveforwardcd = 7;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                    }
                }
                if (moveforwardcd > 0) {
                    moveforwardcd--;
                }

                if (((moveyspd - moveyspoffset) < 0.9 * moveyspthresh && moveystate == 1 && moveforwardcd==0) || ((moveyspd - moveyspoffset) > -0.9 * moveyspthresh && moveystate == -1)) {
                    moveystate = 0;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                    }
                }

                if ((moveyspd - moveyspoffset) < -moveyspthresh && moveystate >= 0) {
                    moveystate = -1;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                        isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_S);
                    }
                }

                if ((movexspd - movexspoffset) > movexspthresh && movexstate <= 0) {
                    movexstate = 1;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_A);
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                    }
                }

                if (((movexspd - movexspoffset) < 0.9 * movexspthresh && movexstate == 1 && moveforwardcd == 0) || ((movexspd - movexspoffset) > -0.9 * movexspthresh && movexstate == -1)) {
                    movexstate = 0;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                    }
                }

                if ((movexspd - movexspoffset) < -movexspthresh && movexstate >= 0) {
                    movexstate = -1;
                    if (enablemotion && onwindow) {
                        isim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                        isim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_D);
                    }
                }

                if (Math.Abs(mousexsp) < mousedeadzone) {
                    bool neg = mousexsp < 0;
                    if (actionstate != 0) mousexsp /= 4;
                    mousexsp = Math.Pow(Math.Abs(mousexsp) / mousedeadzone, mousedeadzonefactor) * mousedeadzone;
                    if (neg) mousexsp = -mousexsp;
                }

                //actions
                const int Lch = 0, Rch = 1, Ach = 2, Dch = 3, ECGRch = 4;
                switch (actionstate) {
                    case 0:
                        if (actioncooldown == 0) {
                            if (drr.emgamplitude[Ach] > 0) {
                                actioncooldown = actioncooldownmax = 10;
                                actionstate = 3;
                                AppUtils.AddLabelToEMGPanel(new PanelLabel("A", Globals.emgchannelcols[8], 0));
                                if (enablemotion && onwindow) {
                                    isim.Mouse.LeftButtonDown();
                                }
                            }else if (drr.emgamplitude[Lch] > 10) {
                                actioncooldown = actioncooldownmax = 40;
                                actionstate = 1;
                                AppUtils.AddLabelToEMGPanel(new PanelLabel("L", Globals.emgchannelcols[8], 0));
                                if (enablemotion && onwindow) {
                                    isim.Mouse.VerticalScroll(1);
                                }
                            } else if (drr.emgamplitude[Rch] > 10) {
                                actioncooldown = actioncooldownmax = 40;
                                actionstate = 2;
                                AppUtils.AddLabelToEMGPanel(new PanelLabel("R", Globals.emgchannelcols[8], 0));
                                if (enablemotion && onwindow) {
                                    isim.Mouse.VerticalScroll(-1);
                                }
                            }
                            if ((drr.emgamplitude[Dch]) > 25) {
                                Dchcount++;
                                if (Dchcount > 5) {
                                    Dchcount = 0;
                                    actioncooldown = actioncooldownmax = 10;
                                    actionstate = 4;
                                    AppUtils.AddLabelToEMGPanel(new PanelLabel("D", Globals.emgchannelcols[8], 0));
                                    if (enablemotion && onwindow) {
                                        isim.Mouse.RightButtonDown();
                                    }
                                }
                            } else {
                                Dchcount=0;
                            }
                        }
                        break;
                    case 1:
                    case 2:
                        if (actioncooldown == 0) {
                            actioncooldown = actioncooldownmax = 10;
                            actionstate = 0;
                        }
                        break;
                    case 3:
                        if (drr.emgamplitude[Ach] > 0) {
                            actioncooldown = actioncooldownmax = 10;
                            actionstate = 3;
                        } else if (actioncooldown == 0) {
                            actioncooldown = actioncooldownmax = 10;
                            actionstate = 0;
                            if (enablemotion && onwindow) {
                                isim.Mouse.LeftButtonUp();
                            }
                        }
                        break;
                    case 4:
                        if ((drr.emgamplitude[Dch]) > 25) {
                            actioncooldown = actioncooldownmax = 10;
                            actionstate = 4;
                        } else if (actioncooldown == 0) {
                            actioncooldown = actioncooldownmax = 10;
                            actionstate = 0;
                            if (enablemotion && onwindow) {
                                isim.Mouse.RightButtonUp();
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            //send to panels
            List<byte> txbytes = new List<byte> { appuid };
            txbytes.Add(mouseenable ? (byte)0x01 : (byte)0x00);
            txbytes.Add(AppUtils.ValueMapToByte(mousexsp, -50, 50));
            txbytes.Add(AppUtils.ValueMapToByte(mouseysp, -50, 50));
            txbytes.Add(AppUtils.ValueMapToByte(movexspd, -20, 20));
            txbytes.Add(AppUtils.ValueMapToByte(moveyspd, -20, 20));
            txbytes.Add(AppUtils.ValueMapToByte(movexspoffset, -20, 20));
            txbytes.Add(AppUtils.ValueMapToByte(moveyspoffset, -20, 20));
            txbytes.Add((byte)actionstate);
            txbytes.Add(AppUtils.ValueMapToByte((double)actioncooldown / actioncooldownmax, 0, 1));
            DataRecord txdr = new DataRecord(0x00, txbytes);
            Globals.appdatbuf.Push(txdr);


            if (actioncooldown > 0) {
                actioncooldown--;
            }

            //check on window
            onwindowchecknum++;
            if (onwindowchecknum > 10) {
                onwindowchecknum = 0;
                string title = ActiveApplTitle();
                //Console.WriteLine(title);
                //Console.WriteLine(string.Compare(title, mctitle));
                onwindow = string.Compare(title, mctitle) > 0;
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
