using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMinecraftView : PanelGroupApp {
        public PanelMinecraftView() {
            caption = "Navigation";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 200,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        double xspp = 0.0, yspp = 0.0;
        double xsp = 0.0, ysp = 0.0;
        double mxspp = 0.0, myspp = 0.0;
        double mxsp = 0.0, mysp = 0.0;
        double mxsoff = 0.0, mysoff = 0.0;
        public override void DrawContents(DataRecord rec) {
            if (rec != null) {  //new record
                if (rec.content[0] == 0x09) {   //check appuid
                    if (rec.content[1] == 0x01) {//mouse enabled
                        xspp = AppUtils.ValueMapToDouble(rec.content[2], -15, 15);
                        yspp = AppUtils.ValueMapToDouble(rec.content[3], -15, 15);
                    }
                    mxspp = -AppUtils.ValueMapToDouble(rec.content[4], -15, 15);
                    myspp = AppUtils.ValueMapToDouble(rec.content[5], -15, 15);
                    mxsoff = -AppUtils.ValueMapToDouble(rec.content[6], -15, 15);
                    mysoff = AppUtils.ValueMapToDouble(rec.content[7], -15, 15);
                }
            }
            xsp = 0.8 * xsp + 0.2 * xspp;
            ysp = 0.8 * ysp + 0.2 * yspp;
            mxsp = 0.8 * mxsp + 0.2 * mxspp;
            mysp = 0.8 * mysp + 0.2 * myspp;
            RendererWrapper.DrawCoordinate(boundingbox, new RGBAColor(1, 1, 1, 0.3));
            List<Point2D> mpts = new List<Point2D>();
            mpts.Add(new Point2D(boundingbox.Width / 2 + 7 * mxsoff, boundingbox.Height / 2));
            mpts.Add(new Point2D(boundingbox.Width / 2, boundingbox.Height / 2 + 7 * mysoff));
            RendererWrapper.DrawMarkers(boundingbox, mpts, Globals.emgchannelcols[7], 0, 10);
            RendererWrapper.DrawArrow(boundingbox, new Point2D(boundingbox.Width / 2 , boundingbox.Height / 2 ),
                new Point2D(boundingbox.Width / 2 + 7 * mxsp - 7 * mxsoff, boundingbox.Height / 2 ),
                Globals.emgchannelcols[7], thickness: 2, headsize: 10);
            RendererWrapper.DrawArrow(boundingbox, new Point2D(boundingbox.Width / 2 , boundingbox.Height / 2 ),
                new Point2D(boundingbox.Width / 2 , boundingbox.Height / 2 + 7 * mysp - 7 * mysoff),
                Globals.emgchannelcols[7], thickness: 2, headsize: 10);
            RendererWrapper.DrawArrow(boundingbox, new Point2D(boundingbox.Width / 2, boundingbox.Height / 2),
                new Point2D(boundingbox.Width / 2 + 7 * xsp, boundingbox.Height / 2 + 7 * ysp),
                Globals.emgchannelcols[8], thickness: 2, headsize: 25);
        }
    }
}
