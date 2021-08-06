using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class RegistrationNextButton : UIButton
	{
		public RegistrationNextButton (UIButtonType type) : base (type)
		{
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 14);
			this.BackgroundColor = UIColor.White;
			this.Layer.BorderColor = UIColor.Clear.FromHexString ("#b9ed5c").CGColor;
			this.Layer.BorderWidth = 4;
			this.Layer.CornerRadius = Styles.RegistrationNextButton.Width / 2.0f;
			this.SetTitleColor (UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen), UIControlState.Normal);
			this.SetTitle (ApplicationStrings.Next, UIControlState.Normal);
			this.TintColor = UIColor.White;
		}
	}
}

