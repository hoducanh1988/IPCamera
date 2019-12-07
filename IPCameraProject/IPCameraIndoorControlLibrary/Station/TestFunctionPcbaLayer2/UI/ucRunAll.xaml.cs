using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function.Custom;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UtilityPack.IO;
using System.Diagnostics;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogUart uc_tablog = new Common.UI.ucTabLogUart();

        public ucRunAll() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingLayer2)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingLayer2);

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
                            bool r = _test_allitem_layer2();
                            st.Stop();
                            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
                            stationVariable.myTesting.logSystem += "KẾT QUẢ KIỂM TRA SẢN PHẨM\n";
                            stationVariable.myTesting.logSystem += string.Format("...Phán định tổng: {0}\n", r ? "Passed" : "Failed");
                            stationVariable.myTesting.logSystem += string.Format("...Tổng thời gian kiểm tra: {0} ms\n", st.ElapsedMilliseconds);
                            uc_tablog.isScroll = false;
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                case "STOP": { return; }
            }
        }

        #region test layer 2

        private bool _test_allitem_layer2() {
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
                else _item_get_mac_wlan(camera_indoor);

                //test wifi
                r = _item_test_wifi(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG; }

                //test sd card
                r = _item_test_sdcard(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG; }

                //test ethernet
                r = _item_test_ethernet(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG; }

                //test rgb led
                r = _item_test_rgbled(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG; }

                //test button
                r = _item_test_button(camera_indoor);
                if (!r) {
                    ret = false;
                    goto NG;
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

        //get mac wlan
        private bool _item_get_mac_wlan(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "ĐỌC ĐỊA CHỈ MAC WLAN\n";
            stationVariable.myTesting.macWlan = camera_indoor.getMacWlan();
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", stationVariable.myTesting.macWlan);
            return true;
        }

        //test wifi
        private bool _item_test_wifi(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckWifi) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI GIAO TIẾP WIFI\n";
            var ex_test_wifi = new Common.Excute.exTestWiFi<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_wifi.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_wifi.excuteUart();
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

        #endregion
    }
}
