using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom {

    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingInformation() {

            //mac header
            vnptMacHeader = "A06518:A4F4C2:D49AA0";

            //serial number
            vnptProductNumber = "120";
            productionFactory = "1";
            productionYear = "";
            productionWeek = "";
            hardwareVersion = "1";

            //uid
            vnptUidHeader = "VNTIPC";

            //ms access file
            fileMsAccess = "";

            //test mode
            FailAndStop = "Yes";
            RetryTime = "3";

        }

        #region mac

        string _vnpt_mac_header;
        public string vnptMacHeader {
            get { return _vnpt_mac_header; }
            set {
                _vnpt_mac_header = value;
                OnPropertyChanged(nameof(vnptMacHeader));
            }
        }

        #endregion

        #region serial number

        string _vnpt_product_number;
        public string vnptProductNumber {
            get { return _vnpt_product_number; }
            set {
                _vnpt_product_number = value;
                OnPropertyChanged(nameof(vnptProductNumber));
            }
        }
        string _production_factory;
        public string productionFactory {
            get { return _production_factory; }
            set {
                _production_factory = value;
                OnPropertyChanged(nameof(productionFactory));
            }
        }
        string _production_year;
        public string productionYear {
            get { return _production_year; }
            set {
                _production_year = value;
                OnPropertyChanged(nameof(productionYear));
            }
        }
        string _production_week;
        public string productionWeek {
            get { return _production_week; }
            set {
                _production_week = value;
                OnPropertyChanged(nameof(productionWeek));
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
        string _product_color;
        public string productColor {
            get { return _product_color; }
            set {
                _product_color = value;
                OnPropertyChanged(nameof(productColor));
            }
        }

        #endregion

        #region uid

        string _vnpt_uid_header;
        public string vnptUidHeader {
            get { return _vnpt_uid_header; }
            set {
                _vnpt_uid_header = value;
                OnPropertyChanged(nameof(vnptUidHeader));
            }
        }

        #endregion

        #region file ms access

        string _file_ms_access;
        public string fileMsAccess {
            get { return _file_ms_access; }
            set {
                _file_ms_access = value;
                OnPropertyChanged(nameof(fileMsAccess));
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


    }
}
