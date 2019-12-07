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
    /// Interaction logic for ucTabLogUart.xaml
    /// </summary>
    public partial class ucTabLogUart : UserControl {

        public bool isScroll = false;

        public ucTabLogUart() {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (o, e) => {
                if (isScroll) {
                    this.Scr_LogSystem.ScrollToEnd();
                    this.Scr_LogUart.ScrollToEnd();
                }
            };
            timer.Start();

        }
    }
}
