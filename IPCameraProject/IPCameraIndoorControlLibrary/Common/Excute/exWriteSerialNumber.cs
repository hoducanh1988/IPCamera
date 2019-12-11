using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exWriteSerialNumber<T> where T : class, new() {

        T testingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "";

        public exWriteSerialNumber(Dut.IPCamera<T> _camera, T _testingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            retry_time = _retry;
            std_value = (string)testingInfo.GetType().GetProperty("serialFromBarcode").GetValue(testingInfo);
        }


        //ghi sn qua cong UART
        public bool excuteUart() {
            return false;
        }

        //ghi sn qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;
            var prop_serialresult = testingInfo.GetType().GetProperty("serialResult");
            prop_serialresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                int count = 0;
                RE:
                count++;
                //set serial
                log_value += string.Format("...ghi serial number \"{0}\" vào camera.\n", std_value);
                prop_logsystem.SetValue(testingInfo, log_value);

                ret = camera.setSerialNumber(std_value);
                if (!ret) {
                    if (count < retry_time) goto RE;
                    else goto END;
                }

                //verify serial after set
                ret = false;
                log_value += string.Format("...đọc giá trị serial number sau khi ghi\n");
                prop_logsystem.SetValue(testingInfo, log_value);

                string data = camera.getSerialNumber();
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
            prop_serialresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }



    }
}
