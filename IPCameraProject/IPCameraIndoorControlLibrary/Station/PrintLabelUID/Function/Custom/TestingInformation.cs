using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom {

    public class TestingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        //Constructor ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public TestingInformation() {
            InitParameters();
        }

        //Method ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void InitParameters() {
            MacAddress = "";
            SerialNumber = "";
            UidCode = "";
            TotalResult = "-";
            ErrorMessage = "";
            TestTime = "";
        }
        public void WaitingParameters() {
            TotalResult = "Waiting...";
            ErrorMessage = "";
        }
        public void FailParameters() {
            TotalResult = "Failed";
        }
        public void PassParameters() {
            TotalResult = "Passed";
        }


        //Property ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _mac_address;
        public string MacAddress {
            get { return _mac_address; }
            set {
                _mac_address = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }
        string _serial_number;
        public string SerialNumber {
            get { return _serial_number; }
            set {
                _serial_number = value;
                OnPropertyChanged(nameof(SerialNumber));
            }
        }
        string _uid_code;
        public string UidCode {
            get { return _uid_code; }
            set {
                _uid_code = value;
                OnPropertyChanged(nameof(UidCode));
            }
        }
        string _totalresult;
        public string TotalResult {
            get { return _totalresult; }
            set {
                _totalresult = value;
                OnPropertyChanged(nameof(TotalResult));
            }
        }
        string _errormessage;
        public string ErrorMessage {
            get { return _errormessage; }
            set {
                _errormessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        string _testtime;
        public string TestTime {
            get { return _testtime; }
            set {
                _testtime = value;
                OnPropertyChanged(nameof(TestTime));
            }
        }

    }
}
