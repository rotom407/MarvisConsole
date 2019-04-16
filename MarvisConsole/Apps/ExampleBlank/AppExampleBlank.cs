using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class AppExampleBlank : AppBase {
        public List<PanelGroupApp> panels = new List<PanelGroupApp>();
        public List<ClickableArea> clickables = new List<ClickableArea>();
        public override List<PanelGroupApp> Panels { get => panels; set => throw new NotImplementedException(); }
        public override List<ClickableArea> Clickables { get => clickables; set => throw new NotImplementedException(); }
        const int appuid = 0x02;

        public bool enablemotion;
        void applymotion(ClickableArea o) {
            enablemotion = !enablemotion;
        }

        public AppExampleBlank() {
            panels.Add(new PanelExampleBlank());

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
