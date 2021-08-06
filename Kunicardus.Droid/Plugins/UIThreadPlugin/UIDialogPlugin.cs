using System;
using Kuni.Core.Plugins.IUIThreadPlugin;
using Android.App;
using MvvmCross;
using Android.Widget;
using Android.OS;
using MvvmCross.Platform.Droid.Platform;
using Plugin.CurrentActivity;

namespace Kunicardus.Droid.Plugins.UIThreadPlugin
{
	public class UIThreadPlugin : IUIThreadPlugin
	{
		Activity activity;

		public UIThreadPlugin ()
		{
			//var top = Mvx.Resolve<IMvxAndroidCurrentTopActivity> ();
			//activity = top.Activity;
		}

		public void InvokeUIThread (Action action)
		{
            CrossCurrentActivity.Current.Activity.RunOnUiThread(() => action ());
		}

	}
}

