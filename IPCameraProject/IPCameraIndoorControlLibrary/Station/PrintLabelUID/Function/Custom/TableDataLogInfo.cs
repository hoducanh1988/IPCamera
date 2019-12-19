using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom {

    public class TableDataLogInfo {

        public string tb_ID { get; set; }
        public string dateTimeCreated { get; set; }
        public string macAddress { get; set; }
        public string serialNumber { get; set; }
        public string uidCode { get; set; }
        public string totalResult { get; set; }
        public string errorMessage { get; set; }

    }
}
