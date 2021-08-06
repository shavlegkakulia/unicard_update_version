using Android.Widget;
using MvvmCross;
using Android.App;
using Kuni.Core.Plugins.UIDialogPlugin;
using MvvmCross.Platform.Droid.Platform;
using Plugin.CurrentActivity;

namespace Kunicardus.Droid.Plugins.UIDialogPlugin
{
	public class DroidUIDialogPlugin : IUIDialogPlugin
	{
		Activity activity;
		ProgressDialog _progressDialog;

		public DroidUIDialogPlugin ()
		{
			//var top = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity> ();
			//activity = // top.Activity;
		}

		public void ShowToast (string msg)
		{
			var toast = Toast.MakeText (Application.Context, msg, ToastLength.Short);
//			toast.SetGravity (Android.Views.GravityFlags.Center, 0, 0);
			toast.Show ();
		}

		public void ShowProgressDialog (string message)
		{
			//_progressDialog = ProgressDialog.Show (Application.Context, null, message);
		}

		public void ShowAlertDialog (string title, string message)
		{
		}

		public void DismissProgressDialog ()
		{
			if (_progressDialog != null) {
				_progressDialog.Dismiss ();
			}
		}
	}
}

