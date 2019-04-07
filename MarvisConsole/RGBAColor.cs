using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class RGBAColor {
        public double R;
        public double G;
        public double B;
        public double A;
        public RGBAColor() {
            R = G = B = A = 1.0f;
        }
        public RGBAColor(double r, double g, double b, double a) {
            R = r;G = g;B = b;A = a;
        }
        public RGBAColor(RGBAColor col) {
            R = col.R;G = col.G;B = col.B;A = col.A;
        }
        public RGBAColor Fade(double a) {
            return new RGBAColor(R, G, B, A * a);
        }
        public RGBAColor Mix(RGBAColor a,RGBAColor b,double percentage) {
            return new RGBAColor(a.R*percentage+b.R*(1-percentage),
                a.G * percentage + b.G * (1 - percentage),
                a.B * percentage + b.B * (1 - percentage),
                a.A * percentage + b.A * (1 - percentage));
        }
    }
}
