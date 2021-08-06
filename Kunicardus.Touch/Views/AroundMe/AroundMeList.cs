using System;
using UIKit;
using CoreGraphics;
using Kunicardus.Touch.Helpers.UI;

namespace Kunicardus.Touch
{
	public class AroundMeList : UIView
	{
		public AroundMeList (CGRect frame) : base (frame)
		{
			this.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
		}
	}
}

