using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilityPack.IO;
using IPCameraIndoorControlLibrary.Common.Base;
using IPCameraIndoorControlLibrary.Common.Log;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function.Custom;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogUart uc_tablog = new Common.UI.ucTabLogUart();

        public ucRunAll() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingLayer3)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingLayer3);
        
            //binding data
            this.DataContext = stationVariable.myTesting;

            //init tablog
            uc_tablog.DataContext = stationVariable.myTesting;

            //add tab log
            this.grid_debug.Children.Clear();
            this.grid_debug.Children.Add(uc_tablog);

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_content = (string)b.Content;

            switch (b_content) {
                case "START": {
                        Thread t = new Thread(new ThreadStart(() => {
                            Stopwatch st = new Stopwatch();
                            uc_tablog.isScroll = true;
                            st.Start();
                            bool r = _test_allitem_layer3();
                            st.Stop();
                            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
                            stationVariable.myTesting.logSystem += "KẾT QUẢ KIỂM TRA SẢN PHẨM\n";
                            stationVariable.myTesting.logSystem += string.Format("...Phán định tổng: {0}\n", r ? "Passed" : "Failed");
                            stationVariable.myTesting.logSystem += string.Format("...Tổng thời gian kiểm tra: {0} ms\n", st.ElapsedMilliseconds);
                            uc_tablog.isScroll = false;
                            //save log
                            _save_log();
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                case "STOP": { return; }
            }
        }


        #region test layer 3

        List<string> listTestItem_Layer3 = new List<string>() {
            "_item_test_usb", //test usb
            "_item_test_sdcard", //test sd card
            "_item_test_ethernet", //test ethernet
             "_item_test_imagesensor", //check image sensor
             "_item_test_audio", //check audio
             "_item_test_ircut", //check ir cut
             "_item_test_irled", //check ir led
             "_item_test_rgbled", //check rgb led
             "_item_test_lightsensor", //check light sensor
             "_item_test_button", //check button
        };

        private bool _test_allitem_layer3() {
            try {
                bool r = false;
                bool ret = true;

                //init control
                Common.Dut.IPCamera<TestingInformation> camera_indoor = null;
                stationVariable.myTesting.Ready();
                stationVariable.myTesting.Checking();

                //login to camera
                r = _item_test_login(ref camera_indoor);
                if (!r) {
                    ret = false;
                    goto NG;
                }
                else _item_get_mac_ethernet(camera_indoor);

                //test all item
                foreach (var testItem in listTestItem_Layer3) {
                    MethodInfo method = this.GetType().GetMethod(testItem, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method == null) { continue; }

                    var func = (Func<Common.Dut.IPCamera<TestingInformation>, bool>)method.CreateDelegate(typeof(Func<Common.Dut.IPCamera<TestingInformation>, bool>), this);
                    r = func(camera_indoor);
                    if (!r) {
                        ret = false;
                        if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                    }
                }


                if (ret) goto OK;
                else goto NG;

                OK:
                stationVariable.myTesting.Pass();
                goto END;

                NG:
                stationVariable.myTesting.Fail();
                goto END;

                END:
                if (camera_indoor != null && camera_indoor.IsConnected() == true) camera_indoor.Close();
                //save log
                return ret;
            }
            catch {
                return false;
            }
        }

        //login
        private bool _item_test_login(ref Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "LOGIN VÀO IP CAMERA\n";
            stationVariable.myTesting.logSystem += string.Format("...Cổng {0}, tốc độ baud {1}\n", stationVariable.mySetting.SerialPortName, stationVariable.mySetting.SerialBaudRate);
            camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, stationVariable.mySetting.SerialPortName, int.Parse(stationVariable.mySetting.SerialBaudRate), 8);
            camera_indoor.Login();
            r = camera_indoor.IsConnected();
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //get mac ethernet
        private bool _item_get_mac_ethernet(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "ĐỌC ĐỊA CHỈ MAC ETHERNET\n";
            stationVariable.myTesting.macEthernet = camera_indoor.getMacEthernet();
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", stationVariable.myTesting.macEthernet);
            return true;
        }

        //test usb
        private bool _item_test_usb(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckUsb) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI GIAO TIẾP USB\n";
            var ex_test_usb = new Common.Excute.exTestUSB<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_usb.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_usb.excuteUart();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test sd card
        private bool _item_test_sdcard(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckSdCard) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI GIAO TIẾP SD CARD\n";
            var ex_test_sdcard = new Common.Excute.exTestSDCard<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_sdcard.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_sdcard.excuteUart();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test ethernet
        private bool _item_test_ethernet(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckEthernet) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI GIAO TIẾP ETHERNET\n";
            var ex_test_ethernet = new Common.Excute.exTestEthernet<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_ethernet.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_ethernet.excuteUart();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test image sensor
        private bool _item_test_imagesensor(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckImageSensor) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA CẢM BIẾN ẢNH\n";
            var ex_test_imagesensor = new Common.Excute.exTestImageSensor<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_imagesensor.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_imagesensor.excuteUart();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test audio
        private bool _item_test_audio(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckAudio) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI AUDIO\n";
            var ex_test_audio = new Common.Excute.exTestAudio<TestingInformation>(camera_indoor, stationVariable.myTesting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_audio.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_audio.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test ir cut
        private bool _item_test_ircut(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckIrCut) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI IR CUT\n";
            var ex_test_ircut = new Common.Excute.exTestIRCut<TestingInformation>(camera_indoor, stationVariable.myTesting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_ircut.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_ircut.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test ir led
        private bool _item_test_irled(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckIrLed) return true;
            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA IR LED\n";
            var ex_test_irled = new Common.Excute.exTestIRLed<TestingInformation>(camera_indoor, stationVariable.myTesting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_irled.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_irled.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test rgb led
        private bool _item_test_rgbled(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckRgbLed) return true;
            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA RGB LED\n";
            var ex_test_rgbled = new Common.Excute.exTestRGBLed<TestingInformation>(camera_indoor, stationVariable.myTesting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_rgbled.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_rgbled.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test light sensor
        private bool _item_test_lightsensor(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckLightSensor) return true;
            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA CẢM BIẾN ÁNH SÁNG\n";
            var ex_test_lightsensor = new Common.Excute.exTestLightSensor<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_lightsensor.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_lightsensor.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test button
        private bool _item_test_button(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckButton) return true;
            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA NÚT NHẤN\n";
            var ex_test_button = new Common.Excute.exTestButton<TestingInformation>(camera_indoor, stationVariable.myTesting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_button.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_button.excuteUart(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //save log
        private void _save_log() {
            //save log uart
            new LogUart(
                globalParameter.LogStationName.Layer3.ToString(),
                stationVariable.myTesting.macEthernet,
                stationVariable.myTesting.TotalResult
                )
            .saveDataToLogFile(stationVariable.myTesting.logUart);

            //save log system
            new LogSystem(
                globalParameter.LogStationName.Layer3.ToString(),
                stationVariable.myTesting.macEthernet,
                stationVariable.myTesting.TotalResult
                )
            .saveDataToLogFile(stationVariable.myTesting.logSystem);

            //save log total
            new LogTotal(
                 globalParameter.LogStationName.Layer3.ToString()
                )
            .saveDataToLogFile(
                "macEthernet", stationVariable.myTesting.macEthernet,
                "UsbResult", stationVariable.myTesting.UsbResult,
                "sdCardResult", stationVariable.myTesting.sdCardResult,
                "ethernetResult", stationVariable.myTesting.ethernetResult,
                "imageSensorResult", stationVariable.myTesting.imageSensorResult,
                "audioResult", stationVariable.myTesting.audioResult,
                "rgbLedResult", stationVariable.myTesting.rgbLedResult,
                "lightSensorResult", stationVariable.myTesting.lightSensorResult,
                "buttonResult", stationVariable.myTesting.buttonResult,
                "TotalResult", stationVariable.myTesting.TotalResult
                );
        }

        #endregion

    }
}
