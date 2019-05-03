using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMinecraftView : PanelGroupApp {
        public PanelMinecraftView() {
            caption = "Example";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        double xspp = 0.0, yspp = 0.0;
        double xsp = 0.0, ysp = 0.0;
        public override void DrawContents(DataRecord rec) {
            if (rec != null) {  //new record
                if (rec.content[0] == 0x09) {   //check appuid
                    if (rec.content[1] == 0x01) {//mouse enabled
                        xspp = AppUtils.ValueMapToDouble(rec.content[2], -15, 15);
                        yspp = AppUtils.ValueMapToDouble(rec.content[3], -15, 15);
                    }
                }
            }
            xsp = 0.8 * xsp + 0.2 * xspp;
            ysp = 0.8 * ysp + 0.2 * yspp;
            RendererWrapper.DrawCoordinate(boundingbox, new RGBAColor(1, 1, 1, 0.3));
            RendererWrapper.DrawArrow(boundingbox, new Point2D(boundingbox.Width / 2, boundingbox.Height / 2),
                new Point2D(boundingbox.Width / 2 + 7 * xsp, boundingbox.Height / 2 + 7 * ysp),
                Globals.emgchannelcols[8], thickness: 2, headsize: 25);
        }
    }
}
