using System;
using System.Threading.Tasks;
using Android.Graphics;
using System.Net;

namespace Kunicardus.Droid.Plugins
{
	public static class ImageDownloader
	{
        public static async Task<Bitmap> Download(this string url)
        {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(url);
                byte[] imageBytes = null;

                try
                {
                    imageBytes = await webClient.DownloadDataTaskAsync(uri);
                }
                catch (Exception e)
                {
                    return null;
                }

                Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
                return bitmap;
            }
        }
	}
}