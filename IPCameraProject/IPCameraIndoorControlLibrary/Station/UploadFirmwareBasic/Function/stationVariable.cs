using System;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function {
    public class stationVariable {

        public static string settingFWBasic = string.Format("{0}setting\\setting_fwbasic.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideFWBasic = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTestingCamera1 = new Custom.TestingInformation();
        public static Custom.TestingInformation myTestingCamera2 = new Custom.TestingInformation();
        public static Custom.TestingInformation myTestingCamera3 = new Custom.TestingInformation();
        public static Custom.TestingInformation myTestingCamera4 = new Custom.TestingInformation();

        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();
    }
}
