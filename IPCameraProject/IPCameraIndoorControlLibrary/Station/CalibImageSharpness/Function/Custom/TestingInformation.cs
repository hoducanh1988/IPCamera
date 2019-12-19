using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IPCameraIndoorControlLibrary.Station.CalibImageSharpness.Function.Custom {


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

            buttonContent = "START";
            TotalResult = "-";
            logSystem = "";
            logTelnet = "";
            macEthernet = "-";
            totalTime = "00:00:00";
            imageSource = null;
            imageCrop = null;
        }
        public void Checking() {
            TotalResult = "Waiting...";
            buttonContent = "STOP";
        }
        public void Pass() {
            TotalResult = "Passed";
            buttonContent = "START";
        }
        public void Fail() {
            TotalResult = "Failed";
            buttonContent = "START";
        }
        public void Finish() {
            buttonContent = "START";
        }

        string _total_time;
        public string totalTime {
            get { return _total_time; }
            set {
                _total_time = value;
                OnPropertyChanged(nameof(totalTime));
            }
        }
        string _mac_ethernet;
        public string macEthernet {
            get { return _mac_ethernet; }
            set {
                _mac_ethernet = value;
                OnPropertyChanged(nameof(macEthernet));
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
        string _total_result;
        public string TotalResult {
            get { return _total_result; }
            set {
                _total_result = value;
                OnPropertyChanged(nameof(TotalResult));
            }
        }
        string _button_content;
        public string buttonContent {
            get { return _button_content; }
            set {
                _button_content = value;
                OnPropertyChanged(nameof(buttonContent));
            }
        }
        ImageSource _image_source;
        public ImageSource imageSource {
            get { return _image_source; }
            set {
                _image_source = value;
                OnPropertyChanged(nameof(imageSource));
            }
        }
        BitmapImage _image_crop;
        public BitmapImage imageCrop {
            get { return _image_crop; }
            set {
                _image_crop = value;
                OnPropertyChanged(nameof(imageCrop));
            }
        }

    }

}
