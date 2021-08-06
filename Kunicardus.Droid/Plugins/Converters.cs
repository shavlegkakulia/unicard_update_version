using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using MvvmCross.Converters;

namespace Kunicardus.Droid.Plugins
{
    public static class Converters
	{

		public static Color GetPointsColor (double parameter)
		{
			double points = parameter;

			if (points < 0) {
				return new Color (242, 142, 45);
			} else {
				return new Color (140, 189, 58);
			}
		}


		public static string ConvertPoints (double value)
		{
			if (value > 0) {
				return string.Format ("+{0}", value);
			}
			return value.ToString ();
		}
	}

    public class UrlToBitmapConverter : MvxValueConverter<string, Bitmap>
    {
        protected override Bitmap Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Bitmap bm = null;

                if (value == null)
                {
                    return Bitmap.CreateBitmap(0, 0, Bitmap.Config.Argb8888);
                }

                var imageName = value.Replace("/", " ");
                var filePath = System.IO.Path.Combine(Setup.PicturesPath, imageName);

                if (File.Exists(filePath))
                {
                    bm = BitmapFactory.DecodeFile(filePath);
                    return bm;
                }
                else
                {
                    bm = Task.Run(() => { return value.Download(); }).Result;
                    return bm;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
       }
    }
}