using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MarvisConsole {
    //Global structs
    public static class Globals {
        public static RawDataBuffer datbuf = new RawDataBuffer();
        public static AppDataBuffer appdatbuf = new AppDataBuffer();
        public static RawPanelRegistry panelreg = new RawPanelRegistry();
        public static ClickableAreaRegistry clickablereg = new ClickableAreaRegistry();
        public static AppRegistry appreg = new AppRegistry();
        public static SerialWorker sworker = new SerialWorker();
        public volatile static Queue<byte> serialcommand = new Queue<byte>();
        public static AppWorker aworker = new AppWorker();
        public static Thread thserial = new Thread(sworker.DoWork);
        public static Thread thapp = new Thread(aworker.DoWork);
        public const int defaultwindowwidth = 1300;
        public const int defaultwindowheight = 600;
        public static int currentwindowwidth = 1300;
        public static int currentwindowheight = 600;
        public static int mousex = 0;
        public static int mousey = 0;
        public const int panelspacingtoleft = 60;
        public const int panelspacingbetween = 30;
        public const int panelspacingtotop = 60;
        public const int panelspacingcolumn = 60;
        public const int panelemgheight = 300;
        public static bool panelanimated = false;
        public const double panelanimationratio = 0.8;
        public const bool appcontrolenable = false;

        public static string[] appnames = {
            "Mouse Control",
            "Presentation",
            "Interop Test",
            "Minecraft"
        };
        public static AppBase GetApp(int appid) {
            switch (appid) {
                case 0:
                    return new AppMouse();
                case 1:
                    return new AppPresentation();
                case 2:
                    return new AppInteropTest();
                case 3:
                    return new AppMinecraft();
                default:
                    throw new NotImplementedException();
            }
        }
        public const int appnum = 4;
        public static int appselected = 0;

        public static byte GetRawChannelCommand(int cmdid) {
            return (byte)((byte)'1' + (byte)cmdid);
        }
        public const int rawchcmdnum = 8;
        public static int rawchselected = 0;

        public static string serialport = /*"COM17"*/"COM20";
        public const bool enableeffects = true;
        public const bool demomode = true;

        public static RGBAColor[] emgchannelcols = {
            /*
            new RGBAColor(1.0,0.7,0.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.7,1.0,0.0,1.0),
            new RGBAColor(1.0,0.3,0.0,1.0),
            new RGBAColor(1.0,0.0,0.7,1.0),
            new RGBAColor(0.3,0.0,1.0,1.0),
            new RGBAColor(0.0,1.0,0.7,1.0),
            new RGBAColor(0.3,1.0,0.0,1.0)*/
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(0.0,0.7,1.0,1.0),
            new RGBAColor(1.0,0.7,0.0,1.0)
        };

        public static RGBAColor[] motionchannelcols = {
            /*
            new RGBAColor(1.0,0.0,0.0,1.0),
            new RGBAColor(0.0,1.0,0.0,1.0),
            new RGBAColor(0.0,0.0,1.0,1.0),
            new RGBAColor(1.0,0.5,0.0,1.0),
            new RGBAColor(0.0,1.0,0.5,1.0),
            new RGBAColor(0.0,0.5,1.0,1.0)*/
            new RGBAColor(0.0,0.0,1.0,1.0),
            new RGBAColor(0.0,0.5,1.0,1.0),
            new RGBAColor(0.0,1.0,1.0,1.0),
            new RGBAColor(1.0,0.5,0.0,1.0),
            new RGBAColor(0.0,1.0,0.5,1.0),
            new RGBAColor(0.0,0.5,1.0,1.0)
        };

        public static RGBAColor[] derivedchannelcols = {
            new RGBAColor(1.0,1.0,1.0,1.0),
            new RGBAColor(0.5,0.5,0.5,1.0)
        };

        public static RGBAColor[] guicolors = {
            new RGBAColor(0.0 / 255.0, 122.0 / 255.0, 204.0 / 255.0, 1.0),
            new RGBAColor(200.0 / 255.0, 80.0 / 255.0, 0.0 / 255.0, 1.0)
        };

        //configs
        public enum RawChannel { EMG, Motion };
        public static RawChannel rawchannel = RawChannel.EMG;
        public static int rawchannelid = 0;
    }
}
