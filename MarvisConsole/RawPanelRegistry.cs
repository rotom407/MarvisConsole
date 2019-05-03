using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Controls groups of panels
    public class RawPanelRegistry {
        public List<PanelGroupRaw> panellistraw = new List<PanelGroupRaw>();
        public RawPanelRegistry() {
            panellistraw.Add(new PanelEMG());
            panellistraw.Add(new PanelRaw());
            panellistraw.Add(new PanelMotion());
        }
        public void Update() {
            DataRecord rdr=null;
            DataRecordRaw rdrr = null;
            if (Globals.datbuf.bufgui.Count > 100) {
                Console.WriteLine("Buffer: " + Globals.datbuf.bufgui.Count.ToString());
                while (Globals.datbuf.bufgui.Count > 100) {
                    Globals.datbuf.Pop(RawDataBuffer.ConsumerName.GUI);
                }
            }
            do {
                rdr = Globals.datbuf.Pop(RawDataBuffer.ConsumerName.GUI);
                if (rdr != null)
                    rdrr = new DataRecordRaw(rdr);
                foreach (var p in panellistraw) {
                    p.Draw(rdrr);
                }
            } while (Globals.datbuf.bufgui.Count > 0);
        }
    }
}
