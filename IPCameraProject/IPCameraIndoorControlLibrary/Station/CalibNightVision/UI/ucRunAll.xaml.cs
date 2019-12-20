using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using IPCameraIndoorControlLibrary.Station.CalibNightVision.Function;
using IPCameraIndoorControlLibrary.Station.CalibNightVision.Function.Custom;
using IPCameraIndoorControlLibrary.Common.Base;
using UtilityPack.IO;
using System.Windows.Threading;
using IPCameraIndoorControlLibrary.Common.Log;

namespace IPCameraIndoorControlLibrary.Station.CalibNightVision.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogTelnet uc_tablog = new Common.UI.ucTabLogTelnet();

        public ucRunAll() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingcalibNightVision)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingcalibNightVision);

            //binding data
            this.DataContext = stationVariable.myTesting;
            this.datagrid_calibnightvision.DataContext = stationVariable.myCalibNightVisionInfo;

            //set combobox itemsource
            this.cbb_cameraquantity.ItemsSource = globalParameter.list_number;


            //init tablog
            uc_tablog.DataContext = stationVariable.myTesting;

            //add tab log
            this.grid_debug.Children.Clear();
            this.grid_debug.Children.Add(uc_tablog);

            //set timer scroll log
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (o, s) => {
                uc_tablog.isScroll = stationVariable.myTesting.TotalResult.Equals("Waiting...");
            };
            timer.Start();
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_tag = (string)b.Tag;

            switch (b_tag) {
                case "start_calib": {
                        grid_main.Visibility = Visibility.Collapsed;
                        grid_cameraquantity.Visibility = Visibility.Visible;
                        cbb_cameraquantity.Focus();
                        stationVariable.myTesting.Ready();
                        break;
                    }
                case "select_camera_quantity": {
                        grid_cameraquantity.Visibility = Visibility.Collapsed;
                        grid_main.Visibility = Visibility.Visible;
                        stationVariable.myTesting.Checking();
                        stationVariable.myCalibNightVisionInfo.Clear();

                        stationVariable.myTesting.logSystem += string.Format("...{0}, chọn số lượng calib là: {1}\n", DateTime.Now, stationVariable.myTesting.cameraQuantity);

                        //start thread count elapsed time
                        Thread thrd_counttime = new Thread(new ThreadStart(() => {

                            int count = 0;
                            int delay_ms = 500;
                        RE:
                            count++;
                            bool r = stationVariable.myTesting.TotalResult.Equals("Waiting...");
                            if (r) {
                                Thread.Sleep(delay_ms);
                                stationVariable.myTesting.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan(count * delay_ms);
                                goto RE;
                            }

                        }));
                        thrd_counttime.IsBackground = true;
                        thrd_counttime.Start();

                        //start thread change ip
                        Thread thrd_changeip = new Thread(new ThreadStart(() => {
                            int index = 0;
                            bool r = false;
                            stationVariable.myTesting.progressIndex = 1;
                        RE:
                            Application.Current.Dispatcher.Invoke(new Action(() => { index = stationVariable.myCalibNightVisionInfo.Count; }));
                            if (index < int.Parse(stationVariable.myTesting.cameraQuantity)) {
                                stationVariable.myTesting.logSystem += string.Format("...{0}, đọc số lượng camera đã đổi ip: {1}/{2}\n", DateTime.Now, index, stationVariable.myTesting.cameraQuantity);
                                r = _change_ip_camera(index);
                                Thread.Sleep(1000);
                                goto RE;
                            }

                            //call user control calib light ...
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                grid_main.Visibility = Visibility.Collapsed;
                                grid_waitlight.Visibility = Visibility.Visible;
                            }));
                        }));
                        thrd_changeip.IsBackground = true;
                        thrd_changeip.Start();

                        break;
                    }

                case "calib_light": {
                        grid_main.Visibility = Visibility.Visible;
                        grid_waitlight.Visibility = Visibility.Collapsed;

                        //thread calib light
                        
                        Thread thrd_caliblight = new Thread(new ThreadStart(() => {
                            stationVariable.myTesting.progressIndex = 2;
                            stationVariable.myTesting.logSystem += string.Format("...{0}, calib nightvision high threshold\n", DateTime.Now);
                            int index = 0;

                        RE:
                            CalibNightVisionItemInfo calibItem = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                calibItem = stationVariable.myCalibNightVisionInfo[index];
                                index++;
                            }));

                            _calib_light(calibItem);
                            if (index < int.Parse(stationVariable.myTesting.cameraQuantity)) goto RE;

                            //call user control calib dark ...
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                grid_main.Visibility = Visibility.Collapsed;
                                grid_waitdark.Visibility = Visibility.Visible;
                            }));

                        }));
                        thrd_caliblight.IsBackground = true;
                        thrd_caliblight.Start();

                        break;
                    }
                case "calib_dark": {

                        grid_main.Visibility = Visibility.Visible;
                        grid_waitdark.Visibility = Visibility.Collapsed;

                        //thread calib dark
                        
                        Thread thrd_calibdark = new Thread(new ThreadStart(() => {
                            stationVariable.myTesting.progressIndex = 3;
                            stationVariable.myTesting.logSystem += string.Format("...{0}, calib nightvision low threshold\n", DateTime.Now);
                            int index = 0;
                        RE_CALIB_DARK:
                            CalibNightVisionItemInfo calibItem = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                calibItem = stationVariable.myCalibNightVisionInfo[index];
                                index++;
                            }));

                            _calib_dark(calibItem);
                            if (index < int.Parse(stationVariable.myTesting.cameraQuantity)) goto RE_CALIB_DARK;

                            //read calib value
                            stationVariable.myTesting.progressIndex = 4;
                            stationVariable.myTesting.logSystem += string.Format("...{0}, read calib value\n", DateTime.Now);
                            index = 0;
                        RE_READ_CALIB:
                            calibItem = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                calibItem = stationVariable.myCalibNightVisionInfo[index];
                                index++;
                            }));

                            _get_calib_value(calibItem);
                            if (index < int.Parse(stationVariable.myTesting.cameraQuantity)) goto RE_READ_CALIB;


                            //judgement
                            stationVariable.myTesting.logSystem += string.Format("...{0}, judge calib value\n", DateTime.Now);
                            index = 0;
                        RE_JUDGEMENT:
                            calibItem = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                calibItem = stationVariable.myCalibNightVisionInfo[index];
                                index++;
                            }));

                            _calib_judgement(calibItem);
                            if (index < int.Parse(stationVariable.myTesting.cameraQuantity)) goto RE_JUDGEMENT;

                            //total judgement
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                bool ret = true;
                                foreach (var item in stationVariable.myCalibNightVisionInfo) {
                                    if (item.Result.Equals("Passed") == false) {
                                        ret = false;
                                        break;
                                    }
                                }
                                
                                if (ret) stationVariable.myTesting.Pass();
                                else stationVariable.myTesting.Fail();

                            }));

                            //save log
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                foreach (var item in stationVariable.myCalibNightVisionInfo) {
                                    _save_log(item);
                                }
                            }));

                        }));
                        thrd_calibdark.IsBackground = true;
                        thrd_calibdark.Start();

                        break;
                    }
            }
        }



        #region calib night vision

        private bool _change_ip_camera(int index) {
            try {
                bool ret = false;
                int count = 0;

                //ping to camera
                count = 0;
                int c = 0;
            RE_PING:
                count++;
                ret = globalUtility.pingNetwork(stationVariable.mySetting.cameraIP);
                stationVariable.myTesting.logSystem += string.Format("......ping {0} is {1}\n", stationVariable.mySetting.cameraIP, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_PING; }
                    else return false;
                }
                else {
                    c++;
                    if (c < 3) { Thread.Sleep(500); goto RE_PING; }
                }
                //login telnet to camera
                count = 0;
            RE_LOGIN:
                count++;
                Common.Dut.IPCamera<TestingInformation> camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, stationVariable.mySetting.cameraIP, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
                camera_indoor.Login();
                ret = camera_indoor.IsConnected();
                stationVariable.myTesting.logSystem += string.Format("......login to {0} is {1}\n", stationVariable.mySetting.cameraIP, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_LOGIN; }
                    else return false;
                }

                //get camera mac ethernet
                string mac_ethernet = camera_indoor.getMacEthernet();
                stationVariable.myTesting.logSystem += string.Format("......get mac {0} is {1}\n", stationVariable.mySetting.cameraIP, mac_ethernet);

                //check same mac or not
                bool macExisted = false;
                string tmpIP = "";
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    foreach (var item in stationVariable.myCalibNightVisionInfo) {
                        if (item.macEthernet.ToLower().Contains(mac_ethernet.ToLower())) {
                            macExisted = true;
                            tmpIP = item.ipAddress;
                            break;
                        }
                    }
                }));

                //change ip address
                string new_ip_address = "";
                if (macExisted == false) {
                    string[] buffer = stationVariable.mySetting.cameraIPCalibFrom.Split('.');
                    new_ip_address = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + index);
                }
                else new_ip_address = tmpIP;

                stationVariable.myTesting.logSystem += string.Format("......gen new ip {0} is {1}\n", mac_ethernet, new_ip_address);
                camera_indoor.changeIPWithoutSave(new_ip_address);

                //wait after change ip
                count = 0;
            RE_WAIT:
                count++;
                ret = globalUtility.pingNetwork(new_ip_address);
                if (!ret) {
                    if (count < 30) { Thread.Sleep(500); goto RE_WAIT; }
                    else {
                        stationVariable.myTesting.logSystem += string.Format("......change to new ip {0} is {1}\n", new_ip_address, ret);
                        return false;
                    }
                }
                stationVariable.myTesting.logSystem += string.Format("......change to new ip {0} is {1}\n", new_ip_address, ret);

                //add camera to collection
                if (macExisted == false) {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        stationVariable.myCalibNightVisionInfo.Add(new CalibNightVisionItemInfo() {
                            macEthernet = mac_ethernet,
                            ipAddress = new_ip_address,
                            changeIPResult = "Passed",
                            calibDarkResult = "-",
                            calibLightResult = "-",
                            verifyResult = "-",
                            Result = "Waiting..."
                        });
                    }));
                }
                
                return true;
            }
            catch { return false; }
        }

        private bool _calib_light(CalibNightVisionItemInfo calibItem) {
            try {
                bool ret = false;
                int count = 0;

                stationVariable.myTesting.logSystem += string.Format("......camera {0}\n", calibItem.macEthernet);
                calibItem.calibLightResult = "Waiting...";

                //login telnet to camera
                count = 0;
            RE_LOGIN:
                count++;
                Common.Dut.IPCamera<TestingInformation> camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, calibItem.ipAddress, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
                camera_indoor.Login();
                ret = camera_indoor.IsConnected();
                stationVariable.myTesting.logSystem += string.Format(".........login to {0} is {1}\n", calibItem.ipAddress, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_LOGIN; }
                    else {
                        camera_indoor.Close();
                        calibItem.calibLightResult = "Failed";
                        return false;
                    }
                }

                //calib light
                count = 0;
            RE_CALIB:
                count++;
                ret = camera_indoor.calibNightVisionModeLight();
                stationVariable.myTesting.logSystem += string.Format(".........calib high threshold result {0}\n", ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_CALIB; }
                    else {
                        camera_indoor.Close();
                        calibItem.calibLightResult = "Failed";
                        return false;
                    }
                }

                camera_indoor.Close();
                calibItem.calibLightResult = "Passed";
                return true;
            }
            catch {
                calibItem.calibLightResult = "Failed";
                return false;
            }
        }

        private bool _calib_dark(CalibNightVisionItemInfo calibItem) {
            try {
                bool ret = false;
                int count = 0;

                stationVariable.myTesting.logSystem += string.Format("......camera {0}\n", calibItem.macEthernet);
                calibItem.calibDarkResult = "Waiting...";

                //login telnet to camera
                count = 0;
            RE_LOGIN:
                count++;
                Common.Dut.IPCamera<TestingInformation> camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, calibItem.ipAddress, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
                camera_indoor.Login();
                ret = camera_indoor.IsConnected();
                stationVariable.myTesting.logSystem += string.Format(".........login to {0} is {1}\n", calibItem.ipAddress, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_LOGIN; }
                    else {
                        camera_indoor.Close();
                        calibItem.calibDarkResult = "Failed";
                        return false;
                    }
                }

                //calib dark
                count = 0;
            RE_CALIB:
                count++;
                ret = camera_indoor.calibNightVisionModeDark();
                stationVariable.myTesting.logSystem += string.Format(".........calib low threshold result {0}\n", ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_CALIB; }
                    else {
                        camera_indoor.Close();
                        calibItem.calibDarkResult = "Failed";
                        return false;
                    }
                }

                camera_indoor.Close();
                calibItem.calibDarkResult = "Passed";
                return true;
            }
            catch {
                calibItem.calibDarkResult = "Failed";
                return false;
            }
        }

        private bool _get_calib_value(CalibNightVisionItemInfo calibItem) {
            try {
                bool ret = false;
                int count = 0;

                stationVariable.myTesting.logSystem += string.Format("......camera {0}\n", calibItem.macEthernet);
                calibItem.verifyResult = "...";

                //login telnet to camera
                count = 0;
            RE_LOGIN:
                count++;
                Common.Dut.IPCamera<TestingInformation> camera_indoor = new Common.Dut.IPCamera<TestingInformation>(stationVariable.myTesting, calibItem.ipAddress, stationVariable.mySetting.cameraTelnetUser, stationVariable.mySetting.cameraTelnetPassword);
                camera_indoor.Login();
                ret = camera_indoor.IsConnected();
                stationVariable.myTesting.logSystem += string.Format(".........login to {0} is {1}\n", calibItem.ipAddress, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_LOGIN; }
                    else {
                        camera_indoor.Close();
                        calibItem.verifyResult = "null,null";
                        return false;
                    }
                }

                //verify after calib
                count = 0;
            RE_CALIB:
                count++;
                string data = camera_indoor.getCalibNightVisionValue();
                ret = data.Contains("lowthreshold") && data.Contains("highthreshold");
                stationVariable.myTesting.logSystem += string.Format(".........calib value {0}, result {1}\n", data, ret);
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_CALIB; }
                    else {
                        camera_indoor.Close();
                        calibItem.verifyResult = "null,null";
                        return false;
                    }
                }

                camera_indoor.Close();
                string sssssssssssssss = data.Replace("\r", "")
                                             .Replace("\n", "")
                                             .Replace("cat /usr/conf/nightvision.cfg", "")
                                             .Replace("~ #", "")
                                             .Trim();

                string[] buffer = sssssssssssssss.Split(',');
                calibItem.verifyResult = buffer[1].Replace("\"", "").Replace("threshold", "").Trim() + "," + buffer[2].Replace("\"", "").Replace("threshold", "").Trim();

                return true;
            }
            catch {
                calibItem.verifyResult = "null,null";
                return false;
            }
        }

        private bool _calib_judgement(CalibNightVisionItemInfo calibItem) {
            try {

                if (!calibItem.calibDarkResult.Equals("Passed")) {
                    calibItem.Result = "Failed";
                    return false;
                }
                if (!calibItem.calibLightResult.Equals("Passed")) {
                    calibItem.Result = "Failed";
                    return false;
                }
                if (calibItem.verifyResult.Equals("null,null")) {
                    calibItem.Result = "Failed";
                    return false;
                }
                if (calibItem.verifyResult.Contains("low:") == false || calibItem.verifyResult.Contains("high:") == false) {
                    calibItem.Result = "Failed";
                    return false;
                }

                string[] buffer = calibItem.verifyResult.Split(',');
                int low_value = int.Parse(buffer[0].Replace("low:", "").Trim());
                int high_value = int.Parse(buffer[1].Replace("high:", "").Trim());

                int ll = stationVariable.mySetting.nightVisionDarkLower;
                int lu = stationVariable.mySetting.nightVisionDarkUpper;
                int hl = stationVariable.mySetting.nightVisionLightLower;
                int hu = stationVariable.mySetting.nightVisionLightUpper;

                if (low_value > high_value) {
                    calibItem.Result = "Failed";
                    return false;
                }

                if (low_value < ll || low_value > lu) {
                    calibItem.Result = "Failed";
                    return false;
                }

                if (high_value < hl || high_value > hu) {
                    calibItem.Result = "Failed";
                    return false;
                }

                calibItem.Result = "Passed";
                return true;
            }
            catch {
                calibItem.Result = "Failed";
                return false;
            }
        }

        private void _save_log(CalibNightVisionItemInfo calibItem) {
            //save log telnet
            new LogTelnet(
                globalParameter.LogStationName.CalibNight.ToString(),
                calibItem.macEthernet,
                calibItem.Result
                )
            .saveDataToLogFile(stationVariable.myTesting.logTelnet);

            //save log system
            new LogSystem(
                globalParameter.LogStationName.CalibNight.ToString(),
                 calibItem.macEthernet,
                calibItem.Result
                )
            .saveDataToLogFile(stationVariable.myTesting.logSystem);

            //save log total
            new LogTotal(
                 globalParameter.LogStationName.CalibNight.ToString()
                )
            .saveDataToLogFile(
                "macEthernet", calibItem.macEthernet,
                "calibDarkResult", calibItem.calibDarkResult,
                "calibLightResult", calibItem.calibLightResult,
                "verifyResult", calibItem.verifyResult,
                "TotalResult", calibItem.Result
                );
        }

        #endregion


    }
}
