using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelMouseLocation : PanelGroupApp {
        public Point2D cursorpos = new Point2D(0, 0);
        public List<Point2D> leftclicks = new List<Point2D>();
        public List<Point2D> rightclicks = new List<Point2D>();
        private double boundary = 100;
        public double xsp, ysp;

        public PanelMouseLocation() {
            caption = "Cursor Location";
            boundingbox = new RectangleBox((Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop-200,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            boundingbox.left = boundingbox.right - 1;
            bordercolor = Globals.guicolors[1];
        }

        public override void DrawContents(DataRecord rec) {
            if (rec != null) {
                if (rec.content[0] == 0x01) {
                    xsp = (rec.content[1] - 128) / 50.0;
                    ysp = (rec.content[2] - 128) / 50.0;
                    if (rec.content[3] == 0x01) {
                        leftclicks.Add(new Point2D(cursorpos.x + boundingbox.Width / 2, cursorpos.y + boundingbox.Height / 2));
                    }
                    if (rec.content[4] == 0x01) {
                        rightclicks.Add(new Point2D(cursorpos.x + boundingbox.Width / 2, cursorpos.y + boundingbox.Height / 2));
                    }
                }
            }
            cursorpos.x += xsp;
            cursorpos.y += ysp;
            if (cursorpos.x > boundary) cursorpos.x = boundary;
            if (cursorpos.x < -boundary) cursorpos.x = -boundary;
            if (cursorpos.y > boundary) cursorpos.y = boundary;
            if (cursorpos.y < -boundary) cursorpos.y = -boundary;
            List<Point2D> curpos = new List<Point2D>();
            curpos.Add(new Point2D(cursorpos.x + boundingbox.Width / 2, cursorpos.y + boundingbox.Height / 2));
            RendererWrapper.DrawCoordinate(boundingbox, new RGBAColor(1, 1, 1, 0.3));
            RendererWrapper.DrawMarkers(boundingbox, leftclicks, new RGBAColor(1.0, 0.5, 0.0, 1.0), 1, 8);
            RendererWrapper.DrawMarkers(boundingbox, rightclicks, new RGBAColor(0.0, 0.3, 1.0, 1.0), 1, 8);
            RendererWrapper.DrawMarkers(boundingbox, curpos, new RGBAColor(Globals.emgchannelcols[8]), 0, 8);
        }
    }
}
