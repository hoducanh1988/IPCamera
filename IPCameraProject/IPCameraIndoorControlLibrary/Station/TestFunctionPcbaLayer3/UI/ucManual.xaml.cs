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
using System.IO;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.Function.Custom;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.UI {
    /// <summary>
    /// Interaction logic for ucManual.xaml
    /// </summary>
    public partial class ucManual : UserControl {
        public ucManual() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingLayer3)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingLayer3);

            //binding data
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
        }
    }
}
