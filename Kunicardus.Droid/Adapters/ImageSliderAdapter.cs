﻿using Android.Support.V4.View;
using Android.Content;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;
using Java.Net;
using Android.Graphics;
using System.Threading.Tasks;


namespace Kunicardus.Droid
{
	public class ImageSliderAdapter : PagerAdapter
	{
		Context _context;
		List<string> _imageUrls;

		public ImageSliderAdapter (Context context, List<string> imageUrls)
		{
			_imageUrls = imageUrls;
			_context = context;
		}

		public override bool IsViewFromObject (Android.Views.View view, Java.Lang.Object @object)
		{
			return view == ((LinearLayout)@object);
		}

		public override int Count {
			get {
				return _imageUrls.Count;
			}
		}

		public override void DestroyItem (ViewGroup container, int position, Java.Lang.Object objectValue)
		{
		}

		public override Java.Lang.Object InstantiateItem (ViewGroup container, int position)
		{

			View view = container;
			var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
			view = inflater.Inflate (Resource.Layout.image_slider_item, null);
			var child = view.FindViewById<ImageView> (Resource.Id.image_slider_item);
            child.Click += (o, e) =>
            {

            };

			Bitmap image = null;
			Task.Run (() => {
				URL url = new URL (_imageUrls [position]);
				image = BitmapFactory.DecodeStream (url.OpenConnection ().InputStream);
			}).ContinueWith (t => {
				(_context as MainView).RunOnUiThread (() => {
					child.SetImageBitmap (image);
					if ((_context as MainView)._dialog != null)
						(_context as MainView)._dialog.Dismiss ();
				});
			});

			container.AddView (view);
			return view;
		}
	}
}