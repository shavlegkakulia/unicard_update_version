using System;
using UIKit;
using Kunicardus.Core.Models.DB;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class DeliveryMethodButton : UIView
	{
		public event EventHandler Clicked;

		public DeliveryMethod DeliveryMethod {
			get;
			set;
		}

		private UIButton _button;

		public UIButton Button {
			get {
				return _button;
			}
			set {
				_button = value;
				this.AddSubview (_button);
				_button.TouchUpInside += DeliveryMethodButton_TouchUpOutside;
			}
		}

		public DeliveryMethodButton (CGRect frame) : base ()
		{
			this.Frame = frame;
			this.TintColor = UIColor.White;
		}

		void DeliveryMethodButton_TouchUpOutside (object sender, EventArgs e)
		{
			if (Clicked != null) {
				Clicked (this, null);
			}
		}
	}
}

