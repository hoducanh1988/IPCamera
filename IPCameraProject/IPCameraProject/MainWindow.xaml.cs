using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace IPCameraProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //
        List<Label> labels;

        public MainWindow() {
            //init control
            InitializeComponent();
            //binding data
            this.DataContext = myGlobal.myInputInfo;

            //set window size
            setWindowSize(0.95, 0.995);

            labels = new List<Label>() { lblRunAll, lblRework, lblSetting, lblLog, lblHelp, lblAbout };

            var _control = load_User_Control("runall");
            if (_control != null) grid_Content.Children.Add(_control);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            Label lb = sender as Label;

            //change label foreground --------------//
            foreach (var label in labels) label.Foreground = Brushes.Black;
            lb.Foreground = Brushes.Orange;

            //change under bar ---------------------//
            this.border_Underline.Margin = lb.Margin;
            this.border_Underline.Width = lb.Width - 10;

            //load control -------------------------//
            grid_Content.Children.Clear();
            var _control = load_User_Control(lb.Content.ToString());
            if (_control != null) grid_Content.Children.Add(_control);
        }


        UserControl load_User_Control(string classname) {
            UserControl myControl = null;
            try {
                string control_project = productNameToProjectControlLibrary(myGlobal.myInputInfo.ProductName);
                string control_station = stationNameToProjectStation(myGlobal.myInputInfo.StationName);
                string control_user = labelContentToUserControlName(classname);

                //load SmartHomeControlLibrary.dll
                Assembly asm = Assembly.LoadFile(string.Format("{0}{1}.dll", AppDomain.CurrentDomain.BaseDirectory, control_project));
                Type[] tlist = asm.GetTypes();
                foreach (Type t in tlist) {
                    //IPCameraIndoorControlLibrary.TestFunctionPcbaLayer2.UI
                    string _namespace = string.Format("{0}.Station.{1}.UI", control_project, control_station).ToLower();
                    if (t.Namespace.ToLower().Equals(_namespace) && t.Name.ToLower().Equals(control_user.ToLower())) {
                        myControl = Activator.CreateInstance(t) as UserControl;
                        break;
                    }
                }
            }
            catch { }
            return myControl;
        }


        void setWindowSize(double scaleX, double scaleY) {
            this.Height = SystemParameters.WorkArea.Height * scaleY;
            this.Width = SystemParameters.WorkArea.Width * scaleX;
            this.Top = (SystemParameters.WorkArea.Height * (1 - scaleY)) / 2;
            this.Left = (SystemParameters.WorkArea.Width * (1 - scaleX)) / 2;
        }

        string labelContentToUserControlName(string label_content) {
            switch (label_content.ToUpper()) {
                case "RUNALL": return "ucRunAll";
                case "MANUAL": return "ucManual";
                case "SETTING": return "ucSetting";
                case "LOG": return "ucLog";
                case "HELP": return "ucHelp";
                case "ABOUT": return "ucAbout";
                default: return null;
            }
        }

        string productNameToProjectControlLibrary(string product_name) {
            switch (product_name.ToUpper()) {
                case "IP CAMERA INDOOR": return "IPCameraIndoorControlLibrary";
                case "IP CAMERA OUTDOOR": return "IPCameraOutdoorControlLibrary";
                default: return null;
            }
        }

        string stationNameToProjectStation(string station_name) {
            switch (station_name.ToUpper()) {
                case "UPLOAD FIRMWARE BASIC": return "UploadFirmwareBasic";
                case "SUPPORT CALIBRATION THE IMAGE SHARPNESS": return "CalibImageSharpness";
                case "TEST FUNCTION PCBA LAYER2": return "TestFunctionPcbaLayer2";
                case "TEST FUNCTION PCBA LAYER3": return "TestFunctionPcbaLayer3";
                case "UPLOAD FIRMWARE BUSINESS": return "UploadFirmwareBusiness";
                case "TEST FUNCTION ASM": return "TestFunctionAsm";
                case "IN TEM UID": return "PrintLabelUID";
                case "CALIB NIGHT VISION": return "CalibNightVision";
                default: return null;
            }
        }
    }
}
