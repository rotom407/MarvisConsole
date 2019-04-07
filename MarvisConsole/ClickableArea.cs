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
        public delegate void DMouseOver(bool inside);
        public DMouseOver MouseOver = new DMouseOver((x) => { });
        public delegate void DMouseDown();
        public DMouseDown MouseDown = new DMouseDown(() => { });
        public delegate void DMouseUp();
        public DMouseUp MouseUp = new DMouseUp(() => { });
        public delegate void DMouseWheel(bool incr);
        public DMouseWheel MouseWheel = new DMouseWheel((x) => { });
        public bool pressed = false;
        public bool hover = false;

        public ClickableArea(RectangleBox box) {
            boundingbox = box;
        }

        public void UpdateActions(int mousex,int mousey, MouseAction act) {
            if (boundingbox.IsInbox(mousex, mousey)) {
                hover = true;
                MouseOver(true);
                switch (act) {
                    case MouseAction.Down:
                        MouseDown();
                        break;
                    case MouseAction.Up:
                        MouseUp();
                        break;
                    case MouseAction.WheelDown:
                        MouseWheel(false);
                        break;
                    case MouseAction.WheelUp:
                        MouseWheel(true);
                        break;
                }
            } else {
                hover = false;
                MouseOver(false);
            }
        }

        public virtual void UpdateGraphics() {
            //do nothing
        }
    }
}
