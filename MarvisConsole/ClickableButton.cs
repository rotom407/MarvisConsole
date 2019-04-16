using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class ClickableButton:ClickableArea {
        public RGBAColor col = new RGBAColor(0.0 / 255.0, 122.0 / 255.0, 204.0 / 255.0, 1.0);
        double animationratio = 0.0;
        public bool border = false;
        public bool inapp = false;
        //public bool animated = true;
        public ClickableButton(RectangleBox box):base(box) {
            boundingbox = box;
        }
        public override void UpdateGraphics() {
            if (!inapp) {
                boundingbox.targetleft = boundingbox.origleft * Globals.panelanimationratio;
                boundingbox.targetright = boundingbox.origright * Globals.panelanimationratio;
            } else {
                boundingbox.origleft = boundingbox.targetright - 1;
            }
            
            boundingbox.animateupdate(Globals.panelanimated);
            if (hover) {
                animationratio = 0.7 * animationratio + 0.3 * 0.2;
            } else {
                animationratio = 0.7 * animationratio + 0.3 * 0.0;
            }
            RectangleBox highlight = new RectangleBox(boundingbox.left, boundingbox.right, boundingbox.bottom,
                animationratio*boundingbox.top + (1- animationratio)*boundingbox.bottom);
            if (!border) {
                RendererWrapper.DrawRectangle(boundingbox, col, -1);
                RendererWrapper.DrawRectangle(highlight, col.Mix(col, new RGBAColor(1, 1, 1, 1), 0.5), -1);
            } else {
                RendererWrapper.DrawRectangle(boundingbox, new RGBAColor(0, 0, 0, 1), -1);
                RendererWrapper.DrawRectangle(highlight, col.Mix(col, new RGBAColor(1, 1, 1, 1), 0.5), -1);
                RendererWrapper.DrawRectangle(boundingbox, col, 2);
            }
            RendererWrapper.DrawString(boundingbox.left+16, boundingbox.bottom+boundingbox.Height/2-12.0/2, caption,new RGBAColor(1.0,1.0,1.0,1.0));
        }
    }
}
