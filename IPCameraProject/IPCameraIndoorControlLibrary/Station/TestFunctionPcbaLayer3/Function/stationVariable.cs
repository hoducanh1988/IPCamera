using System;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function {
    public class stationVariable {

        public static string settingLayer3 = string.Format("{0}setting\\setting_layer3.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideLayer3 = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();
    }
}
