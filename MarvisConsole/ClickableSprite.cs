using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace MarvisConsole {
    public class ClickableSprite : ClickableArea {
        public int tID;
        public ClickableSprite(RectangleBox box, string spritefilename) : base(box) {
            boundingbox = box;
            Bitmap im = new Bitmap(spritefilename);
            im.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle r = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData bd = im.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Gl.glGenTextures(1, out tID);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tID);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, im.Width, im.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bd.Scan0);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            im.UnlockBits(bd);
            im.Dispose();
        }
        public override void UpdateGraphics() {
            boundingbox.targetleft = boundingbox.origleft * Globals.panelanimationratio;
            boundingbox.targetright = boundingbox.origleft * Globals.panelanimationratio + boundingbox.origright - boundingbox.origleft;
            boundingbox.animateupdate(Globals.panelanimated);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tID);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glTranslated(0, 0, RendererWrapper.currentdepth);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_BLEND);
            Gl.glBegin(Gl.GL_QUAD_STRIP);
            Gl.glColor4d(1, 1, 1, 1);
            Gl.glTexCoord2f(0, 1); Gl.glVertex2d(boundingbox.left, boundingbox.top);
            Gl.glTexCoord2f(1, 1); Gl.glVertex2d(boundingbox.right, boundingbox.top);
            Gl.glTexCoord2f(0, 0); Gl.glVertex2d(boundingbox.left, boundingbox.bottom);
            Gl.glTexCoord2f(1, 0); Gl.glVertex2d(boundingbox.right, boundingbox.bottom);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
    }
}
