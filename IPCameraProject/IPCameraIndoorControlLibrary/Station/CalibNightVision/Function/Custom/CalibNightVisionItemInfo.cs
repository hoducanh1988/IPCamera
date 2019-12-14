using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.CalibNightVision.Function.Custom {

    public class CalibNightVisionItemInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public CalibNightVisionItemInfo() {
            macEthernet = "";
            ipAddress = "";
            changeIPResult = "-";
            calibLightResult = "-";
            calibDarkResult = "-";
            verifyResult = "-";
            Result = "-";
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
        string _change_ip_result;
        public string changeIPResult {
            get { return _change_ip_result; }
            set {
                _change_ip_result = value;
                OnPropertyChanged(nameof(changeIPResult));
            }
        }
        string _calib_light_result;
        public string calibLightResult {
            get { return _calib_light_result; }
            set {
                _calib_light_result = value;
                OnPropertyChanged(nameof(calibLightResult));
            }
        }
        string _calib_dark_result;
        public string calibDarkResult {
            get { return _calib_dark_result; }
            set {
                _calib_dark_result = value;
                OnPropertyChanged(nameof(calibDarkResult));
            }
        }
        string _verify_result;
        public string verifyResult {
            get { return _verify_result; }
            set {
                _verify_result = value;
                OnPropertyChanged(nameof(verifyResult));
            }
        }
        string _result;
        public string Result {
            get { return _result; }
            set {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

    }
}
