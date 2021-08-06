using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross;
using Kuni.Core;
using System.Threading.Tasks;
using Android.Support.V4;
using Android.Util;

namespace Kunicardus.Droid.Fragments
{

	public enum SettingsPinPages
	{
		SetPin,
		ChangePin,
		RemovePin,
		ConfirmPin
	}

	public class SettingsFragment : BaseMvxFragment
	{
		#region Private Variables

		private Dialog _dialog;
		private Context _context;
		private View _view;
		private MainView _mainView;
		private RelativeLayout _removePinLayout, _setPinLayout, _changePinLayout, _changePasswordLayout;
		private SettingsViewModel _currentViewModel;

		#endregion

		#region Constructor Implementation

		public SettingsFragment ()
		{
			if (this.ViewModel == null)
                this.ViewModel = (MvvmCross.ViewModels.IMvxViewModel)Mvx.IoCConstruct<SettingsViewModel>();
		}

		#endregion

		#region Fragment Native Methods

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			_view = this.BindingInflate (Resource.Layout.SettingsLayout, null);

			_context = this.Activity;
			_mainView = this.Activity as MainView;

//			var toolbar = _view.FindViewById<RelativeLayout> (Resource.Id.settings_toolbar);
			var menu = _view.FindViewById<ImageButton> (Resource.Id.settings_menu);
			menu.Click += OpenMenu;

			_removePinLayout = _view.FindViewById<RelativeLayout> (Resource.Id.remove_pin_layout);
			_setPinLayout = _view.FindViewById<RelativeLayout> (Resource.Id.set_pin_layout);
			_changePinLayout = _view.FindViewById<RelativeLayout> (Resource.Id.change_pin_layout);
			_changePasswordLayout = _view.FindViewById<RelativeLayout> (Resource.Id.change_password_layout);

			_removePinLayout.Click += (sender, e) => OpenSetPinPage (SettingsPinPages.RemovePin);
			_setPinLayout.Click += (sender, e) => OpenSetPinPage (SettingsPinPages.SetPin);
			_changePinLayout.Click += (sender, e) => OpenSetPinPage (SettingsPinPages.ChangePin);
			_changePasswordLayout.Click += (sender, e) => OpenChangePasswordPage ();

			var packageInfo = _mainView.PackageManager.GetPackageInfo (_mainView.PackageName, 0);

			_view.FindViewById<TextView> (Resource.Id.version).Text = string.Format ("ვერსია: V{0}.{1}", packageInfo.VersionName, packageInfo.VersionCode);

			_currentViewModel = (this.ViewModel as SettingsViewModel);
			InitUserInfo ();
			return _view;
		}

		#endregion

		#region Fragment Methods&logic

		private void InitUserInfo ()
		{
			var fullName = _view.FindViewById<BaseTextView> (Resource.Id.settings_fullname);
			var userName = _view.FindViewById<BaseTextView> (Resource.Id.settings_username);
			var fb_icon = _view.FindViewById<ImageView> (Resource.Id.fb_icon);
			var logout = _view.FindViewById<BaseButton> (Resource.Id.settings_logout);
			var byWandio = _view.FindViewById<Button> (Resource.Id.by_wandio);

			byWandio.Click += (sender, e) => {
				OpenWebPage ("http://www.wandio.com");
			};
			if (_currentViewModel.UserInfo != null) {
				fullName.Text = String.Format ("{0} {1}", _currentViewModel.UserInfo.FirstName, _currentViewModel.UserInfo.LastName);
				userName.Text = _currentViewModel.UserInfo.Username;
				if (!_currentViewModel.UserInfo.IsFacebookUser)
					fb_icon.Visibility = ViewStates.Invisible;
				else
					fb_icon.Visibility = ViewStates.Visible;
				logout.Click += (sender, e) => {
					try {
						_currentViewModel.Logout ();
						Intent intent = new Intent (_mainView, typeof(LoginView));
						intent.SetFlags (ActivityFlags.ClearTop | ActivityFlags.NewTask);
						_mainView.Finish ();
						StartActivity (intent);
					} catch {
						Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
					}
				};
			}
		}

		private void OpenWebPage (string address)
		{
			try {
				var uri = Android.Net.Uri.Parse (address);
				var intent = new Intent (Intent.ActionView, uri);
				StartActivity (intent);
			} catch {
				Toast.MakeText (Activity, Resource.String.error_occured, ToastLength.Long).Show ();
			}
		}

		private void OpenMenu (object sender, EventArgs e)
		{
			(_mainView as MainView).ShowMenu ();	
		}

		private void OpenChangePasswordPage ()
		{
			ChangePasswordFragment changePasswordFragment = new ChangePasswordFragment ();
			var fragmentTransaction = _mainView.SupportFragmentManager.BeginTransaction ();
			fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_in_back);
			fragmentTransaction.AddToBackStack ("change_passsword_fragment");
			fragmentTransaction.Add (Resource.Id.main_fragment, changePasswordFragment).Commit ();
		}

		private void OpenSetPinPage (SettingsPinPages settingsPage)
		{
			SettingsPinFragment changePasswordFragment = new SettingsPinFragment (settingsPage);
			var fragmentTransaction = _mainView.SupportFragmentManager.BeginTransaction ();
			fragmentTransaction.SetCustomAnimations (Resource.Animation.slide_in, Resource.Animation.slide_in_back);
			fragmentTransaction.AddToBackStack ("set_pin_fragment");
			fragmentTransaction.Add (Resource.Id.main_fragment, changePasswordFragment).Commit ();
		}

		#endregion

		#region Interface Implemented method

		public override void OnActivate ()
		{
			ISharedPreferences prefs = _mainView.GetSharedPreferences ("pref", FileCreationMode.Private);
			var hideRemovePinLayout = prefs.GetInt ((this.ViewModel as SettingsViewModel).UserInfo.UserId, 0);

			if (_currentViewModel != null && _currentViewModel.UserInfo.IsFacebookUser) {
				_changePasswordLayout.Visibility = ViewStates.Gone;
			}

			this.Activity.RunOnUiThread (() => {
				
				switch (hideRemovePinLayout) {
				case 0:
					_removePinLayout.Visibility = ViewStates.Gone;
					_changePinLayout.Visibility = ViewStates.Gone;
					_setPinLayout.Visibility = ViewStates.Visible;
					break;
				case 1:
					_removePinLayout.Visibility = ViewStates.Visible;
					_changePinLayout.Visibility = ViewStates.Visible;
					_setPinLayout.Visibility = ViewStates.Gone;
					break;
				case 2:
					_removePinLayout.Visibility = ViewStates.Gone;
					_changePinLayout.Visibility = ViewStates.Gone;
					_setPinLayout.Visibility = ViewStates.Visible;
					break;
				}
			});
		}

		#endregion

	}
}