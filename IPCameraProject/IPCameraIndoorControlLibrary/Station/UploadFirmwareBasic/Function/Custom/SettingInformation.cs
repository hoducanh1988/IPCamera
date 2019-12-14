using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function.Custom {
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
            cameraIPUploadFrom = "192.168.1.10";
            cameraStaticIP = "192.168.1.253";

            //usb to uart
            SerialPortName1 = "";
            SerialPortName2 = "";
            SerialPortName3 = "";
            SerialPortName4 = "";

            //standard
            vnptMacHeader = "A06518:A4F4C2:D49AA0";

            //test mode
            FailAndStop = "Yes";
            RetryTime = "3";

            //test item
            IsWriteMacEthernet = true;
            IsUploadFirmwareBasic = true;
            IsWriteStaticIP = true;
        }

        #region camera

        string _camera_ip_upload_from;
        public string cameraIPUploadFrom {
            get { return _camera_ip_upload_from; }
            set {
                _camera_ip_upload_from = value;
                OnPropertyChanged(nameof(cameraIPUploadFrom));
            }
        }
        string _camera_static_ip;
        public string cameraStaticIP {
            get { return _camera_static_ip; }
            set {
                _camera_static_ip = value;
                OnPropertyChanged(nameof(cameraStaticIP));
            }
        }

        #endregion

        #region usb to uart

        string _serial_port_name_1;
        public string SerialPortName1 {
            get { return _serial_port_name_1; }
            set {
                _serial_port_name_1 = value;
                OnPropertyChanged(nameof(SerialPortName1));
            }
        }
        string _serial_port_name_2;
        public string SerialPortName2 {
            get { return _serial_port_name_2; }
            set {
                _serial_port_name_2 = value;
                OnPropertyChanged(nameof(SerialPortName2));
            }
        }
        string _serial_port_name_3;
        public string SerialPortName3 {
            get { return _serial_port_name_3; }
            set {
                _serial_port_name_3 = value;
                OnPropertyChanged(nameof(SerialPortName3));
            }
        }
        string _serial_port_name_4;
        public string SerialPortName4 {
            get { return _serial_port_name_4; }
            set {
                _serial_port_name_4 = value;
                OnPropertyChanged(nameof(SerialPortName4));
            }
        }

        #endregion

        #region standard

        string _vnpt_mac_header;
        public string vnptMacHeader {
            get { return _vnpt_mac_header; }
            set {
                _vnpt_mac_header = value;
                OnPropertyChanged(nameof(vnptMacHeader));
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

        bool _is_write_mac;
        public bool IsWriteMacEthernet {
            get { return _is_write_mac; }
            set {
                _is_write_mac = value;
                OnPropertyChanged(nameof(IsWriteMacEthernet));
            }
        }
        bool _is_upload_fw_basic;
        public bool IsUploadFirmwareBasic {
            get { return _is_upload_fw_basic; }
            set {
                _is_upload_fw_basic = value;
                OnPropertyChanged(nameof(IsUploadFirmwareBasic));
            }
        }
        bool _is_write_static_ip;
        public bool IsWriteStaticIP {
            get { return _is_write_static_ip; }
            set {
                _is_write_static_ip = value;
                OnPropertyChanged(nameof(IsWriteStaticIP));
            }
        }

        #endregion
    }
}
