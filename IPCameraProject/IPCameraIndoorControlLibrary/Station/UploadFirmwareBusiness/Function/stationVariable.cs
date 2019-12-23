using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function {
    public class stationVariable {

        public static string settingFWBusiness = string.Format("{0}setting\\setting_fwbusiness.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideFWBusiness = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();
        public static ObservableCollection<Custom.UploadFwItemInfo> myUploadFWInfo = new ObservableCollection<Custom.UploadFwItemInfo>();
    }
}
