using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraProject {

    public class myGlobal {

        public static inputInformation myInputInfo = null;
        public static string myStationConfigFile = string.Format("{0}StationConfig.xml", AppDomain.CurrentDomain.BaseDirectory);
        
    }
}
