using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using IPCameraIndoorControlLibrary.Common.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UtilityPack.IO;

namespace IPCameraIndoorControlLibrary.Station.TestFunctionAsm.UI {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {

        VideoCapture capture;
        bool flag_liveStream = false;
        bool flag_startCalSharpness = false;

        liveStreamInfo areaStreamInfo = new liveStreamInfo();
        liveStreamInfo sharpnessStreamInfo = new liveStreamInfo();

        List<double> listScale = null;

        public ucSetting() {

            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(Function.stationVariable.settingAsm)) Function.stationVariable.mySetting = XmlHelper<Function.Custom.SettingInformation>.FromXmlFile(Function.stationVariable.settingAsm);

            //binding data
            this.grid_setting.DataContext = Function.stationVariable.mySetting;
            this.grid_setarea.DataContext = areaStreamInfo;
            this.grid_setsharpness.DataContext = sharpnessStreamInfo;


            //add combobox itemsource
            cbb_delayserializessid.ItemsSource = globalParameter.list_number;
            cbb_retrytime.ItemsSource = globalParameter.list_number;
            cbb_tolerancenightvision.ItemsSource = globalParameter.list_number;
            cbb_failandstop.ItemsSource = new List<string>() { "Yes", "No" };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                //save setting
                case "save_setting": {
                        XmlHelper<Function.Custom.SettingInformation>.ToXmlFile(Function.stationVariable.mySetting, Function.stationVariable.settingAsm); //save setting to xml file
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }

                //set area test chart
                case "set_area_test_chart": {

                        if (!UtilityPack.Validation.Parse.IsIPAddress(Function.stationVariable.mySetting.cameraIP)) {
                            MessageBox.Show("Vui lòng thiết lập địa chỉ ip của camera trước.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (!globalUtility.pingNetwork(Function.stationVariable.mySetting.cameraIP)) {
                            MessageBox.Show("Vui lòng kết nối mạng với ip camera trước.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        this.grid_setarea.Visibility = Visibility.Visible;
                        this.grid_setting.Visibility = Visibility.Collapsed;
                        areaStreamInfo.imageSource = null;

                        flag_liveStream = true;
                        setAreaStream();

                        if (Function.stationVariable.mySetting.areaRectangle != null &&
                            Function.stationVariable.mySetting.areaRectangle.Trim().Length > 0) {

                            string[] buffer = Function.stationVariable.mySetting.areaRectangle.Split(',');
                            _rect_area.Margin = new Thickness(double.Parse(buffer[0]), double.Parse(buffer[1]), 0, 0);
                            _rect_area.Width = double.Parse(buffer[2]);
                            _rect_area.Height = double.Parse(buffer[3]);

                            if (_rect_area.Visibility != Visibility.Visible) { _rect_area.Visibility = Visibility.Visible; }
                        }

                        break;
                    }
                case "finish_set_area": {
                        if (posLeft != 0 && posTop != 0) {
                            //set area rectangle
                            Function.stationVariable.mySetting.areaRectangle = string.Format("{0},{1},{2},{3}",
                                                                                             posLeft,
                                                                                             posTop,
                                                                                             _rect_area.Width,
                                                                                             _rect_area.Height);
                            //set area test chart
                            scaleWidth = image_act_width / image_area.ActualWidth;
                            scaleHeight = image_act_height / image_area.ActualHeight;
                            Function.stationVariable.mySetting.areaTestChart = string.Format("{0},{1},{2},{3}",
                                                                                             Math.Round(posLeft * scaleWidth, 0),
                                                                                             Math.Round(posTop * scaleHeight, 0),
                                                                                             Math.Round(_rect_area.Width * scaleWidth, 0),
                                                                                             Math.Round(_rect_area.Height * scaleHeight), 0);

                        }

                        flag_liveStream = false;
                        this.grid_setarea.Visibility = Visibility.Collapsed;
                        this.grid_setting.Visibility = Visibility.Visible;
                        break;
                    }

                //set sharpness standard value
                case "set_sharpness_standard_value": {

                        if (!UtilityPack.Validation.Parse.IsIPAddress(Function.stationVariable.mySetting.cameraIP)) {
                            MessageBox.Show("Vui lòng thiết lập địa chỉ ip của camera trước.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (!globalUtility.pingNetwork(Function.stationVariable.mySetting.cameraIP)) {
                            MessageBox.Show("Vui lòng kết nối mạng với ip camera trước.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        this.grid_setsharpness.Visibility = Visibility.Visible;
                        this.grid_setting.Visibility = Visibility.Collapsed;
                        sharpnessStreamInfo.imageSource = null;
                        sharpnessStreamInfo.imageCrop = null;

                        flag_liveStream = true;
                        setSharpnessStream();

                        if (Function.stationVariable.mySetting.areaRectangle != null &&
                            Function.stationVariable.mySetting.areaRectangle.Trim().Length > 0) {

                            string[] buffer = Function.stationVariable.mySetting.areaRectangle.Split(',');
                            _rect_sharpness.Margin = new Thickness(double.Parse(buffer[0]), double.Parse(buffer[1]), 0, 0);
                            _rect_sharpness.Width = double.Parse(buffer[2]);
                            _rect_sharpness.Height = double.Parse(buffer[3]);

                            if (_rect_sharpness.Visibility != Visibility.Visible) { _rect_sharpness.Visibility = Visibility.Visible; }
                        }

                        break;
                    }
                case "start_set_sharpness": {
                        b.Tag = "finish_set_sharpness";
                        b.Content = "Hoàn thành";
                        flag_startCalSharpness = true;
                        break;
                    }
                case "finish_set_sharpness": {
                        b.Tag = "start_set_sharpness";
                        b.Content = "Bắt đầu";

                        if (listScale.Count > 0) {
                            Function.stationVariable.mySetting.sharpnessStandard = Math.Round(listScale.Average(), 2);
                        }

                        flag_startCalSharpness = false;
                        flag_liveStream = false;
                        this.grid_setsharpness.Visibility = Visibility.Collapsed;
                        this.grid_setting.Visibility = Visibility.Visible;
                        break;
                    }

                default: break;
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }

        ~ucSetting() {
            if (capture != null) capture.Dispose();
        }

        #region set area

        double posLeft = 0;
        double posTop = 0;
        Boolean isDragging = false;
        int image_act_width = 0;
        int image_act_height = 0;
        double scaleWidth = 1;
        double scaleHeight = 1;

        private void image_area_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                if (isDragging == false) {
                    isDragging = true;
                    posLeft = e.GetPosition(border_area).X;
                    posTop = e.GetPosition(border_area).Y;
                }
            }
        }

        private void image_area_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                double x = e.GetPosition(border_area).X;
                double y = e.GetPosition(border_area).Y;

                _rect_area.Margin = new Thickness(posLeft, posTop, 0, 0);
                _rect_area.Width = Math.Abs(x - posLeft);
                _rect_area.Height = Math.Abs(y - posTop);

                //< anzeigen >
                if (_rect_area.Visibility != Visibility.Visible) { _rect_area.Visibility = Visibility.Visible; }
                //</ anzeigen >
            }
        }

        private void image_area_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                if (isDragging) {
                    isDragging = false;
                }
            }
        }

        private void setAreaStream() {
            int c = 0;
            Thread t = new Thread(new ThreadStart(() => {
                capture = new VideoCapture(Function.stationVariable.mySetting.cameraRtspLink);
                if (capture.IsOpened) {
                    while (flag_liveStream) {
                        Mat m = new Mat();
                        if (capture != null) capture.Read(m);

                        if (!m.IsEmpty) {
                            image_act_width = m.Width;
                            image_act_height = m.Height;

                            var bi = globalUtility.ToBitmapSource(m.Bitmap);
                            bi.Freeze();
                            areaStreamInfo.imageSource = bi;
                        }

                        if (c == 0) {
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                if (btn_finish_set_area.IsEnabled == false) btn_finish_set_area.IsEnabled = true;
                                c++;
                            }));
                        }

                        Thread.Sleep(10);
                    }
                    if (capture != null) capture.Dispose();
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        #endregion

        #region set sharpness

        private void setSharpnessStream() {
            int c = 0;
            listScale = new List<double>();
            int cx = 0;

            Thread t = new Thread(new ThreadStart(() => {
                capture = new VideoCapture(Function.stationVariable.mySetting.cameraRtspLink);
                if (capture.IsOpened) {
                    while (flag_liveStream) {
                        Mat m = new Mat();
                        if (capture != null) capture.Read(m);

                        if (!m.IsEmpty) {
                            Image<Bgr, Byte> img = m.ToImage<Bgr, Byte>();

                            //init rectangle
                            string[] buffer = Function.stationVariable.mySetting.areaTestChart.Split(',');
                            var rect = new System.Drawing.Rectangle(int.Parse(buffer[0]), int.Parse(buffer[1]), int.Parse(buffer[2]), int.Parse(buffer[3]));
                            //crop image
                            Image<Gray, byte> image_crop = globalUtility.CropFromImage(img, rect);

                            //cal sharpness
                            if (flag_startCalSharpness) {
                                int s = globalUtility.getSharpnessValueFromImage(image_crop);
                                int pixel = image_crop.Width * image_crop.Height;
                                double scale = Math.Round(s / (pixel * 1.0), 2);
                                listScale.Add(scale);

                                CvInvoke.PutText(m, "Value: " + Math.Round(listScale.Average(), 5).ToString(), new System.Drawing.Point(rect.Left, rect.Top + rect.Height + 100), FontFace.HersheySimplex, 2, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                                CvInvoke.PutText(m, "Sampling: " + cx.ToString(), new System.Drawing.Point(rect.Left, rect.Top + rect.Height + 200), FontFace.HersheySimplex, 2, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                                cx++;
                            }

                            //show image
                            var bi = globalUtility.ToBitmapSource(m.Bitmap);
                            bi.Freeze();
                            sharpnessStreamInfo.imageSource = bi;
                            //show crop image
                            var ci = globalUtility.Bitmap2BitmapImage(image_crop.Bitmap);
                            ci.Freeze();
                            sharpnessStreamInfo.imageCrop = ci;
                        }

                        if (c == 0) {
                            Application.Current.Dispatcher.Invoke(new Action(() => {
                                if (btn_start_set_sharpness.IsEnabled == false) btn_start_set_sharpness.IsEnabled = true;
                                c++;
                            }));
                        }

                        Thread.Sleep(10);
                    }
                    if (capture != null) capture.Dispose();
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        #endregion

        public class liveStreamInfo : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            public liveStreamInfo() {
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
    }
}
