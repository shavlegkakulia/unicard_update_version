using System;
using UIKit;

namespace Kunicardus.Touch.Helpers.UI
{
	public  static class Screen
	{
		public static bool IsTall {
			get {
				return UIDevice.CurrentDevice.UserInterfaceIdiom
				== UIUserInterfaceIdiom.Phone
				&& UIScreen.MainScreen.Bounds.Size.Height
				* UIScreen.MainScreen.Scale >= 1136;
			}
		}
	}
}

