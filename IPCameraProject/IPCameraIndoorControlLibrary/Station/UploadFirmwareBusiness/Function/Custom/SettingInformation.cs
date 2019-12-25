using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function.Custom {

    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingInformation() {

            //ip camera
            cameraIP = "192.168.1.253";
            cameraIPFwBusinessFrom = "192.168.1.10";
            cameraTelnetUser = "root";
            cameraTelnetPassword = "vnpt@123";

            //fw thương mại
            fileFirmware = "";

            //tiêu chuẩn
            firmwareBuildTime = "";
            hardwareVersion = "1";
            vnptMacHeader = "A06518:A4F4C2:D49AA0";
            productMacCode = "D49AA0=G";
            vnptUidHeader = "VNTIPC";
            vnptProductNumber = "120";

            //chế độ
            FailAndStop = "Yes";
            RetryTime = "3";

            //dach sách test item
            IsUploadFirmwareBusiness = true;
            IsCheckFirmwareBuildTime = true;
            IsCheckMacEthernet = true;
            IsCheckSerialNumber = true;
            IsCheckUID = true;
            IsCheckHardwareVersion = true;

        }

        #region method

        public bool isTestFunction() {
            return IsCheckFirmwareBuildTime ||
                   IsCheckMacEthernet ||
                   IsCheckSerialNumber ||
                   IsCheckUID ||
                   IsCheckHardwareVersion;
        }

        public bool isNeedReboot() {
            return isTestFunction() && IsUploadFirmwareBusiness;
        }

        #endregion


        #region ip camera 

        string _camera_ip;
        public string cameraIP {
            get { return _camera_ip; }
            set {
                _camera_ip = value;
                OnPropertyChanged(nameof(cameraIP));
            }
        }
        string _camera_ip_fw_from;
        public string cameraIPFwBusinessFrom {
            get { return _camera_ip_fw_from; }
            set {
                _camera_ip_fw_from = value;
                OnPropertyChanged(nameof(cameraIPFwBusinessFrom));
            }
        }
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

        #endregion

        #region FW thương mại

        string _file_fw;
        public string fileFirmware {
            get { return _file_fw; }
            set {
                _file_fw = value;
                OnPropertyChanged(nameof(fileFirmware));
            }
        }

        #endregion

        #region Tiêu chuẩn

        string _firmware_build_time;
        public string firmwareBuildTime {
            get { return _firmware_build_time; }
            set {
                _firmware_build_time = value;
                OnPropertyChanged(nameof(firmwareBuildTime));
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
        string _product_mac_code;
        public string productMacCode {
            get { return _product_mac_code; }
            set {
                _product_mac_code = value;
                OnPropertyChanged(nameof(productMacCode));
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

        #region chế độ

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

        #region danh sách test item

        bool _is_upload_fw_tm; //upload firmware TM
        public bool IsUploadFirmwareBusiness {
            get { return _is_upload_fw_tm; }
            set {
                _is_upload_fw_tm = value;
                OnPropertyChanged(nameof(IsUploadFirmwareBusiness));
                stationVariable.myTesting.IsUploadFirmwareBusiness = value;
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
        bool _is_check_mac_ethernet; //check mac ethernet
        public bool IsCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(IsCheckMacEthernet));
                stationVariable.myTesting.IsCheckMacEthernet = value;
            }
        }
        bool _is_check_serial; //check serial number
        public bool IsCheckSerialNumber {
            get { return _is_check_serial; }
            set {
                _is_check_serial = value;
                OnPropertyChanged(nameof(IsCheckSerialNumber));
                stationVariable.myTesting.IsCheckSerialNumber = value;
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
        bool _is_check_hardware_version; //check hardware version
        public bool IsCheckHardwareVersion {
            get { return _is_check_hardware_version; }
            set {
                _is_check_hardware_version = value;
                OnPropertyChanged(nameof(IsCheckHardwareVersion));
                stationVariable.myTesting.IsCheckHardwareVersion = value;
            }
        }
       
        #endregion

    }
}
