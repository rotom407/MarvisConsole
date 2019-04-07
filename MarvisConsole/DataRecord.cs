using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Uninterpreted data record
    public class DataRecord {
        public byte timestamp;
        public List<byte> content;
        public DataRecord(byte ts,List<byte> c) {
            timestamp = ts;
            content = c;
        }
    }
}
