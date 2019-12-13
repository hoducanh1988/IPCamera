using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.CalibNightVision.Function.Custom {
    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingInformation() {
            //camera
            cameraIP = "192.168.1.253";
            cameraTelnetUser = "root";
            cameraTelnetPassword = "vnpt@123";

            //test mode
            FailAndStop = "Yes";
            RetryTime = "3";
        }

        #region ip camera

        string _camera_telnet_user;
        public string cameraTelnetUser {
            get { return _camera_telnet_user; }
            set {
                _camera_telnet_user = value;
                OnPropertyChanged(nameof(cameraTelnetUser));
            }
        }
        string _camera_telnet_pass;
        public string cameraTelnetPassword {
            get { return _camera_telnet_pass; }
            set {
                _camera_telnet_pass = value;
                OnPropertyChanged(nameof(cameraTelnetPassword));
            }
        }
        string _camera_ip;
        public string cameraIP {
            get { return _camera_ip; }
            set {
                _camera_ip = value;
                OnPropertyChanged(nameof(cameraIP));
            }
        }

        #endregion


        #region test mode

        string _fail_and_stop;
        public string FailAndStop {
            get { return _fail_and_stop; }
            set {
                _fail_and_stop = value;
                OnPropertyChanged(nameof(FailAndStop));
            }
        }
        string _retry_time;
        public string RetryTime {
            get { return _retry_time; }
            set {
                _retry_time = value;
                OnPropertyChanged(nameof(RetryTime));
            }
        }

        #endregion
    }
}
