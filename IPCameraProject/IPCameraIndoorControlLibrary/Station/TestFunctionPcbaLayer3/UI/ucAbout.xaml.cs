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

namespace IPCameraIndoorControlLibrary.Station.TestFunctionPcbaLayer3.UI {
    /// <summary>
    /// Interaction logic for ucAbout.xaml
    /// </summary>
    public partial class ucAbout : UserControl {

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }

        List<history> listHist = new List<history>();

        public ucAbout() {
            InitializeComponent();

            listHist.Add(new history() {
                ID = "1",
                VERSION = "1.0.0.0",
                CONTENT = "- Phát hành lần đầu",
                DATE = "06/08/2018",
                CHANGETYPE = "Tạo mới",
                PERSON = "Lê Việt Lợi, Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "2",
                VERSION = "1.0.0.1",
                CONTENT = "- Thay đổi lệnh ghi âm từ hw:0,1 => hw:1,1 (update theo firmware)",
                DATE = "22/08/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "3",
                VERSION = "2.0.0.0",
                CONTENT = "- Phần mềm tự động phán định độ nét IP camera, night vision.",
                DATE = "19/11/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "4",
                VERSION = "2.0.0.1",
                CONTENT = "- Cập nhật tool theo tài liệu test item mới \"IPCamera_HW Item test for mass production manufaturing_26-11-2019.docx\"\n,firmware basic mới và dải mac mới D49AA0.",
                DATE = "03/12/2019",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            this.GridAbout.ItemsSource = listHist;
        }

    }
}
