using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Base {
    public class globalParameter {

        public enum LogStationName { Layer2, FwBasic, Layer3, CalibSharpness, UidLabel, ASM, CalibNight, FwBusiness };

        public static List<string> list_baudrate = null;
        public static List<string> list_comport = null;
        public static List<string> list_number = null;
        public static List<string> list_product_color = null;
        public static List<string> list_log_type = null;

        static globalParameter() {
            //list baud rate
            list_baudrate = new List<string>() { "2400", "4800", "9600", "14400", "19200", "38400", "57600", "115200" };

            //list comport
            list_comport = new List<string>();
            for (int i = 1; i < 100; i++) { list_comport.Add("COM" + i.ToString()); }

            //list number
            list_number = new List<string>();
            for (int i = 0; i < 100; i++) { list_number.Add(i.ToString()); }

            //list product color
            list_product_color = new List<string>() { "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                                                      "Q", "R","S", "T", "U", "V", "W", "X", "Y", "Z"
            };

            //list log type
            list_log_type = new List<string>() { "LogTotal", "LogSystem", "LogUart", "LogTelnet", "LogImage", "LogESOP" };
        }

    }
}
