using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class ClickableArea {
        public RectangleBox boundingbox = new RectangleBox();
        public enum MouseAction {
            None,
            Down,
            Up,
            WheelDown,
            WheelUp
        }
        //use delegate for more dynamics
        public delegate void DMouseOver(ClickableArea o, bool inside);
        public DMouseOver MouseOver = new DMouseOver((o, x) => { });
        public delegate void DMouseDown(ClickableArea o);
        public DMouseDown MouseDown = new DMouseDown((o) => { });
        public delegate void DMouseUp(ClickableArea o);
        public DMouseUp MouseUp = new DMouseUp((o) => { });
        public delegate void DMouseWheel(ClickableArea o, bool incr);
        public DMouseWheel MouseWheel = new DMouseWheel((o, x) => { });
        public bool pressed = false;
        public bool hover = false;
        public int ival = 0;
        public string caption = "Button";

        public ClickableArea(RectangleBox box) {
            boundingbox = box;
        }

        public void UpdateActions(int mousex,int mousey, MouseAction act) {
            if (boundingbox.IsInbox(mousex, mousey)) {
                hover = true;
                MouseOver(this,true);
                switch (act) {
                    case MouseAction.Down:
                        MouseDown(this);
                        break;
                    case MouseAction.Up:
                        MouseUp(this);
                        break;
                    case MouseAction.WheelDown:
                        MouseWheel(this,false);
                        break;
                    case MouseAction.WheelUp:
                        MouseWheel(this,true);
                        break;
                }
            } else {
                hover = false;
                MouseOver(this,false);
            }

        }

        public virtual void UpdateGraphics() {
            //do nothing
        }
    }
}
