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

namespace IPCameraIndoorControlLibrary.Station.CalibNightVision.UI {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {
        
        public ucSetting() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(Function.stationVariable.settingcalibNightVision)) Function.stationVariable.mySetting = XmlHelper<Function.Custom.SettingInformation>.FromXmlFile(Function.stationVariable.settingcalibNightVision);

            //binding data
            this.DataContext = Function.stationVariable.mySetting;

            //add combobox itemsource
            cbb_retrytime.ItemsSource = globalParameter.list_number;
            cbb_failandstop.ItemsSource = new List<string>() { "Yes", "No" };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                //save setting
                case "save_setting": {
                        XmlHelper<Function.Custom.SettingInformation>.ToXmlFile(Function.stationVariable.mySetting, Function.stationVariable.settingcalibNightVision); //save setting to xml file
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
