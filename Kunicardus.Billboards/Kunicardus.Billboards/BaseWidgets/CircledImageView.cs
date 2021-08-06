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
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Kunicardus.Billboards
{
    public class CircledImageView : ImageView
    {
        public CircledImageView(Context context, IAttributeSet attrs) : base(context,attrs)
        {
        }

        protected override void OnDraw(Canvas canvas) {

              Drawable drawable = Drawable;

              if (drawable == null) {
                     return;
              }

              if (Width == 0 || Height == 0) {
                     return;
              }
              Bitmap b = ((BitmapDrawable) drawable).Bitmap;
              Bitmap bitmap = b.Copy(Bitmap.Config.Argb8888, true);

              int w = Width, h = Height;

              Bitmap roundBitmap = getRoundedCroppedBitmap(bitmap, w);
              canvas.DrawBitmap(roundBitmap, 0, 0, null);

       }

       public static Bitmap getRoundedCroppedBitmap(Bitmap bitmap, int radius) {
              Bitmap finalBitmap;
              if (bitmap.Height != radius || bitmap.Height != radius)
                     finalBitmap = Bitmap.CreateScaledBitmap(bitmap, radius, radius,false);
              else
                     finalBitmap = bitmap;
              Bitmap output = Bitmap.CreateBitmap(finalBitmap.Height,
                           finalBitmap.Height, Bitmap.Config.Argb8888);
              Canvas canvas = new Canvas(output);

              Paint paint = new Paint();
              Rect rect = new Rect(0, 0, finalBitmap.Width, finalBitmap.Height);

              paint.AntiAlias = true;
              paint.FilterBitmap = true;
              paint.Dither = true;
              canvas.DrawARGB(0, 0, 0, 0);
              paint.Color = Color.ParseColor("#e6eced");
              canvas.DrawCircle(finalBitmap.Width / 2 + 0.7f, finalBitmap.Height / 2 + 0.7f, finalBitmap.Width / 2 + 0.1f, paint);
              paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
              canvas.DrawBitmap(finalBitmap, rect, rect, paint);

              return output;
       }
    }
}