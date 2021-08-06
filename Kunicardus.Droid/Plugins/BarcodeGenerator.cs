using System;
using Android.Graphics;
using ZXing;
using ZXing.Mobile;
//using Google.ZXing;
//using Google.ZXing.Common;
//using Google.ZXing.QRCode;
//using ZXing;
//using ZXing.Rendering;

namespace Kunicardus.Droid.Plugins
{
    public static class BarcodeGenerator
    {
        public static Bitmap Generate(string cardNumber, float height, float width)
        {

            //var heightImg = Convert.ToInt32(height * 0.7);
            //var widthImg = Convert.ToInt32(width * 1.4);

            //MultiFormatWriter writer = new MultiFormatWriter();

            //BitMatrix bm = writer.Encode(cardNumber, BarcodeFormat.Code128, widthImg, heightImg);
            //Bitmap ImageBitmap = Bitmap.CreateBitmap(widthImg, heightImg, Bitmap.Config.Argb8888);

            //var writer = new QRCodeWriter
            //{
            //    Format = BarcodeFormat.Code128,
            //    Renderer = new BitmapRenderer()
            //    {
            //        Background = Color.White,
            //        Foreground = Color.Black
            //    }
            //};
            //writer.Options.Height = Convert.ToInt32(height * 0.7);
            //writer.Options.Width = Convert.ToInt32(width * 1.4);
            ////writer.Options.Margin = 1;
            //var bitmap = writer.Write(cardNumber);

            var writer = new BarcodeWriter();
           // writer.Format = BarcodeFormat.CODE_128;
            writer.Renderer = new BitmapRenderer()
            {
                Background = Color.White,
                Foreground = Color.Black
            };
            writer.Options.Height = Convert.ToInt32(height * 0.7);
            writer.Options.Width = Convert.ToInt32(width * 1.4);
            //writer.Options.Margin = 1;
            var bitmap = writer.Write(cardNumber);
            return bitmap;
        }
    }
}