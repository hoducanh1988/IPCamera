using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function.Custom;
using IPCameraIndoorControlLibrary.Common.Base;
using System;
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

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.UI {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {

        public ucSetting() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingLayer2)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingLayer2);

            //binding data
            this.DataContext = stationVariable.mySetting;

            //add combobox itemsource
            cbb_comport.ItemsSource = globalParameter.list_comport;
            cbb_baudrate.ItemsSource = globalParameter.list_baudrate;
            cbb_retrytime.ItemsSource = globalParameter.list_number;
            cbb_failandstop.ItemsSource = new List<string>() { "Yes", "No" };
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "save_setting": {
                        XmlHelper<SettingInformation>.ToXmlFile(stationVariable.mySetting, stationVariable.settingLayer2); //save setting to xml file
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
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
