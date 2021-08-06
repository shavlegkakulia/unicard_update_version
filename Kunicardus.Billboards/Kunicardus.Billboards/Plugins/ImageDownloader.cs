using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net;
using Android.Graphics;

namespace Kunicardus.Billboards.Plugins
{
    public class ImageDownloader
    {
        public static async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            var webClient = new WebClient();
            var uri = new Uri(url);
            byte[] imageBytes = null;

            try
            {
                imageBytes = await webClient.DownloadDataTaskAsync(uri);
            }
            catch (Exception e)
            {
                //Toast.MakeText(parentActivity, Resource.String.errorImageLoading, ToastLength.Short).Show();
                return null;
            }

            Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
            return bitmap;
        }
    }
}