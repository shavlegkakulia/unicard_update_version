using System;
using UIKit;
using Kunicardus.Core.Models.DB;
using System.Collections.Generic;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class ChooseDeliveryMethod : UIView
	{
		public event EventHandler<DeliveryMethod> DeliveryMethodSelected;

		public ChooseDeliveryMethod (List<DeliveryMethod> deliveryMethods)
		{
			nfloat height = 100;
			this.Frame = new CoreGraphics.CGRect (10, (BaseMvxViewController.APP.Window.Frame.Height - height) / 2.0f, BaseMvxViewController.APP.Window.Frame.Width - 20, height);
			this.BackgroundColor = UIColor.Clear.FromHexString ("#f5f5f5");
			this.Layer.CornerRadius = 5f;
			this.Layer.BorderColor = UIColor.Clear.FromHexString ("#a6a6a6", 0.7f).CGColor;

			UILabel title = new UILabel ();
			title.TextColor = UIColor.Black;
			title.TextAlignment = UITextAlignment.Center;
			title.Text = ApplicationStrings.DeliveryForm;
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 20);
			title.SizeToFit ();
			title.Frame = new CGRect (0, 15, this.Frame.Width, title.Frame.Height);
			this.AddSubview (title);

			nfloat tmpTop = title.Frame.Bottom + 20;
			nfloat buttonLeftPadding = 10;
			nfloat buttonWidth = this.Frame.Width - (buttonLeftPadding * 2);
			foreach (var item in deliveryMethods) {
				DeliveryMethodButton deliveryMethod = new DeliveryMethodButton (new CGRect (buttonLeftPadding, tmpTop, buttonWidth, 38));
				deliveryMethod.Layer.CornerRadius = 5f;
				UIButton button = new UIButton (UIButtonType.RoundedRect);
				button.Frame = new CGRect (0, 0, buttonWidth, 38);
				button.Layer.CornerRadius = 5f;
				button.BackgroundColor = UIColor.Clear.FromHexString ("#ebebeb");
				button.SetTitleColor (UIColor.Black, UIControlState.Normal);
				button.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 15);
				button.SetTitle (item.Name, UIControlState.Normal);
				deliveryMethod.Button = button;
				deliveryMethod.DeliveryMethod = item;
				deliveryMethod.Clicked += delegate(object sender, EventArgs e) {
					var b = sender as DeliveryMethodButton;
					if (DeliveryMethodSelected != null) {
						DeliveryMethodSelected (this, b.DeliveryMethod);
					}
				};
				this.AddSubview (deliveryMethod);
				tmpTop = deliveryMethod.Frame.Bottom + 5;
			}
			tmpTop += 15;
			height = tmpTop;
			this.Frame = new CoreGraphics.CGRect (10, (BaseMvxViewController.APP.Window.Frame.Height - height) / 2.0f, BaseMvxViewController.APP.Window.Frame.Width - 20, height); 
		}
	}
}

