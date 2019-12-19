using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.AccessTable {

    public class tableReport {

        TableReportInfo reportlabel = null;

        public tableReport() {
            reportlabel = new TableReportInfo();

            reportlabel.Mac = stationVariable.myTesting.MacAddress;
            reportlabel.Serial = stationVariable.myTesting.SerialNumber;
            reportlabel.uidCode = stationVariable.myTesting.UidCode;
            reportlabel.uidHeader = stationVariable.mySetting.vnptUidHeader;
            reportlabel.uidPart1 = reportlabel.uidCode.Substring(6, 8);
            reportlabel.uidPart2 = reportlabel.uidCode.Substring(14, 8);
            reportlabel.uidPart3 = reportlabel.uidCode.Substring(22, 8);
            reportlabel.uidPart4 = reportlabel.uidCode.Substring(30, 8);
        }

        public bool Report_WriteData() {
            try {
                var box = stationVariable.msAccessReport;
                return box.Input_New_DataRow_To_Access_DB_Table<TableReportInfo>("tb_Report", this.reportlabel);
            }
            catch {
                return false;
            }
        }

        public bool Report_DeleteAll() {
            try {
                var box = stationVariable.msAccessReport;
                return box.Delete_All_DataRow_From_Access_DB_Table("tb_Report");
            }
            catch {
                return false;
            }
        }
    }
}
