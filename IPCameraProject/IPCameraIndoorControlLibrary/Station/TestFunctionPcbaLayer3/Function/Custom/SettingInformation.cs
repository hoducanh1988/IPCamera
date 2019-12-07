using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function.Custom {

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

            lightSensorDarkLower = 10;
            lightSensorDarkUpper = 60;
            lightSensorLightLower = 100;
            lightSensorLightUpper = 1024;

            IsCheckUsb = true;
            IsCheckSdCard = true;
            IsCheckEthernet = true;
            IsCheckImageSensor = true;
            IsCheckAudio = true;
            IsCheckLightSensor = true;
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
        int _light_sensor_dark_lower;
        public int lightSensorDarkLower {
            get { return _light_sensor_dark_lower; }
            set {
                _light_sensor_dark_lower = value;
                OnPropertyChanged(nameof(lightSensorDarkLower));
            }
        }
        int _light_sensor_dark_upper;
        public int lightSensorDarkUpper {
            get { return _light_sensor_dark_upper; }
            set {
                _light_sensor_dark_upper = value;
                OnPropertyChanged(nameof(lightSensorDarkUpper));
            }
        }
        int _light_sensor_light_lower;
        public int lightSensorLightLower {
            get { return _light_sensor_light_lower; }
            set {
                _light_sensor_light_lower = value;
                OnPropertyChanged(nameof(lightSensorLightLower));
            }
        }
        int _light_sensor_light_upper;
        public int lightSensorLightUpper {
            get { return _light_sensor_light_upper; }
            set {
                _light_sensor_light_upper = value;
                OnPropertyChanged(nameof(lightSensorLightUpper));
            }
        }

        #region enable/disable test item

        bool _is_check_usb; //check usb
        public bool IsCheckUsb {
            get { return _is_check_usb; }
            set {
                _is_check_usb = value;
                OnPropertyChanged(nameof(IsCheckUsb));
                stationVariable.myTesting.IsCheckUsb = value;
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
        bool _is_check_imagesensor; //check image sensor
        public bool IsCheckImageSensor {
            get { return _is_check_imagesensor; }
            set {
                _is_check_imagesensor = value;
                OnPropertyChanged(nameof(IsCheckImageSensor));
                stationVariable.myTesting.IsCheckImageSensor = value;
            }
        }
        bool _is_check_audio; //check audio
        public bool IsCheckAudio {
            get { return _is_check_audio; }
            set {
                _is_check_audio = value;
                OnPropertyChanged(nameof(IsCheckAudio));
                stationVariable.myTesting.IsCheckAudio = value;
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
        bool _is_check_lightsensor; //check light sensor
        public bool IsCheckLightSensor {
            get { return _is_check_lightsensor; }
            set {
                _is_check_lightsensor = value;
                OnPropertyChanged(nameof(IsCheckLightSensor));
                stationVariable.myTesting.IsCheckLightSensor = value;
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
