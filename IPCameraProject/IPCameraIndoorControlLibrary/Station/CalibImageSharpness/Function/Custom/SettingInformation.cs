using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.CalibImageSharpness.Function.Custom {

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

            //standard value
            areaTestChart = "0,0,400,300"; //left,top,width,height
            areaRectangle = "0,0,400,300"; //left,top,width,height
            sharpnessStandard = 9;
            toleranceSharpness = 0.5;

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
        string _camera_rtsp;
        public string cameraRtspLink {
            get { return _camera_rtsp; }
            set {
                _camera_rtsp = value;
                OnPropertyChanged(nameof(cameraRtspLink));
            }
        }
        string _camera_ip;
        public string cameraIP {
            get { return _camera_ip; }
            set {
                _camera_ip = value;
                OnPropertyChanged(nameof(cameraIP));
                cameraRtspLink = string.Format("rtsp://{0}:43794/profile1", value);
            }
        }

        #endregion


        #region standard

        string _area_test_chart;
        public string areaTestChart {
            get { return _area_test_chart; }
            set {
                _area_test_chart = value;
                OnPropertyChanged(nameof(areaTestChart));
            }
        }
        string _area_rectangle;
        public string areaRectangle {
            get { return _area_rectangle; }
            set {
                _area_rectangle = value;
                OnPropertyChanged(nameof(areaRectangle));
            }
        }
        double _tolerance_sharpness;
        public double toleranceSharpness {
            get { return _tolerance_sharpness; }
            set {
                _tolerance_sharpness = value;
                OnPropertyChanged(nameof(toleranceSharpness));
            }
        }
        double _sharpness_standard;
        public double sharpnessStandard {
            get { return _sharpness_standard; }
            set {
                _sharpness_standard = value;
                OnPropertyChanged(nameof(sharpnessStandard));
            }
        }

        #endregion


    }

}
