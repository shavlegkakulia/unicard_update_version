using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Kuni.Core;
using Kuni.Core.ViewModels;

namespace Kunicardus.Droid
{
	public class ChoosePinSettingsDialogFragment : DialogFragment
	{
		#region Private Variables

		private MainView currentActivity;
		private View _View;
		private ISharedPreferences prefs;
		private ISharedPreferencesEditor editor;
		private HomePageViewModel _homePageViewModel;

		#endregion

		#region Constructor Implementation

		public ChoosePinSettingsDialogFragment (HomePageViewModel homePageViewModel)
		{
			_homePageViewModel = homePageViewModel;
		}

		#endregion

		#region Fragment native methods

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			currentActivity = this.Activity as MainView;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_View = inflater.Inflate (Resource.Layout.ChoosePinSet, null);
			prefs = currentActivity.GetSharedPreferences ("pref", FileCreationMode.Private);

			currentActivity = this.Activity as MainView;
			var withPinRadioButton = _View.FindViewById<ImageView> (Resource.Id.set_pin_radiobutton);
			var withOutPinRadioButton = _View.FindViewById<ImageView> (Resource.Id.set_without_pin_radiobutton);

			var withPinTextView = _View.FindViewById<BaseTextView> (Resource.Id.set_pin_textview);
			var withOutPinTextView = _View.FindViewById<BaseTextView> (Resource.Id.set_without_pin_textview);

			withPinTextView.Click += (sender, e) => {
				WithPin ();
			}; 
			withOutPinTextView.Click += (sender, e) => {
				WithoutPin ();
			};

			withPinRadioButton.Click += (sender, e) => {
				WithPin ();
			};
			withOutPinRadioButton.Click += (sender, e) => {
				WithoutPin ();
			};

			return	_View;
		}

		#endregion

		#region Methods

		private void WithPin ()
		{
			Dismiss ();
			var setPinDialog = new SetPinDialogFragment (_homePageViewModel);
			setPinDialog.Cancelable = false;
			setPinDialog.SetStyle (DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBar);
			setPinDialog.Show (currentActivity.FragmentManager, "");
		}

		private void WithoutPin ()
		{
			(currentActivity.ViewModel as MainViewModel).InsertSettingsInfo (true, true, false, null);
			editor = prefs.Edit ();
			editor.PutInt (_homePageViewModel.UserId, 2);
			editor.Apply ();
			Dismiss ();
		}

		#endregion
	}
}

