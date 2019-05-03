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
        const int udpportsvr = 7920;
        const int udpportcli = 7920;
        public byte udptimestamp = 0;
        public bool enabledatatransfer = false;
        const string svrip = "47.93.244.190";
        UdpClient client;
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(svrip), udpportsvr);

        public bool enablemotion;
        void applymotion(ClickableArea o,bool right) {
            enablemotion = !enablemotion;
        }

        public void UDPrecvinterrupt(IAsyncResult res) {
            bool fail = false;
            byte[] recv = new byte[] { };
            IPEndPoint remoteipendpt = new IPEndPoint(IPAddress.Any, udpportsvr);
            try {
                recv=client.EndReceive(res, ref remoteipendpt);
            } catch (Exception e)when (e is ArgumentException || e is ObjectDisposedException) {
                fail = true;
                if(connected)
                    Console.WriteLine(e.ToString());
            }
            if (!fail) {
                Console.Write("Received: ");
                Console.WriteLine(Encoding.UTF8.GetString(recv));
                client.BeginReceive(new AsyncCallback(UDPrecvinterrupt), null);
            }
        }

        public bool connected=false;
        void ConnectToServer(ClickableArea o,bool right) {
            connected = !connected;
            if (connected) {
                o.caption = "Disconnect";
                try {
                    client.Connect(ep);
                    client.BeginReceive(new AsyncCallback(UDPrecvinterrupt), null);
                    Console.WriteLine("Connected.");
                } catch (SocketException e) {
                    connected = false;
                    Console.WriteLine("CONNECTION FAILED!!! WOW");
                    o.caption = "Connect";
                }
            } else {
                client.Close();
                client = new UdpClient(udpportcli);
                o.caption = "Connect";
                Console.WriteLine("Disconnected.");
            }
        }

        void EnableDataTransfer(ClickableArea o,bool right) {
            enabledatatransfer = !enabledatatransfer;
            if (enabledatatransfer) {
                o.caption = "Disable";
                Console.WriteLine("Data transfer enabled.");
            } else {
                o.caption = "Enable";
                Console.WriteLine("Data transfer disabled.");
            }
        }

        void TestConnection(ClickableArea o) {  //not used
            if (connected) {
                client.Send(Encoding.ASCII.GetBytes("hello"), 5);
            }
        }

        public AppInteropTest() {
            client = new UdpClient(udpportcli);

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
            btntest.caption = "Enable";
            btntest.MouseDown = EnableDataTransfer;
            clickables.Add(btntest);
        }

        public override void Run(DataRecord rec) {
            if (rec != null) {  //valid data
                DataRecordRaw drr = new DataRecordRaw(rec); //translation
                if (connected && enabledatatransfer) {
                    byte[] cont = new byte[rec.content.Count + 2];
                    udptimestamp++;
                    for(int i=2;i< rec.content.Count + 2; i++) {
                        cont[i] = rec.content[i - 2];
                    }
                    cont[1] = udptimestamp;
                    cont[0] = (byte)(rec.content.Count + 2);
                    client.Send(cont, rec.content.Count + 2);
                }
            }
            /** to send data to app panels
            List<byte> txbytes = new List<byte> { appuid };
            txbytes.Add(0x01);  //exampledata
            DataRecord txdr = new DataRecord(0x00, txbytes);
            Globals.appdatbuf.Push(txdr);
            */
        }

        public override void Kill() {
            base.Kill();
            client.Close();
        }
    }
}
