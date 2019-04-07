using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //From APP to GUI
    public class AppDataBuffer {
        public volatile Queue<DataRecord> buf = new Queue<DataRecord>();
        public bool Push(DataRecord rec) {
            lock (buf) {
                buf.Enqueue(rec);
            }
            return true;
        }
        public DataRecord Pop() {
            DataRecord recout;
            lock (buf) {
                if (buf.Count > 0) recout = buf.Dequeue();
                else recout = null;
            }
            return recout;
        }
    }
}
