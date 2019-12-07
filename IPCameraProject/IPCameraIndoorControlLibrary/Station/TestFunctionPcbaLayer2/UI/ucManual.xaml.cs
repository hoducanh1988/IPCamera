using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer2.Function.Custom;
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
    /// Interaction logic for ucManual.xaml
    /// </summary>
    public partial class ucManual : UserControl {
        public ucManual() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingLayer2)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingLayer2);

            //binding data

        }
    }
}
