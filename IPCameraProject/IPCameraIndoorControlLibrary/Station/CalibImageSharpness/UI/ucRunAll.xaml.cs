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
using System.Windows.Threading;
using System.IO;
using UtilityPack.IO;
using IPCameraIndoorControlLibrary.Station.CalibImageSharpness.Function;
using IPCameraIndoorControlLibrary.Station.CalibImageSharpness.Function.Custom;
using IPCameraIndoorControlLibrary.Common.Excute;
using IPCameraIndoorControlLibrary.Common.Log;
using IPCameraIndoorControlLibrary.Common.Base;

namespace IPCameraIndoorControlLibrary.Station.CalibImageSharpness.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogTelnet uc_tablog = new Common.UI.ucTabLogTelnet();
        exCalibImageSharpness<TestingInformation, SettingInformation> ex_calib_sharpness = null;

        public ucRunAll() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingCalibSharpness)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingCalibSharpness);

            //binding data
            this.DataContext = stationVariable.myTesting;

            //init tablog
            uc_tablog.DataContext = stationVariable.myTesting;

            //add tab log
            this.grid_debug.Children.Clear();
            this.grid_debug.Children.Add(uc_tablog);

            //set timer scroll log
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (o, s) => {
                uc_tablog.isScroll = stationVariable.myTesting.buttonContent.Equals("STOP");
            };
            timer.Start();
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_content = (string)b.Content;

            switch (b_content) {
                case "START": {

                        Thread t = new Thread(new ThreadStart(() => {
                            stationVariable.myTesting.Ready();
                            stationVariable.myTesting.Checking();
                            
                            ex_calib_sharpness = new exCalibImageSharpness<TestingInformation, SettingInformation>(stationVariable.myTesting, stationVariable.mySetting);
                            bool r = ex_calib_sharpness.excuteTelnet();

                            if (r) stationVariable.myTesting.Pass();
                            else stationVariable.myTesting.Fail();

                            //save log
                            _save_log();

                        }));
                        t.IsBackground = true;
                        t.Start();

                        Thread ct = new Thread(new ThreadStart(() => {
                            Thread.Sleep(500);
                            int count = 0;
                        RE:
                            count++;
                            stationVariable.myTesting.totalTime = UtilityPack.Converter.myConverter.intToTimeSpan(count * 500);
                            bool r = stationVariable.myTesting.buttonContent.Equals("START");
                            if (!r) {
                                Thread.Sleep(500);
                                goto RE;
                            }
                            
                        }));
                        ct.IsBackground = true;
                        ct.Start();

                        break;
                    }
                case "STOP": {
                        if (ex_calib_sharpness != null) ex_calib_sharpness.Dispose();
                        break;
                    }
            }
        }

        private void _save_log() {
            //save log telnet
            new LogTelnet(
                globalParameter.LogStationName.CalibSharpness.ToString(),
                stationVariable.myTesting.macEthernet,
                stationVariable.myTesting.TotalResult
                )
            .saveDataToLogFile(stationVariable.myTesting.logTelnet);

            //save log system
            new LogSystem(
                globalParameter.LogStationName.CalibSharpness.ToString(),
                stationVariable.myTesting.macEthernet,
                stationVariable.myTesting.TotalResult
                )
            .saveDataToLogFile(stationVariable.myTesting.logSystem);

            //save log total
            new LogTotal(
                 globalParameter.LogStationName.CalibSharpness.ToString()
                )
            .saveDataToLogFile(
                "macEthernet", stationVariable.myTesting.macEthernet,
                "TotalResult", stationVariable.myTesting.TotalResult
                );

            //save log image
            new LogImage(
                globalParameter.LogStationName.CalibSharpness.ToString(),
                stationVariable.myTesting.macEthernet
                )
                .saveDataToLogFile(new List<string>() { "image_sharpness.png" }, new List<ImageSource>() { stationVariable.myTesting.imageSource });
        }

    }
}
