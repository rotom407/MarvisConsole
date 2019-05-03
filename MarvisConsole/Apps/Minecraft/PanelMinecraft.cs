using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMinecraft : PanelGroupApp {
        public PanelMinecraft() {
            caption = "Example";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        public override void DrawContents(DataRecord rec) {
            if (rec != null) {  //new record
                if (rec.content[0] == 0x02) {   //check appuid
                    throw new NotImplementedException();
                }
            }
        }
    }
}
