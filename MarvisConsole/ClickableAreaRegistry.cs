using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class ClickableAreaRegistry {    //clickables appregistry
        public List<ClickableArea> clickables=new List<ClickableArea>();
        public double animatei = 0;
        ClickableSprite demologo = null;
        void msd() {
            Console.WriteLine("Pressed");
        }

        void SerialConnect(ClickableArea o,bool right) {
            //Console.WriteLine("Input Port:");
            //Globals.serialport = Console.ReadLine();
            //Console.WriteLine("OK");
            //needs terminal, read config instead
            if (Globals.demomode)
                Globals.sworker.usefakedata = true;
            else
                Globals.sworker.SetPortOpened(true, Globals.serialport);
        }

        void AppStart(ClickableArea o, bool right) {
            Globals.panelanimated = !Globals.panelanimated;
            if (Globals.panelanimated) {
                Globals.appreg.applist.Add(Globals.GetApp(Globals.appselected));
            } else {
                Globals.appreg.KillAllApps();
            }
        }

        void SendCommands(ClickableArea o,bool right) {
            if (!right) {
                switch (o.ival) {
                    case 0://change
                        lock (Globals.serialcommand) {
                            Globals.serialcommand.Enqueue((byte)'r');
                            Globals.serialcommand.Enqueue(Globals.GetRawChannelCommand(Globals.rawchselected));
                        }
                        break;
                    case 1://disable
                        lock (Globals.serialcommand) {
                            Globals.serialcommand.Enqueue((byte)'d');
                            Globals.serialcommand.Enqueue(Globals.GetRawChannelCommand(Globals.rawchselected));
                        }
                        break;
                    case 2://enable
                        lock (Globals.serialcommand) {
                            Globals.serialcommand.Enqueue((byte)'e');
                            Globals.serialcommand.Enqueue(Globals.GetRawChannelCommand(Globals.rawchselected));
                        }
                        break;
                }
            } else {
                ChangeCommands(o, true);
            }
        }

        void ChangeCommands(ClickableArea o, bool wheelup) {
            if (wheelup) {
                o.ival++; if (o.ival > 2) o.ival = 0;
            } else {
                o.ival--; if (o.ival < 0) o.ival = 2;
            }
            switch (o.ival) {
                case 0:
                    o.caption = "Change";
                    break;
                case 1:
                    o.caption = "Disable";
                    break;
                case 2:
                    o.caption = "Enable";
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        void SwitchRawChannel(ClickableArea o,bool wheelup) {
            if (wheelup) {
                o.ival++; if (o.ival >= Globals.rawchcmdnum) o.ival = 0;
            } else {
                o.ival--; if (o.ival < 0) o.ival = Globals.rawchcmdnum - 1;
            }
            Globals.rawchselected = o.ival;
            o.caption = "Raw Channel: EMGCH" + (o.ival + 1).ToString();
        }

        void SwitchApp(ClickableArea o, bool wheelup) {
            if (wheelup) {
                o.ival++;if (o.ival >= Globals.appnum) o.ival = 0;
            } else {
                o.ival--;if (o.ival < 0) o.ival = Globals.appnum - 1;
            }
            Globals.appselected = o.ival;
            o.caption = "App: " + Globals.appnames[o.ival];
        }

        public ClickableAreaRegistry() {
            ClickableButton btnappselect = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 300,
                350,
                380));
            //btnappselect.MouseDown = msd;
            btnappselect.border = true;
            btnappselect.col = new RGBAColor(200.0 / 255.0, 80.0 / 255.0, 0.0 / 255.0, 1.0);
            btnappselect.caption = "App: Mouse Control";
            btnappselect.MouseDown = (o,x)=> { SwitchApp(o, !x); } ;
            btnappselect.MouseWheel = SwitchApp;
            clickables.Add(btnappselect);

            ClickableButton btnappapply = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 +310,
                Globals.defaultwindowwidth-Globals.panelspacingtoleft,
                350,
                380));
            btnappapply.caption = "Start";
            btnappapply.col = new RGBAColor(200.0 / 255.0, 80.0 / 255.0, 0.0 / 255.0, 1.0);
            btnappapply.MouseDown = AppStart;

            clickables.Add(btnappapply);

            ClickableButton btnrawselect = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 300,
                390,
                420));
            btnrawselect.border = true;
            btnrawselect.MouseDown = (o, x) => { SwitchRawChannel(o, !x); };
            btnrawselect.MouseWheel = SwitchRawChannel;
            btnrawselect.caption = "Raw Channel: EMGCH1";
            clickables.Add(btnrawselect);

            ClickableButton btnrawapply = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 310,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft,
                390,
                420));
            btnrawapply.caption = "Change";
            btnrawapply.MouseDown = SendCommands;
            btnrawapply.MouseWheel = ChangeCommands;
            clickables.Add(btnrawapply);

            ClickableButton btnconnect = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 120,
                430,
                460));
            btnconnect.caption = "Connect";
            btnconnect.MouseDown = SerialConnect;
            clickables.Add(btnconnect);

            ClickableButton btninfo = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 130,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft,
                430,
                460));
            btninfo.caption = " ";
            btninfo.border = true;
            clickables.Add(btninfo);

            ClickableSprite logo = new ClickableSprite(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 344,
                480,
                480 + 64), "../../Assets/marvisconsole.png");
            logo.alpha = 1;
            clickables.Add(logo);

            demologo = new ClickableSprite(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 344,
                480,
                480 + 64), "../../Assets/marvisconsoledemo.png");
            demologo.alpha = 0;
            clickables.Add(demologo);
        }
        public void UpdateActions(int mousex,int mousey,ClickableArea.MouseAction act) {
            foreach(var c in clickables) {
                c.UpdateActions(mousex, mousey, act);
            }
        }
        public void UpdateGraphics() {
            if (Globals.demomode) {
                animatei += 0.05;
                if (animatei >= Math.PI * 2)
                    animatei -= Math.PI * 2;
                demologo.alpha = (Math.Sin(animatei) + 1) / 2.0;
            } else {
                demologo.alpha = 0;
            }
            foreach(var c in clickables) {
                c.UpdateGraphics();
            }
        }
    }
}
