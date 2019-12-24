using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityPack.Validation;

namespace IPCameraIndoorControlLibrary.Station.PrintLabelUID.Function.Custom {
    public class SettingParameter {


        public bool Validation(out string msg) {
            msg = "";
            try {
                bool r = false;
                //Cai dat mac
                r = validate_caidat_mac(out msg);
                if (!r) return false;

                //cai dat serial
                r = validate_caidat_serial(out msg);
                if (!r) return false;

                //cai dat uid
                r = validate_caidat_uid(out msg);
                if (!r) return false;

                //cai dat file access
                r = validate_caidat_file_access(out msg);
                if (!r) return false;

                return r;
            }
            catch (Exception ex) {
                msg = ex.ToString();
                return false;
            }
        }

        bool validate_caidat_mac(out string errorMessage) {
            errorMessage = "";
            bool r = !(string.IsNullOrEmpty(stationVariable.mySetting.vnptMacHeader) || string.IsNullOrWhiteSpace(stationVariable.mySetting.vnptMacHeader));
            errorMessage += r ? "" : "VNPT Mac header chưa được cài đặt hoặc cài đặt sai giá trị.";

            return r;
        }

        bool validate_caidat_serial(out string errorMessage) {
            errorMessage = "";

            bool r1 = Parse.IsVnptProductNumber(stationVariable.mySetting.vnptProductNumber);
            bool r2 = Parse.IsVnptFactory(stationVariable.mySetting.productionFactory);
            bool r3 = Parse.IsVnptProductVersion(stationVariable.mySetting.hardwareVersion);
            bool r4 = !(string.IsNullOrEmpty(stationVariable.mySetting.productMacCode) || string.IsNullOrWhiteSpace(stationVariable.mySetting.productMacCode));

            bool r = r1 && r2 && r3 && r4;

            errorMessage += r1 ? "" : "VNPT Product Number chưa được cài đặt hoặc cài đặt sai giá trị.";
            errorMessage += r2 ? "" : "Nhà máy sản xuất chưa được cài đặt hoặc cài đặt sai giá trị.";
            errorMessage += r3 ? "" : "Hardware Version chưa được cài đặt hoặc cài đặt sai giá trị.";
            errorMessage += r4 ? "" : "Mã phân biệt dải mac chưa được cài đặt.";

            return r;
        }

        bool validate_caidat_uid(out string errorMessage) {
            errorMessage = "";
            bool r = !(string.IsNullOrEmpty(stationVariable.mySetting.vnptUidHeader) || string.IsNullOrWhiteSpace(stationVariable.mySetting.vnptUidHeader));
            errorMessage += r ? "" : "VNPT uid header chưa được cài đặt hoặc cài đặt sai giá trị.";

            return r;
        }

        bool validate_caidat_file_access(out string errorMessage) {
            errorMessage = "";
            bool r = !(string.IsNullOrEmpty(stationVariable.mySetting.fileMsAccess) || string.IsNullOrWhiteSpace(stationVariable.mySetting.fileMsAccess));
            errorMessage += r ? "" : "fileMsAccess chưa được cài đặt hoặc cài đặt sai giá trị.";

            return r;
        }

    }
}
