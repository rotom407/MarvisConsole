using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace MarvisConsole {
    public class SerialWorker {
        FakeRawDataGenerator fakegen = new FakeRawDataGenerator();
        private volatile bool running = true;
        public volatile bool usefakedata = false;
        public volatile SerialPort Serial1;
        public void DoWork() {
            Serial1 = new SerialPort(/*"COM13"*/"COM20", 115200, Parity.None, 8, StopBits.One) {
                DtrEnable = false,
                RtsEnable = false,
                ReadTimeout = 500
            };
            while (running) {
                if (usefakedata) {
                    DataRecord rec = new DataRecord(0x10, fakegen.MakeFakeData());
                    Globals.datbuf.Push(rec);
                    Thread.Sleep(50);
                } else {
                    if (Serial1.IsOpen) {
                        byte tx=(byte)'s';
                        lock (Globals.serialcommand) {
                            if (Globals.serialcommand.Count > 0)
                                tx = Globals.serialcommand.Dequeue();
                        }
                        byte[] bytesnd = new byte[1] { tx };
                        byte[] msglenrecv = new byte[10];
                        byte[] byterecv = new byte[100];
                        byte[] datrecv = new byte[100];
                        int msglen=0;
                        bool failed = false;
                        int readnum = 0, offset = 0;
                        Serial1.DiscardInBuffer();
                        Serial1.Write(bytesnd, 0, 1);
                        try {
                            Serial1.Read(msglenrecv, 0, 1);
                        } catch (TimeoutException) {
                            failed = true;
                        }
                        msglen = msglenrecv[0] - 1;
                        if (msglen <= 0 || msglen >= 90) {
                            failed = true;
                        }
                        if (!failed) {
                            do {    //it cannot be guaranteed that we would get all the data with one Serial.Read() because the serial port is not that fast and there may be remaining bytes to be transferred
                                try {
                                    readnum = Serial1.Read(byterecv, offset, msglen - offset);
                                    offset += readnum;
                                } catch (TimeoutException) {
                                    failed = true;  //when serial read timed out
                                }
                            } while (offset < msglen && failed == false);
                            if (!failed) {
                                Array.Copy(byterecv, 1, datrecv, 0, msglen - 1);
                                DataRecord rec = new DataRecord(0xAA, datrecv.ToList());
                                Globals.datbuf.Push(rec);
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
                
            }
        }
        public void SetPortOpened(bool opened, string name) {
            if (opened) {
                if (!Serial1.IsOpen) {
                    Serial1.PortName = name;
                    Serial1.Open();
                }
            } else {
                if (Serial1.IsOpen) {
                    Serial1.Close();
                }
            }
        }
        public void Kill() => running = false;
    }
}
