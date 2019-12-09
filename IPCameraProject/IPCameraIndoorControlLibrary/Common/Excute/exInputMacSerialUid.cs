using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exInputMacSerialUid<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        public string std_value = "Mac 12 ký tự 0-9A-F\nSerial có 6 kí tự cuối trùng khớp với MAC\nUID trùng khớp với MAC.";
        UI.ucInputMacSerialUid uc_inputmac;

        public exInputMacSerialUid(T _testingInfo, S _settingInfo) {
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
        }


        //kiem tra barcode qua cong UART
        public bool excuteUart() {
            return false;
        }


        //Kiem tra barcode qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            bool ret = false;

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            string pdnumber = (string) settingInfo.GetType().GetProperty("vnptProductNumber").GetValue(settingInfo);
            string uidheader = (string) settingInfo.GetType().GetProperty("vnptUidHeader").GetValue(settingInfo);

            try {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_inputmac = new UI.ucInputMacSerialUid(pdnumber, uidheader);
                    uc_inputmac.DataContext = testingInfo;
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_inputmac);
                }));

                RE:
                ret = uc_inputmac.inputResult != -1;
                if (!ret) {
                    Thread.Sleep(100);
                    goto RE;
                }

                log_value += string.Format("mac address: {0}\n", testingInfo.GetType().GetProperty("macFromBarcode").GetValue(testingInfo));
                log_value += string.Format("serial number: {0}\n", testingInfo.GetType().GetProperty("serialFromBarcode").GetValue(testingInfo));
                log_value += string.Format("uid: {0}\n", testingInfo.GetType().GetProperty("uidFromBarcode").GetValue(testingInfo));
                prop_logsystem.SetValue(testingInfo, log_value);

                ret = uc_inputmac.inputResult == 0;
                goto END;
            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            return ret;
        }

    }
}
