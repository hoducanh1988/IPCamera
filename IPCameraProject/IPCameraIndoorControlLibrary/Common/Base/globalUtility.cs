using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Net.NetworkInformation;
using UtilityPack.Validation;

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

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap) {
            using (var memory = new MemoryStream()) {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static Bitmap BitmapSource2Bitmap(BitmapSource bitmapsource) {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static Image<Gray, byte> CropFromImage(Image<Bgr, Byte> imageInput, System.Drawing.Rectangle rect) {
            try {
                Image<Bgr, byte> _imageRef = null;
                imageInput.ROI = rect;
                _imageRef = imageInput.CopyBlank();
                imageInput.CopyTo(_imageRef);
                imageInput.ROI = System.Drawing.Rectangle.Empty;

                Image<Gray, byte> _imgGray = new Image<Gray, byte>(_imageRef.Width, _imageRef.Height);
                ConvertImageFromBgrToGray(_imageRef, ref _imgGray);

                return _imgGray;
            }
            catch {
                return null;
            }
        }

        public static bool ConvertImageFromBgrToGray(Image<Bgr, byte> imageIn, ref Image<Gray, byte> imageOut) {
            if (imageIn == null) return false;

            try {
                /*Change color space from BGR to Gray ---------------*/
                Image<Gray, byte> imgGray = new Image<Gray, byte>(imageIn.Width, imageIn.Height, new Gray(0));
                CvInvoke.CvtColor(imageIn, imgGray, ColorConversion.Rgb2Gray);

                /*Apply threshold to convert to binary image -------*/
                Image<Gray, byte> img_binary = new Image<Gray, byte>(imageIn.Width, imageIn.Height);
                CvInvoke.Threshold(imgGray, img_binary, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                imageOut = img_binary;
                return true;
            }
            catch {
                return false;
            }
        }

        public static byte px(Image<Gray, byte> img, int x, int y) {
            return img.Data[y, x, 0];
        }

        public static int getSharpnessValueFromImage(Image<Gray, byte> image) {
            int height = image.Height;
            int width = image.Width;

            int sum = 0;
            for (int x = 0; x < width - 1; x++) {
                for (int y = 0; y < height; y++) {
                    sum += Math.Abs(px(image, x, y) - px(image, x + 1, y));
                }
            }

            return sum;
        }

        public static bool pingNetwork(string ip) {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            // Use the default Ttl value which is 128, 
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 60;

            try {
                PingReply reply = pingSender.Send(ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        public static void WriteJpeg(string fileName, int quality, BitmapSource bs) {

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            BitmapFrame outputFrame = BitmapFrame.Create(bs);
            encoder.Frames.Add(outputFrame);
            encoder.QualityLevel = quality;

            using (FileStream file = File.OpenWrite(fileName)) {
                encoder.Save(file);
            }
        }
    }
}
