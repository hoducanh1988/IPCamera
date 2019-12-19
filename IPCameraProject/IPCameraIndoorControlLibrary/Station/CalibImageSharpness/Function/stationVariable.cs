using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.CalibImageSharpness.Function {
    public class stationVariable {

        public static string settingCalibSharpness = string.Format("{0}setting\\setting_calibsharpness.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideCalibSharpness = string.Format("{0}guide\\guide_calibsharpness.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();

    }
}
