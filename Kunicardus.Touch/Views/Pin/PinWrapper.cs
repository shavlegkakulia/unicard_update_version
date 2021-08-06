using System;
using UIKit;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class PinWrapper : UIView
	{
		public PinWrapper (CGRect frame) : base (frame)
		{
			UIView transparentBGView = new UIView (frame);
			transparentBGView.BackgroundColor = UIColor.Black;
			transparentBGView.Alpha = 0.4f;
			AddSubview (transparentBGView);
		}
	}
}

