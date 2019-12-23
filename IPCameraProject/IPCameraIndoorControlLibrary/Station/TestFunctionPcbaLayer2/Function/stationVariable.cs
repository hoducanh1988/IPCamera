using System;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function {
    public class stationVariable {

        public static string settingLayer2 = string.Format("{0}setting\\setting_layer2.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideLayer2 = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();

    }
}
