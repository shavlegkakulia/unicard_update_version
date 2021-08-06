using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class ChoosePinSetup : UIView
	{
		public UIButton WithPin { get; set; }

		public UIButton WithoutPin { get; set; }

		public ChoosePinSetup (CGRect frame) : base (frame)
		{
			this.BackgroundColor = UIColor.White;
			this.Layer.CornerRadius = 12f;
			this.Layer.BorderWidth = 3.5f;
			this.Layer.BorderColor = UIColor.Clear.FromHexString ("#a6a6a6", 0.7f).CGColor;
			
			UILabel title = new UILabel (new CGRect (0, 15, this.Frame.Width, 40));
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 22);
			title.TextColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			title.Text = ApplicationStrings.PinCode;
			title.TextAlignment = UITextAlignment.Center;
			this.AddSubview (title);


			UIView tmpView = new UIView (new CGRect (0, 0, 28, 28));
			tmpView.BackgroundColor = UIColor.White;
			tmpView.Layer.BorderWidth = 2;
			tmpView.Layer.BorderColor = UIColor.Clear.FromHexString ("#cccccc").CGColor;
			tmpView.Layer.CornerRadius = 14;
			tmpView.Layer.BackgroundColor = UIColor.White.CGColor;
			UIView tmpViewWrapper = new UIView (new CGRect (0, 0, 28, 28));
			tmpViewWrapper.BackgroundColor = UIColor.White;
			tmpViewWrapper.AddSubview (tmpView);
			UIImage buttonImg = ImageFromView (tmpViewWrapper);

			WithPin = new UIButton (UIButtonType.System);
			WithPin.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
			WithPin.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			WithPin.BackgroundColor = UIColor.White;
			WithPin.SetImage (buttonImg.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			WithPin.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			WithPin.TintColor = UIColor.Red;
			WithPin.SetTitle (ApplicationStrings.RegisterPinCode, UIControlState.Normal);
			WithPin.TitleEdgeInsets = new UIEdgeInsets (0, 10, 0, 0);
			WithPin.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			WithPin.Frame = new CGRect (20, title.Frame.Bottom + 10, this.Frame.Width - 40, 40);
			this.AddSubview (WithPin);

			WithoutPin = new UIButton (UIButtonType.System);
			WithoutPin.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
			WithoutPin.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			WithoutPin.BackgroundColor = UIColor.White;
			WithoutPin.SetImage (buttonImg.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			WithoutPin.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			WithoutPin.TintColor = UIColor.Red;
			WithoutPin.SetTitle (ApplicationStrings.WithouPin, UIControlState.Normal);
			WithoutPin.TitleEdgeInsets = new UIEdgeInsets (0, 10, 0, 0);
			WithoutPin.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			WithoutPin.Frame = new CGRect (20, WithPin.Frame.Bottom + 5, this.Frame.Width - 40, 40);
			this.AddSubview (WithoutPin);



		}

		private UIImage ImageFromView (UIView view)
		{
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, view.Opaque, 0.0f);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return img;
		}
	}
}

