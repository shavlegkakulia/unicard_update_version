using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using Kuni.Core;
using MvvmCross;
using Kunicardus.Droid.Views;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid.Fragments
{
	[Activity (Label = "Unicard", LaunchMode = LaunchMode.SingleTask)]
	public class ResetPasswordSMSVerificationFragment : MvxFragment
	{
		#region private variables

		private string _phoneNumber;
		private string _userId;
		private bool _otpWasSentSuccesfully;

		#endregion

		#region Constructor Implementation

		public ResetPasswordSMSVerificationFragment (UserModelForResetPassword userData)
		{
			_phoneNumber = userData.UserPhoneNumber.Substring (3);
			_userId = userData.UserId;
			if (this.ViewModel == null)
                this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCProvider.IoCConstruct<ResetPasswordSMSVerificationViewModel>();
			(this.ViewModel as ResetPasswordSMSVerificationViewModel).UserPhoneNumber = _phoneNumber;
			(this.ViewModel as ResetPasswordSMSVerificationViewModel).UserID = _userId;
			(this.ViewModel as ResetPasswordSMSVerificationViewModel).UserName = userData.UserName;
			(this.ViewModel as ResetPasswordSMSVerificationViewModel).NewPassword = userData.NewPassword;
			_otpWasSentSuccesfully = (this.ViewModel as ResetPasswordSMSVerificationViewModel).SendOTP (_userId, _phoneNumber);

		}

		#endregion

		#region Native Methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.ResetPasswordSMSVerification, null);
			var phoneTextView = View.FindViewById<BaseTextView> (Resource.Id.sms_phone_number);
			if (_otpWasSentSuccesfully)
				phoneTextView.Text = _phoneNumber.Substring (0, 3) + "xxxx" + _phoneNumber.Substring (7);
			else
				phoneTextView.Text = string.Empty;
			
			var backButton = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backButtonLayout = backButton.FindViewById<LinearLayout> (Resource.Id.back_button_layout);

			var currentLayout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			currentLayout.Click += (sender, e) => {
				(this.Activity as BaseResetPasswordView).HideKeyboard ();
			};

			backButtonLayout.Click += (sender, e) => {
				this.Activity.OnBackPressed ();
			};
			var toolbarTitle = backButton.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.resetPassword);
			return View;
		}

		#endregion
	}
}