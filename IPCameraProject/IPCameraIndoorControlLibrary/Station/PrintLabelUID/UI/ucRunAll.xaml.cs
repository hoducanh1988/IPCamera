using System;
using System.Collections.Generic;
using System.Globalization;
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
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.UI {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {


        public ucRunAll() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(stationVariable.settingPrintUIDLabel)) stationVariable.mySetting = XmlHelper<SettingInformation>.FromXmlFile(stationVariable.settingPrintUIDLabel);

            string year = "";
            int t = int.Parse(DateTime.Now.ToString("yyyy").Substring(2, 2)) - 13;
            year = t < 10 ? t.ToString() : Convert.ToChar(55 + t).ToString().Trim();
            stationVariable.mySetting.productionYear = year;


            string week = "";
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            week = weekNum.ToString().Trim();
            stationVariable.mySetting.productionWeek = week;

            //binding data
            this._grid_setting.DataContext = stationVariable.mySetting;
            this._grid_testing.DataContext = stationVariable.myTesting;

            //Load combobox ItemSource
            this.cbb_list_result.ItemsSource = new List<string>() { "PASS", "FAIL" };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            //switch (tag) {
            //    case "search_datalog": {
            //            List<tbDataLog> listdatalog = MyGlobal.MasterBox.Get_Specified_DataRow_From_Access_DB_Table<tbDataLog>("tb_DataLog", int.Parse(MyGlobal.mySetting.VisibleLogQuantity), "tb_ID", "MacAddress", txt_search_datalog_mac.Text, "TotalResult", cbb_list_result.Text, "", "");
            //            this.datagrid_recentdatalog.ItemsSource = listdatalog;
            //            MessageBox.Show("Success.", "Search Log MSAccess", MessageBoxButton.OK, MessageBoxImage.Information);
            //            break;
            //        }
            //    case "export_excel": {
            //            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //            saveFileDialog.Filter = "Excel 1997 - 2003 (*.xls)|*.xls";
            //            if (saveFileDialog.ShowDialog() == true) {
            //                string file_name = saveFileDialog.FileName;
            //                new msaccdb_tbDataLog().ExportToExcel(file_name);
            //                MessageBox.Show("Success.", "Export Log MSAccess To Excel File", MessageBoxButton.OK, MessageBoxImage.Information);
            //            }
            //            break;
            //        }
            //    default: break;
            //}
        }

        private void Txt_MAC_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {

                ////check setting parameters
                //SettingParameter settingparams = new SettingParameter();
                //string msg = "";
                //bool r = settingparams.Validation(out msg);
                //if (!r) {
                //    MessageBox.Show(msg, "Error Setting", MessageBoxButton.OK, MessageBoxImage.Error);
                //    MyGlobal.myTesting.MacAddress = "";
                //}
                //else {
                //    txt_MAC.IsEnabled = false; //
                //    MyGlobal.myTesting.MacAddress = MyGlobal.myTesting.MacAddress.ToUpper();
                //    _run_All();
                //}


            }
        }

        private void Txt_MAC_TextChanged(object sender, TextChangedEventArgs e) {
            //if (MyGlobal.myTesting.MacAddress.Length > 0) { MyGlobal.myTesting.WaitingParameters(); }
            //else {
            //    if (MyGlobal.myTesting.TotalResult.ToLower().Equals("waiting...")) MyGlobal.myTesting.TotalResult = "";
            //}
        }


    }
}
