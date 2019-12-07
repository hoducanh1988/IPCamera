using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exTestLightSensor<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "";
        UI.ucLightSensorDarkMode uc_darkmode;
        UI.ucLightSensorLightMode uc_lightmode;

        int dark_lowerlimit;
        int dark_upperlimit;
        int light_lowerlimit;
        int light_upperlimit = -1;

        public exTestLightSensor(Dut.IPCamera<T> _camera, T _testingInfo, S _settingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;

            dark_lowerlimit = (int)settingInfo.GetType().GetProperty("lightSensorDarkLower").GetValue(settingInfo);
            dark_upperlimit = (int)settingInfo.GetType().GetProperty("lightSensorDarkUpper").GetValue(settingInfo);
            light_lowerlimit = (int)settingInfo.GetType().GetProperty("lightSensorLightLower").GetValue(settingInfo);

            std_value = string.Format("dark mode: {0} ~ {1}, light mode: {2} ~ {3}", dark_lowerlimit, dark_upperlimit, light_lowerlimit, light_upperlimit);
        }

        //kiem tra light sensor qua cong UART
        public bool excuteUart(Grid grid_container) {
            bool ret = false;
            bool ret_darkmode = false;
            bool ret_lightmode = false;

            var prop_ligthsensorresult = testingInfo.GetType().GetProperty("lightSensorResult");
            prop_ligthsensorresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                //dark mode
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_darkmode = new UI.ucLightSensorDarkMode(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_darkmode);
                }));
                RE_DARK:
                int dark_value = camera.getLightSensorValue();
                ret_darkmode = dark_value > dark_lowerlimit && dark_value < dark_upperlimit;
                if (!ret_darkmode) {
                    if (uc_darkmode.timeOut > 0) {
                        Thread.Sleep(500);
                        goto RE_DARK;
                    }
                }

                //wait
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    grid_container.Children.Clear();
                }));
                Thread.Sleep(500);

                //light mode
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_lightmode = new UI.ucLightSensorLightMode(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_lightmode);
                }));
                RE_LIGHT:
                int light_value = camera.getLightSensorValue();
                ret_lightmode = light_value > light_lowerlimit;
                if (!ret_lightmode) {
                    if (uc_lightmode.timeOut > 0) {
                        Thread.Sleep(500);
                        goto RE_LIGHT;
                    }
                }

                log_value += string.Format("...Chế độ tối: \"{0}\", \"{1}\"\n", dark_value, ret_darkmode ? "Passed" : "Failed");
                log_value += string.Format("...Chế độ sáng: \"{0}\", \"{1}\"\n", light_value, ret_lightmode ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);

                ret = ret_darkmode && ret_lightmode;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_ligthsensorresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra light sensor qua cong telnet
        public bool excuteTelnet() {
            try {


                return true;
            }
            catch { return false; }
        }



    }
}
