using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function.Custom {

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
            macEthernet = "";

            UsbResult = "-";
            sdCardResult = "-";
            ethernetResult = "-";
            imageSensorResult = "-";
            audioResult = "-";
            irCutResult = "-";
            irLedResult = "-";
            rgbLedResult = "-";
            lightSensorResult = "-";
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
        string _mac_ethernet;
        public string macEthernet {
            get { return _mac_ethernet; }
            set {
                _mac_ethernet = value;
                OnPropertyChanged(nameof(macEthernet));
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
        string _usb_result;
        public string UsbResult {
            get { return _usb_result; }
            set {
                _usb_result = value;
                OnPropertyChanged(nameof(UsbResult));
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
        string _image_sensor_result;
        public string imageSensorResult {
            get { return _image_sensor_result; }
            set {
                _image_sensor_result = value;
                OnPropertyChanged(nameof(imageSensorResult));
            }
        }
        string _audio_result;
        public string audioResult {
            get { return _audio_result; }
            set {
                _audio_result = value;
                OnPropertyChanged(nameof(audioResult));
            }
        }
        string _ir_cut_result;
        public string irCutResult {
            get { return _ir_cut_result; }
            set {
                _ir_cut_result = value;
                OnPropertyChanged(nameof(irCutResult));
            }
        }
        string _ir_led_result;
        public string irLedResult {
            get { return _ir_led_result; }
            set {
                _ir_led_result = value;
                OnPropertyChanged(nameof(irLedResult));
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
        string _light_sensor_result;
        public string lightSensorResult {
            get { return _light_sensor_result; }
            set {
                _light_sensor_result = value;
                OnPropertyChanged(nameof(lightSensorResult));
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

        bool _is_check_usb; //check usb
        public bool IsCheckUsb {
            get { return _is_check_usb; }
            set {
                _is_check_usb = value;
                OnPropertyChanged(nameof(IsCheckUsb));
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
        bool _is_check_imagesensor; //check image sensor
        public bool IsCheckImageSensor {
            get { return _is_check_imagesensor; }
            set {
                _is_check_imagesensor = value;
                OnPropertyChanged(nameof(IsCheckImageSensor));
            }
        }
        bool _is_check_audio; //check audio
        public bool IsCheckAudio {
            get { return _is_check_audio; }
            set {
                _is_check_audio = value;
                OnPropertyChanged(nameof(IsCheckAudio));
            }
        }
        bool _is_check_ir_cut; //check ir cut
        public bool IsCheckIrCut {
            get { return _is_check_ir_cut; }
            set {
                _is_check_ir_cut = value;
                OnPropertyChanged(nameof(IsCheckIrCut));
            }
        }
        bool _is_check_ir_led; //check ir led
        public bool IsCheckIrLed {
            get { return _is_check_ir_led; }
            set {
                _is_check_ir_led = value;
                OnPropertyChanged(nameof(IsCheckIrLed));
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
        bool _is_check_lightsensor; //check light sensor
        public bool IsCheckLightSensor {
            get { return _is_check_lightsensor; }
            set {
                _is_check_lightsensor = value;
                OnPropertyChanged(nameof(IsCheckLightSensor));
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
