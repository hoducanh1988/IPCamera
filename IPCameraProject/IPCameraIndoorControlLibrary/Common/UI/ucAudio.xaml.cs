using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace IPCameraIndoorControlLibrary.Common.UI {
    /// <summary>
    /// Interaction logic for ucAudio.xaml
    /// </summary>
    public partial class ucAudio : UserControl {
        public bool isPlayBack = false;
        public int audioResult = -1;
        public string audioMessage = "";

        public ucAudio() {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            rb.Foreground = Brushes.Red;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            rb.Foreground = Brushes.Black;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_tag = (string)b.Tag;

            switch (b_tag) {
                case "playback": {
                        if (isPlayBack == false) isPlayBack = true;
                        break;
                    }
                case "confirm": {
                        if (rb_pass.IsChecked == false && rb_fail_1.IsChecked == false && rb_fail_2.IsChecked == false) {
                            MessageBox.Show("Bạn chưa click chọn xác nhận trạng thái khối audio.", "Cảnh báo!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        }

                        if (rb_pass.IsChecked == true) { audioResult = 0; audioMessage = (string)rb_pass.Content; }
                        if (rb_fail_1.IsChecked == true) { audioResult = 1; audioMessage = (string)rb_fail_1.Content; }
                        if (rb_fail_2.IsChecked == true) { audioResult = 2; audioMessage = (string)rb_fail_2.Content; }
                        break;
                    }
                default: break;
            }
        }
    }
}
