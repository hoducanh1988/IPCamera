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
using Microsoft.Win32;
using UtilityPack.IO;
using UtilityPack.Validation;
using UtilityPack.Converter;
using System.Threading;
using System.Diagnostics;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.AccessTable;
using IPCameraIndoorControlLibrary.Common.IO;
using IPCameraIndoorControlLibrary.Common.Log;
using IPCameraIndoorControlLibrary.Common.Base;


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
            this.cbb_list_result.ItemsSource = new List<string>() { "Passed", "Failed" };

            //set ms access file
            string access_file = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, stationVariable.mySetting.fileMsAccess);
            stationVariable.msAccessReport = new MsAccessDatabase(access_file);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "search_datalog": {
                        var box = stationVariable.msAccessReport;
                        List<TableDataLogInfo> listdatalog = box.Get_Specified_DataRow_From_Access_DB_Table<TableDataLogInfo>("tb_DataLog", 100, "tb_ID", "macAddress", txt_search_datalog_mac.Text, "totalResult", cbb_list_result.Text, "", "");
                        this.datagrid_recentdatalog.ItemsSource = listdatalog;
                        MessageBox.Show("Success.", "Search Log MSAccess", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "export_excel": {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        saveFileDialog.Filter = "Excel 1997 - 2003 (*.xls)|*.xls";
                        if (saveFileDialog.ShowDialog() == true) {
                            string file_name = saveFileDialog.FileName;
                            new tableDataLog().ExportToExcel(file_name);
                            MessageBox.Show("Success.", "Export Log MSAccess To Excel File", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    }
                default: break;
            }
        }

        private void Txt_MAC_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {

                //check setting parameters
                SettingParameter settingparams = new SettingParameter();
                string msg = "";
                bool r = settingparams.Validation(out msg);
                if (!r) {
                    MessageBox.Show(msg, "Lỗi thông tin cài đặt in tem", MessageBoxButton.OK, MessageBoxImage.Error);
                    stationVariable.myTesting.MacAddress = "";
                }
                else {
                    txt_MAC.IsEnabled = false; //
                    stationVariable.myTesting.MacAddress = stationVariable.myTesting.MacAddress.ToUpper();
                    _run_All();
                }
            }
        }

        private void Txt_MAC_TextChanged(object sender, TextChangedEventArgs e) {
            if (stationVariable.myTesting.MacAddress.Length > 0) { stationVariable.myTesting.WaitingParameters(); }
            else {
                if (stationVariable.myTesting.TotalResult.ToLower().Equals("waiting...")) stationVariable.myTesting.TotalResult = "-";
            }
        }

        #region print label

        //normal production
        bool _run_All() {
            Thread zzz = new Thread(new ThreadStart(() => {
                //run check
                var runall = new imp_RunAll();
                bool r = runall.Execute();
                if (!r) _jud_fail();
                else _jud_pass();

                //save log
                new LogSystem(globalParameter.LogStationName.UidLabel.ToString(), stationVariable.myTesting.MacAddress, stationVariable.myTesting.TotalResult).saveDataToLogFile(stationVariable.myTesting.ErrorMessage);
            }));
            zzz.IsBackground = true;
            zzz.Start();

            return true;
        }


        void _jud_pass() {
            Stopwatch st = new Stopwatch();
            st.Start();

            //display PASS
            stationVariable.myTesting.ErrorMessage += string.Format("Mac address is valid.");
            stationVariable.myTesting.PassParameters();

            //save log
            _save_log_();
            st.Stop();
            stationVariable.myTesting.TestTime += string.Format("<save_log={0}>", st.ElapsedMilliseconds);

            //print label
            st.Reset();
            st.Restart();
            stationVariable.msAccessReport.Print_Access_Report("Report");
            Thread.Sleep(100);
            st.Stop();
            stationVariable.myTesting.TestTime += string.Format("<print_label={0}>", st.ElapsedMilliseconds);

            //load ms database
            st.Reset();
            st.Restart();
            _load_ms_datatable_();
            st.Stop();
            stationVariable.myTesting.TestTime += string.Format("<load_dblog={0}>", st.ElapsedMilliseconds);

            Dispatcher.Invoke(new Action(() => { txt_MAC.Clear(); txt_MAC.IsEnabled = true; txt_MAC.Focus(); }));  //set focus for txt_ID
        }

        void _jud_fail() {
            stationVariable.myTesting.FailParameters(); //display fail
            _save_log_(); //save log

            _load_ms_datatable_(); //load ms database

            Dispatcher.Invoke(new Action(() => { txt_MAC.Clear(); txt_MAC.IsEnabled = true; txt_MAC.Focus(); }));  //set focus for txt_ID
        }

        bool _save_log_() {
            bool r = false;
            Dispatcher.Invoke(new Action(() => {
                r = new tableDataLog().WriteData(); //save log to tb_datalog
                //write data to print label
                if (stationVariable.myTesting.TotalResult == "Passed") {
                    tableReport tbreport = new tableReport();
                    tbreport.Report_DeleteAll();
                    tbreport.Report_WriteData();
                }
            }));
            return r;
        }

        bool _load_ms_datatable_() {
            bool r = false;
            Dispatcher.Invoke(new Action(() => {
                this.datagrid_recentdatalog.ItemsSource = new tableDataLog().ReadData(); //read log from tb_datalog
            }));
            return r;
        }



        public class imp_RunAll {

            public bool Execute() {
                bool r = false;
                Stopwatch st = new Stopwatch();
                st.Start();

                try {
                    //init log
                    stationVariable.myTesting.ErrorMessage = "";
                    stationVariable.myTesting.TestTime = "";
                    stationVariable.myTesting.ErrorMessage = string.Format("Mac address: \"{0}\"\r\n", stationVariable.myTesting.MacAddress);

                    //validate mac address
                    r = Parse.IsVnptMacAddress(stationVariable.myTesting.MacAddress, stationVariable.mySetting.vnptMacHeader);
                    if (!r) {
                        stationVariable.myTesting.ErrorMessage += string.Format("Địa chỉ mac {0} không hợp lệ.", stationVariable.myTesting.MacAddress);
                        goto END;
                    }

                    //generate serial number
                    stationVariable.myTesting.SerialNumber = myConverter.FromMACToSerialNumberNewFormat(
                        stationVariable.myTesting.MacAddress,
                        stationVariable.mySetting.vnptProductNumber,
                        stationVariable.mySetting.hardwareVersion,
                        stationVariable.mySetting.productMacCode,
                        stationVariable.mySetting.productionFactory);

                    r = Parse.IsVnptProductSerialNumber(stationVariable.myTesting.SerialNumber);
                    if (!r) {
                        stationVariable.myTesting.ErrorMessage += string.Format("Serial number {0} không hợp lệ.", stationVariable.myTesting.SerialNumber);
                        goto END;
                    }
                    stationVariable.myTesting.ErrorMessage += string.Format("Serial number: \"{0}\"\r\n", stationVariable.myTesting.SerialNumber);

                    //generate uid code
                    stationVariable.myTesting.UidCode = myConverter.FromMACToUIDCode(
                        stationVariable.myTesting.MacAddress,
                        stationVariable.mySetting.vnptUidHeader );

                    r = Parse.IsVnptUidCode(stationVariable.myTesting.UidCode, stationVariable.mySetting.vnptUidHeader);
                    if (!r) {
                        stationVariable.myTesting.ErrorMessage += string.Format("UID code {0} không hợp lệ.", stationVariable.myTesting.UidCode);
                        goto END;
                    }
                    stationVariable.myTesting.ErrorMessage += string.Format("UID code: \"{0}\"\r\n", stationVariable.myTesting.UidCode);

                    r = true;
                    goto END;

                }
                catch {
                    goto END;
                }


            END:
                return r;
            }


           

        }


        #endregion

    }
}
