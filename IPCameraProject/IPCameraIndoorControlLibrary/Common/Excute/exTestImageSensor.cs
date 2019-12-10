using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exTestImageSensor<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        int retry_time = 3;
        Dut.IPCamera<T> camera;
        public string std_value = "/dev/video51  /dev/video52  /dev/video53  /dev/video54";
        UI.ucImageSensor uc_imagesensor;

        public exTestImageSensor(Dut.IPCamera<T> _camera, T _testingInfo, S _settingInfo, int _retry) {
            camera = _camera;
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
            retry_time = _retry;
        }

        //kiem tra ethernet qua cong UART
        public bool excuteUart() {
            bool ret = false;
            var prop_imagesensorresult = testingInfo.GetType().GetProperty("imageSensorResult");
            prop_imagesensorresult.SetValue(testingInfo, "Waiting...");
            try {
                if (!camera.IsConnected()) goto END;
                //get logsytem
                var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
                string log_value = (string)prop_logsystem.GetValue(testingInfo);

                int count = 0;
                RE:
                count++;
                string data = camera.getImageSensorInterface();
                log_value += data;
                prop_logsystem.SetValue(testingInfo, log_value);

                bool r = data.ToUpper().Contains(std_value.ToUpper());
                if (!r) {
                    if (count < retry_time) goto RE;
                }

                ret = r;
            }
            catch { goto END; }

            END:
            prop_imagesensorresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra ethernet qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            bool ret = false;
            var prop_imagesensorresult = testingInfo.GetType().GetProperty("imageSensorResult");
            prop_imagesensorresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            string rtsp_link = (string)settingInfo.GetType().GetProperty("cameraRtspLink").GetValue(settingInfo);
            double sharpness = (double)settingInfo.GetType().GetProperty("sharpnessStandard").GetValue(settingInfo);
            double tolerance = (double)settingInfo.GetType().GetProperty("toleranceSharpness").GetValue(settingInfo);
            string areatest = (string)settingInfo.GetType().GetProperty("areaTestChart").GetValue(settingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                //chuyen camera sang mode BGR
                camera.switchCameraMode(false);
                Application.Current.Dispatcher.Invoke(new Action(() => { uc_imagesensor = new UI.ucImageSensor(13, areatest, sharpness, tolerance, rtsp_link); }));
                Thread.Sleep(3000);

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_imagesensor);
                }));

                RE:
                ret = uc_imagesensor.imageResult != -1 || uc_imagesensor.timeOut == 0;
                if (!ret) { Thread.Sleep(100); goto RE; }
                if (uc_imagesensor.imageResult != 0) ret = false;

                //close stream
                uc_imagesensor.Dispose();

                //chuyen camera ve mode normal
                camera.switchCameraMode(false);
                Thread.Sleep(1000);

                log_value += uc_imagesensor.imageMessage;
                prop_logsystem.SetValue(testingInfo, log_value);

                goto END;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_imagesensorresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
