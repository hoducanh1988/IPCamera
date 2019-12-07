using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function.Custom {

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
            cameraTelnetPassword = "";

            //wifi router
            wifiSSID2G = "IPCAM-TEST-2G";
            wifiSSID5G = "IPCAM-TEST-5G";

            //standard value
            firmwareVersion = "";
            hardwareVersion = "1";
            vnptMacHeader = "A06518:A4F4C2:D49AA0";
            vnptUidHeader = "VNTIPC";
            vnptProductNumber = "120";

            //test mode
            FailAndStop = "Yes";
            RetryTime = "3";

            //enable/disable test item
            IsCheckMacEthernet = true;
            IsCheckFirmwareBuildTime = true;
            IsCheckUID = true;
            IsWriteHardwareVersion = true;
            IsWriteSerialNumber = true;
            IsCheckSdCard = true;
            IsCheckImageSensor = true;
            IsCheckNightVision = true;
            IsCheckRgbLed = true;
            IsCheckIrLed = true;
            IsCheckAudio = true;
            IsCheckButton = true;
            IsCheckWifi = true;
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

        #region wifi router

        string _wifi_ssid_2g;
        public string wifiSSID2G {
            get { return _wifi_ssid_2g; }
            set {
                _wifi_ssid_2g = value;
                OnPropertyChanged(nameof(wifiSSID2G));
            }
        }
        string _wifi_ssid_5g;
        public string wifiSSID5G {
            get { return _wifi_ssid_5g; }
            set {
                _wifi_ssid_5g = value;
                OnPropertyChanged(nameof(wifiSSID5G));
            }
        }

        #endregion

        #region standard

        string _firmware_version;
        public string firmwareVersion {
            get { return _firmware_version; }
            set {
                _firmware_version = value;
                OnPropertyChanged(nameof(firmwareVersion));
            }
        }
        string _hardware_version;
        public string hardwareVersion {
            get { return _hardware_version; }
            set {
                _hardware_version = value;
                OnPropertyChanged(nameof(hardwareVersion));
            }
        }
        string _vnpt_mac_header;
        public string vnptMacHeader {
            get { return _vnpt_mac_header; }
            set {
                _vnpt_mac_header = value;
                OnPropertyChanged(nameof(vnptMacHeader));
            }
        }
        string _vnpt_uid_header;
        public string vnptUidHeader {
            get { return _vnpt_uid_header; }
            set {
                _vnpt_uid_header = value;
                OnPropertyChanged(nameof(vnptUidHeader));
            }
        }
        string _vnpt_product_number;
        public string vnptProductNumber {
            get { return _vnpt_product_number; }
            set {
                _vnpt_product_number = value;
                OnPropertyChanged(nameof(vnptProductNumber));
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

        #region enable/disable test item

        bool _is_check_mac_ethernet; //check mac ethernet
        public bool IsCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(IsCheckMacEthernet));
                stationVariable.myTesting.IsCheckMacEthernet = value;
            }
        }
        bool _is_check_fw_build_time; //check firmware build time
        public bool IsCheckFirmwareBuildTime {
            get { return _is_check_fw_build_time; }
            set {
                _is_check_fw_build_time = value;
                OnPropertyChanged(nameof(IsCheckFirmwareBuildTime));
                stationVariable.myTesting.IsCheckFirmwareBuildTime = value;
            }
        }
        bool _is_check_uid; //check uid code
        public bool IsCheckUID {
            get { return _is_check_uid; }
            set {
                _is_check_uid = value;
                OnPropertyChanged(nameof(IsCheckUID));
                stationVariable.myTesting.IsCheckUID = value;
            }
        }
        bool _is_write_hardware_version; //write hardware version
        public bool IsWriteHardwareVersion {
            get { return _is_write_hardware_version; }
            set {
                _is_write_hardware_version = value;
                OnPropertyChanged(nameof(IsWriteHardwareVersion));
                stationVariable.myTesting.IsWriteHardwareVersion = value;
            }
        }
        bool _is_write_serial; //write serial number
        public bool IsWriteSerialNumber {
            get { return _is_write_serial; }
            set {
                _is_write_serial = value;
                OnPropertyChanged(nameof(IsWriteSerialNumber));
                stationVariable.myTesting.IsWriteSerialNumber = value;
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
        bool _is_check_imagesensor; //check image sensor
        public bool IsCheckImageSensor {
            get { return _is_check_imagesensor; }
            set {
                _is_check_imagesensor = value;
                OnPropertyChanged(nameof(IsCheckImageSensor));
                stationVariable.myTesting.IsCheckImageSensor = value;
            }
        }
        bool _is_check_night_vision; //check night vision
        public bool IsCheckNightVision {
            get { return _is_check_night_vision; }
            set {
                _is_check_night_vision = value;
                OnPropertyChanged(nameof(IsCheckNightVision));
                stationVariable.myTesting.IsCheckNightVision = value;
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
        bool _is_check_ir_led; //check ir led
        public bool IsCheckIrLed {
            get { return _is_check_ir_led; }
            set {
                _is_check_ir_led = value;
                OnPropertyChanged(nameof(IsCheckIrLed));
                stationVariable.myTesting.IsCheckIrLed = value;
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
        bool _is_check_button; //check button
        public bool IsCheckButton {
            get { return _is_check_button; }
            set {
                _is_check_button = value;
                OnPropertyChanged(nameof(IsCheckButton));
                stationVariable.myTesting.IsCheckButton = value;
            }
        }
        bool _is_check_wifi; //check wifi
        public bool IsCheckWifi {
            get { return _is_check_wifi; }
            set {
                _is_check_wifi = value;
                OnPropertyChanged(nameof(IsCheckWifi));
                stationVariable.myTesting.IsCheckWifi = value;
            }
        }
        
        #endregion

    }

}
