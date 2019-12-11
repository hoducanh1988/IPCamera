using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestWiFi<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "wlan0     Link encap:Ethernet";

        public exTestWiFi(Dut.IPCamera<T> _camera, T _testingInfo, S _settingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
            retry_time = _retry;
        }

        //kiem tra wifi qua cong UART
        public bool excuteUart() {
            bool ret = false;
            var prop_wifiresult = testingInfo.GetType().GetProperty("wifiResult");
            prop_wifiresult.SetValue(testingInfo, "Waiting...");

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
            prop_wifiresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra wifi qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;
            var prop_wifiresult = testingInfo.GetType().GetProperty("wifiResult");
            prop_wifiresult.SetValue(testingInfo, "Waiting...");

            string ssid_2g = (string)settingInfo.GetType().GetProperty("wifiSSID2G").GetValue(settingInfo);
            string ssid_5g = (string)settingInfo.GetType().GetProperty("wifiSSID5G").GetValue(settingInfo);
            int delaysec = (int)settingInfo.GetType().GetProperty("delaySerializeSSID").GetValue(settingInfo);

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);
            log_value += string.Format("...{0},{1}\n", ssid_2g.ToUpper(), ssid_5g.ToUpper());
            prop_logsystem.SetValue(testingInfo, log_value);

            try {
                if (!camera.IsConnected()) goto END;
                int count = 0;
                RE:
                count++;
                string data = camera.serializeWifiSSID(delaysec);
                log_value += data;
                prop_logsystem.SetValue(testingInfo, log_value);

                if (data != null) ret = data.ToUpper().Contains(ssid_2g.ToUpper()) && data.ToUpper().Contains(ssid_5g.ToUpper());
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
            prop_wifiresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
