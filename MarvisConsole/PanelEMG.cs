using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelEMGData {
        public byte[] amp = new byte[8];
        public byte[] freq = new byte[8];
        public List<PanelLabel> labels = new List<PanelLabel>();
    }
    public class PanelEMG : PanelGroupRaw, IPanelwithLabels {
        RGBAColor baselinecol = new RGBAColor(1.0, 1.0, 1.0, 0.2);
        CyclicBuffer<PanelEMGData> dispbuf = new CyclicBuffer<PanelEMGData>(100);
        double offsetsamps, samplelen, interpolaterate=0.40;
        double[] effectstrengthslow = new double[8];
        double[] effectstrengthfast = new double[8];

        public PanelEMG() {
            caption = "EMG Activations";
            boundingbox = new RectangleBox(Globals.panelspacingtoleft,
                Globals.defaultwindowwidth*0.61 - Globals.panelspacingbetween/2,
                Globals.defaultwindowheight - Globals.panelspacingtotop - Globals.panelemgheight,
                Globals.defaultwindowheight - Globals.panelspacingtotop);
            samplelen = 1.0 / (dispbuf.maxlen - 3) * boundingbox.Width;
            offsetsamps = -2.0;
        }

        public void PushLabel(PanelLabel lab) {
            dispbuf[dispbuf.maxlen - 1].labels.Add(lab);
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
                RendererWrapper.DrawEMGChannel(boundingbox, offsetsamps * samplelen, boundingbox.Height - boundingbox.Height * i / 9.0,
                    Globals.emgchannelcols[i - 1], ref dispbuf, i - 1, i == 1);

                bool excited = dispbuf[dispbuf.maxlen - 1].amp[i - 1] > 0;
                RendererWrapper.DrawChannelLabel(boundingbox, 35, boundingbox.Height - boundingbox.Height * i / 9.0,
                    Globals.emgchannelcols[i - 1], "CH" + i.ToString(), excited);

                if(excited) {
                    effectstrengthslow[i - 1] = 0.9 * effectstrengthslow[i - 1] + 0.1 * 0.5;
                    if (0.2 * dispbuf[dispbuf.maxlen - 1].amp[i - 1] / 255.0 > effectstrengthfast[i - 1])
                        effectstrengthfast[i - 1] = 0.5 * effectstrengthfast[i - 1] + 0.5 * 0.3 * dispbuf[dispbuf.maxlen - 1].amp[i - 1] / 255.0;
                } else {
                    effectstrengthslow[i - 1] = 0.95 * effectstrengthslow[i - 1];
                }
                effectstrengthfast[i - 1] = 0.9 * effectstrengthfast[i - 1];
                RendererWrapper.DrawEffectExcitation(
                    new RectangleBox(
                        boundingbox.left,
                        boundingbox.right,
                        boundingbox.bottom + boundingbox.Height * (9 - i) / 9.0 - boundingbox.Height / 18.0,
                        boundingbox.bottom + boundingbox.Height * (10 - i) / 9.0 - boundingbox.Height / 18.0
                        ),
                    new RGBAColor(0, 122.0 / 255, 204.0 / 255, 0.7),
                    effectstrengthslow[i - 1]);
                RendererWrapper.DrawEffectExcitation(
                    new RectangleBox(
                        boundingbox.left,
                        boundingbox.right,
                        boundingbox.bottom + boundingbox.Height * (9 - i) / 9.0 - boundingbox.Height / 18.0,
                        boundingbox.bottom + boundingbox.Height * (10 - i) / 9.0 - boundingbox.Height / 18.0
                        ),
                    new RGBAColor(1, 1, 1, 0.9),
                    effectstrengthfast[i - 1]);
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
