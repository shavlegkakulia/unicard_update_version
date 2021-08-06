using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class PasswordHint:UILabel
	{
		public PasswordHint (CGRect frame) : base (frame)
		{
			this.Lines = 0;
			this.LineBreakMode = UILineBreakMode.WordWrap;
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 7f);
			this.Text = ApplicationStrings.PasswordHint;
			this.TextColor = UIColor.White;
			this.SizeToFit ();
		}
	}
}

