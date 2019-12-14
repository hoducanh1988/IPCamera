﻿using System;
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

namespace IPCameraIndoorControlLibrary.Station.UploadFirmwareBasic.UI {
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
                DATE = "22/11/2018",
                CHANGETYPE = "Tạo mới",
                PERSON = "Trần Đức Hòa"
            });

            listHist.Add(new history() {
                ID = "2",
                VERSION = "1.0.0.1",
                CONTENT = "- Tích hợp vào chung 1 tool với các trạm test khác.\n" + 
                          "- Giảm số lượng hỗ trợ nạp multi từ 8 => 4 sản phẩm cùng lúc.\n" + 
                          "- Update command và file firmware basic mới.\n" + 
                          "- Update thêm dải mac mới D49AA0.",
                DATE = "14/12/2019",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            
            this.GridAbout.ItemsSource = listHist;
        }

    }
}
