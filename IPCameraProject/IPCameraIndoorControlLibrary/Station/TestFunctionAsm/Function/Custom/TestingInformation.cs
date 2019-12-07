using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function.Custom {

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
            serialNumber = "";
            uidCode = "";

            macResult = "-";
            firmwareResult = "-";
            uidResult = "-";
            hardwareResult = "-";
            serialResult = "-";
            sdCardResult = "-";
            imageSensorResult = "-";
            nightVisionResult = "-";
            rgbLedResult = "-";
            irLedResult = "-";
            audioResult = "-";
            buttonResult = "-";
            wifiResult = "-";

            logSystem = "";
            logTelnet = "";
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
        string _uid_code;
        public string uidCode {
            get { return _uid_code; }
            set {
                _uid_code = value;
                OnPropertyChanged(nameof(uidCode));
            }
        }
        string _serial_number;
        public string serialNumber {
            get { return _serial_number; }
            set {
                _serial_number = value;
                OnPropertyChanged(nameof(serialNumber));
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

        string _mac_result;
        public string macResult {
            get { return _mac_result; }
            set {
                _mac_result = value;
                OnPropertyChanged(nameof(macResult));
            }
        }
        string _firmware_result;
        public string firmwareResult {
            get { return _firmware_result; }
            set {
                _firmware_result = value;
                OnPropertyChanged(nameof(firmwareResult));
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
        string _hardware_result;
        public string hardwareResult {
            get { return _hardware_result; }
            set {
                _hardware_result = value;
                OnPropertyChanged(nameof(hardwareResult));
            }
        }
        string _serial_result;
        public string serialResult {
            get { return _serial_result; }
            set {
                _serial_result = value;
                OnPropertyChanged(nameof(serialResult));
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
        string _image_sensor_result;
        public string imageSensorResult {
            get { return _image_sensor_result; }
            set {
                _image_sensor_result = value;
                OnPropertyChanged(nameof(imageSensorResult));
            }
        }
        string _night_vision_result;
        public string nightVisionResult {
            get { return _night_vision_result; }
            set {
                _night_vision_result = value;
                OnPropertyChanged(nameof(nightVisionResult));
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
        string _ir_led_result;
        public string irLedResult {
            get { return _ir_led_result; }
            set {
                _ir_led_result = value;
                OnPropertyChanged(nameof(irLedResult));
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
        string _button_result;
        public string buttonResult {
            get { return _button_result; }
            set {
                _button_result = value;
                OnPropertyChanged(nameof(buttonResult));
            }
        }
        string _wifi_result;
        public string wifiResult {
            get { return _wifi_result; }
            set {
                _wifi_result = value;
                OnPropertyChanged(nameof(wifiResult));
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
        string _log_telnet;
        public string logTelnet {
            get { return _log_telnet; }
            set {
                _log_telnet = value;
                OnPropertyChanged(nameof(logTelnet));
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

        bool _is_check_mac_ethernet; //check mac ethernet
        public bool IsCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(IsCheckMacEthernet));
            }
        }
        bool _is_check_fw_build_time; //check firmware build time
        public bool IsCheckFirmwareBuildTime {
            get { return _is_check_fw_build_time; }
            set {
                _is_check_fw_build_time = value;
                OnPropertyChanged(nameof(IsCheckFirmwareBuildTime));
            }
        }
        bool _is_check_uid; //check uid code
        public bool IsCheckUID {
            get { return _is_check_uid; }
            set {
                _is_check_uid = value;
                OnPropertyChanged(nameof(IsCheckUID));
            }
        }
        bool _is_write_hardware_version; //write hardware version
        public bool IsWriteHardwareVersion {
            get { return _is_write_hardware_version; }
            set {
                _is_write_hardware_version = value;
                OnPropertyChanged(nameof(IsWriteHardwareVersion));
            }
        }
        bool _is_write_serial; //write serial number
        public bool IsWriteSerialNumber {
            get { return _is_write_serial; }
            set {
                _is_write_serial = value;
                OnPropertyChanged(nameof(IsWriteSerialNumber));
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
        bool _is_check_imagesensor; //check image sensor
        public bool IsCheckImageSensor {
            get { return _is_check_imagesensor; }
            set {
                _is_check_imagesensor = value;
                OnPropertyChanged(nameof(IsCheckImageSensor));
            }
        }
        bool _is_check_night_vision; //check night vision
        public bool IsCheckNightVision {
            get { return _is_check_night_vision; }
            set {
                _is_check_night_vision = value;
                OnPropertyChanged(nameof(IsCheckNightVision));
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
        bool _is_check_ir_led; //check ir led
        public bool IsCheckIrLed {
            get { return _is_check_ir_led; }
            set {
                _is_check_ir_led = value;
                OnPropertyChanged(nameof(IsCheckIrLed));
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
        bool _is_check_button; //check button
        public bool IsCheckButton {
            get { return _is_check_button; }
            set {
                _is_check_button = value;
                OnPropertyChanged(nameof(IsCheckButton));
            }
        }
        bool _is_check_wifi; //check wifi
        public bool IsCheckWifi {
            get { return _is_check_wifi; }
            set {
                _is_check_wifi = value;
                OnPropertyChanged(nameof(IsCheckWifi));
            }
        }

        #endregion

    }
}
