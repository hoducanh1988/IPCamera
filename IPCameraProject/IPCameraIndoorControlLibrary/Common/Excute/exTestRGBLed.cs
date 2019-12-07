using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exTestRGBLed<T> where T : class, new() {

        T testingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "RGB LED sáng cả 2 màu xanh và đỏ";
        UI.ucRGBLedRed uc_ledred;
        UI.ucRGBLedGreen uc_ledgreen;

        public exTestRGBLed(Dut.IPCamera<T> _camera, T _testingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
        }

        //kiem tra rgb led qua cong UART
        public bool excuteUart(Grid grid_container) {
            bool ret = false;
            bool ret_ledred = false;
            bool ret_ledgreen = false;

            var prop_rgbledresult = testingInfo.GetType().GetProperty("rgbLedResult");
            prop_rgbledresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;
                camera.initRGBLedControl();

                //led red
                camera.turnRGBLedRedOn();
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_ledred = new UI.ucRGBLedRed(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_ledred);
                }));
                RE_RED:
                ret_ledred = uc_ledred.ledResult == 0 || uc_ledred.ledResult == 1 || uc_ledred.timeOut == 0;
                if (!ret_ledred) { Thread.Sleep(100); goto RE_RED; }
                if (uc_ledred.ledResult != 0) ret_ledred = false;
                camera.turnRGBLedRedOff();

                //wait
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    grid_container.Children.Clear();
                }));
                Thread.Sleep(500);

                //led green
                camera.turnRGBLedGreenOn();
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_ledgreen = new UI.ucRGBLedGreen(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_ledgreen);
                }));
                RE_GREEN:
                ret_ledgreen = uc_ledgreen.ledResult == 0 || uc_ledgreen.ledResult == 1 || uc_ledgreen.timeOut == 0;
                if (!ret_ledgreen) { Thread.Sleep(100); goto RE_GREEN; }
                if (uc_ledgreen.ledResult != 0) ret_ledgreen = false;
                camera.turnRGBLedGreenOff();

                log_value += string.Format("...RGB LED đỏ: \"{0}\"\n", ret_ledred ? "sáng" : "không sáng");
                log_value += string.Format("...RGB LED xanh: \"{0}\"\n", ret_ledgreen ? "sáng" : "không sáng");
                prop_logsystem.SetValue(testingInfo, log_value);

                ret = ret_ledred && ret_ledgreen;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_rgbledresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra rgb led qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            try {
                return excuteUart(grid_container);
            }
            catch { return false; }
        }



    }
}
