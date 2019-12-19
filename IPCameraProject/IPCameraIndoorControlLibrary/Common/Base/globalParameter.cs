using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Base {
    public class globalParameter {

        public static List<string> list_baudrate = null;
        public static List<string> list_comport = null;
        public static List<string> list_number = null;
        public static List<string> list_product_color = null;

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
            list_product_color = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        }

    }
}
