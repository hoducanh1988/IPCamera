using System;
using System.Collections.Generic;
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
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function;
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function.Custom;
using System.IO;
using IPCameraIndoorControlLibrary.Common.Base;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.UI {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {
        public ucSetting() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingFWBasic)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingFWBasic);

            //binding data
            this.DataContext = stationVariable.mySetting;

            //add combobox itemsource
            cbb_comport_1.ItemsSource = globalParameter.list_comport;
            cbb_comport_2.ItemsSource = globalParameter.list_comport;
            cbb_comport_3.ItemsSource = globalParameter.list_comport;
            cbb_comport_4.ItemsSource = globalParameter.list_comport;
            cbb_retrytime.ItemsSource = globalParameter.list_number;
            cbb_failandstop.ItemsSource = new List<string>() { "Yes", "No" };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "save_setting": {
                        XmlHelper<SettingInformation>.ToXmlFile(stationVariable.mySetting, stationVariable.settingFWBasic); //save setting to xml file
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "select_file_firmware": {
                        System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                        fileDialog.InitialDirectory = @"C:\TFTP-Root";
                        fileDialog.Filter = "*.bin|*.bin";
                        if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            //get bin file
                            stationVariable.mySetting.fileFirmware = fileDialog.FileName;
                        }
                        break;
                    }
                default: break;
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }
    }
}
