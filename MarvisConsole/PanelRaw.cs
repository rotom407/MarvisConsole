using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelRaw : PanelGroupRaw {
        RGBAColor baselinecol = new RGBAColor(1.0, 1.0, 1.0, 0.2);
        CyclicBuffer<sbyte> dispbuf = new CyclicBuffer<sbyte>(800);
        double offsetsamps, samplelen, interpolaterate = 0;
        public PanelRaw() {
            caption = "Raw EMG - CH1   ";
            boundingbox = new RectangleBox(Globals.panelspacingtoleft,
                Globals.defaultwindowwidth * 0.61 - Globals.panelspacingbetween / 2,
                Globals.panelspacingtotop,
                Globals.defaultwindowheight - Globals.panelspacingtotop - Globals.panelemgheight - Globals.panelspacingcolumn);
            samplelen = 1.0 / (dispbuf.maxlen - 3) * boundingbox.Width;
            offsetsamps = -20.0;
        }
        public override void DrawContents(DataRecordRaw rec) {
            if (rec != null) {
                int reccount = 0;
                foreach (var data in rec.rawdata) {
                    dispbuf.Push(data);
                    offsetsamps += 1.0;
                    reccount++;
                }
                interpolaterate = 0.999 * interpolaterate + 0.001 * reccount;
                if (offsetsamps > 0) {
                    offsetsamps = 0;
                    interpolaterate *= 1.1;
                }
                if (offsetsamps < -50.0) {
                    offsetsamps = -50.0;
                    interpolaterate *= 0.9;
                }
                //for (int i = 0; i < dispbuf.maxlen; i++)
                //    Console.Write(dispbuf[i]);
                //Console.WriteLine();
                
            } else {
                interpolaterate = 0.999 * interpolaterate + 0.001 * 0.0;
            }
            offsetsamps -= interpolaterate;
            RendererWrapper.DrawBaseline(boundingbox, boundingbox.Height/2, baselinecol);
            RendererWrapper.DrawRawChannel(boundingbox, offsetsamps*samplelen, boundingbox.Height / 2, Globals.emgchannelcols[0], ref dispbuf);
        }
    }
}
