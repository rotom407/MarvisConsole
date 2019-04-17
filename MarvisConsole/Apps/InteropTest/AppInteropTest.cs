using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MarvisConsole {
    public class AppInteropTest:AppBase {
        public List<PanelGroupApp> panels = new List<PanelGroupApp>();
        public List<ClickableArea> clickables = new List<ClickableArea>();
        public override List<PanelGroupApp> Panels { get => panels; set => throw new NotImplementedException(); }
        public override List<ClickableArea> Clickables { get => clickables; set => throw new NotImplementedException(); }
        const int appuid = 0x03;
        UdpClient client;
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);

        public bool enablemotion;
        void applymotion(ClickableArea o) {
            enablemotion = !enablemotion;
        }

        public void UDPrecvinterrupt(IAsyncResult res) {
            IPEndPoint remoteipendpt = new IPEndPoint(IPAddress.Any, 7777);
            byte[] recv = client.EndReceive(res, ref remoteipendpt);
            Console.WriteLine(Encoding.UTF8.GetString(recv));
            client.BeginReceive(new AsyncCallback(UDPrecvinterrupt), null);
        }

        public bool connected=false;
        void ConnectToServer(ClickableArea o) {
            connected = !connected;
            if (connected) {
                o.caption = "Disconnect";
                try {
                    client.Connect(ep);
                    client.BeginReceive(new AsyncCallback(UDPrecvinterrupt), null);
                } catch (SocketException e) {
                    connected = false;
                    Console.WriteLine("CONNECTION FAILED!!! WOW");
                    o.caption = "Connect";
                }
            } else {
                client.Close();
                client = new UdpClient();
                o.caption = "Connect";
            }
        }

        void TestConnection(ClickableArea o) {
            if (connected) {
                client.Send(Encoding.ASCII.GetBytes("hello\0"), 6);
            }
        }

        public AppInteropTest() {
            client = new UdpClient();

            ClickableButton btnapply = new ClickableButton(new RectangleBox(
                (Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio * 1.1,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 - 30,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470
                ));
            btnapply.inapp = true;
            btnapply.boundingbox.left = btnapply.boundingbox.right - 1;
            btnapply.col = Globals.guicolors[1];
            btnapply.border = false;
            btnapply.caption = "Apply";
            btnapply.MouseDown = applymotion;
            clickables.Add(btnapply);

            ClickableButton btnconnect = new ClickableButton(new RectangleBox(
                (Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio * 1.1,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 - 30 + 80,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 + 80
                ));
            btnconnect.inapp = true;
            btnconnect.boundingbox.left = btnapply.boundingbox.right - 1;
            btnconnect.col = Globals.guicolors[1];
            btnconnect.border = false;
            btnconnect.caption = "Connect";
            btnconnect.MouseDown = ConnectToServer;
            clickables.Add(btnconnect);

            ClickableButton btntest = new ClickableButton(new RectangleBox(
                (Globals.defaultwindowwidth - Globals.panelspacingbetween) * Globals.panelanimationratio * 1.1,
                Globals.defaultwindowwidth - Globals.panelspacingtoleft * Globals.panelanimationratio,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 - 30 + 40,
                Globals.defaultwindowheight - Globals.panelspacingtotop - 470 + 40
                ));
            btntest.inapp = true;
            btntest.boundingbox.left = btnapply.boundingbox.right - 1;
            btntest.col = Globals.guicolors[1];
            btntest.border = false;
            btntest.caption = "Test";
            btntest.MouseDown = TestConnection;
            clickables.Add(btntest);
        }

        public override void Run(DataRecord rec) {
            if (rec != null) {  //valid data
                DataRecordRaw drr = new DataRecordRaw(rec); //translation
            }
            /** to send data to app panels
            List<byte> txbytes = new List<byte> { appuid };
            txbytes.Add(0x01);  //exampledata
            DataRecord txdr = new DataRecord(0x00, txbytes);
            Globals.appdatbuf.Push(txdr);
            */
        }
    }
}
