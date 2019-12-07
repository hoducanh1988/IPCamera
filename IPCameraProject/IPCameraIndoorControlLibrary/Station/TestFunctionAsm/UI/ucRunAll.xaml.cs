using IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogTelnet uc_tablog = new Common.UI.ucTabLogTelnet();


        public ucRunAll() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingAsm)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingAsm);

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
                            bool r = _test_allitem_asm();
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


        #region test asm

        private bool _test_allitem_asm() {
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

                //check mac

                //check build time firmware

                //check uid

                //write hardware version

                //write serial number

                //test sd card
                r = _item_test_sdcard(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test image sensor

                //test night vision

                //audio
                r = _item_test_audio(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test ir led
                r = _item_test_irled(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test rgb led
                r = _item_test_rgbled(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                ////test button
                //r = _item_test_button(camera_indoor);
                //if (!r) {
                //    ret = false;
                //    goto NG;
                //}

                //test wifi


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
            stationVariable.myTesting.logSystem += string.Format("...IP \"{0}\", telnet user \"{1}\", telnet pass \"{2}\"\n", stationVariable.mySetting.cameraIP, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
            camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, stationVariable.mySetting.cameraIP, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
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

        //check mac

        //check firmware build time

        //check uid

        //write hardware version

        //write serial number
      
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
            r = ex_test_sdcard.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test image sensor

        //test night vision

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
            r = ex_test_audio.excuteTelnet(grid_debug);
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
            r = ex_test_irled.excuteTelnet(grid_debug);
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
            r = ex_test_rgbled.excuteTelnet(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        ////test button
        //private bool _item_test_button(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
        //    if (!stationVariable.myTesting.IsCheckButton) return true;
        //    bool r = false;

        //    stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
        //    stationVariable.myTesting.logSystem += "KIỂM TRA NÚT NHẤN\n";
        //    var ex_test_button = new Common.Excute.exTestButton<TestingInformation>(camera_indoor, stationVariable.myTesting);
        //    stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", ex_test_button.std_value);
        //    stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
        //    stationVariable.myTesting.logSystem += string.Format("...\n");
        //    r = ex_test_button.excuteUart(grid_debug);
        //    stationVariable.myTesting.logSystem += string.Format("\n...\n");
        //    stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

        //    //add tab log
        //    Dispatcher.Invoke(new Action(() => {
        //        this.grid_debug.Children.Clear();
        //        this.grid_debug.Children.Add(uc_tablog);
        //    }));

        //    return r;
        //}

        //test wifi

        #endregion

    }
}
