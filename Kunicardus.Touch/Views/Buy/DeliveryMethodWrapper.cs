using System;
using UIKit;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class DeliveryMethodWrapper : UIView
	{
		public DeliveryMethodWrapper (CGRect frame) : base (frame)
		{
			UIView transparentBGView = new UIView (frame);
			transparentBGView.BackgroundColor = UIColor.Black;
			transparentBGView.Alpha = 0.4f;
			AddSubview (transparentBGView);

			transparentBGView.UserInteractionEnabled = true;
			UITapGestureRecognizer tap = new UITapGestureRecognizer (() => {
				UIView.Animate (0.3f, () => {
					this.Alpha = 0f;
				}, () => {
					this.RemoveFromSuperview ();
				});		
			});
			transparentBGView.AddGestureRecognizer (tap);
		}

		public void Close ()
		{
			UIView.Animate (0.3f, () => {
				this.Alpha = 0f;
			}, () => {
				this.RemoveFromSuperview ();
			});				
		}

	}
}

