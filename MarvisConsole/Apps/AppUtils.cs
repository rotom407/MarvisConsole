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
    }
}
