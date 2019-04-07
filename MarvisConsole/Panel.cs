using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Panel base
    public abstract class Panel {
        public RectangleBox boundingbox = new RectangleBox(40, 440, 40, 240);
        public string caption ="MOTIONS";
        //public RGBAColor bordercolor = new RGBAColor(Globals.guicols[0]);
        //public RGBAColor backgroundcolor = new RGBAColor(Globals.guicols[1]);
        public RGBAColor bordercolor = new RGBAColor(0.0 / 255.0, 122.0 / 255.0, 204.0 / 255.0, 1.0);
        public RGBAColor backgroundcolor = new RGBAColor(0.0, 0.0, 0.0, 1.0);
        public double captionheight = 24.0, borderwidth = 2.0, glowradius = 8.0;
        //public bool animated = true;
        public void DrawBorder() {
            boundingbox.animateupdate(Globals.panelanimated);
            //RendererWrapper.SetBlendMode(RendererWrapper.BlendModes.Add);
            //RendererWrapper.DrawRectangle(boundingbox.ExpandTop(captionheight), bordercolor.Fade(0.5), glowradius, outer: true, glow: true);
            RendererWrapper.SetBlendMode(RendererWrapper.BlendModes.Normal);
            RendererWrapper.DrawRectangle(boundingbox, bordercolor, borderwidth, outer: true);
            RendererWrapper.DrawRectangle(boundingbox, bordercolor, -1);
            RendererWrapper.DrawRectangle(boundingbox, backgroundcolor, -1); 
            RendererWrapper.DrawCaption(captionheight, boundingbox.left-borderwidth, boundingbox.top+borderwidth, caption, bordercolor);
        }
    }
}
