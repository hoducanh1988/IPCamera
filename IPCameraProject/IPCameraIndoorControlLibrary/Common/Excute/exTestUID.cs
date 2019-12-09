using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestUID<T> where T : class, new() {

        T testingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "";

        public exTestUID(Dut.IPCamera<T> _camera, T _testingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            retry_time = _retry;
            std_value = (string)testingInfo.GetType().GetProperty("uidFromBarcode").GetValue(testingInfo);
        }


        //kiem tra uid qua cong UART
        public bool excuteUart() {
            return false;
        }

        //Kiem tra uid qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;
            var prop_uidresult = testingInfo.GetType().GetProperty("uidResult");
            prop_uidresult.SetValue(testingInfo, "Waiting...");
            try {
                if (!camera.IsConnected()) goto END;
                //get logsytem
                var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
                string log_value = (string)prop_logsystem.GetValue(testingInfo);

                int count = 0;
                RE:
                count++;
                string data = camera.getUidCode();
                log_value += string.Format("{0}\n", data);
                prop_logsystem.SetValue(testingInfo, log_value);

                bool r = data.ToUpper().Contains(std_value.ToUpper());
                if (!r) {
                    if (count < retry_time) goto RE;
                }

                ret = r;
            }
            catch { goto END; }

            END:
            prop_uidresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
