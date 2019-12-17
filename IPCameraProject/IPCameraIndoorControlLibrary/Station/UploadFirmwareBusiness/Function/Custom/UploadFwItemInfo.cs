using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function.Custom {

    public class UploadFwItemInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public UploadFwItemInfo() {
            macEthernet = "";
            ipAddress = "";
            logTelnet = "";
            logSystem = "";
            totalTime = "00:00:00";

            uploadResult = "-";
            rebootResult = "-";
            firmwareResult = "-";
            macResult = "-";
            serialResult = "-";
            uidResult = "-";
            hardwareResult = "-";
            totalResult = "-";
        }

        #region template
        

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
        string _logsystem;
        public string logSystem {
            get { return _logsystem; }
            set {
                _logsystem = value;
                OnPropertyChanged(nameof(logSystem));
            }
        }
        string _log_telnet;
        public string logTelnet {
            get { return _log_telnet; }
            set {
                _log_telnet = value;
                OnPropertyChanged(nameof(logTelnet));
            }
        }
        string _total_time;
        public string totalTime {
            get { return _total_time; }
            set {
                _total_time = value;
                OnPropertyChanged(nameof(totalTime));
            }
        }

        #endregion

        #region result

        string _upload_result;
        public string uploadResult {
            get { return _upload_result; }
            set {
                _upload_result = value;
                OnPropertyChanged(nameof(uploadResult));
            }
        }
        string _reboot_result;
        public string rebootResult {
            get { return _reboot_result; }
            set {
                _reboot_result = value;
                OnPropertyChanged(nameof(rebootResult));
            }
        }
        string _fw_result;
        public string firmwareResult {
            get { return _fw_result; }
            set {
                _fw_result = value;
                OnPropertyChanged(nameof(firmwareResult));
            }
        }
        string _mac_result;
        public string macResult {
            get { return _mac_result; }
            set {
                _mac_result = value;
                OnPropertyChanged(nameof(macResult));
            }
        }
        string _sn_result;
        public string serialResult {
            get { return _sn_result; }
            set {
                _sn_result = value;
                OnPropertyChanged(nameof(serialResult));
            }
        }
        string _uid_result;
        public string uidResult {
            get { return _uid_result; }
            set {
                _uid_result = value;
                OnPropertyChanged(nameof(uidResult));
            }
        }
        string _hw_result;
        public string hardwareResult {
            get { return _hw_result; }
            set {
                _hw_result = value;
                OnPropertyChanged(nameof(hardwareResult));
            }
        }
        string _total_result;
        public string totalResult {
            get { return _total_result; }
            set {
                _total_result = value;
                OnPropertyChanged(nameof(totalResult));
            }
        }

        #endregion
    
    }
}
