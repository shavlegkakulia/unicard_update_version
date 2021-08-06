using System;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class Toast : UIControl
	{
		public event EventHandler Tapped;

		private nfloat _top = 70f;
		private nfloat _height = 70;
		private nfloat _left = 5f;
		UILabel _textLabel;

		public string Text {
			get {
				return _textLabel.Text;
			}
			set {				
				AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
				nfloat oldWidth = app.Window.Frame.Width - (2 * _left + 4);
				nfloat oldHeight = _height - 4;
				_textLabel.Text = value;
				_textLabel.Frame = new CoreGraphics.CGRect (2, 2, oldWidth, oldHeight);
				_textLabel.SizeToFit ();
				if (_textLabel.Frame.Width < oldWidth && _textLabel.Frame.Height < oldHeight) {
					_textLabel.Frame = new CoreGraphics.CGRect (2, 2, oldWidth, oldHeight);
				} 
				_textLabel.TextAlignment = UITextAlignment.Center;
				this.Frame = new CoreGraphics.CGRect (_left, _top, app.Window.Frame.Width - (2 * _left),
					(_height > _textLabel.Frame.Height + 4 ? _height : _textLabel.Frame.Height + 4));
				_bg.Frame = new CGRect (0, 0, this.Frame.Width, this.Frame.Height);

			}
		}

		UIView _bg;

		public Toast ()
		{
			AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;

			this.BackgroundColor = UIColor.Clear;

			this.Layer.CornerRadius = 6;
			this.Alpha = 0;
			_textLabel = new UILabel ();
			_textLabel.TextColor = UIColor.White;
			_textLabel.Font = UIFont.SystemFontOfSize (16);//UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			_textLabel.LineBreakMode = UILineBreakMode.WordWrap;
			_textLabel.Lines = 0;

			_textLabel.TextAlignment = UITextAlignment.Center;

			this.Frame = new CoreGraphics.CGRect (_left, _top, app.Window.Frame.Width - (2 * _left), _height);
			_bg = new UIView (new CGRect (0, 0, app.Window.Frame.Width - (2 * _left), _height));
			_bg.Layer.CornerRadius = 8;
			_bg.BackgroundColor = UIColor.Black;
			_bg.Alpha = 0.6f;
			_bg.UserInteractionEnabled = true;
			_bg.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				if (Tapped != null) {
					Tapped (this, null);
				}
			}));

			this.AddSubview (_bg);
			this.AddSubview (_textLabel);

			this.UserInteractionEnabled = true;
		}
	}
}