using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMouseSpeed : PanelGroupApp {
        public double t=0.0;
        public double xspp, yspp;
        public double xsp, ysp;
        public PanelMouseSpeed() {
            caption = "Cursor Speed";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200 - Globals.panelspacingcolumn - 200,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200 - Globals.panelspacingcolumn);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        public override void DrawContents(DataRecord rec) {
            if (rec != null) {
                if (rec.content[0] == 0x01) {
                    xspp = (rec.content[1] - 128) / 10.0;
                    yspp = (rec.content[2] - 128) / 10.0;
                }
            }
            xsp = 0.8 * xsp + 0.2 * xspp;
            ysp = 0.8 * ysp + 0.2 * yspp;
            RendererWrapper.DrawCoordinate(boundingbox, new RGBAColor(1, 1, 1, 0.3));
            RendererWrapper.DrawArrow(boundingbox, new Point2D(boundingbox.Width / 2, boundingbox.Height / 2),
                new Point2D(boundingbox.Width / 2 + 7 * xsp, boundingbox.Height / 2 + 7 * ysp),
                Globals.emgchannelcols[8], thickness:2,headsize:25);
        }
    }
}
