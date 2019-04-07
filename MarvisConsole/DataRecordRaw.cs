using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MarvisConsole {
    //Data record interpreted as raw data from hardware
    public class DataRecordRaw {
        public byte[] emgamplitude = new byte[8];
        public byte[] emgfrequency = new byte[8];
        public short[,] accelmeter = new short[2,3];
        public short[,] gyro = new short[2,3];
        public List<sbyte> rawdata = new List<sbyte>();
        public DataRecordRaw(DataRecord rec) {
            for(int i = 0; i < 8; i++) {
                emgamplitude[i] = rec.content[i];
                emgfrequency[i] = 0;
            }
            for(int devid = 0; devid < 2; devid++) {
                for(int i = 0; i < 3; i++) {
                    accelmeter[devid, i] = BitConverter.ToInt16(rec.content.ToArray(), 8 + i * 2 + 12 * devid);
                }
                for (int i = 3; i < 6; i++) {
                    gyro[devid, i - 3] = BitConverter.ToInt16(rec.content.ToArray(), 8 + i * 2 + 12 * devid);
                }
            }
            int rawdatcnt = rec.content[8 + 2 * 12];
            for(int i = 0; i < rawdatcnt; i++) {
                rawdata.Add((sbyte)rec.content[8 + 2 * 12 + i + 1]);
            }
        }
    }
}