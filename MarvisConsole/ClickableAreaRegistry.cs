using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class ClickableAreaRegistry {    //clickables appregistry
        public List<ClickableArea> clickables=new List<ClickableArea>();
        void msd() {
            Console.WriteLine("Pressed");
        }

        void SerialConnect(ClickableArea o) {
            Globals.sworker.usefakedata = true;
            //Globals.sworker.SetPortOpened(true, Globals.serialport);
        }

        void AppStart(ClickableArea o) {
            Globals.panelanimated = !Globals.panelanimated;
            if (Globals.panelanimated) {
                Globals.appreg.applist.Add(Globals.GetApp(Globals.appselected));
            } else {
                Globals.appreg.applist.Clear();
            }
        }

        void SendCommands(ClickableArea o) {
            lock (Globals.serialcommand) {
                Globals.serialcommand.Enqueue((byte)'R');
                Globals.serialcommand.Enqueue(Globals.GetRawChannelCommand(Globals.rawchselected));
            }
        }

        void SwitchRawChannel(ClickableArea o) {
            o.ival = (o.ival + 1) % Globals.rawchcmdnum;
            Globals.rawchselected = o.ival;
            o.caption = "Raw Channel: EMGCH" + (o.ival + 1).ToString();
        }

        void SwitchApp(ClickableArea o) {
            o.ival = (o.ival + 1) % Globals.appnum;
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
            btnappselect.MouseDown = SwitchApp;
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
            btnrawselect.MouseDown = SwitchRawChannel;
            btnrawselect.caption = "Raw Channel: EMGCH1";
            clickables.Add(btnrawselect);

            ClickableButton btnrawapply = new ClickableButton(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 310,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft,
                390,
                420));
            btnrawapply.caption = "Change";
            btnrawapply.MouseDown = SendCommands;
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
            btninfo.caption = "PPS: 20";
            btninfo.border = true;
            clickables.Add(btninfo);

            ClickableSprite logo = new ClickableSprite(new RectangleBox(
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2,
                Globals.defaultwindowwidth * 0.61 + Globals.panelspacingbetween / 2 + 344,
                480,
                480+64), "./Assets/marvisconsole.png");
            clickables.Add(logo);
        }
        public void UpdateActions(int mousex,int mousey,ClickableArea.MouseAction act) {
            foreach(var c in clickables) {
                c.UpdateActions(mousex, mousey, act);
            }
        }
        public void UpdateGraphics() {
            foreach(var c in clickables) {
                c.UpdateGraphics();
            }
        }
    }
}
