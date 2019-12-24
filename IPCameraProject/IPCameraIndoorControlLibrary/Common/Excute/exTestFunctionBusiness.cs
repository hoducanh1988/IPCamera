using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilityPack.Validation;
using UtilityPack.Converter;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestFunctionBusiness<U, S> where U : class, new() where S : class, new() {

        U uploadInfo;
        S settingInfo;
        public string std_value = "";

        public exTestFunctionBusiness(U _uploadInfo, S _settingInfo) {
            uploadInfo = _uploadInfo;
            settingInfo = _settingInfo;
        }

        //Nap fw thuong mai qua cong telnet
        public bool excuteTelnet() {
            bool ret = false;
            bool flag_check = false;

            var prop_rebootresult = uploadInfo.GetType().GetProperty("rebootResult");
            prop_rebootresult.SetValue(uploadInfo, "Passed");

            //get logsytem
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            Dut.IPCamera<U> camera = null;

            try {
                //login to camera
                //###############################################################//
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

                //check firmware build time
                //###############################################################//
                flag_check = (bool)settingInfo.GetType().GetProperty("IsCheckFirmwareBuildTime").GetValue(settingInfo);
                if (flag_check) {
                    var prop_fwresult = uploadInfo.GetType().GetProperty("firmwareResult");
                    prop_fwresult.SetValue(uploadInfo, "Waiting...");

                    log_value += string.Format("\n-------------------------------\n");
                    log_value += string.Format("{0}, check firmware business build time\n", DateTime.Now);
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string fw_buildtime = (string)settingInfo.GetType().GetProperty("firmwareBuildTime").GetValue(settingInfo);
                    log_value += string.Format("...tiêu chuẩn: {0}\n", fw_buildtime);
                    log_value += string.Format("...thực tế:\n");
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string act_buildtime = "";
                    ret = _checkFWBuildTime(camera, fw_buildtime, out act_buildtime);
                    log_value = (string)prop_logsystem.GetValue(uploadInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(uploadInfo, log_value);
                    prop_fwresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");

                    uploadInfo.GetType().GetProperty("firmwareBuildTime").SetValue(uploadInfo, act_buildtime);
                    if (!ret) goto END;
                }

                //check mac address
                //###############################################################//
                flag_check = (bool)settingInfo.GetType().GetProperty("IsCheckMacEthernet").GetValue(settingInfo);
                if (flag_check) {
                    var prop_macresult = uploadInfo.GetType().GetProperty("macResult");
                    prop_macresult.SetValue(uploadInfo, "Waiting...");

                    log_value += string.Format("\n-------------------------------\n");
                    log_value += string.Format("{0}, check mac ethernet\n", DateTime.Now);
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string mac_header = (string)settingInfo.GetType().GetProperty("vnptMacHeader").GetValue(settingInfo);

                    ret = _checkMacEthernet(camera, mac_header);
                    log_value = (string)prop_logsystem.GetValue(uploadInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    prop_macresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");
                    if (!ret) goto END;
                }

                //check serial number
                //###############################################################//
                flag_check = (bool)settingInfo.GetType().GetProperty("IsCheckSerialNumber").GetValue(settingInfo);
                if (flag_check) {
                    var prop_serialresult = uploadInfo.GetType().GetProperty("serialResult");
                    prop_serialresult.SetValue(uploadInfo, "Waiting...");

                    log_value += string.Format("\n-------------------------------\n");
                    log_value += string.Format("{0}, check serial number\n", DateTime.Now);
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string vnpt_productnumber = (string)settingInfo.GetType().GetProperty("vnptProductNumber").GetValue(settingInfo);
                    string vnpt_maccode = (string)settingInfo.GetType().GetProperty("productMacCode").GetValue(settingInfo);

                    string act_serial = "";
                    ret = _checkSerialNumber(camera, vnpt_productnumber, vnpt_maccode, out act_serial);
                    log_value = (string)prop_logsystem.GetValue(uploadInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(uploadInfo, log_value);
                    prop_serialresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");

                    uploadInfo.GetType().GetProperty("serialNumber").SetValue(uploadInfo, act_serial);
                    if (!ret) goto END;
                }

                //check uid
                //###############################################################//
                flag_check = (bool)settingInfo.GetType().GetProperty("IsCheckUID").GetValue(settingInfo);
                if (flag_check) {
                    var prop_uidresult = uploadInfo.GetType().GetProperty("uidResult");
                    prop_uidresult.SetValue(uploadInfo, "Waiting...");

                    log_value += string.Format("\n-------------------------------\n");
                    log_value += string.Format("{0}, check uid\n", DateTime.Now);
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string vnpt_uidheader = (string)settingInfo.GetType().GetProperty("vnptUidHeader").GetValue(settingInfo);

                    string act_uid = "";
                    ret = _checkUIDCode(camera, vnpt_uidheader, out act_uid);
                    log_value = (string)prop_logsystem.GetValue(uploadInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(uploadInfo, log_value);
                    prop_uidresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");

                    uploadInfo.GetType().GetProperty("uidCode").SetValue(uploadInfo, act_uid);
                    if (!ret) goto END;
                }

                //check hardware version
                //###############################################################//
                flag_check = (bool)settingInfo.GetType().GetProperty("IsCheckHardwareVersion").GetValue(settingInfo);
                if (flag_check) {
                    var prop_hwresult = uploadInfo.GetType().GetProperty("hardwareResult");
                    prop_hwresult.SetValue(uploadInfo, "Waiting...");

                    log_value += string.Format("\n-------------------------------\n");
                    log_value += string.Format("{0}, check hardware version\n", DateTime.Now);
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string hw_version = (string)settingInfo.GetType().GetProperty("hardwareVersion").GetValue(settingInfo);
                    log_value += string.Format("...tiêu chuẩn: {0}\n", hw_version);
                    log_value += string.Format("...thực tế:\n");
                    prop_logsystem.SetValue(uploadInfo, log_value);

                    string act_hw_ver = "";
                    ret = _checkHardwareVersion(camera, hw_version, out act_hw_ver);
                    log_value = (string)prop_logsystem.GetValue(uploadInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(uploadInfo, log_value);
                    prop_hwresult.SetValue(uploadInfo, ret ? "Passed" : "Failed");

                    uploadInfo.GetType().GetProperty("hardwareVersion").SetValue(uploadInfo, act_hw_ver);
                    if (!ret) goto END;
                }

            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(uploadInfo, log_value);
                goto END;
            }


        END:
            if (camera != null && camera.IsConnected()) camera.Close();
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

        bool _checkFWBuildTime(Dut.IPCamera<U> camera, string std_value, out string fw_build_time) {
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            bool ret = false;
            int count = 0;
            fw_build_time = "";

            RE:
            count++;
            string data = camera.getFirmwareBuildTime();
            log_value += data;
            prop_logsystem.SetValue(uploadInfo, log_value);

            if (string.IsNullOrEmpty(std_value) || string.IsNullOrWhiteSpace(std_value)) return false;
            ret = data.Contains(std_value);
            if (!ret) {
                if (count < 3) goto RE;
            }

            if (data != null && data.Length > 0) {
                fw_build_time = data.Split('\n')[2].Replace("\n", "").Replace("\r", "").Trim();
            }

            return ret;
        }

        bool _checkMacEthernet(Dut.IPCamera<U> camera, string mac_header) {

            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            bool ret = false;
            int count = 0;
        RE:
            count++;

            //đọc mac ethernet
            string mac_ethernet = camera.getMacEthernet();
            log_value += string.Format("...đọc mac ethernet: {0}\n", mac_ethernet);
            prop_logsystem.SetValue(uploadInfo, log_value);

            //check định dạng
            ret = Parse.IsMacAddress(mac_ethernet);
            log_value += string.Format("...check định dạng: {0}\n", ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

            //check header
            ret = mac_header.ToLower().Contains(mac_ethernet.ToLower().Substring(0, 6));
            log_value += string.Format("...check header: {0}\n", ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

        END:
            return ret;
        }

        bool _checkSerialNumber(Dut.IPCamera<U> camera, string vnpt_product_number, string vnpt_mac_code, out string serial_number) {
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            bool ret = false;
            int count = 0;
        RE:
            count++;

            //đọc serial number
            serial_number = camera.getSerialNumber();
            serial_number = serial_number.Replace("[", "")
                                         .Replace("]", "")
                                         .Replace("\n", "")
                                         .Replace("\r", "")
                                         .Replace("~ #", "")
                                         .Trim();

            log_value += string.Format("...đọc serial number: {0}\n", serial_number);
            prop_logsystem.SetValue(uploadInfo, log_value);

            //check header
            ret = serial_number.ToLower().Substring(0, vnpt_product_number.Length).Contains(vnpt_product_number.ToLower());
            log_value += string.Format("...check header: {0}\n", ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

            //check trùng khớp với mac
            string mac = camera.getMacEthernet();
            ret = mac.ToLower().Substring(6, 6).Equals(serial_number.ToLower().Substring(9, 6));
            log_value += string.Format("...check trùng khớp với mac {0}: {1}\n", mac, ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

            //check mac code
            ret = Parse.IsMatchingMacCode(serial_number, mac, vnpt_mac_code);
            log_value += string.Format("...check trùng khớp với mac header {0}: {1}\n", mac, ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

        END:
            return ret;
        }

        bool _checkUIDCode(Dut.IPCamera<U> camera, string vnpt_uid_header, out string uid_code) {
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            bool ret = false;
            int count = 0;
        RE:
            count++;

            //đọc uid
            uid_code = camera.getUidCode();
            uid_code = uid_code.Replace("[", "")
                               .Replace("]", "")
                               .Replace("\n", "")
                               .Replace("\r", "")
                               .Replace("~ #", "")
                               .Trim();

            log_value += string.Format("...đọc uid: {0}\n", uid_code);
            prop_logsystem.SetValue(uploadInfo, log_value);

            //check header
            ret = uid_code.ToLower().Substring(0, vnpt_uid_header.Length).Contains(vnpt_uid_header.ToLower());
            log_value += string.Format("...check header: {0}\n", ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

            //check trùng khớp với mac
            string mac = camera.getMacEthernet();
            string md5_from_mac = myConverter.stringToMD5(mac);
            ret = uid_code.ToLower().Contains(md5_from_mac.ToLower());
            log_value += string.Format("...check trùng khớp với mac md5 {0}: {1}\n", md5_from_mac, ret);
            prop_logsystem.SetValue(uploadInfo, log_value);
            if (!ret) {
                if (count < 3) goto RE;
                else goto END;
            }

        END:
            return ret;
        }

        bool _checkHardwareVersion(Dut.IPCamera<U> camera, string std_value, out string hw_version) {
            var prop_logsystem = uploadInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(uploadInfo);

            bool ret = false;
            int count = 0;

            RE:
            count++;
            string data = camera.getHardwareVersion();
            hw_version = data;
            log_value += data;
            prop_logsystem.SetValue(uploadInfo, log_value);

            if (string.IsNullOrEmpty(std_value) || string.IsNullOrWhiteSpace(std_value)) return false;
            ret = data.Contains(std_value);
            if (!ret) {
                if (count < 3) goto RE;
            }

            return ret;
        }

        #endregion


    }
}
