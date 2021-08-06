using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class UserDiscountsExpandableList : UIView
	{
		public UIButton Button;

		public nfloat contentHeight {
			get;
			set;
		}

		public Action AfterClick { get; set; }

		public UserDiscountsExpandableList (nfloat width, nfloat x, nfloat y, string title)
		{			
//			this.Layer.BorderColor = UIColor.Red.CGColor;
//			this.Layer.BorderWidth = 1;
			nfloat height = 30;
			this.Frame = new CGRect (x, y, width, height);
			this.ClipsToBounds = true;
			UIImageView image = new UIImageView (ImageHelper.MaxResizeImage (UIImage.FromBundle ("gift"), 0, 26));
			image.SizeToFit ();
			image.Frame = new CGRect (0, 2, image.Frame.Width, image.Frame.Height);
			this.AddSubview (image);

			Button = new UIButton (UIButtonType.RoundedRect);
			Button.Frame = new CGRect (image.Frame.Right + 10, 0, this.Frame.Width - image.Frame.Right - 10, height);
			Button.BackgroundColor = UIColor.Clear;
			Button.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
			Button.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			Button.TintColor = UIColor.Black;
			Button.SetImage (UIImage.FromBundle ("arrow_down_32").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			Button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			Button.SetTitle (title, UIControlState.Normal);
			Button.TitleEdgeInsets = 
				new UIEdgeInsets (0, 
				-Button.ImageView.Frame.Size.Width, 0, Button.ImageView.Frame.Size.Width);
			Button.ImageEdgeInsets = 
				new UIEdgeInsets (0, (width - image.Frame.Width - 10) - Button.ImageView.Frame.Width, 0, -((width - image.Frame.Width - 10) - Button.ImageView.Frame.Width));

			Button.TouchUpInside += delegate {
				if (this.Frame.Height <= height) {
					UIView.Animate (0.2f, () => {
						this.Frame = new CGRect (this.Frame.X, this.Frame.Y, this.Frame.Width, contentHeight);
						Button.SetImage (UIImage.FromBundle ("arrow_up_32").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
						if (AfterClick != null)
							AfterClick.Invoke ();							
					}, () => {

					});
				} else {
					UIView.Animate (0.2f, () => {
						this.Frame = new CGRect (this.Frame.X, this.Frame.Y, this.Frame.Width, height);
						Button.SetImage (UIImage.FromBundle ("arrow_down_32").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
						if (AfterClick != null)
							AfterClick.Invoke ();			
					}, () => {

					});
				}					
			};

			this.AddSubview (Button);
		}
	}
}

