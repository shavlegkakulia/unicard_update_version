using System;
using Android.App;
using Android.Views.InputMethods;

namespace Kunicardus.Droid
{
	public class ActivityHelpers
	{
		public static void HideSoftKeyboard (Activity activity)
		{
			InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService (Activity.InputMethodService);
			inputMethodManager.HideSoftInputFromWindow (activity.CurrentFocus.WindowToken, 0);
		}
	}
}

