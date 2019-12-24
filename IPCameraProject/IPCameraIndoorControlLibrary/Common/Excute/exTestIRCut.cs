using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestIRCut<T> where T : class, new() {

        T testingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = "LED giả lập IR Cut nháy sáng 3 lần";
        UI.ucIRCut uc_ircut;

        public exTestIRCut(Dut.IPCamera<T> _camera, T _testingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
        }

        //kiem tra ir cut qua cong UART
        public bool excuteUart(Grid grid_container) {
            bool ret = false;

            var prop_ircutresult = testingInfo.GetType().GetProperty("irCutResult");
            prop_ircutresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                Thread.Sleep(500);

                //show form confirm
                log_value += "...hiển thị giao diện xác nhận trạng thái khối ir cut.\n";
                prop_logsystem.SetValue(testingInfo, log_value);

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_ircut = new UI.ucIRCut();
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_ircut);
                }));


                //play ir cut
                log_value += "...thiết lập camera giả lập ir cut.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.virtualIRCut();

            RE:
                if (uc_ircut.isTryAgain) { camera.virtualIRCut(); uc_ircut.isTryAgain = false; }

                ret = uc_ircut.ircutResult != -1;
                if (!ret) { Thread.Sleep(100); goto RE; }
                if (uc_ircut.ircutResult != 0) ret = false;

                //phán định
                log_value += string.Format("...\n");
                log_value += string.Format("... ===> \"{0}\" <===\n", uc_ircut.ircutMessage);
                log_value += string.Format("...\n");
                prop_logsystem.SetValue(testingInfo, log_value);

                //stop ir cut
                log_value += "...thiết lập camera dừng giả lập ir cut.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.switchCameraMode(false);

                goto END;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

        END:
            prop_ircutresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra ir cut qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            return excuteUart(grid_container);
        }

    }
}
