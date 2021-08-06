using System;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class RegistrationSubHeading : UILabel
	{
		private string _text;
		private nfloat _width;

		public RegistrationSubHeading (string text, nfloat width) : base ()
		{
			_text = text;
			_width = width;

			this.Frame = new CGRect (0, 
				Screen.IsTall ? Styles.RegistrationFormSubHeadingTop - 5 : (Styles.RegistrationFormSubHeadingTop - 25f), 
				_width, 50);

			this.Text = _text;
			this.LineBreakMode = UILineBreakMode.WordWrap;
			this.Lines = 0;
			this.SizeToFit ();
			this.Frame = new CGRect (0, 
				Screen.IsTall ? Styles.RegistrationFormSubHeadingTop : (Styles.RegistrationFormSubHeadingTop - 25f), 
				_width, this.Frame.Height);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			this.TextColor = UIColor.Clear.FromHexString (Styles.Colors.PlaceHolderColor);
			this.TextAlignment = UITextAlignment.Center;


		}
	}
}

