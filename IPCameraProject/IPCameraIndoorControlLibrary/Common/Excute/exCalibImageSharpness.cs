using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using IPCameraIndoorControlLibrary.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPCameraIndoorControlLibrary.Common.Excute {
    public class exCalibImageSharpness<T, S> where T : class, new() where S : class, new() {

        T testingInfo;
        S settingInfo;
        Dut.IPCamera<T> camera;
        VideoCapture capture;
        bool isStreaming = false;

        public exCalibImageSharpness(T _testingInfo, S _settingInfo) {
            testingInfo = _testingInfo;
            settingInfo = _settingInfo;
        }


        public bool excuteUart() {
            return false;
        }

        public bool excuteTelnet() {
            bool ret = false;

            //get setting
            string rtsp_link = (string)settingInfo.GetType().GetProperty("cameraRtspLink").GetValue(settingInfo);
            double std_sharpness = (double)settingInfo.GetType().GetProperty("sharpnessStandard").GetValue(settingInfo);
            double std_tolerance = (double)settingInfo.GetType().GetProperty("toleranceSharpness").GetValue(settingInfo);
            string areatest = (string)settingInfo.GetType().GetProperty("areaTestChart").GetValue(settingInfo);
            double scale = 0;

            //get testing
            var prop_macethernet = testingInfo.GetType().GetProperty("macEthernet");
            var prop_totalresult = testingInfo.GetType().GetProperty("TotalResult");
            var prop_imagesource = testingInfo.GetType().GetProperty("imageSource");
            var prop_imagecrop = testingInfo.GetType().GetProperty("imageCrop");
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");

            string log_value = (string)prop_logsystem.GetValue(testingInfo);

            try {
                //login to camera
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Login to ip camera\n", DateTime.Now);
                prop_logsystem.SetValue(testingInfo, log_value);

                string camera_ip = (string)settingInfo.GetType().GetProperty("cameraIP").GetValue(settingInfo);
                string telnet_user = (string)settingInfo.GetType().GetProperty("cameraTelnetUser").GetValue(settingInfo);
                string telnet_pass = (string)settingInfo.GetType().GetProperty("cameraTelnetPassword").GetValue(settingInfo);

                ret = _loginToCamera(camera_ip, telnet_user, telnet_pass, ref camera);
                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //get mac ethernet
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Đọc địa chỉ mac ethernet\n", DateTime.Now);
                prop_logsystem.SetValue(testingInfo, log_value);

                string mac_ethernet = "";
                ret = _getMacEthernet(camera, out mac_ethernet);
                prop_macethernet.SetValue(testingInfo, mac_ethernet);

                log_value = (string)prop_logsystem.GetValue(testingInfo);
                log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);
                if (!ret) goto END;

                //stream video and judgement
                log_value += string.Format("\n-------------------------------\n");
                log_value += string.Format("{0}, Stream và phán định độ nét hình ảnh camera\n", DateTime.Now);
                log_value += string.Format("...Tiêu chuẩn: > {0}\n", std_sharpness - std_tolerance);
                prop_logsystem.SetValue(testingInfo, log_value);

                capture = new VideoCapture(rtsp_link);
                if (!capture.IsOpened) {
                    ret = false;
                    log_value = (string)prop_logsystem.GetValue(testingInfo);
                    log_value += string.Format("...kết quả {0}\n", ret ? "Passed" : "Failed");
                    prop_logsystem.SetValue(testingInfo, log_value);
                    goto END;
                }

                isStreaming = true;
                int count = 0;
            RE:
                Mat m = new Mat();
                if (capture != null) capture.Read(m);
                if (!m.IsEmpty) {
                    
                    Image<Bgr, Byte> img = m.ToImage<Bgr, Byte>();
                    int iw = img.Width;
                    int ih = img.Height;

                    string[] buffer = areatest.Split(',');
                    int rect_left = (int)double.Parse(buffer[0]);
                    int rect_top = (int)double.Parse(buffer[1]);
                    int rect_width = (int)double.Parse(buffer[2]);
                    int rect_height = (int)double.Parse(buffer[3]);
                    var rect = new System.Drawing.Rectangle(rect_left, rect_top, rect_width, rect_height);

                    //show rectangle
                    CvInvoke.Rectangle(m, rect, new MCvScalar(0, 0, 255), 3, LineType.AntiAlias);

                    //crop image by rectangle
                    Image<Gray, byte> image_crop = globalUtility.CropFromImage(img, rect);

                    //calculate sharpness
                    int s = globalUtility.getSharpnessValueFromImage(image_crop);
                    int pixel = image_crop.Width * image_crop.Height;
                    scale = s / (pixel * 1.0);

                    //judgement
                    double ll_value = std_sharpness - std_tolerance;
                    ret = scale >= ll_value;
                    prop_totalresult.SetValue(testingInfo, ret ? "Passed" : "Failed");

                    count++;

                    //thoi gian
                    CvInvoke.PutText(m,
                                     string.Format("Now is {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")), 
                                     new System.Drawing.Point(30, 50), 
                                     FontFace.HersheySimplex, 
                                     1, 
                                     ret ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255) ,
                                     2, 
                                     LineType.AntiAlias);

                    //do net tieu chuan
                    CvInvoke.PutText(m,
                                     string.Format("Standard value: {0}", ll_value),
                                     new System.Drawing.Point(30, 100), 
                                     FontFace.HersheySimplex,
                                     1,
                                     ret ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255),
                                     2,
                                     LineType.AntiAlias);

                    //do net thuc te
                    CvInvoke.PutText(m,
                                     string.Format("Actual value: {0}", Math.Round(scale, 5)),
                                     new System.Drawing.Point(30, 150),
                                     FontFace.HersheySimplex,
                                     1,
                                     ret ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255),
                                     2,
                                     LineType.AntiAlias);

                    //ket qua
                    CvInvoke.PutText(m,
                                     string.Format("{0}", ret ? "Passed" : "Failed"),
                                     new System.Drawing.Point(30, 350),
                                     FontFace.HersheySimplex,
                                     4,
                                     ret ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255),
                                     2,
                                     LineType.AntiAlias);

                    //mac address
                    CvInvoke.PutText(m,
                                     string.Format("{0}:{1}:{2}:{3}:{4}:{5}", 
                                                    mac_ethernet.Substring(0, 2),
                                                    mac_ethernet.Substring(2, 2),
                                                    mac_ethernet.Substring(4, 2),
                                                    mac_ethernet.Substring(6, 2),
                                                    mac_ethernet.Substring(8, 2),
                                                    mac_ethernet.Substring(10, 2)
                                                    ),
                                     new System.Drawing.Point(30, 650),
                                     FontFace.HersheySimplex,
                                     2,
                                     new MCvScalar(0, 0, 255),
                                     2,
                                     LineType.AntiAlias);


                    //show image
                    var bi = globalUtility.ToBitmapSource(m.Bitmap);
                    bi.Freeze();
                    prop_imagesource.SetValue(testingInfo, bi);

                    //show crop
                    var ci = globalUtility.Bitmap2BitmapImage(image_crop.Bitmap);
                    ci.Freeze();
                    prop_imagecrop.SetValue(testingInfo, ci);

                }

                Thread.Sleep(10);
                if (isStreaming) goto RE;

                log_value += string.Format("...kết quả: {0}, {1}\n", scale, ret ? "Passed" : "Failed");
                prop_logsystem.SetValue(testingInfo, log_value);

            }
            catch (Exception ex) {
                log_value += ex.ToString();
                prop_logsystem.SetValue(testingInfo, log_value);
                goto END;
            }

        END:
            if (capture != null) capture.Dispose();
            if (camera != null && camera.IsConnected()) camera.Close();
            return ret;
        }

        public bool Dispose() {
            isStreaming = false;
            return true;
        }

        ~exCalibImageSharpness() {
            Dispose();
        }

        #region sub-function

        bool _loginToCamera(string ip, string user, string pass, ref Dut.IPCamera<T> camera) {
            bool ret = false;
            int count = 0;
        RE:
            count++;
            camera = new Dut.IPCamera<T>(testingInfo, ip, user, pass);
            camera.Login();
            ret = camera.IsConnected();
            if (!ret) {
                if (count < 3) goto RE;
            }
            return ret;
        }

        bool _getMacEthernet(Dut.IPCamera<T> camera, out string mac_ethernet) {
            var prop_logsystem = testingInfo.GetType().GetProperty("logSystem");
            string log_value = (string)prop_logsystem.GetValue(testingInfo);
            bool ret = false;
            int count = 0;
        RE:
            count++;
            mac_ethernet = camera.getMacEthernet();
            log_value += string.Format("...đọc mac ethernet {0}\n", mac_ethernet);
            prop_logsystem.SetValue(testingInfo, log_value);

            ret = !(string.IsNullOrEmpty(mac_ethernet) || string.IsNullOrWhiteSpace(mac_ethernet));
            if (!ret) {
                if (count < 3) goto RE;
            }
            return ret;
        }

        #endregion

    }
}
