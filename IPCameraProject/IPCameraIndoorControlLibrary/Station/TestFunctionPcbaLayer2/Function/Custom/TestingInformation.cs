using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function.Custom {

    public class TestingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //constructor 
        public TestingInformation() {
            Ready();
        }

        #region Method

        public void Ready() {

            buttonContent = "START";
            TotalResult = "-";
            macWlan = "";

            WifiResult = "-";
            sdCardResult = "-";
            ethernetResult = "-";
            rgbLedResult = "-";
            buttonResult = "-";

            logSystem = "";
            logUart = "";
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

        #endregion

        #region property

        string _mac_wlan;
        public string macWlan {
            get { return _mac_wlan; }
            set {
                _mac_wlan = value;
                OnPropertyChanged(nameof(macWlan));
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
        string _wifi_result;
        public string WifiResult {
            get { return _wifi_result; }
            set {
                _wifi_result = value;
                OnPropertyChanged(nameof(WifiResult));
            }
        }
        string _sd_card_result;
        public string sdCardResult {
            get { return _sd_card_result; }
            set {
                _sd_card_result = value;
                OnPropertyChanged(nameof(sdCardResult));
            }
        }
        string _ethernet_result;
        public string ethernetResult {
            get { return _ethernet_result; }
            set {
                _ethernet_result = value;
                OnPropertyChanged(nameof(ethernetResult));
            }
        }
        string _rgb_led_result;
        public string rgbLedResult {
            get { return _rgb_led_result; }
            set {
                _rgb_led_result = value;
                OnPropertyChanged(nameof(rgbLedResult));
            }
        }
        string _button_result;
        public string buttonResult {
            get { return _button_result; }
            set {
                _button_result = value;
                OnPropertyChanged(nameof(buttonResult));
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
        string _button_content;
        public string buttonContent {
            get { return _button_content; }
            set {
                _button_content = value;
                OnPropertyChanged(nameof(buttonContent));
            }
        }

        #endregion

        #region enable/disable test item

        bool _is_check_wifi; //check wifi
        public bool IsCheckWifi {
            get { return _is_check_wifi; }
            set {
                _is_check_wifi = value;
                OnPropertyChanged(nameof(IsCheckWifi));
            }
        }
        bool _is_check_sdcard; //check sd card
        public bool IsCheckSdCard {
            get { return _is_check_sdcard; }
            set {
                _is_check_sdcard = value;
                OnPropertyChanged(nameof(IsCheckSdCard));
            }
        }
        bool _is_check_ethernet; //check ethernet
        public bool IsCheckEthernet {
            get { return _is_check_ethernet; }
            set {
                _is_check_ethernet = value;
                OnPropertyChanged(nameof(IsCheckEthernet));
            }
        }
        bool _is_check_rgb_led; //check rgb led
        public bool IsCheckRgbLed {
            get { return _is_check_rgb_led; }
            set {
                _is_check_rgb_led = value;
                OnPropertyChanged(nameof(IsCheckRgbLed));
            }
        }
        bool _is_check_button; //check button
        public bool IsCheckButton {
            get { return _is_check_button; }
            set {
                _is_check_button = value;
                OnPropertyChanged(nameof(IsCheckButton));
            }
        }

        #endregion
    }
}
