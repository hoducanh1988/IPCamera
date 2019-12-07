using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraProject
{
    public class inputInformation : INotifyPropertyChanged {

        //Implement interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public inputInformation() {
            ProductName = "";
            StationName = "";
        }

        //Property
        string _productname;
        public string ProductName {
            get { return _productname; }
            set {
                _productname = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }
        string _stationname;
        public string StationName {
            get { return _stationname; }
            set {
                _stationname = value;
                OnPropertyChanged(nameof(StationName));
            }
        }
        
    }
}
