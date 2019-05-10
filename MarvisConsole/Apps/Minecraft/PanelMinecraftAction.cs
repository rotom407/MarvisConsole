using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMinecraftAction : PanelGroupApp {
        RGBAColor coldisable = new RGBAColor(0.5, 0.5, 0.5, 1);

        public PanelMinecraftAction() {
            caption = "Action";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 150 - Globals.panelspacingcolumn - 200,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200 - Globals.panelspacingcolumn);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        int state = 0;
        double percent = 0.7;

        public override void DrawContents(DataRecord rec) {
            if (rec != null) {  //new record
                if (rec.content[0] == 0x09) {   //check appuid
                    state = rec.content[8];
                    percent = AppUtils.ValueMapToDouble(rec.content[9], 0, 1);
                }
            }
            const double spacingy = 0.25,spacingx=0.2;
            RendererWrapper.DrawPieMark(boundingbox, boundingbox.Width / 2 - spacingx * 1.5 * boundingbox.Width,
                boundingbox.Height / 2 + spacingy * boundingbox.Height,
                16, state == 0 ? percent : 0, state == 0 ? Globals.emgchannelcols[8] : coldisable, "0");

            RendererWrapper.DrawPieMark(boundingbox, boundingbox.Width / 2 - spacingx * 1.5 * boundingbox.Width,
                boundingbox.Height / 2 - spacingy * boundingbox.Height,
                16, state == 1 ? percent : 0, state == 1 ? Globals.emgchannelcols[8] : coldisable, "L");

            RendererWrapper.DrawPieMark(boundingbox, boundingbox.Width / 2 - spacingx * 0.5 * boundingbox.Width,
                boundingbox.Height / 2 - spacingy * boundingbox.Height,
                16, state == 2 ? percent : 0, state == 2 ? Globals.emgchannelcols[8] : coldisable, "R");

            RendererWrapper.DrawPieMark(boundingbox, boundingbox.Width / 2 + spacingx * 0.5 * boundingbox.Width,
                boundingbox.Height / 2 - spacingy * boundingbox.Height,
                16, state == 3 ? percent : 0, state == 3 ? Globals.emgchannelcols[8] : coldisable, "A");

            RendererWrapper.DrawPieMark(boundingbox, boundingbox.Width / 2 + spacingx * 1.5 * boundingbox.Width,
                boundingbox.Height / 2 - spacingy * boundingbox.Height,
                16, state == 4 ? percent : 0, state == 4 ? Globals.emgchannelcols[8] : coldisable, "D");
        }
    }
}
