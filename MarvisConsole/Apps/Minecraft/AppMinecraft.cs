using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class AppMinecraft : AppBase {

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();
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
        const int appuid = 0x02;
        const string mctitle = "Minecraft 1.7.10";

        public bool enablemotion;
        public bool onwindow;
        public int onwindowchecknum = 0;
        void applymotion(ClickableArea o,bool right) {
            enablemotion = !enablemotion;
        }

        public AppMinecraft() {
            panels.Add(new PanelMinecraft());

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
        
        public override void Run(DataRecord rec) {
            if (rec != null) {  //valid data
                DataRecordRaw drr = new DataRecordRaw(rec); //translation
                onwindowchecknum++;
                if (onwindowchecknum > 10) {
                    onwindowchecknum = 0;
                    onwindow = ActiveApplTitle() == mctitle;
                }
                if (enablemotion && onwindow) {
                    
                }
            }
            /** to send data to app panels
            List<byte> txbytes = new List<byte> { appuid };
            txbytes.Add(0x01);  //exampledata
            DataRecord txdr = new DataRecord(0x00, txbytes);
            Globals.appdatbuf.Push(txdr);
            */
        }
    }
}
