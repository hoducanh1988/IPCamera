using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using IPCameraIndoorControlLibrary.Common.Base;

namespace IPCameraIndoorControlLibrary.Common.UI {
    /// <summary>
    /// Interaction logic for ucNightVision.xaml
    /// </summary>
    public partial class ucNightVision : UserControl {

        class NightVisionInfo : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            public NightVisionInfo() {
                imageSource = null;
            }


            ImageSource _image_source;
            public ImageSource imageSource {
                get { return _image_source; }
                set {
                    _image_source = value;
                    OnPropertyChanged("imageSource");
                }
            }
        }

        NightVisionInfo nightInfo = new NightVisionInfo();

        public int nightResult = -1;
        public int timeOut = -1;
        int diffValue = 5;
        int count = 0;
        string rtsp_link = "";
        public string nightMessage = "";
        

        public ucNightVision(int _timeout, int _diff_value, string _rtsp_link) {
            InitializeComponent();
            this.DataContext = nightInfo;

            timeOut = _timeout;
            diffValue = _diff_value;
            rtsp_link = _rtsp_link;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => {
                if (timeOut > 0) timeOut--;
                lbl_TimeOut.Content = timeOut.ToString();
            };
            timer.Start();

            //stream video from camera
            Thread thrd_livestream = new Thread(new ThreadStart(() => {
                VideoCapture capture = new VideoCapture(rtsp_link);
                if (capture.IsOpened) {
                    while (timeOut > 0 || nightResult != 0) {
                        Mat m = capture.QueryFrame();
                        Image<Bgr, Byte> img = m.ToImage<Bgr, Byte>();
                        int text_left = 200;
                        int text_top = 100;

                        int rs = img.Rows;
                        int cs = img.Cols;

                        Bgr p1 = img[10, 10];
                        Bgr p2 = img[rs / 2, cs / 2];
                        Bgr p3 = img[rs - 10, cs - 10];

                        bool ret1 = Math.Abs(p1.Red - p1.Green) <= diffValue && Math.Abs(p1.Green - p1.Blue) <= diffValue && Math.Abs(p1.Blue - p1.Red) <= diffValue;
                        bool ret2 = Math.Abs(p2.Red - p2.Green) <= diffValue && Math.Abs(p2.Green - p2.Blue) <= diffValue && Math.Abs(p2.Blue - p2.Red) <= diffValue;
                        bool ret3 = Math.Abs(p3.Red - p3.Green) <= diffValue && Math.Abs(p3.Green - p3.Blue) <= diffValue && Math.Abs(p3.Blue - p3.Red) <= diffValue;

                        string str1 = string.Format("Point1: R-G={0} G-B={1} B-R={2}", Math.Abs(p1.Red - p1.Green), Math.Abs(p1.Green - p1.Blue), Math.Abs(p1.Blue - p1.Red));
                        string str2 = string.Format("Point2: R-G={0} G-B={1} B-R={2}", Math.Abs(p2.Red - p2.Green), Math.Abs(p2.Green - p2.Blue), Math.Abs(p2.Blue - p2.Red));
                        string str3 = string.Format("Point3: R-G={0} G-B={1} B-R={2}", Math.Abs(p3.Red - p3.Green), Math.Abs(p3.Green - p3.Blue), Math.Abs(p3.Blue - p3.Red));


                        CvInvoke.PutText(m, str1, new System.Drawing.Point(text_left, text_top), Emgu.CV.CvEnum.FontFace.HersheySimplex, 1.5, ret1 ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255), 3);
                        CvInvoke.PutText(m, str2, new System.Drawing.Point(text_left, text_top + 70), Emgu.CV.CvEnum.FontFace.HersheySimplex, 1.5, ret2 ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255), 3);
                        CvInvoke.PutText(m, str3, new System.Drawing.Point(text_left, text_top + 140), Emgu.CV.CvEnum.FontFace.HersheySimplex, 1.5, ret3 ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255), 3);

                        bool ret = ret1 && ret2 && ret3;
                        string strR = string.Format("{0}", ret ? "Passed" : "Failed");
                        CvInvoke.PutText(m, strR, new System.Drawing.Point(text_left, text_top + 500), Emgu.CV.CvEnum.FontFace.HersheySimplex, 6, ret ? new MCvScalar(0, 255, 0) : new MCvScalar(3, 182, 255), 10);

                        nightMessage = string.Format("{0}\n{1}\n{2}\n{3}\n", str1, str2, str3, strR);

                        var bi = globalUtility.ToBitmapSource(m.Bitmap);
                        bi.Freeze();
                        nightInfo.imageSource = bi;
                        Thread.Sleep(10);

                        if (ret) count++;
                        if (count >= 10) {
                            nightResult = 0;
                            break;
                        }
                    }
                    if (capture != null) capture.Dispose();
                }
            }));
            thrd_livestream.IsBackground = true;
            thrd_livestream.Start();
        }
    }


}
