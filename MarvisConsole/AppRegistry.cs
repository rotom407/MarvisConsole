using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    //Registers apps
    public class AppRegistry {
        public List<AppBase> applist = new List<AppBase>();

        public AppRegistry() {
            //applist.Add(new AppMouse());
        }

        //Called by GUI
        public void UpdatePanels() {
            DataRecord rdr = null;
            //while len>0?
            do {
                rdr = Globals.appdatbuf.Pop();
                foreach (var app in applist) {
                    foreach (var panel in app.Panels) {
                        panel.Draw(rdr);
                    }
                    foreach (var cl in app.Clickables) {
                        cl.UpdateGraphics();
                    }
                }
            } while (Globals.appdatbuf.buf.Count > 0);
        }
        //Called by inputs
        public void UpdateClickables(int mousex,int mousey, ClickableArea.MouseAction act) {
            foreach(var app in applist) {
                foreach(var cl in app.Clickables) {
                    cl.UpdateActions(mousex, mousey, act);
                }
            }
        }
        //Called by working threads
        public void RunApps() {
            DataRecord rdr = null;
            rdr = Globals.datbuf.Pop(RawDataBuffer.ConsumerName.APP);
            foreach (var app in applist) {
                app.Run(rdr);
            }
        }

        public void KillAllApps() {
            foreach(var app in applist) {
                app.Kill();
            }
            Globals.appreg.applist.Clear();
        }
    }
}
