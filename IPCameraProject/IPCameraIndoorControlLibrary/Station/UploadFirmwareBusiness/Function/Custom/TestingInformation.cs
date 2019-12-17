using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function.Custom {

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
            cameraQuantity = "4";
            progressIndex = 0;
            totalTime = "00:00:00";
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

        string _total_time;
        public string totalTime {
            get { return _total_time; }
            set {
                _total_time = value;
                OnPropertyChanged(nameof(totalTime));
            }
        }
        string _camera_quantity;
        public string cameraQuantity {
            get { return _camera_quantity; }
            set {
                _camera_quantity = value;
                OnPropertyChanged(nameof(cameraQuantity));
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
        int progress_index;
        public int progressIndex {
            get { return progress_index; }
            set {
                progress_index = value;
                OnPropertyChanged(nameof(progressIndex));
            }
        }


        #region danh sách test item

        bool _is_upload_fw_tm; //upload firmware TM
        public bool IsUploadFirmwareBusiness {
            get { return _is_upload_fw_tm; }
            set {
                _is_upload_fw_tm = value;
                OnPropertyChanged(nameof(IsUploadFirmwareBusiness));
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
        bool _is_check_mac_ethernet; //check mac ethernet
        public bool IsCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(IsCheckMacEthernet));
            }
        }
        bool _is_check_serial; //check serial number
        public bool IsCheckSerialNumber {
            get { return _is_check_serial; }
            set {
                _is_check_serial = value;
                OnPropertyChanged(nameof(IsCheckSerialNumber));
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
        bool _is_check_hardware_version; //check hardware version
        public bool IsCheckHardwareVersion {
            get { return _is_check_hardware_version; }
            set {
                _is_check_hardware_version = value;
                OnPropertyChanged(nameof(IsCheckHardwareVersion));
            }
        }

        #endregion

    }
}
