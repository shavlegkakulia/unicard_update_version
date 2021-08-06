using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Text;
using Kuni.Core;
using Android.Views.InputMethods;
using Kuni.Core.ViewModels;

namespace Kunicardus.Droid
{
	public class SetPinDialogFragment : DialogFragment,IDialogInterfaceOnKeyListener
	{
		#region KeyListener

		public bool OnKey (IDialogInterface dialog, Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back && isConfirming) {
				_pin = "";
				_confirmPin = "";
				_all.Text = "";
				_dialogTitle.Text = Resources.GetString (Resource.String.enter_pin);
				_currentActivity.ClearDigits (_first, _second, _third, _forth);
				isConfirming = false;
				return false;
			} else
				return false;
		}

		#endregion

		#region Private Variables

		private View _View;
		private string _pin, _confirmPin;
		private TextView _dialogTitle;
		private bool isConfirming;
		private TextView _first, _second, _third, _forth;
		private FocusableBaseEditText _all;
		private HomePageViewModel _homePageViewModel;
		private ISharedPreferences prefs;
		private ISharedPreferencesEditor editor;
		private MainView _currentActivity;

		#endregion

		#region Constructor Implementation

		public SetPinDialogFragment (HomePageViewModel homePageViewModel)
		{
			_homePageViewModel = homePageViewModel;
		}

		#endregion

		#region Fragment Native Methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_View = inflater.Inflate (Resource.Layout.PinLayout, null);
			_currentActivity = this.Activity as MainView;

			prefs = _currentActivity.GetSharedPreferences ("pref", FileCreationMode.Private);
			var layoutTitle = _View.FindViewById<BaseTextView> (Resource.Id.pin_layout_title);
			layoutTitle.Text = Resources.GetString (Resource.String.enter_pin);

			OpenSetPinInputDialog ();

			return _View;
		}

		#endregion

		#region Methods

		private void OpenSetPinInputDialog ()
		{
			isConfirming = false;
			Dialog.Window.SetSoftInputMode (SoftInput.StateAlwaysVisible);
			_dialogTitle = _View.FindViewById<BaseTextView> (Resource.Id.pin_layout_title);
			_first = _View.FindViewById<TextView> (Resource.Id.first_digit_of_pin);
			_second = _View.FindViewById<TextView> (Resource.Id.second_digit_of_pin);
			_third = _View.FindViewById<TextView> (Resource.Id.third_digit_of_pin);
			_forth = _View.FindViewById<TextView> (Resource.Id.fourth_digit_of_pin);
			_all = _View.FindViewById<FocusableBaseEditText> (Resource.Id.all_pincode_text);
			var dialogLayout = _View.FindViewById<LinearLayout> (Resource.Id.dialog_layout);
			dialogLayout.Click += (sender, e) => {
				ToggleKeyBoard ();
			};
			_all.RequestFocus ();
			_all.FocusChange += delegate(object sender, View.FocusChangeEventArgs e) {
				if (!e.HasFocus) {
					_all.RequestFocus ();
				}
			};

			_all.TextChanged += delegate(object sender, TextChangedEventArgs e) {
				switch (_all.Length ()) {
				case 0:
					{
						_first.Text = "─";
						_second.Text = "─";
						_third.Text = "─";
						_forth.Text = "─";
						break;
					}
				case 1:
					{
						_first.Text = "•";
						_second.Text = "─";
						_third.Text = "─";
						_forth.Text = "─";
						break;
					}
				case 2:
					{
						_first.Text = "•";
						_second.Text = "•";
						_third.Text = "─";
						_forth.Text = "─";
						break;
					}
				case 3:
					{
						_first.Text = "•";
						_second.Text = "•";
						_third.Text = "•";
						_forth.Text = "─";
						break;
					}
				case 4:
					{
						_first.Text = "•";
						_second.Text = "•";
						_third.Text = "•";
						_forth.Text = "•";

						if (!isConfirming)
							_pin = _all.Text;
						if (_pin.Length != 4) {
						} else if (!isConfirming) {
							isConfirming = true;
							_currentActivity.ClearDigits (_first, _second, _third, _forth);
							_all.Text = "";
							_dialogTitle.Text = Resources.GetString (Resource.String.repeat_pin);
						} else if (isConfirming) {
							_confirmPin = _all.Text;
							if (_pin == _confirmPin) {
								(_currentActivity.ViewModel as MainViewModel).InsertSettingsInfo (true, true, true, _pin);
								Dismiss ();
								ToggleKeyBoard ();
								Toast.MakeText (this.Activity, Resources.GetString (Resource.String.pin_set_successfully), ToastLength.Short).Show ();
								editor = prefs.Edit ();
								editor.PutInt (_homePageViewModel.UserId, 1);
								editor.Apply ();

								//GAService.GetGASInstance ().Track_App_Event ("Set Pin", "from dialog");
							} else {
								Toast.MakeText (this.Activity, Resources.GetString (Resource.String.repeated_pin_incorrect), ToastLength.Short).Show ();
								_currentActivity.ClearDigits (_first, _second, _third, _forth);
								_confirmPin = "";
								_all.Text = "";
								isConfirming = true;
								_dialogTitle.Text = Resources.GetString (Resource.String.repeat_pin);
							}

						}
						break;
					}
				default:
					_first.Text = "─";
					_second.Text = "─";
					_third.Text = "─";
					_forth.Text = "─";
					break;
				}
			};
			Dialog.SetOnKeyListener (this);
		}

		private void ToggleKeyBoard ()
		{
			InputMethodManager inputManager = (InputMethodManager)this.Activity.GetSystemService (Context.InputMethodService);
			inputManager.ToggleSoftInput (ShowFlags.Forced, 0);
		}

		#endregion

	}
}

