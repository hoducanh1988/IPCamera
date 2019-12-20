using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilityPack.Validation;
using IPCameraIndoorControlLibrary.Common.IO;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exUploadFWBasic<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "";
        string vnpt_mac_header = "";

        public exUploadFWBasic(T _testingInfo, S _settingInfo) {
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
        }

        public bool excuteUart() {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            string mac_ethernet = (string)testingInfo.GetType().GetProperty("macEthernet").GetValue(testingInfo);
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, Nhập mac từ barcode reader\n", DateTime.Now);
            log_value += string.Format("...{0}\n", mac_ethernet);
            prop_logsystem.SetValue(testingInfo, log_value);

            try {

                //check mac address
                ret = _isVnptMacAddress(mac_ethernet);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //open serial port
                string serial_port = (string)testingInfo.GetType().GetProperty("serialPortName").GetValue(testingInfo);
                ret = _openSerialPort(serial_port);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //login to uboot (wait 30s)
                ret = _loginToUboot(30);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //set mac ethernet
                
                ret = _setMacEthernet(mac_ethernet);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                testingInfo.GetType().GetProperty("setMacResult").SetValue(testingInfo, ret ? "Passed" : "Failed");
                if (!ret) goto END;

                //upload firmware basic
                string camera_ip = (string)testingInfo.GetType().GetProperty("ipAddress").GetValue(testingInfo);
                string file_firmware = (string)settingInfo.GetType().GetProperty("fileFirmware").GetValue(settingInfo);
                ret = _uploadFirmwareBasic(camera_ip, file_firmware, 90);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                testingInfo.GetType().GetProperty("uploadResult").SetValue(testingInfo, ret ? "Passed" : "Failed");
                if (!ret) goto END;

                //reboot camera and wait camera reboot complete
                ret = _rebootCamera();
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //wait camera reboot complete
                ret = _waitCameraRebootComplete(60);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //set static ip
                string static_ip = (string)settingInfo.GetType().GetProperty("cameraStaticIP").GetValue(settingInfo);
                ret = _setStaticIP(static_ip);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                testingInfo.GetType().GetProperty("setIpResult").SetValue(testingInfo, ret ? "Passed" : "Failed");
                if (!ret) goto END;

                //write uid
                string uid_header = (string)settingInfo.GetType().GetProperty("vnptUidHeader").GetValue(settingInfo);
                string uid_code = ""; ;
                ret = _setUidCode(uid_header, mac_ethernet, out uid_code);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                testingInfo.GetType().GetProperty("uidCode").SetValue(testingInfo, uid_code);
                goto END;

            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

        END:
            if (camera != null && camera.IsConnected()) camera.Close();
            return ret;
        }

        bool _isVnptMacAddress(string mac) {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, Kiểm tra địa chỉ mac nhập từ barcode\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);

            ret = string.IsNullOrEmpty(mac) || string.IsNullOrWhiteSpace(mac);
            log_value += string.Format("...IsNullOrEmpty hoặc IsNullOrWhiteSpace {0}\n", ret);
            prop_logsystem.SetValue(testingInfo, log_value);
            if (ret) return false;

            //check length
            ret = mac.Length == 12;
            log_value += string.Format("...chiều dài 12 kí tự {0}\n", ret);
            prop_logsystem.SetValue(testingInfo, log_value);
            if (!ret) return false;

            //check format
            ret = Parse.IsMacAddress(mac);
            log_value += string.Format("...kí tự 0-9A-F {0}\n", ret);
            prop_logsystem.SetValue(testingInfo, log_value);
            if (!ret) return false;

            //check header
            vnpt_mac_header = (string)settingInfo.GetType().GetProperty("vnptMacHeader").GetValue(settingInfo);
            ret = vnpt_mac_header.ToUpper().Contains(mac.Substring(0, 6).ToUpper());
            log_value += string.Format("...header {0} {1}\n", vnpt_mac_header, ret);
            prop_logsystem.SetValue(testingInfo, log_value);
            if (!ret) return false;

            return true;
        }

        bool _openSerialPort(string serial_port_name) {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, open serial port {1}\n", DateTime.Now, serial_port_name);
            prop_logsystem.SetValue(testingInfo, log_value);

            camera = new Dut.IPCamera<T>(testingInfo, serial_port_name, 57600, 8);
            ret = camera.Open();

            return ret;
        }

        bool _loginToUboot(int time_out) {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            //get log uart
            var prop_loguart = testingInfo.GetType().GetProperty("logUart");

            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, login to uboot, timeout = {1}\n", DateTime.Now, time_out);
            prop_logsystem.SetValue(testingInfo, log_value);

            int count = 0;
            string reg_text = "Hit any key to stop autoboot";
            int delay_ms = 100;
            int max_count = (time_out * 1000) / delay_ms;
        RE:
            count++;
            camera.captureLog();
            string loguart = (string)prop_loguart.GetValue(testingInfo);
            ret = loguart.ToLower().Contains(reg_text.ToLower());

            if ( (count * delay_ms) % 1000 == 0) {
                log_value += string.Format("...{0}\n", (count * delay_ms) / 1000);
                prop_logsystem.SetValue(testingInfo, log_value);
            }

            if (!ret) {
                if (count < max_count) {
                    Thread.Sleep(delay_ms);
                    goto RE;
                }
            }
            else {
                ret = camera.loginToUboot();
            }

            return ret;
        }

        bool _setMacEthernet(string mac) {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            //get log uart
            var prop_loguart = testingInfo.GetType().GetProperty("logUart");

            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, set mac ethernet\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);

            int count = 0;
        RE_SET:
            count++;
            ret = camera.setMacEthernetViaUboot(mac);
            if (!ret) {
                if (count < 3) goto RE_SET;
            }

            return ret;
        }

        bool _uploadFirmwareBasic(string camera_ip, string file_fw, int time_out) {
            bool ret = false;
            int count = 0;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            //set uboot ip
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, set uboot ip\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_IP:
            count++;
            ret = camera.setIPUboot(camera_ip, "192.168.1.1", "255.255.255.0");
            if (!ret) {
                if (count < 3) goto RE_IP;
            }
            if (!ret) return false;
            log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
            prop_logsystem.SetValue(testingInfo, log_value);

            //request camera upload firmware
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, request camera upload firmware\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_REQUEST:
            count++;
            ret = camera.uploadFirmwareBasic();
            if (!ret) {
                if (count < 3) goto RE_REQUEST;
            }
            if (!ret) return false;
            log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
            prop_logsystem.SetValue(testingInfo, log_value);

            //request transfer firmware file from pc to camera
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, transfer file firmware from pc to camera\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_TRANSFER:
            count++;
            // Connect request failed
            string data = CommandPrompt.Query(string.Format("tftp -i {0} put {1}", camera_ip, file_fw));
            log_value += data;
            prop_logsystem.SetValue(testingInfo, log_value);
            ret = data.Contains("successful");

            if (!ret) {
                if (count < 3) goto RE_TRANSFER;
            }
            if (!ret) return false;
            log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
            prop_logsystem.SetValue(testingInfo, log_value);


            //wait camera upload firmware complete
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, wait camera upload firmware complete\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_WAIT:
            count++;
            ret = camera.captureLog().Contains("update done");
            if (!ret) {
                if (count < time_out) {
                    Thread.Sleep(1000);
                    goto RE_WAIT;
                }
            }
            
            //return result
            return ret;
        }

        bool _rebootCamera() {
            bool ret = false;
            int count = 0;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            //reboot camera
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, reboot camera\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_REBOOT:
            count++;
            ret = camera.rebootViaUboot();
            if (!ret) {
                if (count < 3) goto RE_REBOOT;
            }

            return ret;
        }

        bool _waitCameraRebootComplete(int time_out) {
            bool ret = false;
            int count = 0;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);
            
            //get log uart
            var prop_loguart = testingInfo.GetType().GetProperty("logUart");

            //wait camera reboot complete
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, wait camera reboot complete\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
            string reg_text = "Please press Enter to activate this console.";

        RE_REBOOT:
            count++;
            camera.captureLog();
            string loguart = (string)prop_loguart.GetValue(testingInfo);
            ret = loguart.ToLower().Contains(reg_text.ToLower());

            if (!ret) {
                if (count < time_out) { Thread.Sleep(1000); goto RE_REBOOT; }
            }

            return ret;
        }

        bool _setStaticIP(string static_ip) {
            bool ret = false;
            int count = 0;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            //set static ip
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, set static ip\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE_SET:
            count++;
            ret = camera.setStaticIP(static_ip);
            if (!ret) {
                if (count < 3) goto RE_SET;
            }

            return ret;
        }

        bool _setUidCode(string uid_header, string mac, out string uid_code) {
            bool ret = false;
            int count = 0;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            string uid = string.Format("{0}{1}", uid_header, UtilityPack.Converter.myConverter.stringToMD5(mac));
            uid_code = uid;

            //set uid code
            log_value += string.Format("\n-------------------------------\n");
            log_value += string.Format("{0}, set uid code\n", DateTime.Now);
            prop_logsystem.SetValue(testingInfo, log_value);
            count = 0;
        RE:
            count++;
            ret = camera.setUIdCode(uid);
            if (!ret) {
                if (count < 3) goto RE;
            }

            return ret;
        }

    }
}
