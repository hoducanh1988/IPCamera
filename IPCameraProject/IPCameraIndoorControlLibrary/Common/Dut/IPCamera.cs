using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IPCameraIndoorControlLibrary.Common.Dut {

    public class IPCamera<T> where T : class, new() {

        Protocol.IProtocol camera;
        string telnet_user = "root";
        string telnet_pass = "";

        //telnet
        public IPCamera(T t, string ip, string _telnet_user, string _telnet_pass) {
            camera = new Protocol.Telnet<T>(t, ip, 23);
            telnet_user = _telnet_user;
            telnet_pass = _telnet_pass;
        }

        //uart
        public IPCamera(T t, string port_name, int baud_rate, int data_bit) {
            camera = new Protocol.Rs232<T>(t, port_name, baud_rate, data_bit);
        }

        //check connection status
        public bool IsConnected() {
            return camera.IsConnected();
        }

        //open connect to camera
        public bool Open() {
            if (camera.IsConnected() == false) {
                camera.Open();
                if (camera.IsConnected() == false) return false;
            }
            return true;
        }

        //close connect camere
        public bool Close() {
            return camera.Close();
        }

        //reboot camera from uboot
        public bool rebootViaUboot() {
            try {
                bool r = false;
                string cmd = string.Format("reset");
                camera.WriteLine(cmd);
                Thread.Sleep(3000);
                r = camera.Read().Contains("set watchdog, resetting...");
                return r;

            } catch { return false; }
        }

        //reboot camera from firmware
        public bool rebootViaFirmware() {
            try {
                bool r = false;
                string cmd = string.Format("reboot");
                camera.WriteLine(cmd);
                Thread.Sleep(1000);
                return r;
            }
            catch { return false; }
        }

        //login to camera
        public bool Login() {
            try {
                if (camera.IsConnected() == false) {
                    camera.Open();
                    if (camera.IsConnected() == false) return false;
                }

                if (camera is Protocol.Telnet<T>) {
                    string data = camera.Query("\n");

                    if (data.Contains("~ #")) return true;
                    else if (data.Contains("login:")) {
                        string s = camera.Query(telnet_user);
                        if (s.Contains("~ #")) return true;
                        else if (s.ToLower().Contains("password:")) {
                            return camera.Query(telnet_pass).Contains("~ #");
                        }
                        else return false;
                    }
                    else return false;
                }
                else return camera.Query("\n").Contains("~ #");
            }
            catch { return false; }
        }

        //login to uboot
        public bool loginToUboot() {
            try {
                return camera.Query("\n").Contains("rlxboot#");
            }
            catch { return false; }
        }

        //get mac ethernet
        public string getMacEthernet() {
            try {
                string data = camera.Query("cat /sys/class/net/eth0/address");
                string mac_ethernet = data.Replace("cat /sys/class/net/eth0/address", "")
                                          .Replace("~ #", "")
                                          .Replace("\r", "")
                                          .Replace("\n", "")
                                          .Replace(":", "")
                                          .ToUpper()
                                          .Trim();
                return mac_ethernet;
            }
            catch { return null; }
        }

        //set mac ethernet via uboot
        public bool setMacEthernetViaUboot(string mac) {
            try {
                bool r = false;
                string tmp = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                           mac.Substring(0, 2),
                                           mac.Substring(2, 2),
                                           mac.Substring(4, 2),
                                           mac.Substring(6, 2),
                                           mac.Substring(8, 2),
                                           mac.Substring(10, 2));

                string cmd = string.Format("setethaddr {0}", tmp);
                camera.WriteLine(cmd);
                Thread.Sleep(500);
                r = camera.Read().Contains("rlxboot#");
                if (!r) return false;

                cmd = "printenv ethaddr";
                camera.WriteLine(cmd);
                Thread.Sleep(500);
                r = camera.Read().ToLower().Contains(tmp.ToLower());
                if (!r) return false;

                cmd = "saveenv";
                camera.WriteLine(cmd);
                Thread.Sleep(1000);
                r = camera.Read().Contains("rlxboot#");

                return r;
            }
            catch { return false; }
        }

        //thiết lập địa chỉ ip qua uboot
        public bool setIPUboot(string ip, string getway, string subnet_mask) {
            try {
                bool r = false;
                string cmd = "";

                //set ip
                cmd = string.Format("setipaddr {0}", ip);
                camera.WriteLine(cmd);
                Thread.Sleep(500);
                r = camera.Read().Contains("rlxboot#");
                if (!r) return false;

                //set getway
                cmd = string.Format("setenv gateway {0}", getway);
                camera.WriteLine(cmd);
                Thread.Sleep(500);
                r = camera.Read().Contains("rlxboot#");
                if (!r) return false;

                //set subnet mask
                cmd = string.Format("setenv netmask {0}", subnet_mask);
                camera.WriteLine(cmd);
                Thread.Sleep(500);
                r = camera.Read().Contains("rlxboot#");
                if (!r) return false;

                //save and verify
                cmd = string.Format("saveenv");
                camera.WriteLine(cmd);
                Thread.Sleep(1000);
                string data = camera.Read();
                r = data.Contains(ip) && data.Contains(getway) && data.Contains(subnet_mask);

                return r;
            } catch { return false; }
        }

        //gửi lệnh chờ upload firmware
        public bool uploadFirmwareBasic() {
            try {
                bool r = false;
              
                string cmd = "update all";
                camera.WriteLine(cmd);
                Thread.Sleep(1000);
                r = camera.Read().Contains("Loading");

                return r;
            } catch { return false; }
        }

        //gửi lệnh upload firmware thương mại
        public bool uploadFirmwareBusiness(string cmd) {
            try {
                bool r = false;

                camera.WriteLine(cmd);
                Thread.Sleep(1000);
                string data = camera.Read();
                r = data.Contains("Connecting to") && (!data.Contains("can't connect"));
                return r;
            }
            catch { return false; }
        }

        //get mac wlan
        public string getMacWlan() {
            try {
                string data = camera.Query("ifconfig | grep wlan0");
                string mac_wlan = data.Split(new string[] { "HWaddr" }, StringSplitOptions.None)[1]
                                      .Replace("~ #", "")
                                      .Replace("\r", "")
                                      .Replace("\n", "")
                                      .Replace(":", "")
                                      .ToUpper()
                                      .Trim();
                return mac_wlan;
            }
            catch { return null; }
        }

        //get uid code
        public string getUidCode() {
            try {
                string data = camera.Query("cat /usr/conf/uuid.txt");
                string uid_code = data.Replace("cat /usr/conf/uuid.txt", "")
                                      .Replace("~ #", "")
                                      .Replace("\r", "")
                                      .Replace("\n", "")
                                      .Replace(":", "")
                                      .ToUpper()
                                      .Trim();

                return uid_code;
            }
            catch { return null; }
        }

        //set uid code
        public bool setUIdCode(string uid_code) {
            try {
                string data = camera.Query(string.Format("echo {0} > /usr/conf/uuid.txt", uid_code));
                return data.Contains("~ #");
            }
            catch { return false; }
        }

        //get serial number
        public string getSerialNumber() {
            try {
                string data = camera.Query("cat /usr/conf/serial.txt");
                string serial_number = data.Replace("cat /usr/conf/serial.txt", "")
                                           .Replace("~ #", "")
                                           .Replace("\r", "")
                                           .Replace("\n", "")
                                           .Replace(":", "")
                                           .ToUpper()
                                           .Trim();

                return serial_number;
            }
            catch { return null; }
        }

        //set serial number
        public bool setSerialNumber(string serial_number) {
            try {
                string data = camera.Query(string.Format("echo {0} > /usr/conf/serial.txt", serial_number));
                return data.Contains("~ #");
            }
            catch { return false; }
        }

        //set static ip
        public bool setStaticIP(string static_ip) {
            try {
                camera.WriteLine("\n");
                Thread.Sleep(1000);

                camera.WriteLine(string.Format("nm_cfg wan dhcp 0 ipaddr {0} netmask 255.255.255.0 gateway 192.168.1.1", static_ip));
                Thread.Sleep(1000);
                return camera.Read().Contains("~ #");

            }
            catch { return false; }
        }

        //get firmware build time
        public string getFirmwareBuildTime() {
            try {
                string data = camera.Query("cat /etc/version");
                string fw_buildtime = data;
                return fw_buildtime;
            }
            catch { return null; }
        }

        //get firmware version
        public string getFirmwareVersion() {
            try {
                string data = camera.Query("cat /etc/version");
                string fw_version = data;
                return fw_version;
            }
            catch { return null; }
        }

        //get hardware version
        public string getHardwareVersion() {
            try {
                string data = camera.Query("cat /usr/conf/hwversion.txt");
                string hw_version = data.Replace("cat /usr/conf/hwversion.txt", "")
                                        .Replace("~ #", "")
                                        .Replace("\r", "")
                                        .Replace("\n", "")
                                        .Replace(":", "")
                                        .ToUpper()
                                        .Trim();
                return hw_version;
            }
            catch { return null; }
        }

        //set hardware version
        public bool setHardwareVersion(string hardware_version) {
            try {
                string data = camera.Query(string.Format("echo {0} > /usr/conf/hwversion.txt", hardware_version));
                return data.Contains("~ #");
            }
            catch { return false; }
        }

        //get sdk version
        public string getSDKVersion() {
            try {
                string data = camera.Query("cat /etc/version");
                string sdk_version = data;
                return sdk_version;
            }
            catch { return null; }
        }

        //get manufacture
        public string getManufacture() {
            try {
                string data = camera.Query("cat /etc/version");
                string menufacture = data;
                return menufacture;
            }
            catch { return null; }
        }

        //serialize all wifi ssid
        public string serializeWifiSSID(int delay_sec) {
            try {
                camera.WriteLine("iwlist wlan0 scan | grep ESSID | awk -F \":\" '{print$2}'");
                Thread.Sleep(delay_sec * 1000);
                string data = camera.Read();
                return data;
            }
            catch { return null; }
        }

        //get wlan interface
        public string getWlanInterface() {
            return camera.Query("ifconfig | grep wlan0");
        }

        //get image sensor interface
        public string getImageSensorInterface() {
            return camera.Query("ls /dev/video*");
        }

        //capture log
        public string captureLog() {
            return camera.Read();
        }

        //mount sd card
        public string mountSdCard() {
            return camera.Query("mount");
        }

        //get ethernet state
        public string getEthernetState() {
            return camera.Query("cat /sys/devices/platform/rts3901-r8168/net/eth0/operstate");
        }

        //init control rgb led
        public bool initRGBLedControl() {
            //kill last control
            camera.WriteLine("killall controlprocess");
            //request control
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm1/request");
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm2/request");
            //set period
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm1/period_ns");
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm2/period_ns");
            return true;
        }

        //turn rgb led red on
        public bool turnRGBLedRedOn() {
            camera.WriteLine("echo 0 > /sys/devices/platform/pwm_platform/settings/pwm1/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm1/enable");
            Thread.Sleep(50);
            return true;
        }

        //turn rgb led red off
        public bool turnRGBLedRedOff() {
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm1/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm1/enable");
            Thread.Sleep(50);
            return true;
        }

        //turn rgb led green on
        public bool turnRGBLedGreenOn() {
            camera.WriteLine("echo 0 > /sys/devices/platform/pwm_platform/settings/pwm2/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm2/enable");
            Thread.Sleep(50);
            return true;
        }

        //turn rgb led green off
        public bool turnRGBLedGreenOff() {
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm2/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm2/enable");
            Thread.Sleep(50);
            return true;
        }

        //turn ir led on
        public bool turnIRLedOn() {
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm3/request");
            Thread.Sleep(50);
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm3/period_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1000000 > /sys/devices/platform/pwm_platform/settings/pwm3/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm3/enable");
            Thread.Sleep(50);
            return true;
        }

        //turn ir led off
        public bool turnIRLedOff() {
            camera.WriteLine("echo 0 > /sys/devices/platform/pwm_platform/settings/pwm3/duty_ns");
            Thread.Sleep(50);
            camera.WriteLine("echo 1 > /sys/devices/platform/pwm_platform/settings/pwm3/enable");
            Thread.Sleep(50);
            return true;
        }

        //get light sensor adc value
        public int getLightSensorValue() {
            string data = camera.Query("cat /sys/devices/platform/rts_saradc.0/in0_input");
            data = data.Replace("cat /sys/devices/platform/rts_saradc.0/in0_input", "")
                       .Replace("~ #", "")
                       .Replace("\n", "")
                       .Replace("\r", "")
                       .Trim();

            int adc_value;
            bool r = int.TryParse(data, out adc_value);
            return r ? adc_value : -1;
        }

        //capture audio from mic to file
        public bool captureAudio() {
            try {
                camera.WriteLine("killall lark");
                Thread.Sleep(50);
                camera.WriteLine("amixer -c 1 sset Master playback 127 capture 87");
                Thread.Sleep(50);
                camera.WriteLine("arecord -D hw:1,1 /tmp/audio_record.wav &");
                Thread.Sleep(50);
            }
            catch { return false; }
            return true;
        }

        //stop capture
        public bool stopCaptureAudio() {
            try {
                camera.WriteLine("killall arecord");
                Thread.Sleep(100);
            }
            catch { return false; }
            return true;
        }

        //stop capture audio
        public bool stopPlayBack() {
            try {
                camera.WriteCtrlBreak();
                Thread.Sleep(100);
                camera.WriteLine("killall lark");
                Thread.Sleep(50);
                camera.WriteLine("killall peacock");
                Thread.Sleep(50);
                camera.WriteLine("killall rtspd");
                Thread.Sleep(50);
                camera.WriteLine("peacock -p profile1 -v 24 &");
                Thread.Sleep(100);
                camera.WriteLine("lark -c /usr/conf/peacock.json -v 24 &");
                Thread.Sleep(100);
                camera.WriteLine("rtspd -c /usr/conf/peacock.json -v 32 &");
                Thread.Sleep(100);
            }
            catch { return false; }
            return true;
        }

        //play audio file to speaker
        public bool playBackAudio() {
            try {
                camera.WriteLine("aplay  -D hw:1,1 /tmp/audio_record.wav");
                Thread.Sleep(50);
            }
            catch { return false; }
            return true;
        }

        //switch camera to mode
        public bool switchCameraMode(bool isNightVision) {
            return isNightVision ? camera.WriteLine("nightvision on") : camera.WriteLine("nightvision off");
        }

        //init capture log from button pressed
        public bool initCaptureLogFromButton() {
            camera.WriteLine("userwps");
            Thread.Sleep(50);
            return true;
        }

        //change ip without save
        public bool changeIPWithoutSave(string ip) {
            try {
                camera.WriteLine(string.Format("ifconfig br0 {0} netmask 255.255.255.0", ip));
                Thread.Sleep(100);
            }
            catch { return false; }
            return true;
        }

        //calib nightvision mode dark
        public bool calibNightVisionModeDark() {
            camera.WriteLine("calibnv dark");
            Thread.Sleep(3000);
            return camera.Read().Contains("~ #");
        }

        //calib night vision mode light
        public bool calibNightVisionModeLight() {
            camera.WriteLine("calibnv light");
            Thread.Sleep(3000);
            return camera.Read().Contains("~ #");
        }

        //đọc kết quả calib nightvision
        public string getCalibNightVisionValue() {
            return camera.Query("cat /usr/conf/nightvision.cfg");
        }
    

    }
}
