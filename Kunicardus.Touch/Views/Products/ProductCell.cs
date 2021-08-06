using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Kunicardus.Core.Models.DB;
using Kunicardus.Touch.Helpers.UI;
using Accelerate;

namespace Kunicardus.Touch
{
	[Register ("ProductCell")]
	public class ProductCell : MvxCollectionViewCell
	{
		public static readonly NSString Key = new NSString ("ProductCell");

		public ProductCell (IntPtr handle) : base ("", handle)
		{
			CreateLayout ();
			CreateBindings ();
		}

		UILabel _title;
		PointsButton _pointsButton;
		private UIImageView _image;
		private MvxImageViewLoader _loader;
		UIView discountView;
		UILabel discountValue;

		private void CreateLayout ()
		{
			this.BackgroundColor = UIColor.Clear;
//			this.Layer.BorderColor = UIColor.Red.CGColor;
//			this.Layer.BorderWidth = 1;

			nfloat pointsWidth = 70f;
			if (!Screen.IsTall) {
				pointsWidth = 60f;
			}

			nfloat padding = 3f;
			nfloat borderWidth = 1.9f;
			nfloat imageFramWidth = ContentView.Frame.Width - (padding * 2f);
			nfloat imageFrameHeight = ContentView.Frame.Width - (padding * 2f);
			nfloat titleFrameWidth = ContentView.Frame.Width - (padding * 2f);
			nfloat titleFrameHeight = ContentView.Frame.Height - padding - imageFrameHeight + borderWidth;

			UIView imageFrame = new UIView (new CGRect (padding, padding, imageFramWidth, imageFrameHeight));
			imageFrame.BackgroundColor = UIColor.White;
			imageFrame.Layer.BorderColor = UIColor.Clear.FromHexString ("#e6eced").CGColor;
			imageFrame.Layer.BorderWidth = borderWidth;
			ContentView.AddSubview (imageFrame);
					

			_image = new UIImageView (new CGRect (0, 0, imageFrame.Frame.Width, imageFrame.Frame.Height));
			_image.Layer.MasksToBounds = false;
			_image.ClipsToBounds = true;
			_image.ContentMode = UIViewContentMode.ScaleAspectFit;
			_loader = new MvxImageViewLoader (() => _image);
			imageFrame.AddSubview (_image);

			// Discount
			discountView = new UIView (new CGRect (imageFramWidth - borderWidth - 35, borderWidth, 35, 25));
			discountView.BackgroundColor = UIColor.Clear.FromHexString ("#f5a72b");
			discountValue = new UILabel (new CGRect (0, 0, discountView.Frame.Width, discountView.Frame.Height));
			discountValue.TextColor = UIColor.White;
			discountValue.TextAlignment = UITextAlignment.Center;
			discountValue.Font = UIFont.BoldSystemFontOfSize (14);
			discountView.AddSubview (discountValue);
			imageFrame.AddSubview (discountView);


			UIView titleFrame = new UIView (new CGRect (padding, imageFrame.Frame.Bottom - borderWidth, titleFrameWidth, titleFrameHeight));
			titleFrame.BackgroundColor = UIColor.White;
			titleFrame.Layer.BorderColor = UIColor.Clear.FromHexString ("#e6eced").CGColor;
			titleFrame.Layer.BorderWidth = borderWidth;
			ContentView.AddSubview (titleFrame);

			_pointsButton = new PointsButton (
				new CoreGraphics.CGRect (0, 0, pointsWidth, pointsWidth));		
			ContentView.AddSubview (_pointsButton);

			_title = new UILabel ();
			_title.Frame = new CGRect (2, 2, titleFrame.Frame.Width - 4, titleFrame.Frame.Height - 4);
			_title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			_title.LineBreakMode = UILineBreakMode.WordWrap;
			_title.Lines = 0;
			_title.TextAlignment = UITextAlignment.Center;
			titleFrame.AddSubview (_title);


		}

		private void CreateBindings ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<ProductCell, ProductsInfo> ();
				set.Bind (_pointsButton).For (b => b.Score).To (vm => vm.DiscountPrice);
				set.Bind (_title).To (vm => vm.ProductName);
				set.Bind (discountValue).To (vm => vm.DiscountPercentString);
				set.Bind (_loader).To (vm => vm.ImageURL);
				set.Bind (discountView).For (x => x.Hidden).To (vm => vm.DiscountPercent).WithConversion ("ProductPercent");
				set.Apply ();
			});
		}
	}
}

