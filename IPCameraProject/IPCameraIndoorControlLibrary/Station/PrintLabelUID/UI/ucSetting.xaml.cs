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

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.UI {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {
        public ucSetting() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(Function.stationVariable.settingPrintUIDLabel)) Function.stationVariable.mySetting = XmlHelper<Function.Custom.SettingInformation>.FromXmlFile(Function.stationVariable.settingPrintUIDLabel);

            //binding data
            this.DataContext = Function.stationVariable.mySetting;

            //add combobox itemsource
            cbb_factory.ItemsSource = globalParameter.list_number;
            cbb_hardware.ItemsSource = globalParameter.list_number;
            cbb_retrytime.ItemsSource = globalParameter.list_number;
            cbb_productcolor.ItemsSource = globalParameter.list_product_color;
            cbb_failandstop.ItemsSource = new List<string>() { "Yes", "No" };
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {

                //save setting
                case "save_setting": {
                        XmlHelper<Function.Custom.SettingInformation>.ToXmlFile(Function.stationVariable.mySetting, Function.stationVariable.settingPrintUIDLabel); //save setting to xml file
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "select_file_ms_access": {
                        System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                        fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        fileDialog.Filter = "*.accdb|*.accdb";
                        if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            //get bin file
                            Function.stationVariable.mySetting.fileMsAccess = fileDialog.SafeFileName;
                        }
                        break;
                    }
            }
        }


    }
}
