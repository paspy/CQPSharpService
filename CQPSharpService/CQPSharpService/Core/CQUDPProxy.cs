using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CQPSharpService.Core {

    public class CQUDPProxy {
        //internal delegate void ThreadCallBackDelegate(string msg);

        private static object __syncRoot = new object();

        private static AutoResetEvent myResetEvent = new AutoResetEvent(false);
        private static AutoResetEvent keepConnectionResetEvent = new AutoResetEvent(false);

        private static volatile CQUDPProxy instance;

        private string _strMessage = string.Empty;
        private string _keepConnectionMsg = string.Empty;

        private NLog.Logger nLogger = NLog.LogManager.GetCurrentClassLogger();

        //private ThreadCallBackDelegate _callback;
        private EndPoint LocalPoint;
        private EndPoint ServerPoint;
        private EndPoint RemotePoint;
        private Socket mySocket;
        private bool RunningFlag;

        internal int ClientTimeout { get; set; }
        internal int FramePrefixSize { get; set; }
        internal int FramePayloadSize { get; set; }
        internal int FrameSize { get; set; }

        private Timer keepConnectTimer;

        private CQUDPProxy() {
            //Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
            //_callback = new ThreadCallBackDelegate(AnalyzeMessage);
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ClientTimeout = 500;
            FramePrefixSize = 256;
            FramePayloadSize = 32768;
            FrameSize = 33025;
        }

        public static CQUDPProxy GetInstance() {
            if (instance == null) {
                lock (__syncRoot) {
                    if (instance == null)
                        instance = new CQUDPProxy();
                }
            }
            return instance;
        }

        public void StartProxy() {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            int myPort = 12450;
            LocalPoint = new IPEndPoint(address, myPort);
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mySocket.Bind(LocalPoint);
            int srvPort = 11235;
            ServerPoint = new IPEndPoint(address, srvPort);
            RemotePoint = new IPEndPoint(address, srvPort);
            RunningFlag = true;
            new Thread(new ThreadStart(ReceiveMessage)) {
                IsBackground = true
            }.Start();

            // send initial hello message
            keepConnectTimer = new Timer(SendHelloMessage, null, 5, Timeout.Infinite);
            CQAppContainer.GetInstance();
        }

        private void ReceiveMessage() {
            byte[] numArray = new byte[FrameSize];
            while (RunningFlag) {
                if (mySocket != null) {
                    if (mySocket.Available >= 1) {
                        try {
                            int from = mySocket.ReceiveFrom(numArray, ref RemotePoint);
                            string str = Encoding.Default.GetString(numArray, 0, from);

                            if (str.StartsWith("ServerHello")) {
                                _keepConnectionMsg = str;
                                keepConnectionResetEvent.Set();
                                continue;
                            }

                            if (str.StartsWith("SrvGroupMemberInfo") ||
                                str.StartsWith("SrvGroupMemberList") ||
                                str.StartsWith("SrvStrangerInfo") ||
                                str.StartsWith("SrvCookies") ||
                                str.StartsWith("SrvCsrfToken") ||
                                str.StartsWith("SrvLoginQQ") ||
                                str.StartsWith("SrvLoginNickname") ||
                                str.StartsWith("SrvAppDirectory")) {
                                _strMessage = str;
                                myResetEvent.Set();
                                continue;
                            }
                            new Thread(new ParameterizedThreadStart(AnalyzeMessage)) {
                                IsBackground = true,
                                Name = "QQ Msg Thread"
                            }.Start(str);

                        } catch {
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        private void AnalyzeMessage(object rawData) {
            CQMessageAnalysis.Analyze(rawData.ToString());
        }

        private void SendHelloMessage(object obj) {
            SendMessage(string.Format("ClientHello {0}", ((IPEndPoint)LocalPoint).Port));
            keepConnectionResetEvent.WaitOne();
            try {
                var lst = _keepConnectionMsg.Split(' ');
                ClientTimeout = int.Parse(lst[1]) / 2;
                FramePrefixSize = int.Parse(lst[2]);
                FramePayloadSize = int.Parse(lst[3]);
                FrameSize = int.Parse(lst[4]);
            } catch (Exception e) {
                nLogger.Error(e, "Exception: " + _keepConnectionMsg + " " + e.Message);
            }
            keepConnectTimer.Change(ClientTimeout * 1000, Timeout.Infinite);
        }

        internal void SendMessage(string message) {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            mySocket.SendTo(bytes, bytes.Length, SocketFlags.None, ServerPoint);
        }

        internal string GetStringMessage(string message) {
            byte[] bytes = Encoding.Default.GetBytes(message);
            mySocket.SendTo(bytes, bytes.Length, SocketFlags.None, ServerPoint);
            myResetEvent.WaitOne(3000);
            try {
                string[] strArray = _strMessage.Split(' ');
                _strMessage = string.Empty;
                return strArray[1];
            } catch (Exception e) {
                nLogger.Error(e, "Exception: " + _strMessage + " " + e.Message);
                return string.Empty;
            }
        }
    }
}
