using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace IPCameraIndoorControlLibrary.Common.Base {

    public class globalUtility {

        public static BitmapSource ToBitmapSource(Bitmap bitmap) {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }


        //public static bool isGrayScale(Bitmap img, ref string message) {
        //    bool result = true;
        //    for (Int32 h = 0; h < img.Height; h++)
        //        for (Int32 w = 0; w < img.Width; w++) {
        //            Color color = img.GetPixel(w, h);
        //            message = string.Format("R={0}, G={1}, B={2}, A={3}", color.R, color.G, color.B, color.A);

        //            if ((color.R != color.G || color.G != color.B || color.R != color.B) && color.A != 0) {
        //                result = false;
        //                break;
        //            }
        //        }

        //    return result;
        //}
    }
}
