using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class FakeRawDataGenerator {
        public int[] emgactivation = new int[8];
        public int[] emgacttime = new int[8];
        public double[] emgactchance = new double[] {0.01,0.015,0.006,0.007,0.005,0.006,0.005,0.004 };
        public double emgactchancemultiplier = 1.0;
        public double t,rawt;
        public short[] accel = new short[12];
        public double[] accelfreq = new double[12];
        public Random rand = new Random();

        public FakeRawDataGenerator() {
            for(int i = 0; i < 12; i++) {
                accelfreq[i] = 10 + 4 * rand.NextDouble();
            }
        }

        public List<byte> MakeFakeData() {
            List<byte> dat = new List<byte>();
            double chance;
            //gen emg act
            t+=0.01;
            emgactchancemultiplier = 1.0 + 0.8 * Math.Sin(t);
            for (int i = 0; i < 8; i++) {
                if (emgactivation[i] <= 0) {    //not activated
                    emgacttime[i] = 0;
                    chance = rand.NextDouble();
                    if (chance < emgactchance[i]* emgactchancemultiplier) {
                        emgactivation[i] = (int)(45 * rand.NextDouble());
                    }
                } else {    //activated
                    emgacttime[i]++;
                    chance = rand.NextDouble();
                    if (chance < Math.Exp(-0.2*(double)emgacttime[i])) {
                        emgactivation[i] += (int)(20 * rand.NextDouble());
                        if (emgactivation[i] > 255) emgactivation[i] = 255;
                    } else {
                        emgactivation[i] -= (int)(15 * rand.NextDouble());
                        if (emgactivation[i] <= 0) emgactivation[i] = 0;
                    }
                }
                dat.Add((byte)emgactivation[i]);
            }
            //accel
            for(int i = 0; i < 12; i++) {
                accel[i] = (short)(100.0*Math.Sin(accelfreq[i] * t));
                dat.AddRange(BitConverter.GetBytes(accel[i]));
            }
            //raw
            dat.Add(10);
            double rawdat;
            for (int i = 0; i < 10; i++) {
                rawt += 0.01;
                rawdat = 0.3 * emgactivation[0] * (Math.Sin(50.0 * rawt + 3.0 * rand.NextDouble()) + 
                    Math.Sin(70.0 * rawt + 3.0 * rand.NextDouble()));
                rawdat += 3 * rand.NextDouble();
                rawdat -= 3 * rand.NextDouble();
                if (rawdat > 120.0) rawdat = 120.0;
                if (rawdat < -120.0) rawdat = -120.0;
                dat.Add((byte)rawdat);
            }
            return dat;
        }
    }
}
