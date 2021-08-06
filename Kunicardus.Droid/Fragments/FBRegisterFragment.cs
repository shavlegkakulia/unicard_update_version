using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using Kuni.Core;
using MvvmCross.Binding.BindingContext;
using Xamarin.Facebook.Login;
using Android.Views.InputMethods;

namespace Kunicardus.Droid
{
	public class FBRegisterFragment : MvxFragment
	{
		#region Properties

		private bool _validationSuccess;

		public bool ValidationSuccess {
			get { return _validationSuccess; }
			set {
				if (currentViewModel.ValidationSuccess) {
					var phoneNumber = currentViewModel.PhoneNumber;
					(this.Activity as BaseRegisterView).TransferNewFbUserInfo (currentViewModel.NewFBUser);
					Bundle bundle = new Bundle ();
					bundle.PutString ("phone_number", phoneNumber);
					bundle.PutBoolean ("fb_registration_in_progress", true);
					bundle.PutString ("unicard_number", _unicardNumber);
					(this.Activity as BaseRegisterView).OpenSMSVerificationFragment (bundle);
				}
				_validationSuccess = value;
			}
		}

		#endregion

		#region Private Variables

		private BaseEditText _dateEditText;
		private FBRegisterViewModel currentViewModel;
		private string _unicardNumber;

		#endregion

		#region Fragment Native Methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.FBRegisterView, null);
			var continueRegister = View.FindViewById<ImageButton> (Resource.Id.continue_facebook_register);
			currentViewModel = (this.ViewModel as FBRegisterViewModel);
			if (Arguments != null) {
				_unicardNumber = Arguments.GetString ("unicard_number");
			}
			DatePickerDialog d = new DatePickerDialog (
				                     this.Activity,
				                     (object sender2, DatePickerDialog.DateSetEventArgs e2) => {
					currentViewModel.DateOfBirth = e2.Date;
				},
				                     DateTime.Now.Year,
				                     DateTime.Now.Month,
				                     DateTime.Now.Day);
            
			var backButton = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			backButton.Click += ( sender, e) => {
				LoginManager.Instance.LogOut ();
			};
			var toolbarTitle = backButton.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			var currentLayout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			currentLayout.Click += (sender, e) => HideKeyboard ();
			_dateEditText = View.FindViewById<BaseEditText> (Resource.Id.fb_user_dateofbirth);
			_dateEditText.FocusChange += (object sender, View.FocusChangeEventArgs e) => {
				if (_dateEditText.IsFocused)
					this.Activity.RunOnUiThread (() => {
						d.Show ();
					});
			};
			_dateEditText.Click += (object sender, EventArgs e) => {
				if (!d.IsShowing)
					d.Show ();
			};
			var set = this.CreateBindingSet<FBRegisterFragment , Kuni.Core.FBRegisterViewModel> ();
			set.Bind (this).For (p => p.ValidationSuccess).To (vm => vm.ValidationSuccess);
			set.Apply ();

			return View;
		}

		#endregion

		public void HideKeyboard ()
		{
			View View = this.Activity.CurrentFocus;
			if (View != null) {
				var inputManager = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);
				inputManager.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.None);
			}
		}
	}
}