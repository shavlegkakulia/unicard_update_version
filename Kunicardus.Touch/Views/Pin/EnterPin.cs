using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Foundation;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Kunicardus.Touch
{
	public class EnterPin : UIView
	{
		#region Private Variables

		private UILabel first, second, third, forth;
		private UITextField _all;
		private bool _isConfirming;
		private string _pin, _confirmPin;
		private UILabel _pageTitle;
		private PinStatus _pinStatus;

		#endregion

		#region Constructor Implementation

		public EnterPin (CGRect frame, PinStatus pinStatus) : base (frame)
		{
			nfloat leftPading = 70f;
			nfloat topPadding = 30f;
			float labelWidth = 25f;
			float labelHeight = 40f;
			nfloat innerPadding = (this.Frame.Width - (leftPading * 2f) - (4f * labelWidth)) / 3f;

			this.BackgroundColor = UIColor.White;
			this.Layer.CornerRadius = 12f;
			this.Layer.BorderWidth = 3.5f;
			this.Layer.BorderColor = UIColor.Clear.FromHexString ("#a6a6a6", 0.7f).CGColor;

			_pinStatus = pinStatus;
			_all = new UITextField (new CGRect (0, 0, 1, 1));
			_pageTitle = new UILabel (new CGRect (
				0, topPadding, this.Frame.Width, 30f
			));
			_pageTitle.TextAlignment = UITextAlignment.Center;
			_pageTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
			if (_pinStatus == PinStatus.ShouldEnterPin)
				_pageTitle.Text = ApplicationStrings.enter_pin;
			else
				_pageTitle.Text = ApplicationStrings.enter_new_pin;

			if (_pinStatus == PinStatus.ShouldEnterPin)
				topPadding += _pageTitle.Layer.Frame.Bottom - 10f;
			else
				topPadding += _pageTitle.Layer.Frame.Bottom;
			first = new UILabel (new CGRect (leftPading, topPadding, labelWidth, labelHeight));
			first.Text = "─";
			first.Font = UIFont.SystemFontOfSize (UIFont.ButtonFontSize + 35);
			first.TextAlignment = UITextAlignment.Center;


			leftPading = first.Frame.Right + innerPadding;
			second = new UILabel (new CGRect (leftPading, topPadding, labelWidth, labelHeight));
			second.Text = "─";
			second.Font = UIFont.SystemFontOfSize (UIFont.ButtonFontSize + 35);
			second.TextAlignment = UITextAlignment.Center;


			leftPading = second.Frame.Right + innerPadding;
			third = new UILabel (new CGRect (leftPading, topPadding, labelWidth, labelHeight));
			third.Text = "─";
			third.Font = UIFont.SystemFontOfSize (UIFont.ButtonFontSize + 35);
			third.TextAlignment = UITextAlignment.Center;

			leftPading = third.Frame.Right + innerPadding;
			forth = new UILabel (new CGRect (leftPading, topPadding, labelWidth, labelHeight));
			forth.Text = "─";
			forth.Font = UIFont.SystemFontOfSize (UIFont.ButtonFontSize + 35);
			forth.TextAlignment = UITextAlignment.Center;

			_all.EditingChanged += TextChanged;
			_all.ShouldChangeCharacters = (UITextField t, NSRange range, string replacementText) => {
				nint newLength = t.Text.Length + replacementText.Length - range.Length;
				return (newLength <= 4);
			};
			_all.AutocorrectionType = UITextAutocorrectionType.No;
			_all.KeyboardType = UIKeyboardType.NumberPad;
			_all.UserInteractionEnabled = true;
			_all.BecomeFirstResponder ();

			this.AddSubview (_pageTitle);
			this.AddSubview (first);
			this.AddSubview (second);
			this.AddSubview (third);
			this.AddSubview (forth);
			this.AddSubview (_all);
		}

		#endregion

		#region Methods

		void TextChanged (object sender, EventArgs e)
		{
			switch (_all.Text.Length) {
			case 0:
				{
					first.Text = "─";
					second.Text = "─";
					third.Text = "─";
					forth.Text = "─";
					break;
				}
			case 1:
				{
					first.Text = "•";
					second.Text = "─";
					third.Text = "─";
					forth.Text = "─";
					break;
				}
			case 2:
				{
					first.Text = "•";
					second.Text = "•";
					third.Text = "─";
					forth.Text = "─";
					break;
				}
			case 3:
				{
					first.Text = "•";
					second.Text = "•";
					third.Text = "•";
					forth.Text = "─";
					break;
				}
			case 4:
				{
					first.Text = "•";
					second.Text = "•";
					third.Text = "•";
					forth.Text = "•";
					if (_pinStatus == PinStatus.ShouldEnterPin) {
						_pin = _all.Text;
						if (SetPinFinished != null)
							SetPinFinished (this, _pin);
					} else if (!_isConfirming) {
						_pin = _all.Text;
						_all.Text = string.Empty;
						ClearDigits ();
						_isConfirming = true;
						_pageTitle.Text = ApplicationStrings.repeat_new_pin;
					} else {
						_confirmPin = _all.Text;
						if (_pin == _confirmPin) {
							if (SetPinFinished != null)
								SetPinFinished (this, _pin);
						} else if (ConfirmWasIncorrect != null)
							ConfirmWasIncorrect (this, true);
					}
					break;
				}
			}

		}


		public void ClearDigits ()
		{
			_all.Text = string.Empty;
			first.Text = "─";
			second.Text = "─";
			third.Text = "─";
			forth.Text = "─";
		}

		#endregion

		#region Event properties

		public event EventHandler<string> SetPinFinished;
		public event EventHandler<bool> ConfirmWasIncorrect;

		#endregion
	}
}

