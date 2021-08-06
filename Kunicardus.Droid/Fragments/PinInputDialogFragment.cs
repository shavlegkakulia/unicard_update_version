using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Views.InputMethods;
using Kunicardus.Droid.Views;

namespace Kunicardus.Droid
{
	public class PinInputDialogFragment : DialogFragment
	{
		#region Constuctor Implementation

		bool _fromMerchants, _fromNews;

		public PinInputDialogFragment (string pinFromDb, bool fromMerchants = false, bool fromNews = false)
		{
			_pinFromDb = pinFromDb;
			_fromMerchants = fromMerchants;
			_fromNews = fromNews;
		}

		#endregion

		#region Private Variables

		private View _View;
		private string _pinFromDb;
		private FocusableBaseEditText all;

		#endregion

		#region Fragment Native Methods

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_View = inflater.Inflate (Resource.Layout.PinLayout, null);
			Dialog.Window.SetSoftInputMode (SoftInput.StateAlwaysVisible);
			var title = _View.FindViewById<BaseTextView> (Resource.Id.pin_layout_title);
			var first = _View.FindViewById<TextView> (Resource.Id.first_digit_of_pin);
			var second = _View.FindViewById<TextView> (Resource.Id.second_digit_of_pin);
			var third = _View.FindViewById<TextView> (Resource.Id.third_digit_of_pin);
			var forth = _View.FindViewById<TextView> (Resource.Id.fourth_digit_of_pin);
			all = _View.FindViewById<FocusableBaseEditText> (Resource.Id.all_pincode_text);
			var dialogLayout = _View.FindViewById<LinearLayout> (Resource.Id.dialog_layout);
			dialogLayout.Click += (sender, e) => {
				ToggleKeyboard ();
			};
			all.RequestFocus ();
			all.FocusChange += delegate(object sender, View.FocusChangeEventArgs e) {
				if (!e.HasFocus) {
					all.RequestFocus ();
				}
			};

			title.Text = Resources.GetString (Resource.String.enter_pin);

			all.TextChanged += delegate(object sender, TextChangedEventArgs e) {
				#region First three textchanges
				switch (all.Length ()) {
				case 0:
					{
						first.Text = "─";
						second.Text = "─";
						third.Text = "─";
						forth.Text = "─";
						break;
					}
				case 1:
					{
						first.Text = "•";
						second.Text = "─";
						third.Text = "─";
						forth.Text = "─";
						break;
					}
				case 2:
					{
						first.Text = "•";
						second.Text = "•";
						third.Text = "─";
						forth.Text = "─";
						break;
					}
				case 3:
					{
						first.Text = "•";
						second.Text = "•";
						third.Text = "•";
						forth.Text = "─";
						break;
					}
					#endregion
				case 4:
					{
						first.Text = "•";
						second.Text = "•";
						third.Text = "•";
						forth.Text = "•";
						string pin = all.Text;
						if (pin == _pinFromDb) {
							Dismiss ();
							ToggleKeyboard ();
							if (_fromMerchants) {
								(this.Activity as MerchantsView)._pinIsOpened = false;
							} else if (_fromNews) {
								(this.Activity as NewsDetailsView)._pinIsOpened = false;
							} else {
								(this.Activity as MainView)._pinIsOpened = false;
							}
						} else {
							Toast.MakeText (this.Activity, Resources.GetString (Resource.String.incorrect_pin), ToastLength.Short).Show ();
							(this.Activity as MainView).ClearDigits (first, second, third, forth);
							all.Text = "";
						}
						break;
					}
				default:
					first.Text = "─";
					second.Text = "─";
					third.Text = "─";
					forth.Text = "─";
					break;
				}

			};
			return _View;
		}

		#endregion

		#region Methods

		private void ToggleKeyboard ()
		{
			InputMethodManager inputManager = (InputMethodManager)this.Activity.GetSystemService (Context.InputMethodService);
			inputManager.ToggleSoftInput (ShowFlags.Forced, 0);
		}

		#endregion
	}
}
