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
    /// Interaction logic for ucIRCut.xaml
    /// </summary>
    public partial class ucIRCut : UserControl {

        public bool isTryAgain = false;
        public int ircutResult = -1;
        public string ircutMessage = "";


        public ucIRCut() {
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
                case "playagain": {
                        if (isTryAgain == false) isTryAgain = true;
                        break;
                    }
                case "confirm": {
                        if (rb_pass.IsChecked == false && rb_fail_1.IsChecked == false && rb_fail_2.IsChecked == false) {
                            MessageBox.Show("Bạn chưa click chọn xác nhận trạng thái khối ir cut.", "Cảnh báo!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        }

                        if (rb_pass.IsChecked == true) { ircutResult = 0; ircutMessage = (string)rb_pass.Content; }
                        if (rb_fail_1.IsChecked == true) { ircutResult = 1; ircutMessage = (string)rb_fail_1.Content; }
                        if (rb_fail_2.IsChecked == true) { ircutResult = 2; ircutMessage = (string)rb_fail_2.Content; }
                        break;
                    }
                default: break;
            }
        }
    }
}
