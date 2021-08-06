using System;
using Android.Support.V4.View;
using Android.Content;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;
using Java.Net;
using Android.Graphics;
using System.Threading.Tasks;
using Android.App;
using Kunicardus.Billboards.Plugins;
using Android.Util;


namespace Kunicardus.Billboards.Adapters
{
    public class ImageSliderAdapter : Android.Support.V4.View.PagerAdapter
	{
		Activity _context;
		List<string> _imageUrls;
		LayoutInflater mLayoutInflater;

        public ImageSliderAdapter(Activity context, List<string> imageUrls)
		{
			_imageUrls = imageUrls;
			_context = context;
			mLayoutInflater = (LayoutInflater)_context.GetSystemService (Context.LayoutInflaterService);
		}

		public override bool IsViewFromObject (Android.Views.View view, Java.Lang.Object @object)
		{
			return view == ((RelativeLayout)@object);
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
			view = inflater.Inflate (Resource.Layout.ImageSliderItem, null);
			var child = view.FindViewById<ImageView> (Resource.Id.image_slider_item);
		
			Bitmap image = null;
            Task.Run(async () =>
            {
                try
                {

                    image = await ImageDownloader.GetImageBitmapFromUrl(_imageUrls[position]);
                    if (image != null)
                    {
                        _context.RunOnUiThread(() =>
                        {
                            child.SetImageBitmap(image);
                            ResizeImage(child);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("ImageAdapter Message: ", ex.ToString());
                    throw;
                }
            });

			container.AddView (view);
			return view;
		}

        public void AddImageUrl(List<string> imgUrl)
        {
            _imageUrls = imgUrl;
        }

        public void ResizeImage(ImageView imageViewer)
        {
            var height = imageViewer.Height;
            var width = Convert.ToInt32(height / 1.457);
            imageViewer.LayoutParameters.Height = height;
            imageViewer.LayoutParameters.Width = width;
            imageViewer.RequestLayout();
        }
	}
}