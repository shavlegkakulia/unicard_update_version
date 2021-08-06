using System;
using UIKit;
using CoreGraphics;

namespace iCunOS.BillBoards
{
	public static class UIButtonExtensions
	{
		public static void DropShadowDependingOnBGColor (this UIButton button)
		{
			button.Layer.ShadowColor = button.BackgroundColor.CGColor;
			button.Layer.ShadowOpacity = 0.8f;
			button.Layer.ShadowRadius = 1f;
			button.Layer.ShadowOffset = new CGSize (1, 1);
		}
	}
}

