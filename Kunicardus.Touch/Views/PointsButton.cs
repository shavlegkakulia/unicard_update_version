using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Foundation;

namespace Kunicardus.Touch
{
	public class PointsButton : UIControl
	{
		public UIButton Button {
			get;
			set;
		}

		public string Score {
			get {
				return Button.Title (UIControlState.Normal);
			}
			set {
				Button.SetTitle (value, UIControlState.Normal);
			}
		}

		public PointsButton (CGRect frame) : base (frame)
		{
			Button = new UIButton (UIButtonType.System);
			this.AddSubview (Button);
			Button.Frame = new CGRect (0, 0, frame.Width, frame.Height);
			Button.BackgroundColor = UIColor.White;
			Button.TintColor = UIColor.White;
			Button.Layer.BorderWidth = 3f;
			Button.Layer.BorderColor = UIColor.Clear.FromHexString ("#f5c959").CGColor;
			Button.Layer.CornerRadius = Button.Frame.Width / 2.0f;
			Button.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
			Button.Font = UIFont.BoldSystemFontOfSize (UIFont.ButtonFontSize - (Screen.IsTall ? 0 : 4));
			Button.TitleEdgeInsets = new UIEdgeInsets (-13, 0, 0, 0);


			UIView tmpView = new UIView ();
			tmpView.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.Orange);
			tmpView.Layer.CornerRadius = 3f;

			UILabel score = new UILabel ();
			score.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, (Screen.IsTall ? 12 : 10));
			score.BackgroundColor = UIColor.Clear;
			score.TextColor = UIColor.White;
			score.Text = ApplicationStrings.Score;
			score.SizeToFit ();
			score.Frame = new CGRect (2, 1, score.Frame.Width, score.Frame.Height);
			tmpView.Frame = new CGRect (0, 0, score.Frame.Width + 4, score.Frame.Height + 1);
			tmpView.AddSubview (score);

			var tmpViewWrapper = new UIView (
				                     new CGRect (
					                     (Button.Frame.Width - tmpView.Frame.Width) / 2.0f,
					                     Button.Frame.Height - tmpView.Frame.Height - 16,
					                     tmpView.Frame.Width,
					                     tmpView.Frame.Height));
			tmpViewWrapper.BackgroundColor = UIColor.White;
			tmpViewWrapper.AddSubview (tmpView);

			Button.AddSubview (tmpViewWrapper);
//			Button.SetImage (img.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
//			Button.ImageEdgeInsets = new UIEdgeInsets (30, 20, 0, 0);


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

