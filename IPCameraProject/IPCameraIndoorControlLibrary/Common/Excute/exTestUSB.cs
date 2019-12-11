using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exTestUSB<T> where T : class, new() {

        T testingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "wlan0     Link encap:Ethernet";

        public exTestUSB(Dut.IPCamera<T> _camera, T _testingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            retry_time = _retry;
        }

        //kiem tra usb qua cong UART
        public bool excuteUart() {
            bool ret = false;
            var prop_usbresult = testingInfo.GetType().GetProperty("UsbResult");
            prop_usbresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                int count = 0;
                RE:
                count++;
                string data = camera.getWlanInterface();
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
            prop_usbresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra usb qua cong telnet
        public bool excuteTelnet() {
            return false;
        }


    }
}
