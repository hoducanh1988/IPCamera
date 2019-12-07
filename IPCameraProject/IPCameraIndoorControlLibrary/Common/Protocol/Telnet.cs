using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IPCameraIndoorControlLibrary.Common.Protocol {
    public class Telnet <T> : IProtocol where T : class, new() {

        bool _isConnected = false;
        TcpClient clients;
        string ip = "192.168.1.253";
        int port = 23;
        T obj_TestingInfo;

        public Telnet(T t, string _ip, int _port) {
            ip = _ip;
            port = _port;
            obj_TestingInfo = t;
        }

        public bool IsConnected() {
            return _isConnected;
        }

        public bool Open() {
            this.clients = new TcpClient();
            this.configTCP();

            try { _isConnected = this.clients.ConnectAsync(ip, port).Wait(3000); }
            catch (Exception ex) {
                binding_log_telnet(ex.ToString());
                _isConnected = false;
            }

            return _isConnected;
        }

        void configTCP() {
            // Don't allow another process to bind to this port.
            this.clients.ExclusiveAddressUse = false;
            // sets the amount of time to linger after closing, using the LingerOption public property.
            this.clients.LingerState = new LingerOption(false, 0);
            // Sends data immediately upon calling NetworkStream.Write.
            this.clients.NoDelay = true;
            // Sets the receive buffer size using the ReceiveBufferSize public property.
            this.clients.ReceiveBufferSize = 1024;
            // Sets the receive time out using the ReceiveTimeout public property.
            this.clients.ReceiveTimeout = 5000;
            // Sets the send buffer size using the SendBufferSize public property.
            this.clients.SendBufferSize = 1024;
            // sets the send time out using the SendTimeout public property.
            this.clients.SendTimeout = 5000;
        }

        public bool Close() {
            if (this.clients != null) this.clients.Close();
            return true;
        }

        public string Query(string cmd) {
            if (!_isConnected) return null;
            try {
                WriteLine(cmd);

                Thread.Sleep(500);
                return Read();
            } catch (Exception ex) {
                binding_log_telnet(ex.ToString());
                return null;
            }
        }

        public string Read() {
            if (!_isConnected) return null;
            NetworkStream stream = this.clients.GetStream();

            //wait data available
            int count = 0;
            WAIT_AVAILABLE:
            count++;
            bool _isAvailable = stream.DataAvailable;
            if (!_isAvailable) {
                if (count < 300) {
                    Thread.Sleep(10);
                    goto WAIT_AVAILABLE;
                }
                else return null;
            }

            //read data to end
            StringBuilder sb = new StringBuilder();
            int input = 0;
            WAIT_READ:
            input = this.clients.GetStream().ReadByte();
            sb.Append((char)input);
            if (this.clients.Available > 0) goto WAIT_READ;

            //return data
            string data = sb.ToString();
            binding_log_telnet(data);
            return data;
        }

        public bool Write(string cmd) {
            if (!_isConnected) return false;
            byte[] buf = ASCIIEncoding.ASCII.GetBytes(cmd);
            this.clients.GetStream().Write(buf, 0, buf.Length);
            return true;
        }

        public bool WriteLine(string cmd) {
            if (!_isConnected) return false;
            this.Write(cmd + "\n");
            return true;
        }

        public bool WriteCtrlBreak() {
            if (!_isConnected) return false;
            byte[] buf = ASCIIEncoding.ASCII.GetBytes(new char[] { Convert.ToChar(03) });
            this.clients.GetStream().Write(buf, 0, buf.Length);
            return true;
        }

        void binding_log_telnet(string data) {
            var prop_logtelnet = obj_TestingInfo.GetType().GetProperty("logTelnet");
            string log_value = (string)prop_logtelnet.GetValue(obj_TestingInfo);
            log_value += data;
            prop_logtelnet.SetValue(obj_TestingInfo, log_value);
        }

    }
}
