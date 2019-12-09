using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilityPack.Validation;

namespace IPCameraIndoorControlLibrary.Common.UI {
    /// <summary>
    /// Interaction logic for ucInputMacSerialUid.xaml
    /// </summary>
    public partial class ucInputMacSerialUid : UserControl {

        public int inputResult = -1;
        string product_number = "";
        string uid_header = "";

        public ucInputMacSerialUid(string pd_number, string _uid_header) {
            InitializeComponent();

            product_number = pd_number;
            uid_header = _uid_header;

            if (sp_mac.Visibility == Visibility.Visible) tb_mac.IsEnabled = false;
            if (sp_serial.Visibility == Visibility.Visible) tb_serial.IsEnabled = false;
            if (sp_uid.Visibility == Visibility.Visible) tb_uid.IsEnabled = false;


            Thread t = new Thread(new ThreadStart(() => {
                Thread.Sleep(100);
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    if (sp_mac.Visibility == Visibility.Visible) { tb_mac.IsEnabled = true; tb_mac.Focus(); return; }
                    if (sp_serial.Visibility == Visibility.Visible) { tb_serial.IsEnabled = true; tb_serial.Focus(); return; }
                    if (sp_uid.Visibility == Visibility.Visible) { tb_uid.IsEnabled = true; tb_uid.Focus(); return; }
                }));
            }));
            t.IsBackground = true;
            t.Start();
            
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            
            TextBox tbox = sender as TextBox;
            string txb_name = (string) tbox.Name;
            tbox.IsEnabled = false;
            
            switch (txb_name) {
                case "tb_mac": {
                        //check mac format
                        bool r = Parse.IsMacAddress(tbox.Text.ToUpper());
                        if (!r) {
                            tb_message.Text = string.Format("Địa chỉ mac \"{0}\" sai định dạng.\nVui lòng nhập lại.", tbox.Text);
                            tbox.Clear();
                            tbox.IsEnabled = true;
                            tbox.Focus();
                            return;
                        }

                        //next focus
                        if (sp_serial.Visibility == Visibility.Visible) { tb_serial.IsEnabled = true; tb_serial.Focus(); return; }
                        else if (sp_uid.Visibility == Visibility.Visible) { tb_uid.IsEnabled = true; tb_uid.Focus(); return; }
                        else inputResult = 0;
                        break;
                    }
                case "tb_serial": {
                        //check format
                        bool r = Parse.IsVnptProductSerialNumber(tbox.Text.ToUpper());
                        if (!r) {
                            tb_message.Text = string.Format("Số serial \"{0}\" sai định dạng.\nVui lòng nhập lại.", tbox.Text);
                            tbox.Clear();
                            tbox.IsEnabled = true;
                            tbox.Focus();
                            return;
                        }

                        //check product number of serial
                        string prd_serial = tbox.Text.Substring(0, 3).ToUpper();
                        r = prd_serial.Equals(product_number);
                        if (!r) {
                            tb_message.Text = string.Format("Số serial \"{0}\" không đúng mã product number cài đặt trong phần mềm \"{1}\".\nVui lòng nhập lại.", tbox.Text, product_number);
                            tbox.Clear();
                            tb_mac.Clear();
                            tb_mac.IsEnabled = true;
                            tb_mac.Focus();
                            return;
                        }
                        
                        //check same mac or not
                        string end_mac = tb_mac.Text.Substring(6, 6).ToUpper();
                        string end_serial = tbox.Text.Substring(9, 6).ToUpper();
                        r = end_mac.Equals(end_serial);
                        if (!r) {
                            tb_message.Text = string.Format("Số serial \"{0}\" không trùng với địa chỉ mac \"{1}\".\nVui lòng nhập lại.", tbox.Text, tb_mac.Text);
                            tbox.Clear();
                            tb_mac.Clear();
                            tb_mac.IsEnabled = true;
                            tb_mac.Focus();
                            return;
                        }
                        
                        if (sp_uid.Visibility == Visibility.Visible) { tb_uid.IsEnabled = true; tb_uid.Focus(); }
                        else inputResult = 0;

                        break;
                    }
                case "tb_uid": {
                        //check same mac or not
                        string uid = string.Format("{0}{1}", uid_header, UtilityPack.Converter.myConverter.stringToMD5(tb_mac.Text.ToUpper())).ToUpper();
                        bool r = tbox.Text.ToUpper().Equals(uid);
                        if (!r) {
                            tb_message.Text = string.Format("Mã QRcode UID \"{0}\" không trùng với địa chỉ mac \"{1}\".\nVui lòng nhập lại.", tbox.Text, uid);
                            tbox.Clear();
                            tb_mac.Clear();
                            tb_serial.Clear();
                            tb_mac.IsEnabled = true;
                            tb_mac.Focus();
                            return;
                        }

                        inputResult = 0;
                        break;
                    }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox tbox = sender as TextBox;
            if (tbox.Text.Trim().Length > 0) tb_message.Text = "";
        }
    }
}
