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
using System.Windows.Xps.Packaging;

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBusiness.UI {
    /// <summary>
    /// Interaction logic for ucHelp.xaml
    /// </summary>
    public partial class ucHelp : UserControl {
        public ucHelp() {
            InitializeComponent();

            //
            if (System.IO.File.Exists(Function.stationVariable.guideFWBusiness)) {
                XpsDocument xpsDocument = new XpsDocument(Function.stationVariable.guideFWBusiness, System.IO.FileAccess.Read);
                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                _docViewer.Document = fds;
            }
        }
    }
}
