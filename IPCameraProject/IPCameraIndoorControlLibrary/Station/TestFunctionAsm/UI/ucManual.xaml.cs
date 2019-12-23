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
using IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function;
using IPCameraIndoorControlLibrary.Station.TestFunctionAsm.Function.Custom;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.UI
{
    /// <summary>
    /// Interaction logic for ucManual.xaml
    /// </summary>
    public partial class ucManual : UserControl
    {
        public ucManual()
        {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingAsm)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingAsm);

            //binding data
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
        }
    }
}
