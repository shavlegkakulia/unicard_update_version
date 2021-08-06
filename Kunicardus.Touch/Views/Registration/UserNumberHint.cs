using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class UserNumberHint:UILabel
	{
		public UserNumberHint (CGRect frame) : base (frame)
		{
			this.Lines = 0;
			this.LineBreakMode = UILineBreakMode.WordWrap;
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8f);
			this.Text = ApplicationStrings.UserNumberHint;
			this.TextColor = UIColor.White;
			this.SizeToFit ();
		}
	}
}

