using System;
using System.Collections.Generic;
using System.IO;
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
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function;
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function.Custom;
using IPCameraIndoorControlLibrary.Common.Excute;
using UtilityPack.IO;
using System.Windows.Threading;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogUart uc_tablog1 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog2 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog3 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog4 = new Common.UI.ucTabLogUart();

        public ucRunAll() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingFWBasic)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingFWBasic);

            //set comport
            stationVariable.myTestingCamera1.serialPortName = stationVariable.mySetting.SerialPortName1.Length > 0 ? stationVariable.mySetting.SerialPortName1 : null;
            stationVariable.myTestingCamera2.serialPortName = stationVariable.mySetting.SerialPortName2.Length > 0 ? stationVariable.mySetting.SerialPortName2 : null;
            stationVariable.myTestingCamera3.serialPortName = stationVariable.mySetting.SerialPortName3.Length > 0 ? stationVariable.mySetting.SerialPortName3 : null;
            stationVariable.myTestingCamera4.serialPortName = stationVariable.mySetting.SerialPortName4.Length > 0 ? stationVariable.mySetting.SerialPortName4 : null;

            //set ip
            string[] buffer = stationVariable.mySetting.cameraIPUploadFrom.Split('.');
            stationVariable.myTestingCamera1.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 0);
            stationVariable.myTestingCamera2.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 1);
            stationVariable.myTestingCamera3.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 2);
            stationVariable.myTestingCamera4.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 3);


            //binding data
            this.grid_camera_1.DataContext = stationVariable.myTestingCamera1;
            this.grid_camera_2.DataContext = stationVariable.myTestingCamera2;
            this.grid_camera_3.DataContext = stationVariable.myTestingCamera3;
            this.grid_camera_4.DataContext = stationVariable.myTestingCamera4;

            //init tablog
            uc_tablog1.DataContext = stationVariable.myTestingCamera1;
            uc_tablog2.DataContext = stationVariable.myTestingCamera2;
            uc_tablog3.DataContext = stationVariable.myTestingCamera3;
            uc_tablog4.DataContext = stationVariable.myTestingCamera4;

            //add tab log
            this.grid_debug_1.Children.Clear();
            this.grid_debug_2.Children.Clear();
            this.grid_debug_3.Children.Clear();
            this.grid_debug_4.Children.Clear();
            
            this.grid_debug_1.Children.Add(uc_tablog1);
            this.grid_debug_2.Children.Add(uc_tablog2);
            this.grid_debug_3.Children.Add(uc_tablog3);
            this.grid_debug_4.Children.Add(uc_tablog4);


            //set timer scroll log
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (o, s) => {
                uc_tablog1.isScroll = stationVariable.myTestingCamera1.TotalResult.Equals("Waiting...");
                uc_tablog2.isScroll = stationVariable.myTestingCamera2.TotalResult.Equals("Waiting...");
                uc_tablog3.isScroll = stationVariable.myTestingCamera3.TotalResult.Equals("Waiting...");
                uc_tablog4.isScroll = stationVariable.myTestingCamera4.TotalResult.Equals("Waiting...");
            };
            timer.Start();

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                TextBox tb = sender as TextBox;
                string tb_tag = (string)tb.Tag;

                switch (tb_tag) {
                    case "txt_mac_1": {

                            //upload firmware
                            Thread t = new Thread(new ThreadStart(() => {
                                //init control
                                stationVariable.myTestingCamera1.Ready();
                                Application.Current.Dispatcher.Invoke(new Action(() => { stationVariable.myTestingCamera1.macEthernet = tb.Text.ToUpper(); }));
                                stationVariable.myTestingCamera1.Checking();
                                //upload
                                var ex_upload_fwbasic = new exUploadFWBasic<TestingInformation, SettingInformation>(stationVariable.myTestingCamera1, stationVariable.mySetting);
                                bool r = ex_upload_fwbasic.excuteUart();
                                //judgement
                                if (r) stationVariable.myTestingCamera1.Pass();
                                else stationVariable.myTestingCamera1.Fail();

                                //clear text
                                Application.Current.Dispatcher.Invoke(new Action(() => { tb.Clear(); }));
                            }));
                            t.IsBackground = true;
                            t.Start();

                            //count total time
                            Thread tz = new Thread(new ThreadStart(() => {
                                Thread.Sleep(500);
                                int count = 0;
                            RE:
                                count++;
                                bool r = stationVariable.myTestingCamera1.TotalResult.Contains("Waiting...");
                                if (r) {
                                    Thread.Sleep(500);
                                    stationVariable.myTestingCamera1.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan((count + 1) * 500);
                                    goto RE;
                                }

                            }));
                            tz.IsBackground = true;
                            tz.Start();

                            break;
                        }
                   
                    case "txt_mac_2": {
                            Thread t = new Thread(new ThreadStart(() => {

                                //init control
                                stationVariable.myTestingCamera2.Ready();
                                Application.Current.Dispatcher.Invoke(new Action(() => { stationVariable.myTestingCamera2.macEthernet = tb.Text.ToUpper(); }));
                                stationVariable.myTestingCamera2.Checking();
                                //upload
                                var ex_upload_fwbasic = new exUploadFWBasic<TestingInformation, SettingInformation>(stationVariable.myTestingCamera2, stationVariable.mySetting);
                                bool r = ex_upload_fwbasic.excuteUart();
                                //judgement
                                if (r) stationVariable.myTestingCamera2.Pass();
                                else stationVariable.myTestingCamera2.Fail();

                                //clear text
                                Application.Current.Dispatcher.Invoke(new Action(() => { tb.Clear(); }));
                            }));
                            t.IsBackground = true;
                            t.Start();

                            //count total time
                            Thread tz = new Thread(new ThreadStart(() => {
                                Thread.Sleep(500);
                                int count = 0;
                            RE:
                                count++;
                                bool r = stationVariable.myTestingCamera2.TotalResult.Contains("Waiting...");
                                if (r) {
                                    Thread.Sleep(500);
                                    stationVariable.myTestingCamera2.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan((count + 1) * 500);
                                    goto RE;
                                }

                            }));
                            tz.IsBackground = true;
                            tz.Start();

                            break;
                        }
                   
                    case "txt_mac_3": {
                            Thread t = new Thread(new ThreadStart(() => {

                                //init control
                                stationVariable.myTestingCamera3.Ready();
                                Application.Current.Dispatcher.Invoke(new Action(() => { stationVariable.myTestingCamera3.macEthernet = tb.Text.ToUpper(); }));
                                stationVariable.myTestingCamera3.Checking();
                                //upload
                                var ex_upload_fwbasic = new exUploadFWBasic<TestingInformation, SettingInformation>(stationVariable.myTestingCamera3, stationVariable.mySetting);
                                bool r = ex_upload_fwbasic.excuteUart();
                                //judgement
                                if (r) stationVariable.myTestingCamera3.Pass();
                                else stationVariable.myTestingCamera3.Fail();

                                //clear text
                                Application.Current.Dispatcher.Invoke(new Action(() => { tb.Clear(); }));
                            }));
                            t.IsBackground = true;
                            t.Start();


                            //count total time
                            Thread tz = new Thread(new ThreadStart(() => {
                                Thread.Sleep(500);
                                int count = 0;
                            RE:
                                count++;
                                bool r = stationVariable.myTestingCamera3.TotalResult.Contains("Waiting...");
                                if (r) {
                                    Thread.Sleep(500);
                                    stationVariable.myTestingCamera3.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan((count + 1) * 500);
                                    goto RE;
                                }

                            }));
                            tz.IsBackground = true;
                            tz.Start();

                            break;
                        }
                    
                    case "txt_mac_4": {

                            Thread t = new Thread(new ThreadStart(() => {

                                //init control
                                stationVariable.myTestingCamera4.Ready();
                                Application.Current.Dispatcher.Invoke(new Action(() => { stationVariable.myTestingCamera4.macEthernet = tb.Text.ToUpper(); }));
                                stationVariable.myTestingCamera4.Checking();
                                //upload
                                var ex_upload_fwbasic = new exUploadFWBasic<TestingInformation, SettingInformation>(stationVariable.myTestingCamera4, stationVariable.mySetting);
                                bool r = ex_upload_fwbasic.excuteUart();
                                //judgement
                                if (r) stationVariable.myTestingCamera4.Pass();
                                else stationVariable.myTestingCamera4.Fail();

                                //clear text
                                Application.Current.Dispatcher.Invoke(new Action(() => { tb.Clear(); }));
                            }));
                            t.IsBackground = true;
                            t.Start();

                            //count total time
                            Thread tz = new Thread(new ThreadStart(() => {
                                Thread.Sleep(500);
                                int count = 0;
                            RE:
                                count++;
                                bool r = stationVariable.myTestingCamera4.TotalResult.Contains("Waiting...");
                                if (r) {
                                    Thread.Sleep(500);
                                    stationVariable.myTestingCamera4.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan((count + 1) * 500);
                                    goto RE;
                                }

                            }));
                            tz.IsBackground = true;
                            tz.Start();

                            break;
                        }

                }
            }


        }
    }
}
