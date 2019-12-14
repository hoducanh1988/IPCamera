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
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function;
using IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.Function.Custom;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        Common.UI.ucTabLogUart uc_tablog1 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog2 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog3 = new Common.UI.ucTabLogUart();
        Common.UI.ucTabLogUart uc_tablog4 = new Common.UI.ucTabLogUart();

        public ucRunAll() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingFWBasic)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingFWBasic);

            //set comport
            stationVariable.myTestingCamera1.serialPortName = stationVariable.mySetting.SerialPortName1.Length > 0 ? stationVariable.mySetting.SerialPortName1 : null;
            stationVariable.myTestingCamera2.serialPortName = stationVariable.mySetting.SerialPortName2.Length > 0 ? stationVariable.mySetting.SerialPortName2 : null;
            stationVariable.myTestingCamera3.serialPortName = stationVariable.mySetting.SerialPortName3.Length > 0 ? stationVariable.mySetting.SerialPortName3 : null;
            stationVariable.myTestingCamera4.serialPortName = stationVariable.mySetting.SerialPortName4.Length > 0 ? stationVariable.mySetting.SerialPortName4 : null;

            //set ip
            string[] buffer = stationVariable.mySetting.cameraIPUploadFrom.Split('.');
            stationVariable.myTestingCamera1.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 0);
            stationVariable.myTestingCamera2.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 1);
            stationVariable.myTestingCamera3.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 2);
            stationVariable.myTestingCamera4.ipAddress = string.Format("{0}.{1}.{2}.{3}", buffer[0], buffer[1], buffer[2], int.Parse(buffer[3]) + 3);


            //binding data
            this.grid_camera_1.DataContext = stationVariable.myTestingCamera1;
            this.grid_camera_2.DataContext = stationVariable.myTestingCamera2;
            this.grid_camera_3.DataContext = stationVariable.myTestingCamera3;
            this.grid_camera_4.DataContext = stationVariable.myTestingCamera4;

            //init tablog
            uc_tablog1.DataContext = stationVariable.myTestingCamera1;
            uc_tablog2.DataContext = stationVariable.myTestingCamera2;
            uc_tablog3.DataContext = stationVariable.myTestingCamera3;
            uc_tablog4.DataContext = stationVariable.myTestingCamera4;

            //add tab log
            this.grid_debug_1.Children.Clear();
            //this.grid_debug_2.Children.Clear();
            //this.grid_debug_3.Children.Clear();
            //this.grid_debug_4.Children.Clear();
            this.grid_debug_1.Children.Add(uc_tablog1);
            //this.grid_debug_2.Children.Add(uc_tablog2);
            //this.grid_debug_3.Children.Add(uc_tablog3);
            //this.grid_debug_4.Children.Add(uc_tablog4);

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                TextBox tb = sender as TextBox;
                string tb_tag = (string)tb.Tag;

                switch (tb_tag) {
                    case "txt_mac_1": {
                            stationVariable.myTestingCamera1.Checking();
                            break;
                        }
                }







            }


        }
    }
}
