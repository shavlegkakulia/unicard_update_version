using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Kuni.Core;
using Android.Views.InputMethods;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Binding.BindingContext;

namespace Kunicardus.Droid
{
	public class UnicardInputFragment : MvxFragment
	{
		private EditText cardInput1, cardInput2, cardInput3, cardInput4;
		private	BaseRegisterView _activity;

		public bool DataPopulated {
			get { return true; }
			set {
				if (value) {
					var currentViewModel = this.ViewModel as UnicardNumberInputViewModel;
					var unicardStatus = currentViewModel.UnicardStatus;
					if (unicardStatus != null)
					if (unicardStatus.CardIsValid) {
						Bundle bundle = new Bundle ();
						bundle.PutString ("unicard_number", (this.ViewModel as UnicardNumberInputViewModel).CardNumber);
						bool emailRegisterInProgress = string.IsNullOrEmpty ((_activity.ViewModel as BaseRegisterViewModel).FBRegisterViewModelProperty.FBId);
						bool facebookRegisterInProgress = !emailRegisterInProgress;
						bundle.PutBoolean ("email_register_in_progress", emailRegisterInProgress);
						bundle.PutBoolean ("fb_registration_in_progress", facebookRegisterInProgress);
						if (unicardStatus.ExistsUserData) {
							bundle.PutBoolean ("mask_number", true);
							bundle.PutString ("phone_number", currentViewModel.RetrievedUserPhoneNumber);
							(this.Activity as BaseRegisterView).OpenSMSVerificationFragment (bundle);
						} else {
							if (unicardStatus.HasTransactions) {
								(this.Activity as BaseRegisterView).TransferMerchants ((this.ViewModel as UnicardNumberInputViewModel).Merchants);
								(this.Activity as BaseRegisterView).OpenTransactionVerificationFragment (bundle);
							} 
						}
					}
				}
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.UnicardNumberInputView, null);

			var layout = View.FindViewById<RelativeLayout> (Resource.Id.relativelayout);
			layout.Click += HideKeyboard;

			_activity = ((BaseRegisterView)base.Activity);

			var backToolbar = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backLayout = backToolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);
			backLayout.Click += BackClick;
			var toolbarTitle = backToolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			cardInput1 = View.FindViewById<EditText> (Resource.Id.unicard_number_input);
			cardInput2 = View.FindViewById<EditText> (Resource.Id.editText1);
			cardInput3 = View.FindViewById<EditText> (Resource.Id.editText2);
			cardInput4 = View.FindViewById<EditText> (Resource.Id.editText3);

			cardInput1.TextChanged += cardInput1_TextChanged;
			cardInput1.RequestFocus ();

            //todo
			var set = this.CreateBindingSet<UnicardInputFragment, UnicardNumberInputViewModel> ();
			set.Bind (this).For (v => v.DataPopulated).To (vmod => vmod.DataPopulated);
			set.Apply (); 

			cardInput1.TextChanged += cardInput1_TextChanged;
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

		void cardInput1_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
			if (cardInput1.Text.Length == 4) {
				cardInput2.Enabled = true;
				cardInput2.RequestFocus ();
				cardInput2.TextChanged += cardInput2_TextChanged;
			}
			if (cardInput1.Text.Length < 4 && string.IsNullOrEmpty (cardInput2.Text)) {
				cardInput2.Enabled = false;
			}
		}

		void cardInput2_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{

			if (cardInput2.Text.Length == 0) {
				cardInput1.RequestFocus ();
			}
			if (cardInput2.Text.Length < 4 && string.IsNullOrEmpty (cardInput3.Text)) {
				cardInput3.Enabled = false;
			}
			if (cardInput2.Text.Length == 4) {
				cardInput3.Enabled = true;
				cardInput3.RequestFocus ();
				cardInput3.TextChanged += cardInput3_TextChanged; 
			}
		}

		private void cardInput3_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{

			if (cardInput3.Text.Length == 0) {
				cardInput2.RequestFocus ();
			}
			if (cardInput3.Text.Length < 4 && string.IsNullOrEmpty (cardInput4.Text)) {
				cardInput4.Enabled = false;
			}
			if (cardInput3.Text.Length == 4) {
				cardInput4.Enabled = true;
				cardInput4.RequestFocus ();
				cardInput4.TextChanged += cardInput4_TextChanged; 
			}
		}

		private void cardInput4_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
			if (((EditText)sender).Text.Length == 0) {
				cardInput3.RequestFocus ();
			}
		}


		public void OnClick (View v)
		{
			throw new NotImplementedException ();
		}
	}
}

