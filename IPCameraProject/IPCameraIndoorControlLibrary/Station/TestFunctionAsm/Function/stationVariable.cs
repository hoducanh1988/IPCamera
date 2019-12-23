using System;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function {
    public class stationVariable {

        public static string settingAsm = string.Format("{0}setting\\setting_asm.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string guideAsm = string.Format("{0}guide\\guide_cameraindoor.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static Custom.TestingInformation myTesting = new Custom.TestingInformation();
        public static Custom.SettingInformation mySetting = new Custom.SettingInformation();

    }
}
