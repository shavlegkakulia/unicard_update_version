using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class ImageItemViewController : UIViewController
	{
		public int Index { get; set; }

		public string ImageUrl { get; set; }

		public ImageItemViewController (string imageUrl, int index)
		{
			ImageUrl = imageUrl;
			Index = index;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UIImageView imageView = new UIImageView (new CGRect (0, 100, View.Frame.Width, View.Frame.Height / 2f));
			imageView.Layer.MasksToBounds = false;
			imageView.ClipsToBounds = true;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.BackgroundColor = UIColor.White;
			imageView.Image = ImageHelper.FromUrl (ImageUrl.Replace (@"\", "/"));

			View.AddSubview (imageView);
		}
	}
}

