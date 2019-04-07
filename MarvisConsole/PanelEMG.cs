using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelEMGData {
        public byte[] amp = new byte[8];
        public byte[] freq = new byte[8];
    }
    public class PanelEMG : PanelGroupRaw {
        RGBAColor baselinecol = new RGBAColor(1.0, 1.0, 1.0, 0.2);
        CyclicBuffer<PanelEMGData> dispbuf = new CyclicBuffer<PanelEMGData>(100);
        double offsetsamps, samplelen, interpolaterate=0.40;
        public PanelEMG() {
            caption = "EMG Activations";
            boundingbox = new RectangleBox(Globals.panelspacingtoleft,
                Globals.defaultwindowwidth*0.61 - Globals.panelspacingbetween/2,
                Globals.defaultwindowheight - Globals.panelspacingtotop - Globals.panelemgheight,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            samplelen = 1.0 / (dispbuf.maxlen - 3) * boundingbox.Width;
            offsetsamps = -2.0;
        }
        public void AddAppMarker(DataRecord recapp) {

        }


        public override void DrawContents(DataRecordRaw rec) {
            if (rec != null) {
                dispbuf.Push(new PanelEMGData { amp = rec.emgamplitude, freq = rec.emgfrequency });
                offsetsamps += 1.0;
                if (offsetsamps > 0) {
                    offsetsamps = 0;
                    interpolaterate *= 1.1;
                }
                if (offsetsamps < -2.0) {
                    offsetsamps = -2.0;
                    interpolaterate *= 0.99;
                }
                interpolaterate = 0.995 * interpolaterate + 0.005 * 1.0;
                //Console.WriteLine(interpolaterate);
                //Console.WriteLine(offsetsamps);
            } else {
                interpolaterate = 0.995 * interpolaterate + 0.005 * 0.0;
            }
            offsetsamps -= interpolaterate;
            for(int i = 1; i <= 8; i++) {
                RendererWrapper.DrawBaseline(boundingbox, boundingbox.Height- boundingbox.Height*i / 9.0, baselinecol);
                RendererWrapper.DrawEMGChannel(boundingbox, offsetsamps*samplelen, boundingbox.Height - boundingbox.Height * i / 9.0,
                    Globals.emgchannelcols[i - 1], ref dispbuf, i - 1);
                RendererWrapper.DrawChannelLabel(boundingbox, 35, boundingbox.Height - boundingbox.Height * i / 9.0,
                    Globals.emgchannelcols[i - 1], "CH" + i.ToString(), dispbuf[dispbuf.maxlen - 1].amp[i - 1] > 0);
                //Console.Write(dispbuf[0].amp[i-1].ToString()+" ");
            }
            /*if (rec != null) {
                for (int i = 0; i < 256; i++)
                    Console.Write(dispbuf[i].amp[0].ToString());
                Console.WriteLine();
            }*/
            //Console.WriteLine();
        }
    }
}
