using Android.App;
using Kuni.Core;
using Android.Content;
using Android.Widget;
using Kunicardus.Droid.Fragments;
using Android.Views;
using Android.Views.InputMethods;
using MvvmCross.Droid.Support.V4;

namespace Kunicardus.Droid.Views
{
	[Activity (ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = true)]
	public class BaseResetPasswordView : MvxFragmentActivity
	{
		#region Native Methods

		protected override void OnViewModelSet ()
		{
			base.OnViewModelSet ();
			SetContentView (Resource.Layout.BaseResetPasswordView);
			ResetPasswordFragment _resetPasswordFragment = new ResetPasswordFragment ();
			var supportManager = SupportFragmentManager.BeginTransaction ();
			supportManager.AddToBackStack ("reset_password");
			supportManager.Add (Resource.Id.base_reset_fragment, _resetPasswordFragment).Commit ();
		}

		public override void OnBackPressed ()
		{
			if (SupportFragmentManager.BackStackEntryCount > 1) {
				SupportFragmentManager.PopBackStack ();
			} else {
				try {
					Intent intent = new Intent (this, typeof(LoginAuthView));
					intent.AddFlags (ActivityFlags.ClearTask);
					StartActivity (intent);
					this.Finish ();
				} catch {
					Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
				}
			}
		}

		public void Open (UserModelForResetPassword userData)
		{
			ResetPasswordSMSVerificationFragment resetSMSVerification = new ResetPasswordSMSVerificationFragment (userData);
			var ifAdded = SupportFragmentManager.FindFragmentByTag ("reset_sms_password");
			if (ifAdded == null && !resetSMSVerification.IsAdded) {
				var supportTransaction = SupportFragmentManager.BeginTransaction ();
				supportTransaction.AddToBackStack ("reset_sms_password");
				supportTransaction.Add (Resource.Id.base_reset_fragment, resetSMSVerification).Commit ();
			}
		}

		#endregion

		public void HideKeyboard ()
		{
			View View = CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.None);
			}
		}
	}
}