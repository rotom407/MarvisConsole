using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Cyclic buffer used by plotters
    public class CyclicBuffer<T> where T:new(){
        public T[] buf;
        public int maxlen = 200;
        public int currentpos = 0;
        public CyclicBuffer(int maxlen_){
            maxlen = maxlen_;
            buf = new T[maxlen];
            for (int i = 0; i < maxlen; i++) {
                buf[i] = new T();
            }
        }
        public void Push(T val) {
            buf[currentpos] = val;
            currentpos = (currentpos + 1) % maxlen;
        }
        public T this[int i] {
            get { return buf[(currentpos + i) % maxlen]; }
            set { buf[(currentpos + i) % maxlen] = (T)value; }
        }
    }
}
