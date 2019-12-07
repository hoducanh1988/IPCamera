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
using System.Windows.Threading;

namespace IPCameraIndoorControlLibrary.Common.UI {
    /// <summary>
    /// Interaction logic for ucRGBLedRed.xaml
    /// </summary>
    public partial class ucRGBLedRed : UserControl {

        public int ledResult = -1;
        public int timeOut = -1;

        public ucRGBLedRed(int _timeout) {
            InitializeComponent();
            timeOut = _timeout;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => {
                if (timeOut > 0) timeOut--;
                lbl_TimeOut.Content = timeOut.ToString();
            };
            timer.Start();
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e) {
            Ellipse es = sender as Ellipse;
            string e_tag = (string)es.Tag;

            switch (e_tag) {
                case "pass": { ledResult = 0; return; }
                case "fail": { ledResult = 1; return; }
            }
        }
    }
}
