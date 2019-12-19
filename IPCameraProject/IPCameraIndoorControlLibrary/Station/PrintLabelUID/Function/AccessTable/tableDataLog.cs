using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.AccessTable {
    public class tableDataLog {

        TableDataLogInfo dataLog = null;

        public tableDataLog() {
            this.dataLog = new TableDataLogInfo();

            dataLog.dateTimeCreated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            dataLog.macAddress = stationVariable.myTesting.MacAddress;
            dataLog.serialNumber = stationVariable.myTesting.SerialNumber;
            dataLog.uidCode = stationVariable.myTesting.UidCode;
            dataLog.totalResult = stationVariable.myTesting.TotalResult;
            dataLog.errorMessage = stationVariable.myTesting.ErrorMessage;
        }

        public bool WriteData() {
            try {
                var box = stationVariable.msAccessReport;
                return box.Input_New_DataRow_To_Access_DB_Table<TableDataLogInfo>("tb_DataLog", this.dataLog, "tb_ID");
            }
            catch {
                return false;
            }
        }

        public List<TableDataLogInfo> ReadData() {
            var box = stationVariable.msAccessReport;
            return box.Get_Specified_DataRow_From_Access_DB_Table<TableDataLogInfo>("tb_DataLog", 100, "tb_ID", "serialNumber", "", "totalResult", "", "", "");
        }

        public bool ExportToExcel(string export_filename) {
            try {
                string table_name = "tb_DataLog";
                var box = stationVariable.msAccessReport;
                box.ExportQuery(table_name, export_filename);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
