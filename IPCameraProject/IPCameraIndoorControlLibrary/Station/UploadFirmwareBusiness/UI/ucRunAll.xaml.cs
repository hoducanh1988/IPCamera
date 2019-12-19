using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using IPCameraIndoorControlLibrary.Common.Base;
using IPCameraIndoorControlLibrary.Common.Excute;
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function;
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.Function.Custom;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.UI {

    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogTelnet uc_tablog = new Common.UI.ucTabLogTelnet();

        public ucRunAll() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingFWBusiness)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingFWBusiness);

            //binding data
            this.DataContext = stationVariable.myTesting;
            this.datagrid_uploadfwbusiness.DataContext = stationVariable.myUploadFWInfo;

            //show or hidden datagrid column
            this.dgtc_upload.Visibility = stationVariable.mySetting.IsUploadFirmwareBusiness ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_reboot.Visibility = stationVariable.mySetting.isNeedReboot() ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_firmware.Visibility = stationVariable.mySetting.IsCheckFirmwareBuildTime ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_mac.Visibility = stationVariable.mySetting.IsCheckMacEthernet ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_serial.Visibility = stationVariable.mySetting.IsCheckSerialNumber ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_uid.Visibility = stationVariable.mySetting.IsCheckUID ? Visibility.Visible : Visibility.Collapsed;
            this.dgtc_hardware.Visibility = stationVariable.mySetting.IsCheckHardwareVersion ? Visibility.Visible : Visibility.Collapsed;

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

                case "start_upload": {
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
                        stationVariable.myUploadFWInfo.Clear();

                        stationVariable.myTesting.logSystem += string.Format("...{0}, chọn số lượng camera upload là: {1}\n", DateTime.Now, stationVariable.myTesting.cameraQuantity);

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

                        RE:
                            Application.Current.Dispatcher.Invoke(new Action(() => { index = stationVariable.myUploadFWInfo.Count; }));
                            if (stationVariable.myTesting.TotalResult.Equals("Waiting...")) {
                                r = _change_ip_camera(index);
                                Thread.Sleep(1000);
                                goto RE;
                            }
                        }));
                        thrd_changeip.IsBackground = true;
                        thrd_changeip.Start();

                        //start thread get total result
                        Thread thrd_getresult = new Thread(new ThreadStart(() => {

                        RE:
                            int idx = 0;
                            int camera_qty = int.Parse(stationVariable.myTesting.cameraQuantity);
                            Application.Current.Dispatcher.Invoke(new Action(() => { idx = stationVariable.myUploadFWInfo.Count; }));
                            if (idx < camera_qty) {
                                Thread.Sleep(1000); goto RE;
                            }

                            bool ret = true;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                foreach (var item in stationVariable.myUploadFWInfo) {
                                    if (item.totalResult.Contains("Waiting...")) {
                                        ret = false;
                                        break;
                                    }
                                }
                            }));
                            if (!ret) {
                                Thread.Sleep(1000); goto RE;
                            }

                            ret = true;
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                foreach (var item in stationVariable.myUploadFWInfo) {
                                    if (item.totalResult.Contains("Failed")) {
                                        ret = false;
                                        break;
                                    }
                                }
                            }));
                            if (!ret) stationVariable.myTesting.Fail();
                            else stationVariable.myTesting.Pass();

                        }));
                        thrd_getresult.IsBackground = true;
                        thrd_getresult.Start();


                        break;
                    }
            }
        }


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
                count = 0;
            RE_GETMAC:
                count++;
                string mac_ethernet = camera_indoor.getMacEthernet();
                stationVariable.myTesting.logSystem += string.Format("......get mac {0} is {1}\n", stationVariable.mySetting.cameraIP, mac_ethernet);
                ret = !( string.IsNullOrEmpty(mac_ethernet) || string.IsNullOrWhiteSpace(mac_ethernet));
                if (!ret) {
                    if (count < 3) { Thread.Sleep(500); goto RE_GETMAC; }
                    else return false;
                }


                //check same mac or not
                bool macExisted = false;
                string tmpIP = "";
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    foreach (var item in stationVariable.myUploadFWInfo) {
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
                    string[] buffer = stationVariable.mySetting.cameraIPFwBusinessFrom.Split('.');
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
                if (macExisted == false && index < int.Parse(stationVariable.myTesting.cameraQuantity)) {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        stationVariable.myUploadFWInfo.Add(new UploadFwItemInfo() {
                            macEthernet = mac_ethernet,
                            ipAddress = new_ip_address,
                            totalResult = "Waiting..."
                        });
                    }));
                }

                //call thread control upload FW or verify firmware
                UploadFwItemInfo itemInfo = null;
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    foreach (var item in stationVariable.myUploadFWInfo) {
                        if (item.macEthernet.ToLower().Equals(mac_ethernet.ToLower())) {
                            itemInfo = item;
                            break;
                        }
                    }
                }));

                if (itemInfo != null && itemInfo.totalResult.Equals("Waiting...")) {
                    if (macExisted == false && stationVariable.mySetting.IsUploadFirmwareBusiness == true) {
                        Thread t = new Thread(new ThreadStart(() => {
                            bool r = false;
                            var ex_upfw_tm = new exUploadFWBusiness<UploadFwItemInfo, SettingInformation>(itemInfo, stationVariable.mySetting);
                            r = ex_upfw_tm.excuteTelnet();
                            if (!r) { itemInfo.totalResult = "Failed"; }
                            else { if (!stationVariable.mySetting.isTestFunction()) itemInfo.totalResult = "Passed"; }
                        }));
                        t.IsBackground = true;
                        t.Start();
                    }
                    else {
                        Thread t = new Thread(new ThreadStart(() => {
                            bool r = false;
                            var ex_testfunc_tm = new exTestFunctionBusiness<UploadFwItemInfo, SettingInformation>(itemInfo, stationVariable.mySetting);
                            r = ex_testfunc_tm.excuteTelnet();
                            itemInfo.totalResult = r ? "Passed" : "Failed";
                        }));
                        t.IsBackground = true;
                        t.Start();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private void datagrid_uploadfwbusiness_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            DataGrid dg = sender as DataGrid;
            if (dg.SelectedItem != null) {
                int idx = dg.SelectedIndex;
                uc_tablog.DataContext = stationVariable.myUploadFWInfo[idx];
            }
            else { uc_tablog.DataContext = stationVariable.myTesting; }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem mi = sender as MenuItem;
            string m_header = (string)mi.Header;

            switch (m_header) {
                case "Refresh": {
                        datagrid_uploadfwbusiness.UnselectAll();
                        datagrid_uploadfwbusiness.UnselectAllCells();
                        break;
                    }
            }
        }
    }
}
