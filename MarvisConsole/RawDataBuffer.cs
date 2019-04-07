using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //From hardware to GUI and APP
    public class RawDataBuffer {
        public enum ConsumerName {
            GUI,APP
        }
        public volatile Queue<DataRecord> bufgui = new Queue<DataRecord>();
        public volatile Queue<DataRecord> bufapp = new Queue<DataRecord>();
        public bool Push(DataRecord rec) {
            lock (bufgui) {
                bufgui.Enqueue(rec);
            }
            lock (bufapp) {
                bufapp.Enqueue(rec);
            }
            return true;
        }
        public DataRecord Pop(ConsumerName con) {
            DataRecord recout;
            switch (con) {
                case ConsumerName.GUI:
                    lock (bufgui) {
                        if (bufgui.Count > 0) recout = bufgui.Dequeue();
                        else recout = null;
                    }
                    break;
                case ConsumerName.APP:
                    lock (bufapp) {
                        if (bufapp.Count > 0) recout = bufapp.Dequeue();
                        else recout = null;
                    }
                    break;
                default:
                    recout = null;
                    throw new NotImplementedException();
            }
            return recout;
        }
    }
}
