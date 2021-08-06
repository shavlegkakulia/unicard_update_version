using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using Kuni.Core;
using Android.Views.InputMethods;
using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid
{
	public class SMSVerificationFragment : MvxFragment
	{
		int _index;
		BaseRegisterView _activity;
		private bool _otpWasCorrect;
		private SMSVerificationViewModel currentViewModel;
		private string unicardNumber;

		public bool OTPWasCorrect {
			get { return _otpWasCorrect; }
			set {
				var baseRegisterViewModel = _activity.ViewModel as BaseRegisterViewModel;
				if (currentViewModel.EmailRegistrationIsInProgress && currentViewModel.OtpWasCorrect
				    && string.IsNullOrEmpty (baseRegisterViewModel.FBRegisterViewModelProperty.FBId)) {
					Bundle b = new Bundle ();
					b.PutString ("unicard_number", unicardNumber);
					(_activity as BaseRegisterView).OpenEmailRegistrationFragment (b);
				}
				_otpWasCorrect = value;
			}
		}

		public bool RegisrationCompleted {
			get{ return false; }
			set { 
				if (value) {
					var dialog = new Dialog (_activity,
						             Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
					dialog.SetContentView (Resource.Layout.RegistrationComplete);
					dialog.SetCancelable (false);

					dialog.FindViewById<BaseButton> (Resource.Id.btnOK).Click += (o, e) => {
						currentViewModel.RegistrationCompletedClick ();
						dialog.Dismiss ();
					};
					dialog.Show ();
				}
			}
		}



		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.SMSVerificationView, null);
			currentViewModel = this.ViewModel as SMSVerificationViewModel;

			var layout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			layout.Click += HideKeyboard;

			_activity = ((BaseRegisterView)base.Activity);

			_activity.Window.SetSoftInputMode (SoftInput.AdjustPan);

			var backToolbar = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backLayout = backToolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);
			backLayout.Click += BackClick;

			var toolbarTitle = backToolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			var set = this.CreateBindingSet<SMSVerificationFragment , Kuni.Core.SMSVerificationViewModel> ();
			set.Bind (this).For (p => p.OTPWasCorrect).To (vm => vm.OtpWasCorrect);
			set.Bind (this).For (p => p.RegisrationCompleted).To (vm => vm.RegisrationCompleted);

			set.Apply ();

			currentViewModel.NewCardRegistration = (this.Activity as BaseRegisterView).NewCardRegistration;
			string phoneNumber = "";
			unicardNumber = "";
			if (Arguments != null) {
				phoneNumber = Arguments.GetString ("phone_number");
				currentViewModel.Mask = Arguments.GetBoolean ("mask_number", false);
				unicardNumber = Arguments.GetString ("unicard_number");
				currentViewModel.UnicardNumber = unicardNumber;
				currentViewModel.EmailRegistrationIsInProgress = Arguments.GetBoolean ("email_register_in_progress", false);
				currentViewModel.FBRegistrationIsInProgress = Arguments.GetBoolean ("fb_registration_in_progress", false);
				currentViewModel.PhoneNumber = phoneNumber;
			}
			//mask the number
			var phoneNumberTextView = View.FindViewById<BaseTextView> (Resource.Id.sms_phone_number);
			if (!currentViewModel.Mask)
				phoneNumberTextView.Text = phoneNumber;
			else {
				phoneNumberTextView.Text = phoneNumber.Substring (3, 3) + "xxxx" + phoneNumber.Substring (10, 2);
				View.FindViewById<BaseTextView> (Resource.Id.phonehint).Visibility = ViewStates.Visible;
			}
			ImageButton continueButton = View.FindViewById<ImageButton> (Resource.Id.continue_sms_verification);

			return View;
		}

		private void HideKeyboard (object sender, EventArgs e)
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.NotAlways);
			}
		}

		void BackClick (object sender, EventArgs e)
		{
			HideKeyboard (this, null);
			_activity.BackToPreviousFragment ();
		}
	}
}