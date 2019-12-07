using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestIRLed<T> where T : class, new() {

        T testingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "IR LED sáng màu đỏ";
        UI.ucIRLed uc_irled;

        public exTestIRLed(Dut.IPCamera<T> _camera, T _testingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
        }

        //kiem tra ir led qua cong UART
        public bool excuteUart(Grid grid_container) {
            throw new Exception();
        }

        //Kiem tra ir led qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            bool ret = false;

            var prop_irledresult = testingInfo.GetType().GetProperty("irLedResult");
            prop_irledresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;
                camera.initRGBLedControl();

                //led red
                camera.turnIRLedOn();
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_irled = new UI.ucIRLed(30);
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_irled);
                }));
                RE:
                ret = uc_irled.ledResult != -1;
                if (!ret) {
                    if (uc_irled.timeOut > 0) {
                        Thread.Sleep(100);
                        goto RE;
                    }
                }
                ret = uc_irled.ledResult == 0;
                camera.turnIRLedOff();

                log_value += string.Format("...IR LED: \"{0}\"\n", ret ? "sáng màu đỏ" : "không sáng");
                prop_logsystem.SetValue(testingInfo, log_value);
                
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_irledresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

    }
}
