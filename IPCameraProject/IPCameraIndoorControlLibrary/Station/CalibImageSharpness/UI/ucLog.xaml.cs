﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace IPCameraIndoorControlLibrary.Station.CalibImageSharpness.UI {
    /// <summary>
    /// Interaction logic for ucLog.xaml
    /// </summary>
    public partial class ucLog : UserControl {
        public ucLog() {
            InitializeComponent();

            if (File.Exists(Function.stationVariable.settingCalibSharpness)) Function.stationVariable.mySetting = XmlHelper<Function.Custom.SettingInformation>.FromXmlFile(Function.stationVariable.settingCalibSharpness);
            this.cbb_logtype.ItemsSource = globalParameter.list_log_type;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string log_type = this.cbb_logtype.Text;

            switch (log_type) {
                case "LogTotal": {
                        new LogTotal(globalParameter.LogStationName.CalibSharpness.ToString()).openLogFolder();
                        break;
                    }
                case "LogSystem": {
                        new LogSystem(globalParameter.LogStationName.CalibSharpness.ToString(), "", "").openLogFolder();
                        break;
                    }
                case "LogTelnet": {
                        new LogTelnet(globalParameter.LogStationName.CalibSharpness.ToString(), "", "").openLogFolder();
                        break;
                    }
                case "LogImage": {
                        new LogImage(globalParameter.LogStationName.CalibSharpness.ToString(), "").openLogFolder();
                        break;
                    }
                case "LogUart":
                case "LogESOP":
                default: {
                        MessageBox.Show("Trạm calib độ nét hình ảnh camera không có loại log này.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
            }
        }
    }
}
