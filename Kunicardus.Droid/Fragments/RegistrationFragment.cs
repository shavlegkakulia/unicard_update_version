using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Kuni.Core;
using MvvmCross.Binding.BindingContext;
using Kuni.Core.Models.DataTransferObjects;
using Android.Text.Method;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid
{
	public class RegistrationFragment : MvxFragment
	{
		
		#region Properties

		private bool _userExists;

		public  bool UserExists {
			get{ return _userExists; }
			set {
				if (!currentViewModel.UserExists && currentViewModel.ValidationStatus) {
					TransferUserModel userInfo = new TransferUserModel () {
						Name = currentViewModel.Name,
						Surname = currentViewModel.Surname,
						Password = currentViewModel.Password,
						Email = currentViewModel.Email,
						PersonalId = currentViewModel.IdNumber,
						DateOfBirth = currentViewModel.DateOfBirth,
						PhoneNumber = currentViewModel.PhoneNumber,
						CardNumber = unicardNumber
					};
					_activity.TransferUserInfo (userInfo);
					Bundle b = new Bundle ();
					b.PutString ("phone_number", currentViewModel.PhoneNumber);
					_activity.OpenSMSVerificationFragment (b);
				}
				_userExists = value;
				currentViewModel.RegisterInProgress = false;
			}
		}

		#endregion

		#region Private Variables

		BaseEditText _passwordEditText, _confirmPasswordEdittext;
		private int _selectionStart, _selectionEnd;
		private bool _confirmIsFocused;
		private BaseEditText _dateEditText;
		private BaseRegisterView _activity;
		private RegistrationViewModel currentViewModel;
		private string unicardNumber;

		#endregion

		#region Fragment native methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.RegisterView, null);
	
			currentViewModel = (this.ViewModel as RegistrationViewModel);

			_activity = ((BaseRegisterView)this.Activity);

			var checkBox = View.FindViewById<CheckBox> (Resource.Id.register_checkbox);
			checkBox.CheckedChange += TogglePassoword;

			var backToolbar = View.FindViewById<RelativeLayout> (Resource.Id.backbuttonToolbar);
			var backLayout = backToolbar.FindViewById<LinearLayout> (Resource.Id.back_button_layout);
			backLayout.Click += BackClick;

			var toolbarTitle = backToolbar.FindViewById<BaseTextView> (Resource.Id.toolbar_title);
			toolbarTitle.Text = Resources.GetString (Resource.String.registration);

			var currentLayout = View.FindViewById<RelativeLayout> (Resource.Id.regRelativeLayout);
			currentLayout.Click += (sender, e) => {
				_activity.HideKeyboard ();
			};

			DatePickerDialog d = new DatePickerDialog (
				                     this.Activity,
				                     (object sender2, DatePickerDialog.DateSetEventArgs e2) => {
					currentViewModel.DateOfBirth = e2.Date;
				},
				                     DateTime.Now.Year,
				                     DateTime.Now.Month,
				                     DateTime.Now.Day);


			_dateEditText = View.FindViewById<BaseEditText> (Resource.Id.email_register_date_of_birth);
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
			unicardNumber = "";
			if (Arguments != null) {
				unicardNumber = Arguments.GetString ("unicard_number");
				(this.ViewModel as RegistrationViewModel).UserUnicardNumber = unicardNumber;
			}

			var set = this.CreateBindingSet<RegistrationFragment , Kuni.Core.RegistrationViewModel> ();
			set.Bind (this).For (p => p.UserExists).To (vm => vm.UserExists);
			set.Apply ();

			return View;
		}

		#endregion

		#region Methods

		void BackClick (object sender, EventArgs e)
		{
			_activity.HideKeyboard ();
			_activity.BackToPreviousFragment ();
		}

		#endregion

		#region Receiving Messages

		void TogglePassoword (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (_passwordEditText == null)
				_passwordEditText = _activity.FindViewById<BaseEditText> (Resource.Id.password);
			if (_confirmPasswordEdittext == null)
				_confirmPasswordEdittext = _activity.FindViewById<BaseEditText> (Resource.Id.confirmPassword);

			if (_passwordEditText.HasFocus) {
				_selectionStart = _passwordEditText.SelectionStart;
				_selectionEnd = _passwordEditText.SelectionEnd;
				_confirmIsFocused = false;
			} else {
				_selectionStart = _confirmPasswordEdittext.SelectionStart;
				_selectionEnd = _confirmPasswordEdittext.SelectionEnd;
				_confirmIsFocused = true;
			}	

			if (e.IsChecked) {
				_passwordEditText.TransformationMethod = HideReturnsTransformationMethod.Instance;
				_confirmPasswordEdittext.TransformationMethod = HideReturnsTransformationMethod.Instance;
			} else {
				_passwordEditText.TransformationMethod = PasswordTransformationMethod.Instance;
				_confirmPasswordEdittext.TransformationMethod = PasswordTransformationMethod.Instance;
			}
			if (!_confirmIsFocused)
				_passwordEditText.SetSelection (_selectionStart, _selectionEnd);
			else
				_confirmPasswordEdittext.SetSelection (_selectionStart, _selectionEnd);
		}

		#endregion
	}
}


