using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Panel group: from applications (interprets data record per app in app.translatedata)
    public abstract class PanelGroupApp : Panel {
        public abstract void DrawContents(DataRecord rec);
        public void Draw(DataRecord rec) {
            boundingbox.origleft = boundingbox.origright-1.0;
            DrawBorder();
            DrawContents(rec);
        }
    }
}
