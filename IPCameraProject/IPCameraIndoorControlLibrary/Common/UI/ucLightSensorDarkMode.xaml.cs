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
    /// Interaction logic for ucLightSensorDarkMode.xaml
    /// </summary>
    public partial class ucLightSensorDarkMode : UserControl {

        public int timeOut = -1;

        public ucLightSensorDarkMode(int _timeout) {
            InitializeComponent();

            timeOut = _timeout;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => {
                if (timeOut > 0) timeOut--;
                lbl_TimeOut.Content = timeOut.ToString();
                this.Background = timeOut % 2 == 0 ? Brushes.White : (SolidColorBrush)new BrushConverter().ConvertFrom("#ecff8f");
            };
            timer.Start();
        }
    }
}
