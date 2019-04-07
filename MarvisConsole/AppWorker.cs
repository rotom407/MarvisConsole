using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarvisConsole {
    //App worker
    public class AppWorker {
        private volatile bool running = true;
        public void DoWork() {
            while (running) {
                Globals.appreg.RunApps();
                Thread.Sleep(15);
            }
        }
        public void Kill() => running = false;
    }
}
