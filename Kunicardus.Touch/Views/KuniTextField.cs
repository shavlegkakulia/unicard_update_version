using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;
using Foundation;

namespace Kunicardus.Touch
{
	public class KuniTextField : UIView
	{
		private UIView _line;
		private UITextField _field;
		private string _placeHolder;
		private KeyboardTopBar _keyboardBar;
		private UIKeyboardType? _keyboardType;

		#region Properties

		public UIView Line {
			get {
				return _line;
			}
			set {
				_line = value;
			}
		}

		public UITextField Field {
			get {
				return _field;
			}
			set {
				_field = value;
			}
		}

		private int _textMaxLength;

		public int TextMaxLength {
			get {
				return _textMaxLength;
			}
			set {
				_textMaxLength = value;
				if (value > 0) {					
					_field.ShouldChangeCharacters = (textField, range, replacementString) => {
						var newLength = textField.Text.Length + replacementString.Length - range.Length;
						return newLength <= value;
					};
				}
			}
		}

		#endregion

		public KuniTextField (CGRect frame, string placeHolder = "", UIKeyboardType? keyboardType = null, UIImage icon = null) : base (frame)
		{
			_placeHolder = placeHolder;
			_keyboardType = keyboardType;
			_line = new UIView ();
			_field = new UITextField ();
			_keyboardBar = new KeyboardTopBar ();
		}

		public void SetNextField (KuniTextField nextField)
		{
			_keyboardBar.NextEnabled = true;
			_keyboardBar.OnNext += delegate {
				_field.ResignFirstResponder ();
				nextField.Field.BecomeFirstResponder ();
			};
			_field.ShouldReturn = delegate {
				_field.ResignFirstResponder ();
				nextField.Field.BecomeFirstResponder ();
				return true;
			};
		}

		public void SetPrevField (KuniTextField prevField)
		{
			_keyboardBar.PreviousEnabled = true;
			_keyboardBar.OnPrev += delegate {
				_field.ResignFirstResponder ();
				prevField.Field.BecomeFirstResponder ();
			};
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			_field.Frame = new CGRect (0, 0, Frame.Width, Frame.Height - 3);
			_field.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			_field.TextColor = UIColor.White;
			_field.AutocapitalizationType = UITextAutocapitalizationType.None;
			_field.AttributedPlaceholder = 
				new NSAttributedString (_placeHolder, UIFont.FromName (Styles.Fonts.BPGExtraSquare, UIFont.LabelFontSize - 3), UIColor.Clear.FromHexString (Styles.Colors.PlaceHolderColor));
			_field.TintColor = UIColor.White;
			_field.InputAccessoryView = _keyboardBar;
			_keyboardBar.OnDone += delegate {
				this._field.ResignFirstResponder ();
			};
			if (_keyboardType != null) {
				_field.KeyboardType = _keyboardType.Value;
			}


			AddSubview (_field);
			_line.BackgroundColor = UIColor.Clear.FromHexString ("#b9f050");
			_line.Frame = new CGRect (0, Frame.Height - 1.5f, Frame.Width, 1.5f);
			AddSubview (_line);
		}

		public void SecureField (bool secured)
		{
			_field.SecureTextEntry = secured;
		}
	}
}

