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
            cameraIPCalibFrom = "192.168.1.10";
            cameraTelnetUser = "root";
            cameraTelnetPassword = "vnpt@123";

            //standard
            nightVisionDarkLower = 0;
            nightVisionDarkUpper = 1000;
            nightVisionLightLower = 0;
            nightVisionLightUpper = 1000;

            //test mode
            FailAndStop = "Yes";
            RetryTime = "3";

            //test item
            IsCalibNightVisionLightMode = true;
            IsCalibNightVisionDarkMode = true;

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
        string _camera_ip_calib_from;
        public string cameraIPCalibFrom {
            get { return _camera_ip_calib_from; }
            set {
                _camera_ip_calib_from = value;
                OnPropertyChanged(nameof(cameraIPCalibFrom));
            }
        }

        #endregion


        #region standard

        int _nightvision_dark_lower;
        public int nightVisionDarkLower {
            get { return _nightvision_dark_lower; }
            set {
                _nightvision_dark_lower = value;
                OnPropertyChanged(nameof(nightVisionDarkLower));
            }
        }
        int _nightvision_dark_upper;
        public int nightVisionDarkUpper {
            get { return _nightvision_dark_upper; }
            set {
                _nightvision_dark_upper = value;
                OnPropertyChanged(nameof(nightVisionDarkUpper));
            }
        }
        int _nightvision_light_lower;
        public int nightVisionLightLower {
            get { return _nightvision_light_lower; }
            set {
                _nightvision_light_lower = value;
                OnPropertyChanged(nameof(nightVisionLightLower));
            }
        }
        int _nightvision_light_upper;
        public int nightVisionLightUpper {
            get { return _nightvision_light_upper; }
            set {
                _nightvision_light_upper = value;
                OnPropertyChanged(nameof(nightVisionLightUpper));
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

        #region test item

        bool _is_calib_nightvision_light_mode;
        public bool IsCalibNightVisionLightMode {
            get { return _is_calib_nightvision_light_mode; }
            set {
                _is_calib_nightvision_light_mode = value;
                OnPropertyChanged(nameof(IsCalibNightVisionLightMode));
                stationVariable.myTesting.IsCalibNightVisionLightMode = value;
            }
        }
        bool _is_calib_nightvision_dark_mode;
        public bool IsCalibNightVisionDarkMode {
            get { return _is_calib_nightvision_dark_mode; }
            set {
                _is_calib_nightvision_dark_mode = value;
                OnPropertyChanged(nameof(IsCalibNightVisionDarkMode));
                stationVariable.myTesting.IsCalibNightVisionDarkMode = value;
            }
        }

        #endregion
    }
}
