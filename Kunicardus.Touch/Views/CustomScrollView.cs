using System;
using UIKit;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class CustomScrollView : UIScrollView
	{
		public CustomScrollView (CGRect frame) : base (frame)
		{
			
		}

		public override bool TouchesShouldCancelInContentView (UIView view)
		{
			return true;
		}
	}
}

