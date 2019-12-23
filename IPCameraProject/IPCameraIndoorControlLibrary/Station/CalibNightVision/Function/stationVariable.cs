using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.CalibNightVision.Function {
    public class stationVariable {

        public static string settingcalibNightVision = string.Format("{0}setting\\setting_calibnightvision.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guidecalibNightVision = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();
        public static ObservableCollection<Custom.CalibNightVisionItemInfo> myCalibNightVisionInfo = new ObservableCollection<Custom.CalibNightVisionItemInfo>();
    }
}
