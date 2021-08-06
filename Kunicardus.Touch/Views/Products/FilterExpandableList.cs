using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class FilterExpandableList : UIView
	{
		public UIButton Button;

		public nfloat contentHeight {
			get;
			set;
		}

		public Action AfterClick { get; set; }

		public FilterExpandableList (nfloat width, nfloat x, nfloat y, string title)
		{			
			nfloat height = 30;
			this.Frame = new CGRect (x, y, width, height);
			this.ClipsToBounds = true;
			Button = new UIButton (UIButtonType.RoundedRect);
			Button.Frame = new CGRect (0, 0, width, height);
			Button.BackgroundColor = UIColor.Clear;
			Button.SetTitleColor (UIColor.White, UIControlState.Normal);
			Button.TintColor = UIColor.White;
			Button.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 13);
			Button.SetImage (UIImage.FromBundle ("arrow_down_32"), UIControlState.Normal);
			Button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			Button.SetTitle (title, UIControlState.Normal);
			Button.TitleEdgeInsets = 
				new UIEdgeInsets (0, 
				-Button.ImageView.Frame.Size.Width + 10, 0, Button.ImageView.Frame.Size.Width - 10);
			Button.ImageEdgeInsets = 
				new UIEdgeInsets (0, width - Button.ImageView.Frame.Width, 0, -(width - Button.ImageView.Frame.Width));

			Button.TouchUpInside += delegate {
				if (this.Frame.Height <= height) {
					UIView.Animate (0.2f, () => {
						this.Frame = new CGRect (this.Frame.X, this.Frame.Y, this.Frame.Width, contentHeight);
						Button.SetImage (UIImage.FromBundle ("arrow_up_32"), UIControlState.Normal);
						if (AfterClick != null)
							AfterClick.Invoke ();							
					}, () => {
						
					});
				} else {
					UIView.Animate (0.2f, () => {
						this.Frame = new CGRect (this.Frame.X, this.Frame.Y, this.Frame.Width, height);
						Button.SetImage (UIImage.FromBundle ("arrow_down_32"), UIControlState.Normal);
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

