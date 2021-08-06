using System;
using Kuni.Core;
using Android.App;
using MvvmCross.ViewModels;
using Android.OS;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects;
using Android.Content;
using Kunicardus.Droid.Fragments;
using Xamarin.Facebook.Login;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using MvvmCross.Droid.Support.V4;

namespace Kunicardus.Droid
{
	[Activity (Label = "MainRegister", NoHistory = false, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = true)]
	[MvxViewFor (typeof(BaseRegisterViewModel))]
	public class BaseRegisterView : MvxFragmentActivity
	{
		#region  Private Variables

		private BaseRegisterViewModel mainRegisterViewModel;
		private ChooseCardExistanceViewFragment chooseCardExistanceViewFragment;
		private RegistrationFragment registerFragment;
		private UnicardInputFragment unicardInputFragment;
		private TransactionVerificationFragment transactionVerificationFragment;
		private FBRegisterFragment fbRegisterFragment;
		private SMSVerificationFragment smsVerificationFragment;
		private EmailRegistrationFragment emailRegistraionFragment;
		private Android.Support.V4.App.Fragment _ifAdded;

		#endregion

		public bool NewCardRegistration;

		protected override void OnViewModelSet ()
		{
			base.OnViewModelSet ();
			//ActionBar.Hide ();
			SetContentView (Resource.Layout.MainRegisterView);

			mainRegisterViewModel = (ViewModel as BaseRegisterViewModel);
			chooseCardExistanceViewFragment = new ChooseCardExistanceViewFragment ();
			chooseCardExistanceViewFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.ChooseCardExistanceViewModelProperty;

			registerFragment = new RegistrationFragment ();
			registerFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.RegistrationViewModelProperty;

			unicardInputFragment = new UnicardInputFragment ();
			unicardInputFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.UnicardNumberInputViewModelProperty;

			transactionVerificationFragment = new TransactionVerificationFragment ();
			transactionVerificationFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.CardVerificationViewModelProperty;

			fbRegisterFragment = new FBRegisterFragment ();
			fbRegisterFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.FBRegisterViewModelProperty;

			smsVerificationFragment = new SMSVerificationFragment ();
			smsVerificationFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.SMSVerificationViewModelProperty;

			emailRegistraionFragment = new EmailRegistrationFragment ();
			emailRegistraionFragment.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)mainRegisterViewModel.EmailRegistrationViewModelProperty;

			var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
			
			fragmentTransaction.AddToBackStack (null);
			fragmentTransaction.Replace (Resource.Id.registerViewFragment, chooseCardExistanceViewFragment, "choose_card_fragment").Commit ();
			// fragmentTransaction.DisallowAddToBackStack ();
		}

		public override void OnBackPressed ()
		{
			BackToPreviousFragment ();
		}

		#region Fragments

		public void TransferMerchants (List<Merchant> merchants)
		{
			(this.ViewModel as BaseRegisterViewModel).CardVerificationViewModelProperty.Merchants = merchants;
		}

		public void TransferNewFbUserInfo (TransferUserModel newFBUser)
		{
			(this.ViewModel as BaseRegisterViewModel).SMSVerificationViewModelProperty.CurrentUSer = newFBUser;
		}

		public void TransferUserInfo (TransferUserModel user)
		{
			(this.ViewModel as BaseRegisterViewModel).SMSVerificationViewModelProperty.CurrentUSer = user;
		}

		public void OpenEmailRegistrationFragment (Bundle b)
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("regisetr_email_fragment");
			if (_ifAdded == null && !emailRegistraionFragment.IsAdded && !emailRegistraionFragment.IsVisible) {
				emailRegistraionFragment.Arguments = b;
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("regisetr_email_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, emailRegistraionFragment).Commit ();
			}
		}

		public void OpenFbRegisterFragment (Bundle b)
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("fb_register_fragment");
			if (_ifAdded == null && !fbRegisterFragment.IsAdded && !fbRegisterFragment.IsVisible) {
				fbRegisterFragment.Arguments = b;
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("fb_register_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, fbRegisterFragment).Commit ();
			}
		}

		public void OpenRegistrationFragment (Bundle b)
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("register_fragment");
			if (_ifAdded == null && !registerFragment.IsAdded && !registerFragment.IsVisible) {
				registerFragment.Arguments = b;
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("register_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, registerFragment).Commit ();
			}
		
		}

		public void OpenUnicardInputFragment ()
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("unicard_input_fragment");
			if (_ifAdded == null && !unicardInputFragment.IsAdded && !unicardInputFragment.IsVisible) {
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("unicard_input_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, unicardInputFragment).Commit ();
			}
		}

		public void OpenTransactionVerificationFragment (Bundle b)
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("transaction_verification_fragment");
			if (_ifAdded == null && !transactionVerificationFragment.IsAdded && !transactionVerificationFragment.IsVisible) {
				transactionVerificationFragment.Arguments = b;
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("transaction_verification_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, transactionVerificationFragment).Commit ();
			}
		}

		public void OpenSMSVerificationFragment (Bundle b)
		{
			_ifAdded = SupportFragmentManager.FindFragmentByTag ("sms_verification_fragment");
			if (_ifAdded == null && !smsVerificationFragment.IsAdded && !smsVerificationFragment.IsVisible) {
				smsVerificationFragment.Arguments = b;
				var fragmentTransaction = SupportFragmentManager.BeginTransaction ();
				fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_out);
				fragmentTransaction.AddToBackStack ("sms_verification_fragment");
				fragmentTransaction.Add (Resource.Id.registerViewFragment, smsVerificationFragment).Commit ();
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

		public void BackToPreviousFragment ()
		{
			try {
				try {
					int count = SupportFragmentManager.BackStackEntryCount;

					if (count <= 1) {

						if (LoginManager.Instance != null) {
							LoginManager.Instance.LogOut ();
						}
						Intent intent = new Intent (this, typeof(LoginView));
						intent.SetFlags (ActivityFlags.ClearTop);
						StartActivity (intent);
						this.Finish ();
					} else {
						SupportFragmentManager.PopBackStack ();
					}
				} catch {
					Toast.MakeText (this, Resource.String.error_occured, ToastLength.Long).Show ();
				}
			} catch (Exception ex) {
				this.Finish ();
			}
		}
	}
}

