using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public static class AppUtils {
        public static void AddLabelToEMGPanel(PanelLabel lab) {
            foreach (var panel in Globals.panelreg.panellistraw) {
                if (panel is IPanelwithLabels) {    //not the best idea
                    IPanelwithLabels panelwithLabels = panel as IPanelwithLabels;
                    panelwithLabels.PushLabel(lab);
                    break;
                }
            }
        }
        public static byte ValueMapToByte(double value,double min,double max) {
            double temp = (value - min) / (max - min);
            if (temp < 0) temp = 0;
            if (temp > 0.99) temp = 0.99;
            return (byte)(temp * 255);
        }
        public static double ValueMapToDouble(byte value,double min,double max) {
            double temp = (double)value / 255;
            return min + temp * (max - min);
        }
    }
}
