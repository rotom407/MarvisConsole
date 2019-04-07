using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public abstract class AppBase {
        public abstract List<PanelGroupApp> Panels { get; set; }
        public abstract List<ClickableArea> Clickables { get; set; }
        public abstract void Run(DataRecord rec);
    }
}
