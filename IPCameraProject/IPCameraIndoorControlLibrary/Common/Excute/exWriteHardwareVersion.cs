using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exWriteHardwareVersion<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "";

        public exWriteHardwareVersion(Dut.IPCamera<T> _camera, T _testingInfo, S _settingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
            retry_time = _retry;
            std_value = (string)settingInfo.GetType().GetProperty("hardwareVersion").GetValue(settingInfo);
        }


        //ghi hw ver qua cong UART
        public bool excuteUart() {
            return false;
        }

        //ghi hw ver qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;
            var prop_hardwareresult = testingInfo.GetType().GetProperty("hardwareResult");
            prop_hardwareresult.SetValue(testingInfo, "Waiting...");
            try {
                if (!camera.IsConnected()) goto END;
                //get logsytem
                var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
                string log_value = (string)prop_logsystem.GetValue(testingInfo);

                int count = 0;
                RE:
                count++;
                //set hw ver
                log_value += string.Format("...ghi hardware version \"{0}\" vào camera.\n", std_value);
                prop_logsystem.SetValue(testingInfo, log_value);

                bool r = camera.setHardwareVersion(std_value);
                if (!r) {
                    if (count < retry_time) goto RE;
                    else goto END;
                }

                //verify hw ver after set
                log_value += string.Format("...đọc giá trị hardware version sau khi ghi\n");
                prop_logsystem.SetValue(testingInfo, log_value);

                string data = camera.getHardwareVersion();
                log_value += data;
                prop_logsystem.SetValue(testingInfo, log_value);

                r = data.ToUpper().Contains(std_value.ToUpper());
                if (!r) {
                    if (count < retry_time) goto RE;
                }

                ret = r;
            }
            catch { goto END; }

            END:
            prop_hardwareresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
