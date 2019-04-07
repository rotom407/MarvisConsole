using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace MarvisConsole {
    public class PanelMotionData {
        public short[] accel = new short[3];
        public short[] gyro = new short[3];
    }
    public class PanelMotion : PanelGroupRaw {
        public double[] chplatedisp = new double[4] { 0, 0, 0, 0 };
        public double[] chplaterec = new double[4] { 0, 0, 0, 0 };
        CyclicBuffer<PanelMotionData> dispch1 = new CyclicBuffer<PanelMotionData>(50);
        CyclicBuffer<PanelMotionData> dispch2 = new CyclicBuffer<PanelMotionData>(50);
        public PanelMotion() {
            caption = "Motions";
            boundingbox = new RectangleBox(Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth -Globals.panelspacingtoleft,
                Globals.panelspacingtotop,
                300);
            enablestencil = false;
        }
        public override void DrawContents(DataRecordRaw rec) {
            if (rec != null) {
                chplaterec[0] = rec.accelmeter[0, 0];
                chplaterec[1] = rec.accelmeter[0, 1];
                chplaterec[2] = rec.accelmeter[1, 0];
                chplaterec[3] = rec.accelmeter[1, 1];
                dispch1.Push(new PanelMotionData {
                    accel = new short[3] { rec.accelmeter[0, 0], rec.accelmeter[0, 1], rec.accelmeter[0, 2], },
                    gyro = new short[3] { rec.gyro[0, 0], rec.gyro[0, 1], rec.gyro[0, 2] }
                });
                dispch2.Push(new PanelMotionData {
                    accel = new short[3] { rec.accelmeter[1, 0], rec.accelmeter[1, 1], rec.accelmeter[1, 2], },
                    gyro = new short[3] { rec.gyro[1, 0], rec.gyro[1, 1], rec.gyro[1, 2] }
                });
            }
            for(int i = 0; i < 4; i++) {
                chplatedisp[i] = 0.9 * chplatedisp[i] + 0.1 * 0.2 * chplaterec[i];
            }
            RendererWrapper.Set3D(new RectangleBox(boundingbox.left, boundingbox.left+boundingbox.Width*0.5, boundingbox.bottom + boundingbox.Height * 0.3, boundingbox.top));
            RendererWrapper.DrawPlate3D(0.0, 0, chplatedisp[0], 0, chplatedisp[1], Globals.emgchannelcols[0]);
            RendererWrapper.Set2D();
            RendererWrapper.Set3D(new RectangleBox(boundingbox.left + boundingbox.Width * 0.5, boundingbox.right, boundingbox.bottom + boundingbox.Height * 0.3, boundingbox.top));
            RendererWrapper.DrawPlate3D(0.0, 0, chplatedisp[2], 0, chplatedisp[3], Globals.emgchannelcols[0]);
            RendererWrapper.Set2D();
            RendererWrapper.DrawGyroChannel(new RectangleBox(
                boundingbox.left + 20, boundingbox.left + boundingbox.Width * 0.5 - 20,
                boundingbox.bottom, boundingbox.bottom + boundingbox.Height * 0.3
                )
                , 0, boundingbox.Height * 0.25, ref dispch1);
            RendererWrapper.DrawGyroChannel(new RectangleBox(
                boundingbox.left + boundingbox.Width * 0.5 + 20, boundingbox.right - 20,
                boundingbox.bottom, boundingbox.bottom + boundingbox.Height * 0.3
                )
                , 0, boundingbox.Height * 0.25, ref dispch2);
        }
    }
}
