using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestButton<T> where T : class, new() {

        T testingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "button is pressed";
        UI.ucButton uc_button;

        public exTestButton(Dut.IPCamera<T> _camera, T _testingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
        }

        //kiem tra sd card qua cong UART
        public bool excuteUart(Grid grid_container) {
            bool ret = false;

            var prop_buttonresult = testingInfo.GetType().GetProperty("buttonResult");
            prop_buttonresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;
                Thread.Sleep(500);

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_button = new UI.ucButton(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_button);
                }));
                RE:
                string data = camera.captureLog();
                log_value += data;
                prop_logsystem.SetValue(testingInfo, log_value);

                if (data != null) ret = data.ToUpper().Contains(std_value.ToUpper());
                if (!ret) {
                    if (uc_button.timeOut > 0) {
                        Thread.Sleep(500);
                        goto RE;
                    }
                }
                goto END;

            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_buttonresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra sd card qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            camera.initCaptureLogFromButton();
            return excuteUart(grid_container);
        }

    }
}
