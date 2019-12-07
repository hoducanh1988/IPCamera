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
using System.Windows.Shapes;
using UtilityPack.IO;
using System.IO;

namespace IPCameraProject {
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window {
        public StationWindow() {
            //init control
            InitializeComponent();

            //add list item for combobox
            this.cbb_product.ItemsSource = myParameter.list_product;
            this.cbb_station.ItemsSource = myParameter.list_station;

            //load setting
            if (File.Exists(myGlobal.myStationConfigFile)) myGlobal.myInputInfo = XmlHelper<inputInformation>.FromXmlFile(myGlobal.myStationConfigFile);
            else myGlobal.myInputInfo = new inputInformation();

            //binding data
            this.DataContext = myGlobal.myInputInfo;
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_tag = (string)b.Tag;

            switch (b_tag) {
                case "agree": {
                        if (myGlobal.myInputInfo.ProductName == "") {
                            MessageBox.Show("Chưa chọn tên sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            cbb_product.Focus();
                            return;
                        }
                        if (myGlobal.myInputInfo.StationName == "") {
                            MessageBox.Show("Chưa chọn trạm test.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            cbb_station.Focus();
                            return;
                        }

                        //save setting
                        XmlHelper<inputInformation>.ToXmlFile(myGlobal.myInputInfo, myGlobal.myStationConfigFile);
                        
                        //show main window
                        MainWindow w = new MainWindow();
                        w.Show();

                        //close this window
                        this.Close();

                        return;
                    }
            }
        }
    }
}
