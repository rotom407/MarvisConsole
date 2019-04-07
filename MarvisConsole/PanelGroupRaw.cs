using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Panel group: raw data from hardware (interprets data record as DataRecordRaw in PanelRegistry)
    public abstract class PanelGroupRaw:Panel {
        public abstract void DrawContents(DataRecordRaw rec);
        protected bool enablestencil=true;

        public void Draw(DataRecordRaw rec) {
            boundingbox.targetleft = boundingbox.origleft * Globals.panelanimationratio;
            boundingbox.targetright = boundingbox.origright * Globals.panelanimationratio;
            DrawBorder();
            if(enablestencil)
            RendererWrapper.SetStencil(boundingbox, true);
            DrawContents(rec);
            RendererWrapper.SetStencil(boundingbox, false);
        }
    }
}
