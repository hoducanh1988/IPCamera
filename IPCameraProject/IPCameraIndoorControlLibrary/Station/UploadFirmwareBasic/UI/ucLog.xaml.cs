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

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.UI {
    /// <summary>
    /// Interaction logic for ucLog.xaml
    /// </summary>
    public partial class ucLog : UserControl {
        public ucLog() {
            InitializeComponent();

            if (File.Exists(Function.stationVariable.settingFWBasic)) Function.stationVariable.mySetting = XmlHelper<Function.Custom.SettingInformation>.FromXmlFile(Function.stationVariable.settingFWBasic);
            this.cbb_logtype.ItemsSource = new List<string>() { "LogTotal", "LogSingle", "LogDetail" };

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;

        }
    }
}
