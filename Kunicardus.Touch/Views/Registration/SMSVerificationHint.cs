using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class SMSVerificationHint:UILabel
	{
		public SMSVerificationHint (CGRect frame) : base (frame)
		{
			this.Lines = 0;
			this.LineBreakMode = UILineBreakMode.WordWrap;
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.ButtonFontSize - 8f);
			this.Text = ApplicationStrings.SMSVerificationHint;
			this.TextColor = UIColor.White;
			this.SizeToFit ();
		}
	}
}

