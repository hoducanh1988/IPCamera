using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exTestNightVision<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "";
        UI.ucNightVision uc_nightvision;

        public exTestNightVision(Dut.IPCamera<T> _camera, T _testingInfo, S _settingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
        }

        //kiem tra night vision qua cong UART
        public bool excuteUart() {
            return false;
        }

        //Kiem tra night vision qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            bool ret = false;
            var prop_nightvisionresult = testingInfo.GetType().GetProperty("nightVisionResult");
            prop_nightvisionresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            string rtsp_link = (string)settingInfo.GetType().GetProperty("cameraRtspLink").GetValue(settingInfo);
            int rgb_diffvalue = (int)settingInfo.GetType().GetProperty("toleranceRGBNightVision").GetValue(settingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                //chuyen camera sang mode night vision
                camera.switchCameraMode(true);
                Application.Current.Dispatcher.Invoke(new Action(() => { uc_nightvision = new UI.ucNightVision(13, rgb_diffvalue, rtsp_link); }));
                Thread.Sleep(3000);

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_nightvision);

                }));
                Thread.Sleep(1000);

                RE:
                ret = uc_nightvision.nightResult != -1 || uc_nightvision.timeOut == 0;
                if (!ret) { Thread.Sleep(100); goto RE; }
                if (uc_nightvision.nightResult != 0) ret = false;

                //close stream
                uc_nightvision.Dispose();

                //chuyen camera ve mode normal
                camera.switchCameraMode(false);
                Thread.Sleep(1000);

                log_value += uc_nightvision.nightMessage;
                prop_logsystem.SetValue(testingInfo, log_value);

                goto END;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_nightvisionresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
