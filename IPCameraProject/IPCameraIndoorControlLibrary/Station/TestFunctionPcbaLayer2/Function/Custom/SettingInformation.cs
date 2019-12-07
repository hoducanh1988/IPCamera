using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function.Custom {

    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingInformation() {

            SerialBaudRate = "57600";
            FailAndStop = "Yes";
            RetryTime = "3";

            IsCheckWifi = true;
            IsCheckSdCard = true;
            IsCheckEthernet = true;
            IsCheckRgbLed = true;
            IsCheckButton = true;
        }

        string _serial_port_name;
        public string SerialPortName {
            get { return _serial_port_name; }
            set {
                _serial_port_name = value;
                OnPropertyChanged(nameof(SerialPortName));
            }
        }
        string _serial_baud_rate;
        public string SerialBaudRate {
            get { return _serial_baud_rate; }
            set {
                _serial_baud_rate = value;
                OnPropertyChanged(nameof(SerialBaudRate));
            }
        }
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

        #region enable/disable test item

        bool _is_check_wifi; //check wifi
        public bool IsCheckWifi {
            get { return _is_check_wifi; }
            set {
                _is_check_wifi = value;
                OnPropertyChanged(nameof(IsCheckWifi));
                stationVariable.myTesting.IsCheckWifi = value;
            }
        }
        bool _is_check_sdcard; //check sd card
        public bool IsCheckSdCard {
            get { return _is_check_sdcard; }
            set {
                _is_check_sdcard = value;
                OnPropertyChanged(nameof(IsCheckSdCard));
                stationVariable.myTesting.IsCheckSdCard = value;
            }
        }
        bool _is_check_ethernet; //check ethernet
        public bool IsCheckEthernet {
            get { return _is_check_ethernet; }
            set {
                _is_check_ethernet = value;
                OnPropertyChanged(nameof(IsCheckEthernet));
                stationVariable.myTesting.IsCheckEthernet = value;
            }
        }
        bool _is_check_rgb_led; //check rgb led
        public bool IsCheckRgbLed {
            get { return _is_check_rgb_led; }
            set {
                _is_check_rgb_led = value;
                OnPropertyChanged(nameof(IsCheckRgbLed));
                stationVariable.myTesting.IsCheckRgbLed = value;
            }
        }
        bool _is_check_button; //check button
        public bool IsCheckButton {
            get { return _is_check_button; }
            set {
                _is_check_button = value;
                OnPropertyChanged(nameof(IsCheckButton));
                stationVariable.myTesting.IsCheckButton = value;
            }
        }

        #endregion



    }
}
