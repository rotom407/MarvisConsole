using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class PanelLabel {
        public string label = "label";
        public RGBAColor col = new RGBAColor(1.0,0.7,0.0,1.0);
        public int position = 0;
        public int bold = 1;
        public PanelLabel() {

        }
        public PanelLabel(string label_, RGBAColor col_, int position_, int bold_ = 1) {
            label = label_;
            col = col_;
            position = position_;
            bold = bold_;
        }
        public PanelLabel(string label_, int position_, int bold_ = 1) {
            label = label_;
            position = position_;
            bold = bold_;
        }
    }
    public interface IPanelwithLabels {
        void PushLabel(PanelLabel lab);
    }
}
