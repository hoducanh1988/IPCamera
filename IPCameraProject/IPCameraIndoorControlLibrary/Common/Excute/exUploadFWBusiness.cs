using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using IPCameraIndoorControlLibrary.Common.Base;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exUploadFWBusiness<U, S> where U : class, new() where S : class, new() {

        U uploadInfo;
        S settingInfo;
        public string std_value = "";

        public exUploadFWBusiness(U _uploadInfo, S _settingInfo) {
            uploadInfo = _uploadInfo;
            settingInfo = _settingInfo;
        }

        //Nap fw thuong mai qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;

            var prop_uploadresult = uploadInfo.GetType().GetProperty("uploadResult");
            prop_uploadresult.SetValue(uploadInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            Dut.IPCamera<U> camera = null;

            try {
                //login to camera
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Login to ip camera\n", DateTime.Now);
                prop_logsystem.SetValue(uploadInfo, log_value);

                string camera_ip = (string)uploadInfo.GetType().GetProperty("ipAddress").GetValue(uploadInfo);
                string telnet_user = (string)settingInfo.GetType().GetProperty("cameraTelnetUser").GetValue(settingInfo);
                string telnet_pass = (string)settingInfo.GetType().GetProperty("cameraTelnetPassword").GetValue(settingInfo);

                ret = _loginToCamera(camera_ip, telnet_user, telnet_pass, ref camera);
                log_value = (string)prop_logsystem.GetValue(uploadInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(uploadInfo, log_value);
                if (!ret) goto END;

                //request camera upload firmware
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Request camera upload firmware business\n", DateTime.Now);
                prop_logsystem.SetValue(uploadInfo, log_value);

                string fw_file = (string)settingInfo.GetType().GetProperty("fileFirmware").GetValue(settingInfo);

                ret = _requestCameraUploadFW(camera, fw_file);
                log_value = (string)prop_logsystem.GetValue(uploadInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(uploadInfo, log_value);
                if (!ret) goto END;

                //wait upload
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Wait camera upload complete\n", DateTime.Now);
                prop_logsystem.SetValue(uploadInfo, log_value);

                ret = _waitCameraUploadComplete(camera, 90);
                log_value = (string)prop_logsystem.GetValue(uploadInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(uploadInfo, log_value);
                if (!ret) goto END;

                //reboot or not
                MethodInfo method = settingInfo.GetType().GetMethod("isNeedReboot", BindingFlags.Public | BindingFlags.Instance);
                if (method != null) {
                    var func = (Func<bool>)method.CreateDelegate(typeof(Func<bool>), settingInfo);
                    bool r = func();
                    if (r) {
                        prop_uploadresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");
                        var prop_rebootresult = uploadInfo.GetType().GetProperty("rebootResult");
                        prop_rebootresult.SetValue(uploadInfo, "Waiting...");

                        log_value += string.Format("\n-------------------------------\n");
                        log_value += string.Format("{0}, Reboot camera\n", DateTime.Now);
                        prop_logsystem.SetValue(uploadInfo, log_value);

                        ret = _rebootCamera(camera, camera_ip, 10);
                        log_value = (string)prop_logsystem.GetValue(uploadInfo);
                        log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                        prop_logsystem.SetValue(uploadInfo, log_value);
                        if (!ret) {
                            prop_rebootresult.SetValue(uploadInfo, "Failed");
                            goto END;
                        }
                    }
                }

            } catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(uploadInfo, log_value);
                goto END;
            }

        END:
            prop_uploadresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        #region sub-function
        
        bool _loginToCamera(string ip, string user, string pass, ref Dut.IPCamera<U> camera) {
            bool ret = false;
            int count = 0;
        RE:
            count++;
            camera = new Dut.IPCamera<U>(uploadInfo, ip, user, pass);
            camera.Login();
            ret = camera.IsConnected();
            if (!ret) {
                if (count < 3) goto RE;
            }
            return ret;
        }

        bool _requestCameraUploadFW(Dut.IPCamera<U> camera, string fw_file) {
            bool ret = false;
            int count = 0;
            string req_str = string.Format(@"updatefirmware {0}//192.168.1.100/{1}", "http:", fw_file);

        RE:
            count++;
            ret = camera.uploadFirmwareBusiness(req_str);
            if (!ret) {
                if (count < 3) goto RE;
            }
            return ret;
        }

        bool _waitCameraUploadComplete(Dut.IPCamera<U> camera, int time_out) {
            bool ret = false;
            int count = 0;
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);
            string data = "";

        RE:
            count++;
            string tmpStr = camera.captureLog();
            data += tmpStr;
            log_value += tmpStr;
            prop_logsystem.SetValue(uploadInfo, log_value);
            ret = string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data) ? false : data.Contains("~ #");
            if (!ret) {
                if (count < time_out) {
                    Thread.Sleep(1000);
                    goto RE;
                }
            }
            if (ret) ret = data.Contains("updatefirmware success");

            return ret;
        }

        bool _rebootCamera(Dut.IPCamera<U> camera, string camera_ip, int time_out) {
            bool ret = false;
            int count = 0;
            camera.rebootViaFirmware();
        RE:
            count++;
            ret = !globalUtility.pingNetwork(camera_ip);
            if (!ret) {
                if (count < time_out) {
                    Thread.Sleep(1000);
                    goto RE;
                }
            }
            return ret;
        }

        #endregion
    }
}
