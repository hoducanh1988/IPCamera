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

                //input barcode reader
                r = _item_input_barcode();
                if (!r) { ret = false; goto NG; }

                //login to camera
                r = _item_test_login(ref camera_indoor);
                if (!r) { ret = false; goto NG; }

                //check mac
                r = _item_test_mac_ethernet(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //check build time firmware
                r = _item_test_firmware_buildtime(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //check uid
                r = _item_test_uid(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //write hardware version
                r = _item_write_hardwareversion(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //write serial number
                r = _item_write_serialnumber(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test wifi
                r = _item_test_wifi(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test sd card
                r = _item_test_sdcard(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test image sensor
                r = _item_test_imagesensor(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

                //test night vision
                r = _item_test_nightvision(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
                }

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

                //test button
                r = _item_test_button(camera_indoor);
                if (!r) {
                    ret = false;
                    if (stationVariable.mySetting.FailAndStop == "Yes") goto NG;
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


        //input barcode
        private bool _item_input_barcode() {
            if (stationVariable.myTesting.IsCheckMacEthernet == false &&
                stationVariable.myTesting.IsWriteSerialNumber == false &&
                stationVariable.myTesting.IsCheckUID == false) {
                return true;
            }

            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "NHẬP THÔNG TIN TRÊN TEM SẢN PHẨM\n";
            var ex_input_barcode = new Common.Excute.exInputMacSerialUid<TestingInformation, SettingInformation>(stationVariable.myTesting, stationVariable.mySetting);
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_input_barcode.excuteTelnet(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
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

        //check mac
        private bool _item_test_mac_ethernet(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckMacEthernet) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA TRÙNG KHỚP ĐỊA CHỈ MAC TRÊN TEM VÀ MAC GHI TRONG CAMERA\n";
            var ex_test_mac = new Common.Excute.exTestMacEthernet<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị mac trên tem: \"{0}\"\n", ex_test_mac.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Giá trị mac ghi trong camera:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_mac.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //check firmware build time
        private bool _item_test_firmware_buildtime(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckFirmwareBuildTime) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA FIRMWARE BASIC BUILD TIME\n";
            var ex_test_fw = new Common.Excute.exTestFirmwareBuildTime<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị setting: \"{0}\"\n", ex_test_fw.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Giá trị thực tế trong camera:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_fw.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //check uid
        private bool _item_test_uid(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckUID) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA TRÙNG KHỚP MÃ UID TRÊN TEM VÀ MÃ UID TRONG CAMERA\n";
            var ex_test_uid = new Common.Excute.exTestUID<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị uid trên tem: \"{0}\"\n", ex_test_uid.std_value);
            stationVariable.myTesting.logSystem += string.Format("...Giá trị uid thực tế trong camera:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_uid.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //write hardware version
        private bool _item_write_hardwareversion(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsWriteHardwareVersion) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "GHI HARDWARE VERSION CHO CAMERA\n";
            var ex_write_hw = new Common.Excute.exWriteHardwareVersion<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị setting: \"{0}\"\n", ex_write_hw.std_value);
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_write_hw.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //write serial number
        private bool _item_write_serialnumber(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsWriteSerialNumber) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "GHI SERIAL NUMBER CHO CAMERA\n";
            var ex_write_sn = new Common.Excute.exWriteSerialNumber<TestingInformation>(camera_indoor, stationVariable.myTesting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị nhập từ tem: \"{0}\"\n", ex_write_sn.std_value);
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_write_sn.excuteTelnet();
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");
            return r;
        }

        //test wifi
        private bool _item_test_wifi(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckWifi) return true;
            bool r = false;
            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA KHỐI GIAO TIẾP WIFI\n";
            var ex_test_wifi = new Common.Excute.exTestWiFi<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting, int.Parse(stationVariable.mySetting.RetryTime));
            stationVariable.myTesting.logSystem += string.Format("...Giá trị tiêu chuẩn: camera liệt kê được ssid 2G \"{0}\" và ssid 5G \"{1}\"\n", stationVariable.mySetting.wifiSSID2G, stationVariable.mySetting.wifiSSID5G);
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_wifi.excuteTelnet();
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
            r = ex_test_sdcard.excuteTelnet();
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
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", string.Format("Giá trị độ nét cảm biến ảnh >= {0}.", 
                                                                                             stationVariable.mySetting.sharpnessStandard - stationVariable.mySetting.toleranceSharpness));
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_imagesensor.excuteTelnet(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

            return r;
        }

        //test night vision
        private bool _item_test_nightvision(Common.Dut.IPCamera<TestingInformation> camera_indoor) {
            if (!stationVariable.myTesting.IsCheckNightVision) return true;
            bool r = false;

            stationVariable.myTesting.logSystem += "\n+++++++++++++++++++++++++++++++++++++++\n";
            stationVariable.myTesting.logSystem += "KIỂM TRA CHẾ ĐỘ NIGHT VISION\n";
            var ex_test_nightvision = new Common.Excute.exTestNightVision<TestingInformation, SettingInformation>(camera_indoor, stationVariable.myTesting, stationVariable.mySetting);
            stationVariable.myTesting.logSystem += string.Format("...Tiêu chuẩn: \"{0}\"\n", string.Format("Độ lệch R,G,B tại 3 điểm p1=[10,10], p2=[width/2, height/2] và p3=[width-10, height-10] phải nhỏ hơn hoặc bằng {0}.", stationVariable.mySetting.toleranceRGBNightVision));
            stationVariable.myTesting.logSystem += string.Format("...Thực tế:\n");
            stationVariable.myTesting.logSystem += string.Format("...\n");
            r = ex_test_nightvision.excuteTelnet(grid_debug);
            stationVariable.myTesting.logSystem += string.Format("\n...\n");
            stationVariable.myTesting.logSystem += string.Format("...Kết quả: {0}\n", r ? "Passed" : "Failed");

            //add tab log
            Dispatcher.Invoke(new Action(() => {
                this.grid_debug.Children.Clear();
                this.grid_debug.Children.Add(uc_tablog);
            }));

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
            r = ex_test_button.excuteTelnet(grid_debug);
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
