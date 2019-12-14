using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function.Custom {
    public class TestingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public TestingInformation() {
            Ready();
        }


        public void Ready() {
            TotalResult = "-";
            macEthernet = "";
            logSystem = "";
            logUart = "";
        }

        public void Checking() {
            TotalResult = "Waiting...";
        }
        public void Pass() {
            TotalResult = "Passed";
        }
        public void Fail() {
            TotalResult = "Failed";
        }


        string _mac_ethernet;
        public string macEthernet {
            get { return _mac_ethernet; }
            set {
                _mac_ethernet = value;
                OnPropertyChanged(nameof(macEthernet));
            }
        }
        string _ip_address;
        public string ipAddress {
            get { return _ip_address; }
            set {
                _ip_address = value;
                OnPropertyChanged(nameof(ipAddress));
            }
        }
        string _serial_port_name;
        public string serialPortName {
            get { return _serial_port_name; }
            set {
                _serial_port_name = value;
                OnPropertyChanged(nameof(serialPortName));
            }
        }
        string _total_result;
        public string TotalResult {
            get { return _total_result; }
            set {
                _total_result = value;
                OnPropertyChanged(nameof(TotalResult));
            }
        }
        string _log_system;
        public string logSystem {
            get { return _log_system; }
            set {
                _log_system = value;
                OnPropertyChanged(nameof(logSystem));
            }
        }
        string _log_uart;
        public string logUart {
            get { return _log_uart; }
            set {
                _log_uart = value;
                OnPropertyChanged(nameof(logUart));
            }
        }


        #region test item

        bool _is_write_mac;
        public bool IsWriteMacEthernet {
            get { return _is_write_mac; }
            set {
                _is_write_mac = value;
                OnPropertyChanged(nameof(IsWriteMacEthernet));
            }
        }
        bool _is_upload_fw_basic;
        public bool IsUploadFirmwareBasic {
            get { return _is_upload_fw_basic; }
            set {
                _is_upload_fw_basic = value;
                OnPropertyChanged(nameof(IsUploadFirmwareBasic));
            }
        }
        bool _is_write_static_ip;
        public bool IsWriteStaticIP {
            get { return _is_write_static_ip; }
            set {
                _is_write_static_ip = value;
                OnPropertyChanged(nameof(IsWriteStaticIP));
            }
        }

        #endregion
    }
}
