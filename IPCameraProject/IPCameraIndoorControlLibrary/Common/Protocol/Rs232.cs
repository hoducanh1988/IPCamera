using System;
using System.IO.Ports;
using System.Threading;

namespace IPCameraIndoorControlLibrary.Common.Protocol {

    public class Rs232<T> : IProtocol where T : class, new() {

        bool _isConnected = false;
        SerialPort serialport = null;
        string port_name = "COM1";
        int baud_rate = 57600;
        int data_bit = 8;
        T obj_TestingInfo;

        public Rs232(T t, string _port_name, int _baud_rate, int _data_bit) {
            port_name = _port_name;
            baud_rate = _baud_rate;
            data_bit = _data_bit;
            obj_TestingInfo = t;
        }

        public bool IsConnected() {
            return _isConnected;
        }

        public bool Open() {
            try {
                serialport = new SerialPort();
                serialport.PortName = port_name;
                serialport.BaudRate = baud_rate;
                serialport.DataBits = data_bit;
                serialport.Parity = Parity.None;
                serialport.StopBits = StopBits.One;

                serialport.Open();
                _isConnected = serialport.IsOpen;
                return _isConnected;
            }
            catch (Exception ex) {
                binding_log_uart(ex.ToString());
                return false;
            }
        }

        public bool Open(string ip, int port) {
            return false;
        }

        public bool Close() {
            if (serialport != null && serialport.IsOpen) {
                serialport.Close();
            }
            return true;
        }

        public string Query(string cmd) {
            try {
                Read();
                WriteLine(cmd);

                Thread.Sleep(500);
                return Read();
            }
            catch (Exception ex) {
                binding_log_uart(ex.ToString());
                return null;
            }
        }

        public string Read() {
            if (!_isConnected) return null;
            try {
                string data = serialport.ReadExisting();
                binding_log_uart(data);
                return data;
            }
            catch (Exception ex) {
                binding_log_uart(ex.ToString());
                return null;
            }
        }

        public bool Write(string cmd) {
            if (!_isConnected) return false;
            try {
                //serialport.Write("\n");
                serialport.Write(cmd);
            }
            catch (Exception ex) {
                binding_log_uart(ex.ToString());
                return false;
            }
            return true;
        }

        public bool WriteLine(string cmd) {
            if (!_isConnected) return false;
            return Write(cmd + "\n");
        }

        public bool WriteCtrlBreak() {
            if (!_isConnected) return false;
            return Write(Convert.ToChar(3).ToString());
        }


        void binding_log_uart(string data) {
            var prop_loguart = obj_TestingInfo.GetType().GetProperty("logUart");
            string log_value = (string)prop_loguart.GetValue(obj_TestingInfo);
            log_value += data;
            prop_loguart.SetValue(obj_TestingInfo, log_value);
        }

    }
}
