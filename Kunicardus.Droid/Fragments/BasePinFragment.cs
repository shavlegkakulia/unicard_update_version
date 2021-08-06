
using Android.Content;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using Android.Views.InputMethods;
using Android.Text;

namespace Kunicardus.Droid
{
	public abstract class BasePinFragment : MvxFragment
	{
		#region Private Variables

		protected TextView _firstDigit, _secondDigit, _thirdDigit, _fourthDigit;
		protected InputMethodManager imm;
		protected FocusableBaseEditText _all;
		protected LinearLayout _pinLayout;

		#endregion

		#region Deriving Methods

		protected void HideKeyboard ()
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				imm.HideSoftInputFromWindow (View.WindowToken, HideSoftInputFlags.None);
			}
		}

		protected void OpenKeyobard ()
		{
			View View = base.Activity.CurrentFocus;
			if (View != null) {
				imm.ShowSoftInput (View, ShowFlags.Forced);
			}
		}

		protected void ClearDigits ()
		{
			_firstDigit.Text = "─";
			_secondDigit.Text = "─";
			_thirdDigit.Text = "─";
			_fourthDigit.Text = "─";
		}

		protected void TextChangeLogic ()
		{
			_all.TextChanged += delegate(object sender, TextChangedEventArgs e) {
				switch (_all.Length ()) {
				case 0:
					{
						_firstDigit.Text = "─";
						_secondDigit.Text = "─";
						_thirdDigit.Text = "─";
						_fourthDigit.Text = "─";
						break;
					}
				case 1:
					{
						_firstDigit.Text = "•";
						_secondDigit.Text = "─";
						_thirdDigit.Text = "─";
						_fourthDigit.Text = "─";
						break;
					}
				case 2:
					{
						_firstDigit.Text = "•";
						_secondDigit.Text = "•";
						_thirdDigit.Text = "─";
						_fourthDigit.Text = "─";
						break;
					}
				case 3:
					{
						_firstDigit.Text = "•";
						_secondDigit.Text = "•";
						_thirdDigit.Text = "•";
						_fourthDigit.Text = "─";
						break;
					}
				case 4:
					{
						_firstDigit.Text = "•";
						_secondDigit.Text = "•";
						_thirdDigit.Text = "•";
						_fourthDigit.Text = "•";
						break;
					}
				}
			};
		}

		protected void ToggleKeyboardLogic (MainView currentActivity)
		{
			_pinLayout.Click += (sender, e) => {
				InputMethodManager inputManager = (InputMethodManager)currentActivity.GetSystemService (Context.InputMethodService);
				inputManager.ToggleSoftInput (ShowFlags.Forced, 0);
			};
		}

		#endregion
	}
}

