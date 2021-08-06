using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Core.Models.DB;
using System.ComponentModel.Design;
using Kunicardus.Touch.Helpers.UI;
using Foundation;

namespace Kunicardus.Touch
{
	public class SettingsPinView
	{
		public event EventHandler<string> PinProgressFinished;

		public SettingsPinView (string pageTitle, UIView v)
		{
			_pageTitle = pageTitle;
			_view = v;
		}

		private string _pageTitle;
		private UIView _view;
		private UILabel first, second, third, forth;
		private UITextField _all;

		public void InitUI (UITextField all, nfloat statusBarHeight)
		{
			nfloat leftPading = 90f;
			nfloat topPadding = 80f + statusBarHeight;
			float labelWidth = 25f;
			float labelHeight = 40f;
			nfloat innerPadding = (_view.Frame.Width - (leftPading * 2f) - (4f * labelWidth)) / 3f;

			_all = all;

			var pageTitle = new UILabel (new CGRect (
				                0, topPadding - 50f, _view.Frame.Width, 30f
			                ));
			pageTitle.TextAlignment = UITextAlignment.Center;
			pageTitle.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3f);
			pageTitle.Text = _pageTitle;

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

			_view.AddSubview (pageTitle);
			_view.AddSubview (first);
			_view.AddSubview (second);
			_view.AddSubview (third);
			_view.AddSubview (forth);

			_all.EditingChanged += TextChanged;
			_all.ShouldChangeCharacters = (UITextField t, NSRange range, string replacementText) => {
				nint newLength = t.Text.Length + replacementText.Length - range.Length;
				return (newLength <= 4);
			};
			_all.AutocorrectionType = UITextAutocorrectionType.No;
			_all.KeyboardType = UIKeyboardType.NumberPad;
		}


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
					if (PinProgressFinished != null) {
						PinProgressFinished (this, _all.Text);
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
	}
}

