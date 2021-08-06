using System;

using Android.Content;
using Android.OS;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross;
using Kuni.Core;
using Android.Views.InputMethods;
using Kunicardus.Droid.Fragments;
using Android.Views;

namespace Kunicardus.Droid
{
	public class SettingsPinFragment : BasePinFragment, View.IOnKeyListener
	{
		#region Key Listener

		public bool OnKey (View v, Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up) {
				BackPressLogic ();
				return true;
			}
			return false;
		}

		#endregion

		#region Private Variables

		private MainView _mainView;
		private bool _fromSetPin;
		private SettingsPinPages _settingsPage;
		private BaseTextView _title, _pageTitle;
		private string _pin, _confirmPin;
		private ISharedPreferences prefs;
		private ISharedPreferencesEditor editor;
		private bool _backPressed;

		#endregion

		#region Constructor Implementation

		public SettingsPinFragment (SettingsPinPages settingsPage)
		{
			if (this.ViewModel == null)
				this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCConstruct<PinViewModel>();
			_settingsPage = settingsPage;
			if (_settingsPage == SettingsPinPages.SetPin)
				_fromSetPin = true;
		}

		#endregion

		#region Native Methods

		public override void OnViewCreated (View View, Bundle savedInstanceState)
		{
			base.OnViewCreated (View, savedInstanceState);
			imm = (InputMethodManager)_mainView.GetSystemService	(
				Context.InputMethodService);
			OpenKeyobard ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			var View = this.BindingInflate (Resource.Layout.SettingsPinLayout, null);

			var titleLayout = View.FindViewById<RelativeLayout> (Resource.Id.change_pin_layout_toolbar);
			_title = titleLayout.FindViewById<BaseTextView> (Resource.Id.pageTitle);
			_pinLayout = View.FindViewById<LinearLayout> (Resource.Id.pin_layout);
			ToggleKeyboardLogic (this.Activity as MainView);

			_pageTitle = View.FindViewById<BaseTextView> (Resource.Id.pin_layout_title);

			_mainView = this.Activity as MainView;
			prefs = _mainView.GetSharedPreferences ("pref", FileCreationMode.Private);
			editor = prefs.Edit ();

			var backButton = View.FindViewById<ImageButton> (Resource.Id.change_password_back);
			backButton.Click += (sender, e) => {
				BackPressLogic ();
			};

			_firstDigit = View.FindViewById<TextView> (Resource.Id.first_digit_of_pin);
			_secondDigit = View.FindViewById<TextView> (Resource.Id.second_digit_of_pin);
			_thirdDigit = View.FindViewById<TextView> (Resource.Id.third_digit_of_pin);
			_fourthDigit = View.FindViewById<TextView> (Resource.Id.fourth_digit_of_pin);
			_all = View.FindViewById<FocusableBaseEditText> (Resource.Id.all_pincode_text);
			_all.RequestFocus ();
			_all.SetOnKeyListener (this);
			_all.FocusChange += delegate(object sender, View.FocusChangeEventArgs e) {
				if (!e.HasFocus) {
					_all.RequestFocus ();
				}
			};

			TextChangeLogic ();
			FourthDigitChangeLogic ();
			SetTitle ();

			return View;
		}

		#endregion

		#region Methods

		private void SetTitle ()
		{
			if (_settingsPage == SettingsPinPages.SetPin) {
				SetPinTitle ();
			} else if (_settingsPage == SettingsPinPages.ChangePin) {
				BaseChangePinTitle ();
			} else if (_settingsPage == SettingsPinPages.RemovePin) {
				RemovePinTitle ();
			}
		}

		private void SetPinTitle ()
		{
			_title.Text = Resources.GetString (Resource.String.set_pin);
			_pageTitle.Text = Resources.GetString (Resource.String.enter_pin);
		}

		private void BaseChangePinTitle ()
		{
			_title.Text = Resources.GetString (Resource.String.change_pin);
			_pageTitle.Text = Resources.GetString (Resource.String.enter_old_pin);
		}

		private void ChangePinTitle ()
		{
			_title.Text = Resources.GetString (Resource.String.change_pin);
			_pageTitle.Text = Resources.GetString (Resource.String.enter_new_pin);
		}

		private void RemovePinTitle ()
		{
			_title.Text = Resources.GetString (Resource.String.remove_pin);
			_pageTitle.Text = Resources.GetString (Resource.String.enter_pin);
		}

		private void ConfirmPinTitle ()
		{
			if (_fromSetPin) {
				_title.Text = Resources.GetString (Resource.String.set_pin);
				_pageTitle.Text = Resources.GetString (Resource.String.repeat_pin);
			} else {
				_title.Text = Resources.GetString (Resource.String.change_pin);
				_pageTitle.Text = Resources.GetString (Resource.String.repeat_new_pin);
			}
		}

		private void SetPinLogic ()
		{
			_pin = _all.Text;
			_all.Text = "";
			_settingsPage = SettingsPinPages.ConfirmPin;
			ConfirmPinTitle ();
		}

		private void ConfirmPin ()
		{
			_confirmPin = _all.Text;
			string toastText = "";

			if (_confirmPin == _pin) {

				if (_fromSetPin) {
					(this.ViewModel as PinViewModel).UpdatePin (_pin);
					toastText = Resources.GetString (Resource.String.pin_set_successfully);
				} else {
					(this.ViewModel as PinViewModel).ChangePin (_pin);
					toastText = Resources.GetString (Resource.String.pin_changed_successfully);
				}

				editor.PutInt ((this.ViewModel as PinViewModel).UserId, 1);
				editor.Apply ();
				//GAService.GetGASInstance ().Track_App_Event ("Set Pin", "from settings");
				Toast.MakeText (this.Activity, toastText, ToastLength.Short).Show ();
				HideKeyboard ();
				_mainView.SupportFragmentManager.BeginTransaction ().Remove (this);
				_mainView.RefreshSettings (SettingsPinPages.ConfirmPin);
			} else {
				ClearDigits ();
				_all.Text = "";
				Toast.MakeText (this.Activity, Resources.GetString (Resource.String.repeated_pin_incorrect), ToastLength.Short).Show ();
			}
		}

		private void RemovePinLogic ()
		{
			string pin = _all.Text;
			var response = (this.ViewModel as PinViewModel).RemovePin (pin);
			if (response) {
				editor = prefs.Edit ();
				editor.PutInt ((this.ViewModel as PinViewModel).UserId, 2);
				editor.Apply ();
				//GAService.GetGASInstance ().Track_App_Event ("Remove Pin", "from settings");
				Toast.MakeText (_mainView, Resources.GetString (Resource.String.removed_pin_successfully), ToastLength.Short).Show ();
				HideKeyboard ();
				_mainView.SupportFragmentManager.BeginTransaction ().Remove (this);
				_mainView.RefreshSettings (SettingsPinPages.RemovePin);
			} else {
				Toast.MakeText (_mainView, Resources.GetString (Resource.String.incorrect_pin), ToastLength.Short).Show ();
				_all.Text = "";
				ClearDigits ();
			}
		}

		private void ChangePin ()
		{
			string pin = _all.Text;
			var response = (this.ViewModel as PinViewModel).PinIsCorrect (pin);
			if (response) {
				_fromSetPin = false;
				_settingsPage = SettingsPinPages.SetPin;
				_pin = _all.Text;
				_all.Text = "";
				ClearDigits ();
				ChangePinTitle ();
				//GAService.GetGASInstance ().Track_App_Event ("Change Pin", "from settings");
			} else {
				Toast.MakeText (this.Activity, Resources.GetString (Resource.String.incorrect_pin), ToastLength.Short).Show ();
				ClearDigits ();
				_all.Text = "";
			}
		}

		private void BackPressLogic ()
		{
			_backPressed = true;
			_all.Text = _pin;
			_all.SetSelection (_all.Text.Length);
			switch (_settingsPage) {
			case SettingsPinPages.ConfirmPin:
				_settingsPage = SettingsPinPages.SetPin;
				if (_fromSetPin) {
					SetPinTitle ();
				} else {
					ChangePinTitle ();
				}
				break;
			case SettingsPinPages.ChangePin:
				HideKeyboard ();
				_mainView.OnBackPressed ();
				break;
			case SettingsPinPages.RemovePin:
				HideKeyboard ();
				_mainView.OnBackPressed ();
				break;
			case SettingsPinPages.SetPin:
				if (_fromSetPin) {
					HideKeyboard ();
					_mainView.OnBackPressed ();
				} else {
					BaseChangePinTitle ();
					_settingsPage = SettingsPinPages.ChangePin;
				}
				break;
			}
		}

		private void FourthDigitChangeLogic ()
		{
			_all.TextChanged += (sender, e) => {
				if (_all.Text.Length == 4 && !_backPressed) {
					_backPressed = false;
					if (_settingsPage == SettingsPinPages.SetPin)
						SetPinLogic ();
					else if (_settingsPage == SettingsPinPages.ConfirmPin) {
						ConfirmPin ();
					} else if (_settingsPage == SettingsPinPages.RemovePin) {
						RemovePinLogic ();
					} else if (_settingsPage == SettingsPinPages.ChangePin) {
						ChangePin ();
					}
				} else {
					_backPressed = false;
				}
			};
		}

		#endregion
	}
}

