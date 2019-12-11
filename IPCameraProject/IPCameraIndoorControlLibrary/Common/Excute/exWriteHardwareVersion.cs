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

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                int count = 0;
                RE:
                count++;
                //set hw ver
                log_value += string.Format("...ghi hardware version \"{0}\" vào camera.\n", std_value);
                prop_logsystem.SetValue(testingInfo, log_value);

                ret = camera.setHardwareVersion(std_value);
                if (!ret) {
                    if (count < retry_time) goto RE;
                    else goto END;
                }

                //verify hw ver after set
                ret = false;
                log_value += string.Format("...đọc giá trị hardware version sau khi ghi\n");
                prop_logsystem.SetValue(testingInfo, log_value);

                string data = camera.getHardwareVersion();
                log_value += data;
                prop_logsystem.SetValue(testingInfo, log_value);

                if (data != null) ret = data.ToUpper().Contains(std_value.ToUpper());
                if (!ret) {
                    if (count < retry_time) goto RE;
                }
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END; 
            }

            END:
            prop_hardwareresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
