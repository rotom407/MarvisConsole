using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class RectangleBox{
        public double left,targetleft,origleft;
        public double right,targetright,origright;
        public double bottom,targetbottom,origbottom;
        public double top,targettop,origtop;

        public double Width { get => right - left; }
        public double Height { get => top - bottom; }
        public RectangleBox(){
            targetleft = origleft = left = targetright = origright = right = targetbottom = origbottom = bottom = targettop = origtop = top = 0.0;
        }
        public RectangleBox(double l, double r, double b, double t) {
            targetleft = origleft = left = l;
            targetright = origright = right = r;
            targetbottom = origbottom = bottom = b;
            targettop = origtop = top = t;
        }
        public RectangleBox ExpandTop(double val) {
            return new RectangleBox(left, right, bottom, top + val);
        }
        public bool IsInbox(double x,double y) {
            return left < x && x < right && bottom < y && y < top;
        }
        public void animateupdate(bool en) {
            if (en) {
                left = left * 0.9 + targetleft * 0.1;
                right = right * 0.9 + targetright * 0.1;
                top = top * 0.9 + targettop * 0.1;
                bottom = bottom * 0.9 + targetbottom * 0.1;
            } else {
                left = left * 0.9 + origleft * 0.1;
                right = right * 0.9 + origright * 0.1;
                top = top * 0.9 + origtop * 0.1;
                bottom = bottom * 0.9 + origbottom * 0.1;
            }
        }
    }
}
