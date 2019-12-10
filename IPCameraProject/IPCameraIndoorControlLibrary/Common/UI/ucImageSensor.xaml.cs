using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using IPCameraIndoorControlLibrary.Common.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

namespace IPCameraIndoorControlLibrary.Common.UI {
    /// <summary>
    /// Interaction logic for ucImageSensor.xaml
    /// </summary>
    public partial class ucImageSensor : UserControl {

        public class ImageSensorInfo : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            public ImageSensorInfo() {
                imageSource = null;
                imageCrop = null;
            }

            ImageSource _image_source;
            public ImageSource imageSource {
                get { return _image_source; }
                set {
                    _image_source = value;
                    OnPropertyChanged("imageSource");
                }
            }
            BitmapImage _image_crop;
            public BitmapImage imageCrop {
                get { return _image_crop; }
                set {
                    _image_crop = value;
                    OnPropertyChanged("imageCrop");
                }
            }
        }

        public ImageSensorInfo imageInfo = new ImageSensorInfo();

        public int imageResult = -1;
        public int timeOut = -1;
        double std_sharpness = -1;
        double std_tolerance = -1;
        string area_testchart = "";
        string rtsp_link = "";
        int count = 0;
        public string imageMessage = "";

        VideoCapture capture;

        public ucImageSensor(int _timeout, string _area, double _std_sharpness, double _std_tolerance, string _rtsp_link) {
            InitializeComponent();
            this.DataContext = imageInfo;

            timeOut = _timeout;
            area_testchart = _area;
            std_sharpness = _std_sharpness;
            std_tolerance = _std_tolerance;
            rtsp_link = _rtsp_link;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => {
                if (timeOut > 0) timeOut--;
                lbl_TimeOut.Content = timeOut.ToString();
            };
            timer.Start();

            //live stream
            //stream video from camera
            Thread thrd_livestream = new Thread(new ThreadStart(() => {
                capture = new VideoCapture(rtsp_link);
                if (capture.IsOpened) {
                    while (timeOut > 0 && imageResult != 0) {
                        if (timeOut == 0) break;
                        if (imageResult == 0) break;

                        Mat m = new Mat();
                        if (capture != null) capture.Read(m);

                        if (!m.IsEmpty) {
                            Image<Bgr, Byte> img = m.ToImage<Bgr, Byte>();
                            int iw = img.Width;
                            int ih = img.Height;

                            //init rectangle
                            //int rect_width = 400;
                            //int rect_height = 300;
                            //int rect_left = iw / 2 - rect_width / 2;
                            //int rect_top = ih / 2 - rect_height / 2;

                            string[] buffer = area_testchart.Split(',');
                            int rect_left = int.Parse(buffer[0]);
                            int rect_top = int.Parse(buffer[1]);
                            int rect_width = int.Parse(buffer[2]);
                            int rect_height = int.Parse(buffer[3]);

                            var rect = new System.Drawing.Rectangle(rect_left, rect_top, rect_width, rect_height);

                            //show rectangle
                            CvInvoke.Rectangle(m, rect, new MCvScalar(0, 0, 255), 3, LineType.AntiAlias);

                            //crop image by rectangle
                            Image<Gray, byte> image_crop = CropFromImage(img, rect);

                            //calculate sharpness
                            int s = getSharpnessValueFromImage(image_crop);
                            int pixel = image_crop.Width * image_crop.Height;
                            double scale = s / (pixel * 1.0);
                            double ll_value = std_sharpness - std_tolerance;
                            bool r = scale >= ll_value;
                            if (r) count++;

                            //put text scale
                            CvInvoke.PutText(m, scale.ToString(), new System.Drawing.Point(rect_left, rect_top - 20), FontFace.HersheySimplex, 2, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);

                            //put text result
                            CvInvoke.PutText(m, r ? "Passed" : "Failed" , new System.Drawing.Point(rect_left, rect_top + rect_height + 150), FontFace.HersheySimplex, 4, r ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255), 4, LineType.AntiAlias);

                            imageMessage = string.Format("Độ nét hình ảnh của camera: {0}\n{1}\n", scale, r ? "Passed" : "Failed");

                            //show image
                            var bi = globalUtility.ToBitmapSource(m.Bitmap);
                            bi.Freeze();
                            imageInfo.imageSource = bi;

                            //show crop
                            var ci = globalUtility.Bitmap2BitmapImage(image_crop.Bitmap);
                            ci.Freeze();
                            imageInfo.imageCrop = ci;
                        }
                        
                        Thread.Sleep(10);

                        if (count >= 30) {
                            imageResult = 0;
                            break;
                        }
                    }
                    if (capture != null) capture.Dispose();
                }
            }));
            thrd_livestream.IsBackground = true;
            thrd_livestream.Start();
        }

        ~ucImageSensor() {
            Dispose();
        }

        public void Dispose() {
            if (capture != null) capture.Dispose();
        }

        #region support function

        private Image<Gray, byte> CropFromImage(Image<Bgr, Byte> imageInput, System.Drawing.Rectangle rect) {
            try {
                Image<Bgr, byte> _imageRef = null;
                imageInput.ROI = rect;
                _imageRef = imageInput.CopyBlank();
                imageInput.CopyTo(_imageRef);
                imageInput.ROI = System.Drawing.Rectangle.Empty;

                Image<Gray, byte> _imgGray = new Image<Gray, byte>(_imageRef.Width, _imageRef.Height);
                ConvertImageFromBgrToGray(_imageRef, ref _imgGray);

                return _imgGray;
            }
            catch {
                return null;
            }
        }

        //private Image<Bgr, byte> CropFromImage(Image<Bgr, Byte> imageInput, System.Drawing.Rectangle rect) {
        //    try {
        //        Image<Bgr, byte> _imageRef = null;
        //        imageInput.ROI = rect;
        //        _imageRef = imageInput.CopyBlank();
        //        imageInput.CopyTo(_imageRef);
        //        imageInput.ROI = System.Drawing.Rectangle.Empty;

        //        return _imageRef;
        //    }
        //    catch {
        //        return null;
        //    }
        //}

        private bool ConvertImageFromBgrToGray(Image<Bgr, byte> imageIn, ref Image<Gray, byte> imageOut) {
            if (imageIn == null) return false;

            try {
                /*Change color space from BGR to Gray ---------------*/
                Image<Gray, byte> imgGray = new Image<Gray, byte>(imageIn.Width, imageIn.Height, new Gray(0));
                CvInvoke.CvtColor(imageIn, imgGray, ColorConversion.Rgb2Gray);

                /*Apply threshold to convert to binary image -------*/
                Image<Gray, byte> img_binary = new Image<Gray, byte>(imageIn.Width, imageIn.Height);
                CvInvoke.Threshold(imgGray, img_binary, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                imageOut = img_binary;
                return true;
            }
            catch {
                return false;
            }
        }

        private byte _px(Image<Gray, byte> img, int x, int y) {
            return img.Data[y, x, 0];
        }

        private int getSharpnessValueFromImage(Image<Gray, byte> image) {
            int height = image.Height;
            int width = image.Width;

            int sum = 0;
            for (int x = 0; x < width - 1; x++) {
                for (int y = 0; y < height; y++) {
                    sum += Math.Abs(_px(image, x, y) - _px(image, x + 1, y));
                }
            }

            return sum;
        }

        #endregion

    }
}
