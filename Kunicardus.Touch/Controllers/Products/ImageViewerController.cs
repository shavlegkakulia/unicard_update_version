using System;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;

namespace Kunicardus.Touch
{
	public class ImageViewerController : UIPageViewController
	{
		#region Vars

		private string _title;
		private List<string> _imageUrls;

		#endregion

		#region Ctors

		public ImageViewerController (string title, List<string> imageUrls, 
		                              UIPageViewControllerTransitionStyle style, 
		                              UIPageViewControllerNavigationOrientation orient, 
		                              UIPageViewControllerSpineLocation spine, int? historyId = null)
			: base (style, orient, spine)
		{
			_title = title;
			_imageUrls = imageUrls;

		}

		#endregion

		#region Overrides

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitUI ();
		}

		public override void ViewWillAppear (bool animated)
		{
			UIApplication.SharedApplication.SetStatusBarHidden (true, false);
			base.ViewWillAppear (animated);
		}

		#endregion

		#region Methods

		private void InitUI ()
		{
			UILabel titleLabel = new UILabel (new CGRect (10, View.Frame.Height - 55, View.Frame.Width - 20, 50));
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.LineBreakMode = UILineBreakMode.WordWrap;
			titleLabel.Lines = 2;
			titleLabel.TextColor = UIColor.White;
			titleLabel.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 18);
			titleLabel.Text = _title;
			View.AddSubview (titleLabel);

			View.BackgroundColor = UIColor.Black;
			View.TintColor = UIColor.White;
			var close = new UIButton (UIButtonType.RoundedRect);
			close.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle ("close"), 22, 0), UIControlState.Normal);
			close.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			close.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			close.BackgroundColor = UIColor.Clear;
			close.SetTitleColor (UIColor.White, UIControlState.Normal);
			close.TintColor = UIColor.White;
			close.SizeToFit ();
			close.Frame = new CGRect (View.Frame.Width - close.Frame.Width - 20, 0, close.Frame.Width + 20, close.Frame.Height + 20);
			close.TouchUpInside += delegate {
				this.DismissModalViewController (true);
				UIApplication.SharedApplication.SetStatusBarHidden (false, false);
			};
			View.AddSubview (close);
			List<ImageItemViewController> pages = new List<ImageItemViewController> ();
			for (int i = 0; i < _imageUrls.Count; i++) {
				pages.Add (new ImageItemViewController (_imageUrls [i], i));
			}
			var imagePageDadaSource = new ImagesPageDataSource (pages);
			DataSource = imagePageDadaSource;
			if (pages.Count > 0)
				SetViewControllers (new UIViewController[] { pages [0] }, UIPageViewControllerNavigationDirection.Forward, false, s => {
				});
		}

		#endregion
	}
}

