using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCameraIndoorControlLibrary.Common.Excute {

    public class exTestAudio<T> where T : class, new() {

        T testingInfo;
        Dut.IPCamera<T> camera;
        public string std_value = " Loa trên thiết bị phát âm thanh to, rõ ràng, không bị rè. Microphone thu được âm thanh trong, rõ nét.";
        UI.ucAudio uc_audio;

        public exTestAudio(Dut.IPCamera<T> _camera, T _testingInfo) {
            camera = _camera;
            testingInfo = _testingInfo;
        }

        //kiem tra sd card qua cong UART
        public bool excuteUart(Grid grid_container) {
            bool ret = false;

            var prop_audioresult = testingInfo.GetType().GetProperty("audioResult");
            prop_audioresult.SetValue(testingInfo, "Waiting...");

            //get logsytem
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                if (!camera.IsConnected()) goto END;

                //set camera record audio
                log_value += "...thiết lập camera thu âm thanh từ mic\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.captureAudio();

                //play audio
                log_value += "...phát file âm thanh sound.wav ra loa máy tính.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                IO.Speaker speaker = new IO.Speaker(string.Format("{0}sound.wav", AppDomain.CurrentDomain.BaseDirectory));
                speaker.Play();

                RE_WAIT:
                bool r = speaker.IsPlaying();
                if (r) { Thread.Sleep(100); goto RE_WAIT; }

                //stop record
                log_value += "...thiết lập camera dừng thu âm.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.stopCaptureAudio();

                //play sound
                log_value += "...thiết lập camera phát file âm thanh vừa thu ra loa.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.playBackAudio();

                //show form confirm
                log_value += "...hiển thị giao diện xác nhận trạng thái khối audio.\n";
                prop_logsystem.SetValue(testingInfo, log_value);

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    uc_audio = new UI.ucAudio();
                    grid_container.Children.Clear();
                    grid_container.Children.Add(uc_audio);
                }));

                RE:
                ret = uc_audio.audioResult != -1;
                if (!ret) { Thread.Sleep(100); goto RE; }
                if (uc_audio.audioResult != 0) ret = false;

                //phán định
                log_value += string.Format("...\n");
                log_value += string.Format("... ===> \"{0}\" <===\n", uc_audio.audioMessage);
                log_value += string.Format("...\n");
                prop_logsystem.SetValue(testingInfo, log_value);


                //stop play back
                log_value += "...thiết lập camera dừng phát âm thanh.\n";
                prop_logsystem.SetValue(testingInfo, log_value);
                camera.stopPlayBack();

                goto END;

            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

            END:
            prop_audioresult.SetValue(testingInfo, ret ? "Passed" : "Failed");
            return ret;
        }

        //Kiem tra sd card qua cong telnet
        public bool excuteTelnet(Grid grid_container) {
            return excuteUart(grid_container);
        }

    }
}
