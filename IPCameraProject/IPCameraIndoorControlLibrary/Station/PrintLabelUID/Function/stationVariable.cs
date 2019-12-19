using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPCameraIndoorControlLibrary.Common.IO;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function {
    public class stationVariable {

        public static string settingPrintUIDLabel = string.Format("{0}setting\\setting_printuidlabel.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guidePrintUIDLabel = string.Format("{0}guide\\guide_printuidlabel.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();
        public static MsAccessDatabase msAccessReport = null;

    }
}
